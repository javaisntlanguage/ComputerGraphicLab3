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
        bool flag;
        Graphics g;
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            flag = false;

        }

        private void PutPixel(Color color, int x, int y) //рисовать пиксель
        {
            bmp.SetPixel(x,y,color);
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

        private void DrawRectangle(Color clr, Point point1, Point point2) //рисовать прямоугольник
        {
            Bresenham4Line(clr, point1.X, point1.Y, point2.X, point1.Y);//верх
            Bresenham4Line(clr, point1.X, point2.Y, point2.X, point2.Y);//низ
            Bresenham4Line(clr, point1.X, point1.Y, point1.X, point2.Y);//лево
            Bresenham4Line(clr, point2.X, point1.Y, point2.X, point2.Y);//право
        }

        void Pixel4(int x, int y, int _x, int _y, Color color) // Рисование пикселя для первого квадранта, и, симметрично, для остальных
        {
            PutPixel(color, x + _x, y + _y);
            PutPixel(color, x + _x, y - _y);
            PutPixel(color, x - _x, y - _y);
            PutPixel(color, x - _x, y + _y);
        }

        void DrawElipse(int x, int y, int a, int b, Color color)
        {
            int _x = 0; // Компонента x
            int _y = b; // Компонента y
            int a_sqr = a * a; // a^2, a - большая полуось
            int b_sqr = b * b; // b^2, b - малая полуось
            int delta = 4 * b_sqr * ((_x + 1) * (_x + 1)) + a_sqr * ((2 * _y - 1) * (2 * _y - 1)) - 4 * a_sqr * b_sqr; // Функция координат точки (x+1, y-1/2)
            while (a_sqr * (2 * _y - 1) > 2 * b_sqr * (_x + 1)) // Первая часть дуги
            {
                Pixel4(x, y, _x, _y, color);
                if (delta < 0) // Переход по горизонтали
                {
                    _x++;
                    delta += 4 * b_sqr * (2 * _x + 3);
                }
                else // Переход по диагонали
                {
                    _x++;
                    delta = delta - 8 * a_sqr * (_y - 1) + 4 * b_sqr * (2 * _x + 3);
                    _y--;
                }
            }
            delta = b_sqr * ((2 * _x + 1) * (2 * _x + 1)) + 4 * a_sqr * ((_y + 1) * (_y + 1)) - 4 * a_sqr * b_sqr; // Функция координат точки (x+1/2, y-1)
            while (_y + 1 != 0) // Вторая часть дуги, если не выполняется условие первого цикла, значит выполняется a^2(2y - 1) <= 2b^2(x + 1)
            {
                Pixel4(x, y, _x, _y, color);
                if (delta < 0) // Переход по вертикали
                {
                    _y--;
                    delta += 4 * a_sqr * (2 * _y + 3);
                }
                else // Переход по диагонали
                {
                    _y--;
                    delta = delta - 8 * b_sqr * (_x + 1) + 4 * a_sqr * (2 * _y + 3);
                    _x++;
                }
            }
        }

        private void pictureBox1_DownClick(object sender, MouseEventArgs e) //зажатие ЛКМ
        {
            if (!flag)//если нет рисунка
            {
                point1 = new Point(e.X, e.Y); //Сохраняем точку
                
            }
            

        }

        private void pictureBox1_UpClick(object sender, MouseEventArgs e)//Отжатие ЛКМ
        {
            if (!flag)//если нет рисунка
            {
                pictureBox1.Image = bmp; //отображаем
                g = Graphics.FromImage(pictureBox1.Image);
                flag = true;//рисунок есть
                point2 = new Point(e.X, e.Y);//сохраняем вторую точку
                
                DrawRectangle(Color.Black, point1, point2);//рисуем прямоугольник
                var cx = Convert.ToInt32(Math.Abs((point2.X + point1.X) / 2));//абсцисса центра 
                var cy = Convert.ToInt32(Math.Abs((point2.Y + point1.Y) / 2));//ордината центра
                var circleCenter = new Point(cx, cy);//сохраняем центр
                var circleLenghtY = Math.Abs(point1.Y - circleCenter.Y);//длина элипса по оси ординат
                var circleLenghtX = Math.Abs(point1.X - circleCenter.X);//длина элипса по оси абсцисс
                DrawElipse(circleCenter.X, circleCenter.Y, circleLenghtX, circleLenghtY, Color.Black);//рисуем элипс
            }
            else
            {
                g.Clear(Color.White);//очищаем область
                pictureBox1.Image = bmp; //отображаем
                flag = false;//рисунка нет
            }

        }

    }
}
