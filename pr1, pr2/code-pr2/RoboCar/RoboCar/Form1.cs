using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
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
        RobCar r1 = new RobCar(), r2 = new RobCar();

        Obj obst = new Obj();
        Obj goal = new Obj();
        private void Form1_Load(object sender, EventArgs e)
        {

            bt_reset_Click(null, null);

            pb.Image = new Bitmap(pb.Width, pb.Height);
            g = Graphics.FromImage(pb.Image);

           
            //g.DrawRectangle(Pens.Blue, 30, 30, 100, 50);

            timer1.Enabled = true;

            var srv = new MyServer(IPAddress.Loopback, 8080);
            srv.onGET = p =>
            {
                //считываем параметры управления 
                //роботом из http-запроса
                var str_id = HttpHelper.
                    GetParam(p.http_url, "id");
                var str_v = HttpHelper.GetParam(p.http_url, "v");
                var str_w = HttpHelper.GetParam(p.http_url, "w");

                RobCar r = null;
                if (str_id == "0") r = r1;
                if (str_id == "1") r = r2;

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

                string info = string.Format(CultureInfo.InvariantCulture, 
                    "{0}; {1}; {2}", r.ToString(), obst.ToString(), goal.ToString());

                return info;
            };
            var t = new Thread(new ThreadStart(srv.listen));
            t.Start();


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            r1.Move();// 1, 3);
            r2.Move();// 2, -3);

            g.Clear(Color.White);
            r1.Draw(g);
            //r2.Draw(g);
            obst.Draw(g);
            goal.Draw(g);
            pb.Invalidate();

            lines.Add(r1.ToString() + "; " + r2.ToString());
        }

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

        List<string> lines=new List<string>();
        private void bt_txt_Click(object sender, EventArgs e)
        {
            File.WriteAllLines("lines.txt", lines);
        }

        public class Obj //произвольный объект (цель или препятствие)
        {
            public float x, y, size;
            public float q, m;//заряд и масса
            public override string ToString()
            {
                return string.Format(
                CultureInfo.InvariantCulture,
                "{0}, {1}, {2}, {3}, {4}", x, y, size, q, m);
            }

            public void Draw(Graphics g)
            {
                g.DrawEllipse(Pens.Blue, x - size / 2, y - size / 2, size, size);
            }
        }

        private void bt_reset_Click(object sender, EventArgs e)
        {
            r1 = new RobCar { x = 300, y = 200, ang_deg = 0 };
           r2 = new RobCar { x = 300, y = 150, ang_deg = 45 };

            obst = new Obj { x = 450, y = 220, size = 100, m = float.MaxValue, q = 1 };
            goal = new Obj { x = 600, y = 250, size = 15, m = float.MaxValue, q = -1 };
        }

        public class RobCar
        {
            public float L=50, w=30;
            public float x, y, ang_deg;

            public float vrot, vlin;

            public void Draw(Graphics g)
            {
                var T = g.Transform;
                g.TranslateTransform(x, y);
                g.RotateTransform(ang_deg);
                g.DrawRectangle(Pens.Blue, -L/2, -w/2, L, w);
                g.DrawRectangle(Pens.Blue, L/3-2, -2, 4, 4);
                
                g.Transform = T;
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
            }

            public override string ToString()
            {
                return string.Format(CultureInfo.InvariantCulture, "{0}, {1}, {2}", x, y, ang_deg);
            }
        }
    }
}
