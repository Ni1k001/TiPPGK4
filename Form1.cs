using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TiPPGK4
{
    public partial class Form1 : Form
    {
        private readonly Graphics _graphics;
        private Bitmap _bmp = null;
        private Timer timer;

        //PolygonSprite r1, r2, t1;
        PolygonSprite player;
        PolygonSprite[] polygons;

        public Form1()
        {
            InitializeComponent();

            _bmp = new Bitmap(1280, 720);
            _graphics = Graphics.FromImage(_bmp);
            pictureBox1.Image = _bmp;

            int count = 40;
            Random random = new Random();

            polygons = new PolygonSprite[count];
            for (int i = 0; i < count; i++)
                polygons[i] = new PolygonSprite(random.Next(0, 1180), random.Next(0, 620), random.Next(30, 100), random.Next(30, 100), random.Next(3, 5), Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255)));

            //r1 = new PolygonSprite(150, 10, 100, 100, 4);
            //r2 = new PolygonSprite(260, 70, 20, 20, 4);
            //t1 = new PolygonSprite(70, 100, 100, 100 * (float)Math.Sqrt(3) / 2 , 3);

            player = new PolygonSprite(random.Next(0, 1180), random.Next(0, 620), random.Next(30, 100), random.Next(30, 100), random.Next(3, 7), Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255)));

            timer = new Timer();
            timer.Interval = 16;
            timer.Tick += OnTick;
            timer.Start();
            Invalidate();
        }

        private void OnTick(object sender, EventArgs e)
        {
            _graphics.Clear(Color.White);

            using (Graphics g = Graphics.FromImage(_bmp))
            {
                foreach (PolygonSprite p in polygons)
                {
                    p.Draw(g);
                    p.GetCollision(player);
                }
                player.Draw(g);
            }

            Refresh();
            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                player.Move(sender, e, 10);
        }

        public void DrawPoints(PolygonSprite p1, PolygonSprite p2)
        {
            p1.CheckCollision(p2);

            for (int i = 0; i < p1.vertices; i++)
            {
                if (i == 0)
                {
                    for (int j = 0; j < p1.vertices; j++)
                        _graphics.DrawRectangle(Pens.Red, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].X, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].Y, 1, 1);
                    for (int j = 0; j < p2.vertices; j++)
                        _graphics.DrawRectangle(Pens.Aqua, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].X, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].Y, 1, 1);
                }
                else if (i == 1)
                {
                    for (int j = 0; j < p1.vertices; j++)
                        _graphics.DrawRectangle(Pens.Green, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].X, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].Y, 1, 1);
                    for (int j = 0; j < p2.vertices; j++)
                        _graphics.DrawRectangle(Pens.Orange, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].X, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].Y, 1, 1);
                }
                else if (i == 2)
                {
                    for (int j = 0; j < p1.vertices; j++)
                        _graphics.DrawRectangle(Pens.Brown, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].X, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].Y, 1, 1);
                    for (int j = 0; j < p2.vertices; j++)
                        _graphics.DrawRectangle(Pens.Violet, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].X, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].Y, 1, 1);
                }
                else if (i == 3)
                {
                    for (int j = 0; j < p1.vertices; j++)
                        _graphics.DrawRectangle(Pens.Blue, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].X, p1.shadows[j + (i * p1.vertices) + (i * p2.vertices)].Y, 1, 1);
                    for (int j = 0; j < p2.vertices; j++)
                        _graphics.DrawRectangle(Pens.DarkSeaGreen, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].X, p1.shadows[p1.vertices + j + (i * p2.vertices) + (i * p1.vertices)].Y, 1, 1);
                }
            }
        }
    }

    public class PolygonSprite
    {
        public float x, y, width, height;
        public int vertices;
        Brush brush;
        PointF[] points;
        bool isColliding = false;
        double[][] normalVector;
        public PointF[] shadows;

        public PolygonSprite(float x, float y, float width, float height, int vertices, Color color)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.vertices = vertices;

            normalVector = new double[vertices][];
            for (int i = 0; i < vertices; i++)
                normalVector[i] = new double[2];

            if (vertices == 3)
            {
                points = new PointF[3];
                
                points[0].X = x;
                points[0].Y = y + height;

                points[1].X = x + width / 2;
                points[1].Y = y;

                points[2].X = x + width;
                points[2].Y = y + height;


            }
            else if (vertices == 4)
            {
                points = new PointF[4];
                
                points[0].X = x;
                points[0].Y = y;

                points[1].X = x + width;
                points[1].Y = y;

                points[2].X = x + width;
                points[2].Y = y + height;

                points[3].X = x;
                points[3].Y = y + height;
            }
            else if (vertices == 5)
            {
                points = new PointF[5];

                points[0].X = x + width / 2;
                points[0].Y = y;

                points[1].X = x + width;
                points[1].Y = y + height / 2.5f;

                points[2].X = x + width - width / 5;
                points[2].Y = y + height;

                points[3].X = x + width / 5;
                points[3].Y = y + height;

                points[4].X = x;
                points[4].Y = y + height / 2.5f;
            }
            else if (vertices == 6)
            {
                points = new PointF[6];

                points[0].X = x;
                points[0].Y = y + height / 2;

                points[1].X = x + width / 4;
                points[1].Y = y;

                points[2].X = x + width - width / 4;
                points[2].Y = y;

                points[3].X = x + width;
                points[3].Y = y + height / 2;

                points[4].X = x + width - width / 4;
                points[4].Y = y + height;

                points[5].X = x + width / 4;
                points[5].Y = y + height;
            }

            brush = new SolidBrush(color);
        }

        public void Draw(Graphics g)
        {
            g.DrawPolygon(Pens.Black, points);
            g.FillPolygon(brush, points);
            if (isColliding)
            {
                g.FillPolygon(Brushes.Black, points);
            }
        }

        public void Update()
        {
            if (vertices == 3)
            {
                points = new PointF[3];
                points[0].X = x;
                points[0].Y = y + height;

                points[1].X = x + width / 2;
                points[1].Y = y;

                points[2].X = x + width;
                points[2].Y = y + height;


            }
            else if (vertices == 4)
            {
                points = new PointF[4];
                points[0].X = x;
                points[0].Y = y;

                points[1].X = x + width;
                points[1].Y = y;

                points[2].X = x + width;
                points[2].Y = y + height;

                points[3].X = x;
                points[3].Y = y + height;
            }
            else if (vertices == 5)
            {
                points = new PointF[5];

                points[0].X = x + width / 2;
                points[0].Y = y;

                points[1].X = x + width;
                points[1].Y = y + height / 2.5f;

                points[2].X = x + width - width / 5;
                points[2].Y = y + height;

                points[3].X = x + width / 5;
                points[3].Y = y + height;

                points[4].X = x;
                points[4].Y = y + height / 2.5f;
            }
            else if (vertices == 6)
            {
                points = new PointF[6];

                points[0].X = x;
                points[0].Y = y + height / 2;

                points[1].X = x + width / 4;
                points[1].Y = y;

                points[2].X = x + width - width / 4;
                points[2].Y = y;

                points[3].X = x + width;
                points[3].Y = y + height / 2;

                points[4].X = x + width - width / 4;
                points[4].Y = y + height;

                points[5].X = x + width / 4;
                points[5].Y = y + height;
            }
        }

        public void GetCollision(PolygonSprite other)
        {
            if (CheckCollision(other))
            {
                //Debug.WriteLine("1. TRUE");
                if (other.CheckCollision(this))
                {
                    //Debug.WriteLine("2. TRUE");
                    isColliding = true;
                }
                else
                {
                    //Debug.WriteLine("2. FALSE");
                    isColliding = false;
                }
            }
            else
            {
                //Debug.WriteLine("1. FALSE");
                isColliding = false;
            }
        }
        public bool CheckCollision(PolygonSprite other)
        {
            double a, b, x, y;
            bool[] ctrl = new bool[vertices * 2];

            shadows = new PointF[(vertices + other.vertices) * vertices];

            for (int i = 0; i < vertices; i++)
            {
                normalVector[i][0] = (points[(i + 1) % vertices].Y - points[i].Y) * -1;
                normalVector[i][1] = (points[(i + 1) % vertices].X - points[i].X);

                if (points[(i + 1) % vertices].Y == points[i].Y)
                    a = normalVector[i][1];
                else
                    //a = (double)(points[i].Y - points[(i + 1) % vertices].Y) / (points[i].X - points[(i + 1) % vertices].X);
                    a = (double)(points[(i + 1) % vertices].X - points[i].X) / (points[(i + 1) % vertices].Y - points[i].Y) * -1;
                
                b =  (double)points[i].Y - (a * points[i].X);

                for (int j = 0; j < vertices; j++)
                {
                    x = (a * points[j].Y + points[j].X - a * b) / (a * a + 1);
                    y = (a * a * points[j].Y + a * points[j].X + b) / (a * a + 1);

                    shadows[j + (i * vertices) + (i * other.vertices)].X = (float)x;
                    shadows[j + (i * vertices) + (i * other.vertices)].Y = (float)y;
                }

                for (int j = 0; j < other.vertices; j++)
                {
                    x = (a * other.points[j].Y + other.points[j].X - a * b) / (a * a + 1);
                    y = (a * a * other.points[j].Y + a * other.points[j].X + b) / (a * a + 1);

                    shadows[vertices + j + (i * other.vertices) + (i * vertices)].X = (float)x;
                    shadows[vertices + j + (i * other.vertices) + (i * vertices)].Y = (float)y;
                }

                /*for (int j = 0; j < vertices; j++)
                {
                    Debug.WriteLine("1. Punkt[" + j + "]:\tx: " + points[j].X + "\ty: " + points[j].Y);
                    Debug.WriteLine("\t\t\tx: " + shadows[j + (i * vertices) + (i * player.vertices)].X + "\ty: " + shadows[j + (i * vertices) + (i * player.vertices)].Y);
                }

                for (int j = 0; j < player.vertices; j++)
                {
                    Debug.WriteLine("2. Punkt[" + (vertices + j) + "]:\tx: " + player.points[j].X + "\ty: " + player.points[j].Y);
                    Debug.WriteLine("\t\t\tx: " + shadows[vertices + j + (i * player.vertices) + (i * vertices)].X + "\ty: " + shadows[vertices + j + (i * player.vertices) + (i * vertices)].Y);
                }*/

                double min1X, min1Y, max1X, max1Y;

                min1X = shadows[(i * vertices) + (i * other.vertices)].X;
                min1Y = shadows[(i * vertices) + (i * other.vertices)].Y;
                max1X = shadows[(i * vertices) + (i * other.vertices)].X;
                max1Y = shadows[(i * vertices) + (i * other.vertices)].Y;

                double min2X, min2Y, max2X, max2Y;

                min2X = shadows[vertices + (i * other.vertices) + (i * vertices)].X;
                min2Y = shadows[vertices + (i * other.vertices) + (i * vertices)].Y;
                max2X = shadows[vertices + (i * other.vertices) + (i * vertices)].X;
                max2Y = shadows[vertices + (i * other.vertices) + (i * vertices)].Y;

                for (int j = 0; j < vertices; j++)
                {
                    min1X = Math.Min(min1X, shadows[j + (i * vertices) + (i * other.vertices)].X);
                    min1Y = Math.Min(min1Y, shadows[j + (i * vertices) + (i * other.vertices)].Y);
                    max1X = Math.Max(max1X, shadows[j + (i * vertices) + (i * other.vertices)].X);
                    max1Y = Math.Max(max1Y, shadows[j + (i * vertices) + (i * other.vertices)].Y);
                }

                for (int j = 0; j < other.vertices; j++)
                {
                    min2X = Math.Min(min2X, shadows[vertices + j + (i * other.vertices) + (i * vertices)].X);
                    min2Y = Math.Min(min2Y, shadows[vertices + j + (i * other.vertices) + (i * vertices)].Y);
                    max2X = Math.Max(max2X, shadows[vertices + j + (i * other.vertices) + (i * vertices)].X);
                    max2Y = Math.Max(max2Y, shadows[vertices + j + (i * other.vertices) + (i * vertices)].Y);
                }

                /*Debug.WriteLine((i * vertices + i * other.vertices));
                Debug.WriteLine((vertices + i * other.vertices + i * vertices));
                Debug.WriteLine("a: " + a + "\tb: " + b);
                Debug.WriteLine("");
                Debug.WriteLine("min1X: " + min1X + "\tmin1Y: " + min1Y + "\tmax1X: " + max1X + "\tmax1Y: " + max1Y);
                Debug.WriteLine("min2X: " + min2X + "\tmin2Y: " + min2Y + "\tmax2X: " + max2X + "\tmax2Y: " + max2Y);
                */
                //Debug.WriteLine("");
                if (min2X <= max1X && max2X >= min1X)
                {
                    //Debug.WriteLine("Dla X jest cien");
                    ctrl[i * 2] = true;
                }
                else
                {
                    //Debug.WriteLine("Dla X nie ma cienia");
                    ctrl[i * 2] = false;
                }
                
                if (min2Y <= max1Y && max2Y >= min1Y)
                {
                    //Debug.WriteLine("Dla Y jest cien");
                    ctrl[1 + i * 2] = true;
                }
                else
                {
                    //Debug.WriteLine("Dla Y nie ma cienia");
                    ctrl[1 + i * 2] = false;
                }

                /*if (min2Y <= max1Y && max2Y >= min1Y)
                {
                    Debug.WriteLine("1. Dla Y jest cien");
                }
                else if (min2Y >= max1Y && max2Y <= min1Y)
                {
                    Debug.WriteLine("2. Dla Y jest cien");
                }
                else
                {
                    Debug.WriteLine("Dla Y nie ma cienia");
                }*/
            }

            for (int i = 0; i < ctrl.Length; i++)
                if (!ctrl[i])
                    return false;
            return true;
        }

        public void Move(object sender, KeyEventArgs e, int speed)
        {
            if (e.KeyCode == Keys.Left && x > 0)
                x -= speed;
            else if (e.KeyCode == Keys.Right && x < 1260)
                x += speed;
            else if (e.KeyCode == Keys.Up && y > 0)
                y -= speed;
            else if (e.KeyCode == Keys.Down && y < 700)
                y += speed;

            Update();
        }
    }
}
