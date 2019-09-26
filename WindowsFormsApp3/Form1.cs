using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        Point point1;
        Point point2;
        List<Point> Coordinates;
        bool flag;
        Graphics g;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            flag = false;
            Coordinates = new List<Point>();

        }

        private void PutPixel(Color color, int x, int y) //рисовать пиксель
        {
            if (x > 0 && x < bmp.Width && y > 0 && y < bmp.Height) bmp.SetPixel(x, y, color);
        }

        public void Bresenham4Line(Color clr, int x0, int y0,int x1, int y1)
        {
            //Изменения координат
            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
            //Направление приращения
            int sx = (x1 >= x0) ? (1) : (-1);
            int sy = (y1 >= y0) ? (1) : (-1);

            if (dy < dx)
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                PutPixel(clr, x0, y0);
                int x = x0 + sx;
                int y = y0;
                for (int i = 1; i <= dx; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        y += sy;
                    }
                    else
                        d += d1;
                    PutPixel(clr, x, y);
                    x += sx;
                }
            }
            else
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                PutPixel(clr, x0, y0);
                int x = x0;
                int y = y0 + sy;
                for (int i = 1; i <= dy; i++)
                {
                    if (d > 0)
                    {
                        d += d2;
                        x += sx;
                    }
                    else
                        d += d1;
                    PutPixel(clr, x, y);
                    y += sy;
                }
            }
        }

        int OutCode(int x, int y, int X1, int Y1, int X2, int Y2) //определить, с какой стороны от прямоугольника точка
        {
            int code = 0;
            if (x < X1) code |= 0x01;//слева
            if (y < Y1) code |= 0x02;//сверху
            if (x > X2) code |= 0x04;//справа
            if (y > Y2) code |= 0x08;//снизу
            return code;
        }

        void Swap(ref int a,ref int b) //поменять переменные местами
        {
            a += b;
            b = a - b;
            a -=b;
        }

        void ClipLine(int x1, int y1, int x2, int y2, int X1, int Y1, int X2, int Y2)
        {
            int code1 = OutCode(x1, y1, X1, Y1, X2, Y2);
            int code2 = OutCode(x2, y2, X1, Y1, X2, Y2);
            bool inside = (code1 | code2) == 0;//внутри
            bool outside = (code1 & code2) != 0;//снаружи
            if (!outside)
            {
                while (!outside && !inside)
                {
                    if (code1 == 0)
                    {
                        Swap(ref x1, ref x2);
                        Swap(ref y1, ref y2);
                        Swap(ref code1, ref code2);
                    }
                    byte res = (byte)code1;
                    if ( (res & 0x01) == 0x01) //слева
                    {
                        y1 += (y2 - y1) * (X1 - x1) / (x2 - x1);
                        x1 = X1;
                    }
                    if ( (res & 0x02) == 0x02)//сверху
                    {
                        x1 += (x2 - x1) * (Y1 - y1) / (y2 - y1);
                        y1 = Y1;
                    }
                    if ( (res & 0x04) == 0x04)//справа
                    {
                        y1 += (y2 - y1) * (X2 - x1) / (x2 - x1);
                        x1 = X2;
                    }
                    if ( (res & 0x08) == 0x08)//снизу
                    {
                        x1 += (x2 - x1) * (Y2 - y1) / (y2 - y1);
                        y1 = Y2;
                    }

                    code1 = OutCode(x1, y1, X1, Y1, X2, Y2);
                    code2 = OutCode(x2, y2, X1, Y1, X2, Y2);
                    outside = (code1 & code2) != 0;
                    inside = (code1 | code2) == 0;
                }

                Bresenham4Line(Color.Blue, x1, y1, x2, y2);
            }
        }

        private void DrawRectangle(Color clr, Point point1, Point point2) //рисовать прямоугольник
        {
            Bresenham4Line(clr, point1.X, point1.Y, point2.X, point1.Y);//верх
            Bresenham4Line(clr, point1.X, point2.Y, point2.X, point2.Y);//низ
            Bresenham4Line(clr, point1.X, point1.Y, point1.X, point2.Y);//лево
            Bresenham4Line(clr, point2.X, point1.Y, point2.X, point2.Y);//право
        }

        private void pictureBox1_DownClick(object sender, MouseEventArgs e) //зажатие клавиши мыши
        {
            if (e.Button == MouseButtons.Right)
            {
                    point1 = new Point(e.X, e.Y); //Сохраняем точку
            }

        }

        private void pictureBox1_UpClick(object sender, MouseEventArgs e)//Отжатие клавиши мыши
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Coordinates.Count > 0)
                {
                    pictureBox1.Image = bmp; //отображаем
                    if (!flag)//если нет рисунка
                    {
                        g = Graphics.FromImage(pictureBox1.Image);
                        flag = true;//рисунок есть
                        point2 = new Point(e.X, e.Y);//сохраняем вторую точку

                        DrawRectangle(Color.Black, point1, point2);//рисуем прямоугольник
                        int X1 = point1.X;
                        int Y1 = point1.Y;
                        int X2 = point2.X;
                        int Y2 = point2.Y;
                        if (X1 > X2) Swap(ref X1, ref X2);
                        if (Y1 > Y2) Swap(ref Y1, ref Y2);
                        for (int i = 0; i < Coordinates.Count - 1; i++)
                        {
                            Bresenham4Line(Color.Red, Coordinates[i].X, Coordinates[i].Y, //соединяет 2 последние точки
                                       Coordinates[i + 1].X, Coordinates[i + 1].Y);
                            ClipLine(Coordinates[i].X, Coordinates[i].Y, Coordinates[i + 1].X, Coordinates[i + 1].Y, X1, Y1, X2, Y2);
                        }
                    }
                    else
                    {
                        g.Clear(Color.White);//очищаем область
                                             //pictureBox1.Image = bmp; //отображаем
                        flag = false;//рисунка нет
                        pictureBox1_UpClick(sender, e);
                    }
                }
            }
            else if(e.Button == MouseButtons.Left)
            {
                var p = new Point(e.X, e.Y);
                Coordinates.Add(p);//добавляем в листок с точками
                if (Coordinates.Count == 1)//если 1 точка
                {
                    PutPixel(Color.Black,e.X, e.Y);//делаем пиксель черным
                } 
                else //если есть точки
                {
                    Bresenham4Line(Color.Red, Coordinates[Coordinates.Count-1].X, Coordinates[Coordinates.Count-1].Y, //соединяет 2 последние точки
                                   Coordinates[Coordinates.Count - 2].X, Coordinates[Coordinates.Count - 2].Y);
                }
            }
            pictureBox1.Image = bmp; //отображаем
        }

        private void pictureBox1_Click(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Space)
            {
                Coordinates.Clear();
                g.Clear(Color.White);//очищаем область
                pictureBox1.Image = bmp; //отображаем

            }
        }
    }
}
