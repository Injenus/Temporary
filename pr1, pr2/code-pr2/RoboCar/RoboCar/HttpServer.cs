using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

// offered to the public domain for any use with no restriction
// and also with no warranty of any kind, please enjoy. - David Jeske. 

// simple HTTP explanation
// http://www.jmarshall.com/easy/http/

namespace RoboCar
{
    public class HttpHelper
    {
        public static string GetParam(string url, string name)
        {
            foreach (string item in url.Split('&'))
            {
                string[] parts = item.Trim('/', '?').Split('=');
                if (parts[0] == name) return parts[1];
            }
            return null;
        }

    }
    public class HttpProcessor
    {
        public TcpClient socket;
        public HttpServer srv;

        private Stream inputStream;
        public StreamWriter outputStream;

        public String http_method;
        public String http_url;
        public String http_protocol_versionstring;
        public Hashtable httpHeaders = new Hashtable();


        private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

        public HttpProcessor(TcpClient s, HttpServer srv)
        {
            this.socket = s;
            this.srv = srv;
        }


        private string streamReadLine(Stream inputStream)
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = inputStream.ReadByte();
                if (next_char == '\n') { break; }
                if (next_char == '\r') { continue; }
                if (next_char == -1) { Thread.Sleep(1); continue; };
                data += Convert.ToChar(next_char);
            }
            return data;
        }
        public void process()
        {
            // we can't use a StreamReader for input, because it buffers up extra data on us inside it's
            // "processed" view of the world, and we want the data raw after the headers
            inputStream = new BufferedStream(socket.GetStream());

            // we probably shouldn't be using a streamwriter for all output from handlers either
            outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
            try
            {
                parseRequest();
                readHeaders();
                if (http_method.Equals("GET"))
                {
                    handleGETRequest();
                }
                else if (http_method.Equals("POST"))
                {
                    handlePOSTRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                writeFailure();
            }
            outputStream.Flush();
            // bs.Flush(); // flush any remaining output
            inputStream = null; outputStream = null; // bs = null;            
            socket.Close();
        }

        public void parseRequest()
        {
            String request = streamReadLine(inputStream);
            string[] tokens = request.Split(' ');
            if (tokens.Length != 3)
            {
                throw new Exception("invalid http request line");
            }
            http_method = tokens[0].ToUpper();
            http_url = tokens[1];
            http_protocol_versionstring = tokens[2];

            Console.WriteLine("starting: " + request);
        }

        public void readHeaders()
        {
            Console.WriteLine("readHeaders()");
            String line;
            while ((line = streamReadLine(inputStream)) != null)
            {
                if (line.Equals(""))
                {
                    Console.WriteLine("got headers");
                    return;
                }

                int separator = line.IndexOf(':');
                if (separator == -1)
                {
                    throw new Exception("invalid http header line: " + line);
                }
                String name = line.Substring(0, separator);
                int pos = separator + 1;
                while ((pos < line.Length) && (line[pos] == ' '))
                {
                    pos++; // strip any spaces
                }

                string value = line.Substring(pos, line.Length - pos);
                Console.WriteLine("header: {0}:{1}", name, value);
                httpHeaders[name] = value;
            }
        }

        public void handleGETRequest()
        {
            srv.handleGETRequest(this);
        }

        private const int BUF_SIZE = 4096;
        public void handlePOSTRequest()
        {
            // this post data processing just reads everything into a memory stream.
            // this is fine for smallish things, but for large stuff we should really
            // hand an input stream to the request processor. However, the input stream 
            // we hand him needs to let him see the "end of the stream" at this content 
            // length, because otherwise he won't know when he's seen it all! 

            Console.WriteLine("get post data start");
            int content_len = 0;
            MemoryStream ms = new MemoryStream();
            if (this.httpHeaders.ContainsKey("Content-Length"))
            {
                content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
                if (content_len > MAX_POST_SIZE)
                {
                    throw new Exception(
                        String.Format("POST Content-Length({0}) too big for this simple server",
                          content_len));
                }
                byte[] buf = new byte[BUF_SIZE];
                int to_read = content_len;
                while (to_read > 0)
                {
                    Console.WriteLine("starting Read, to_read={0}", to_read);

                    int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
                    Console.WriteLine("read finished, numread={0}", numread);
                    if (numread == 0)
                    {
                        if (to_read == 0)
                        {
                            break;
                        }
                        else
                        {
                            throw new Exception("client disconnected during post");
                        }
                    }
                    to_read -= numread;
                    ms.Write(buf, 0, numread);
                }
                ms.Seek(0, SeekOrigin.Begin);
            }
            Console.WriteLine("get post data end");
            srv.handlePOSTRequest(this, new StreamReader(ms));

        }

        public void writeSuccess(string content, bool close)
        {
            outputStream.Write("HTTP/1.0 200 OK\r\n");
            var L = Encoding.Unicode.GetByteCount(content);
            outputStream.Write("Content-Length: " + L + "\r\n");
            outputStream.Write("Content-Type: text/html; charset=UTF-8\r\n");
            outputStream.Write(string.Format("Connection: {0}\r\n", close ? "close" : "keep-alive"));
            outputStream.Write("\r\n");
            outputStream.Write(content);
            outputStream.Flush();
        }

        public void writeSuccessJSON(string content, bool close)
        {
            outputStream.Write("HTTP/1.0 200 OK\r\n");
            var L = Encoding.Unicode.GetByteCount(content);
            outputStream.Write("Content-Length: " + L + "\r\n");
            outputStream.Write("Content-Type: application/json; charset=UTF-8\r\n");
            outputStream.Write(string.Format("Connection: {0}\r\n", close ? "close" : "keep-alive"));
            outputStream.Write("\r\n");
            outputStream.Write(content);
            outputStream.Flush();
        }

        public void writeFailure()
        {
            outputStream.Write("HTTP/1.0 404 File not found\r\n");
            outputStream.Write("Connection: close\r\n");
            outputStream.Write("\r\n");
        }
    }

    public abstract class HttpServer
    {

        protected IPAddress ip;
        protected int port;

        TcpListener listener;

        public HttpServer(IPAddress ip, int port)
        {
            this.ip = ip;
            this.port = port;
        }
        public class ClientInfo
        {
            public TcpClient client;
            public Thread thread;
            public void Disconnect()
            {
                try
                {
                    thread.Abort();
                }
                catch (Exception e)
                {
                    //Console.WriteLine("Exception: " + e.ToString());
                }
            }
        }
        List<ClientInfo> clients = new List<ClientInfo>();
        public void RemoveDisconnectedClients()
        {
            for (int i = 0; i < clients.Count; i++)
                if (!clients[i].client.Connected)
                {
                    clients[i].Disconnect();
                    clients.RemoveAt(i--);
                }
        }

        public void listen()
        {
            listener = new TcpListener(ip, port);
            listener.Start();
            
            listener.BeginAcceptTcpClient(this.OnAcceptConnection,  listener);
        }

        private void OnAcceptConnection(IAsyncResult asyn)
        {
            RemoveDisconnectedClients();

            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)asyn.AsyncState;

            // Get the newly connected TcpClient
            TcpClient client = listener.EndAcceptTcpClient(asyn);

            HttpProcessor processor = new HttpProcessor(client, this);
            var thread = new Thread(new ThreadStart(processor.process ));
            clients.Add(new ClientInfo { client = client, thread = thread });

            thread.Start();
            Thread.Sleep(1);

            listener.BeginAcceptTcpClient(this.OnAcceptConnection, listener);
        }

        public void Stop()
        {
            for (int i = 0; i < clients.Count; i++)
                clients[i].Disconnect();
        }

        public abstract void handleGETRequest(HttpProcessor p);
        public abstract void handlePOSTRequest(HttpProcessor p, StreamReader inputData);
       
    }

    public class MyServer : HttpServer
    {
        public MyServer(IPAddress ip, int port)
            : base(ip, port)
        {
        }

        public Func<HttpProcessor, string> onGET;
        public override void handleGETRequest(HttpProcessor p)
        {
            string userResult = null;
            if (onGET != null) userResult = onGET(p);

            var sb = new StringBuilder();

            //sb.AppendLine("<html><body><h1>test server</h1>");
            //sb.AppendLine("Current Time: " + DateTime.Now.ToString());
            //sb.AppendLine(string.Format("url : {0}", p.http_url));

            //sb.AppendLine("<form method=post action=/form>");
            //sb.AppendLine("<input type=text name=foo value=foovalue>");
            //sb.AppendLine("<input type=submit name=bar value=barvalue>");
            //sb.AppendLine("</form>");
            //sb.AppendLine("</body></html>");

            var ip=p.socket.Client.RemoteEndPoint.ToString();

            sb.AppendLine(string.Format("GET Request from {0}: URL={1}, UserResult={2}", ip, p.http_url, userResult));
            //string param1 = HttpHelper.GetParam(p.http_url, "a"); 

            var s = sb.ToString();
            var L = System.Text.ASCIIEncoding.Unicode.GetByteCount(s);
            p.writeSuccess(s, false);
        }

        public Func<HttpProcessor, string> onPOST;
        public override void handlePOSTRequest(HttpProcessor p, StreamReader inputData)
        {
            string userResult = null;
            if (onPOST != null) userResult = onPOST(p);

            string data = inputData.ReadToEnd();

            var sb = new StringBuilder();

            //sb.AppendLine("<html><body><h1>test server</h1>");
            //sb.AppendLine("<a href=/test>return</a><p>");
            //sb.AppendLine(string.Format("postbody: <pre>{0}</pre>", data));
            //sb.AppendLine("</body></html>");

            sb.AppendLine(string.Format("POST Request from {0}: {1}", ip, data));

            var s = sb.ToString();
            p.writeSuccess(s, false);
        }

    }

}



