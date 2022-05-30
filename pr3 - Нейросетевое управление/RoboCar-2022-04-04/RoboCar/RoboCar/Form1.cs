using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace RoboCar
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics g;
        List<RobCar> robots = new List<RobCar>();
        List<Obj> obstacles = new List<Obj>();

        public Obj GetNearestObst(RobCar r)
        {
            float minD = float.MaxValue;
            Obj res = null;
            for (int i = 0; i < obstacles.Count; i++)
            {
                var dx = obstacles[i].x - r.x;
                var dy = obstacles[i].y - r.y;
                var d = (float)Math.Sqrt(dx * dx + dy * dy);
                if (d < minD) { minD = d; res = obstacles[i]; }
            }
            return res;
        }

        Obj goal = new Obj();


        public string GetIP()
        {
            var ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
            return Dns.GetHostEntry(Dns.GetHostName())
            .AddressList
            .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .ToString();
        }

        MyServer srv;
        private void Form1_Load(object sender, EventArgs e)
        {
            RunServer();
            bt_reset_Click(null, null);

            pb.Image = new Bitmap(pb.Width, pb.Height);
            g = Graphics.FromImage(pb.Image);

            //g.DrawRectangle(Pens.Blue, 30, 30, 100, 50);

            timer1.Enabled = true;
        }

        private void RunServer()
        {
            //192.168.226.199
            tb_ip.Text = GetIP();
            IPAddress ip = IPAddress.Parse(tb_ip.Text);
            if (srv != null) srv.Stop();
            srv = new MyServer(ip, 8080);
            srv.onGET = p =>
            {
                //считываем параметры управления 
                //роботом из http-запроса
                var str_id = HttpHelper.
                    GetParam(p.http_url, "id");
                var str_v = HttpHelper.GetParam(p.http_url, "v");
                var str_w = HttpHelper.GetParam(p.http_url, "w");

                RobCar r = null;

                int ind = -1;
                try
                {
                    ind = int.Parse(str_id);
                    r = robots[ind];
                }
                catch { }

                try
                {
                    r.vlin = float.Parse(str_v, CultureInfo.InvariantCulture);
                }
                catch { }
                try
                {
                    r.vrot = float.Parse(str_w, CultureInfo.InvariantCulture);
                }
                catch { }

                var obst = GetNearestObst(r);
                string info = string.Format(CultureInfo.InvariantCulture,
                    "{0}; {1}; {2}", r.ToString(), obst.ToString(), goal.ToString());

                return info;
            };
            var t = new Thread(new ThreadStart(srv.listen));
            t.Start();
        }

        double t = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int sim = 0; sim < nud_sim_speed.Value; sim++)
            {
                for (int i = 0; i < robots.Count; i++)
                {
                    robots[i].Move();
                }

                for (int i = 0; i < robots.Count; i++)
                {
                    var r = robots[i];
                    var obst = GetNearestObst(r);
                    string info = string.Format(CultureInfo.InvariantCulture,
                    "{0}, {1}, {2}", r.ToString(), obst.ToString(), goal.ToString());
                    if (r.lines.Count == 0 || info != r.lines.Last())
                        r.lines.Add(info);
                }
            }
            
            g.Clear(Color.White);
            for (int i = 0; i < robots.Count; i++)
            {
                robots[i].Draw(g);
            }   
            for (int i = 0; i < obstacles.Count; i++)
            {
                obstacles[i].Draw(g);
            }
            goal.Draw(g);
            pb.Invalidate();


            t += timer1.Interval / 1000.0;

            bool reached = false;
            for (int i = 0; i < robots.Count; i++)
            {
                var dx = robots[i].x - goal.x;
                var dy = robots[i].y - goal.y;
                var d = (float)Math.Sqrt(dx * dx + dy * dy);
                if (d < 50) { reached = true; break; }
            }

            if (reached || cb_auto_change_goal.Checked && t > 30f/(float)nud_sim_speed.Value)
            {
                t = 0;
                int dx = 100, dy = 50, w = pb.Width - 2 * dx, h = pb.Height - 2 * dy;
                Random rnd = new Random(rn_goal++);
                goal = new Obj { 
                    x = rnd.Next(dx, dx + w), 
                    y = rnd.Next(dy, dy + h), 
                    size = 15, q = -1, b = Brushes.Red 
                };
            }
        }
        int rn_goal = 0;

        public class float2
        {
            public float x, y;
            public float2() { }
            public float2(float x, float y) { this.x = x; this.y = y; }

            public float2 Rotate(float ang)
            {
                var v = new float2();
                float s = (float)Math.Sin(ang);
                float c = (float)Math.Cos(ang);
                v.x = x * c - y * s;
                v.y = y * c + x * s;
                return v;
            }
        }

        private void bt_txt_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < robots.Count; i++)
            {
                File.WriteAllLines("lines"+i+".txt", robots[i].lines);
            }
        }

        public class Obj //произвольный объект (цель или препятствие)
        {
            public float x, y, size;
            public float q;//заряд
            public override string ToString()
            {
                return string.Format(
                CultureInfo.InvariantCulture,
                "{0}, {1}, {2}, {3}", x, y, size, q);
            }

            public Brush b = Brushes.Blue;
            public void Draw(Graphics g)
            {
                g.DrawEllipse(Pens.Blue, x - size / 2, y - size / 2, size, size);
                g.FillEllipse(b, x - size / 2, y - size / 2, size, size);
            }
        }

        int rn = 0;
        private void bt_reset_Click(object sender, EventArgs e)
        {
            robots = new List<RobCar>();
            for (int i = 0; i < 10; i++)
            {
                robots.Add(new RobCar { ind = i, x = 100, y = 65 + 65 * i, ang_deg = -45 + i * 10 });
            }

            obstacles = new List<Obj>();
            Random rnd = new Random(rn++);
            int dx = 200, dy = 100, w = pb.Width - 2 * dx, h = pb.Height - 2 * dy;
            for (int i = 0; i < 10; i++)
            {
                obstacles.Add(new Obj { x = rnd.Next(dx, dx + w), y = rnd.Next(dy, dy + h), size = 100, q = 1 });
            }

            goal = new Obj { x = pb.Width - 100, y = pb.Height / 2, size = 15, q = -1, b = Brushes.Red };
        }

        public class RobCar
        {
            public int ind = -1;
            public float L=50, w=30;
            public float x, y, ang_deg;

            public float vrot, vlin;
            public List<string> lines = new List<string>();
            public List<Point> pts = new List<Point>();

            Font f = new Font(FontFamily.GenericSansSerif, 15);
            public void Draw(Graphics g)
            {
                var T = g.Transform;
                g.TranslateTransform(x, y);
                g.RotateTransform(ang_deg);
                g.DrawRectangle(Pens.Blue, -L/2, -w/2, L, w);
                g.DrawRectangle(Pens.Blue, L/3-2, -2, 4, 4);
                g.DrawString("" + ind, f, Brushes.Red, 0, 0);
                
                g.Transform = T;

                for (int i = Math.Max(1, pts.Count-1000); i < pts.Count; i++)
                {
                    g.DrawLine(Pens.Gray, pts[i - 1], pts[i]);
                }
            }

            public void Move(float v, float w)
            {
                this.vlin = v; this.vrot = w;
                Move();
            }
            public void Move()
            {
                var vec = new float2(1, 0).Rotate(ang_deg / 180 * (float)Math.PI);
                x += vlin * vec.x;
                y += vlin * vec.y;

                ang_deg += vrot;

                while (ang_deg > 180) ang_deg -= 360;
                while (ang_deg < -180) ang_deg += 360;

                var pt = new Point((int)x, (int)y);
                if (pts.Count == 0) pts.Add(pt);
                else {
                    var dx = pts.Last().X - x;
                    var dy = pts.Last().Y - y;
                    if (dx * dx + dy * dy < 10 * 10) pts.Add(pt);
                }
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}, {3}, {4}", x, y, ang_deg, vrot, vlin);
            }
        }

        private void bt_folder_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", ".");
        }
    }
}
