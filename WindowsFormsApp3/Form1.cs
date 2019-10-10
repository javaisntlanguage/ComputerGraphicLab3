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
        Bitmap bmp;//определение объекта для будущего создания области для рисования
        Point point1;//точка при зажатой клавише мыши (начало прямоугольника)
        Point point2;//точка при отжатии клавиши мыши (конец прямоугольника)
        List<Point> Coordinates;//определение листа для координат ломаной
        bool flag;//флаг существования рисунка
        Graphics g;//объект графики (нужен потом для очищения рисунка)
        public Form1()
        {
            InitializeComponent();
            bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);//установка размеров поля для рисования 
            flag = false;//рисунка нет
            Coordinates = new List<Point>();//реализация (присвоение переменной) листа для координат ломаной

        }

        private void PutPixel(Color color, int x, int y) //функция, которая будет рисовать пиксель
        {
            if (x > 0 && x < bmp.Width && y > 0 && y < bmp.Height) bmp.SetPixel(x, y, color); //рисует пиксель, если он не выходит
                                                                                              //за границы области рисования
        }

        public void Bresenham4Line(Color clr, int x0, int y0, int x1, int y1)//функция рисования линии
        {
            //Изменения координат
            int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1); //длина линии по x
            int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);//длина линии по y
            //Направление приращения
            int sx = (x1 >= x0) ? (1) : (-1);//будем двигаться вправо или влево в зависимости от направления линии
            int sy = (y1 >= y0) ? (1) : (-1);//будем двигаться вниз или вверх в зависимости от направления линии

            if (dy < dx) //если угол между линией и горизонталью меньше 45 градусов
            {
                int d = (dy << 1) - dx;
                int d1 = dy << 1;
                int d2 = (dy - dx) << 1;
                PutPixel(clr, x0, y0);//рисование пикселя
                int x = x0 + sx;
                int y = y0;
                for (int i = 1; i <= dx; i++)//двигаемся по горизонтали
                {
                    if (d > 0)//если точка между пикселями ближе к следующему по вертикали, чем к текущему, изменяем точку на 1 по y
                    {
                        d += d2;
                        y += sy;
                    }
                    else //если точка между пикселями ближе к текущему по вертикали, оставляем Y таким же
                        d += d1;
                    PutPixel(clr, x, y);//рисование пикселя
                    x += sx;//изменяем x на 1
                }
            }
            else//если угол между линией и горизонталью больше 45 градусов
            {
                int d = (dx << 1) - dy;
                int d1 = dx << 1;
                int d2 = (dx - dy) << 1;
                PutPixel(clr, x0, y0);//рисование пикселя
                int x = x0;
                int y = y0 + sy;
                for (int i = 1; i <= dy; i++)//двигаемся по вертикали
                {
                    if (d > 0)//если точка между пикселями ближе к следующему по горизонтали, чем к текущему, изменяем точку на 1 по х
                    {
                        d += d2;
                        x += sx;
                    }
                    else //если точка между пикселями ближе к текущему по горизонтали, оставляем х таким же
                        d += d1;
                    PutPixel(clr, x, y);//рисование пикселя
                    y += sy;//изменяем y на 1
                }
            }
        }

        int OutCode(double x, double y, int X1, int Y1, int X2, int Y2) //функция определяет, с какой стороны от прямоугольника точка
        {
            int code = 0;//внутри
            if (x < X1) code |= 0x01;//слева
            if (y < Y1) code |= 0x02;//сверху
            if (x > X2) code |= 0x04;//справа
            if (y > Y2) code |= 0x08;//снизу
            return code;
        }

        void Swap<T>(ref T a, ref T b) //поменять переменные местами
        {
            T c = a;
            a = b;
            b = c;
        }

        void ClipLine(double x1, double y1, double x2, double y2, int X1, int Y1, int X2, int Y2)//рисование линии внутри прямоугольника
        {
            int code1 = OutCode(x1, y1, X1, Y1, X2, Y2);//определение положения начала ломаной относительно прямоугольника
            int code2 = OutCode(x2, y2, X1, Y1, X2, Y2);//определение положения конца ломаной относительно прямоугольника
            bool inside = (code1 | code2) == 0;//оба конца внутри
            bool outside = (code1 & code2) != 0;//оба конца снаружи
            if (!outside)//если не снаружи
            {
                while (!outside && !inside)//пока не внутри и не снаружи
                {
                    if (code1 == 0)//если первая точка внутри
                    {
                        Swap(ref x1, ref x2);//меняем координаты начала 
                        Swap(ref y1, ref y2);//и конца отрезка местами
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

                    code1 = OutCode(x1, y1, X1, Y1, X2, Y2);//пересчитываем положения начала ломаной относительно прямоугольника 
                    code2 = OutCode(x2, y2, X1, Y1, X2, Y2);//пересчитываем положения конца ломаной относительно прямоугольника
                    inside = (code1 | code2) == 0;//оба конца внутри
                    outside = (code1 & code2) != 0;//оба конца снаружи             
                }
                if(inside)//если после отсечения вся линия внутри прямоугольника, то рисуем ее
                Bresenham4Line(Color.Blue, (int)Math.Round(x1), (int)Math.Round(y1), (int)Math.Round(x2), (int)Math.Round(y2));
            }
        }

        private void DrawRectangle(Color clr, Point point1, Point point2) //рисовать прямоугольник
        {
            Bresenham4Line(clr, point1.X, point1.Y, point2.X, point1.Y);//верхняя линия
            Bresenham4Line(clr, point1.X, point2.Y, point2.X, point2.Y);//нижняя линия
            Bresenham4Line(clr, point1.X, point1.Y, point1.X, point2.Y);//левая линия
            Bresenham4Line(clr, point2.X, point1.Y, point2.X, point2.Y);//правая линия
        }

        private void pictureBox1_DownClick(object sender, MouseEventArgs e) //зажатие клавиши мыши
        {
            if (e.Button == MouseButtons.Right)//если пкм
            {
                    point1 = new Point(e.X, e.Y); //Сохраняем точку
            }

        }

        private void pictureBox1_UpClick(object sender, MouseEventArgs e)//Отжатие клавиши мыши
        {
            if (e.Button == MouseButtons.Right)//если пкм
            {
                if (Coordinates.Count > 0)//если уже существуют точки
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
                        if (X1 > X2) Swap(ref X1, ref X2);//если рисуем прямоугольник справа налево
                        if (Y1 > Y2) Swap(ref Y1, ref Y2);//если рисуем прямоугольник снизу вверх
                        for (int i = 0; i < Coordinates.Count - 1; i++)//проходим по всем точкам ломаной
                        {
                            Bresenham4Line(Color.Red, Coordinates[i].X, Coordinates[i].Y, //соединяет 2 последние точки
                                       Coordinates[i + 1].X, Coordinates[i + 1].Y);
                            ClipLine(Coordinates[i].X, Coordinates[i].Y, Coordinates[i + 1].X, Coordinates[i + 1].Y, X1, Y1, X2, Y2);
                                //рисуем линию внутри прямоугольника другим цветом
                        }
                    }
                    else//если есть рисунок
                    {
                        g.Clear(Color.White);//очищаем область
                                             //pictureBox1.Image = bmp; //отображаем
                        flag = false;//рисунка нет
                        pictureBox1_UpClick(sender, e);//рекурсивно вызываем эту же функцию отжатия кнопка мыши сначала (теперь рисунка нет)
                    }
                }
            }
            else if(e.Button == MouseButtons.Left)//если лкм
            {
                var p = new Point(e.X, e.Y);//создаем точку в месте нажатия
                Coordinates.Add(p);//добавляем в листок с точками
                if (Coordinates.Count == 1)//если 1 точка
                {
                    PutPixel(Color.Black,e.X, e.Y);//делаем пиксель черным
                } 
                else //если есть точки
                {
                    Bresenham4Line(Color.Red, Coordinates[Coordinates.Count-1].X, Coordinates[Coordinates.Count-1].Y, 
                                   Coordinates[Coordinates.Count - 2].X, Coordinates[Coordinates.Count - 2].Y);//соединяет 2 последние точки
                }
            }
            pictureBox1.Image = bmp; //отображаем
        }

        private void pictureBox1_Click(object sender, KeyEventArgs e)//событие нажатия клавиши на клавиатуре
        {
            if(e.KeyData == Keys.Space)//если нажат пробел
            {
                Coordinates.Clear();//очищаем лист с координатами точек ломаной
                g.Clear(Color.White);//очищаем область
                pictureBox1.Image = bmp; //отображаем

            }
        }
    }
}
