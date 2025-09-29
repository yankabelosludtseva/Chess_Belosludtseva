using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace пр_4.Classes
{
    // класс для пешки в шахматах
    public class Pawn
    {
        // координата x на доске
        public int X { get; set; }

        // координата y на доске
        public int Y { get; set; }

        // выбрана ли фигура для хода
        public bool Select = false;

        // цвет фигуры (true - черная, false - белая)
        public bool Black = false;

        // ссылка на фигуру в интерфейсе
        public Grid Figure { get; set; }

        // конструктор пешки
        public Pawn(int x, int y, bool black)
        {
            this.X = x;
            this.Y = y;
            this.Black = black;
        }

        // метод выбора фигуры
        public void SelectFigure(object sender, MouseButtonEventArgs e)
        {
            // проверяем, атакуют ли нашу фигуру
            bool isAttacked = false;

            // ищем выделенную пешку среди всех пешек
            Pawn selectedPawn = MainWindow.mainWindow.Pawns.Find(x => x.Select == true);

            // если нашли выделенную пешку
            if (selectedPawn != null)
            {
                // проверяем атаку черной пешки на белую
                bool isBlackAttacking = selectedPawn.Black && !Black && this.Y - 1 == selectedPawn.Y &&
                                      (this.X - 1 == selectedPawn.X || this.X + 1 == selectedPawn.X);

                // проверяем атаку белой пешки на черную
                bool isWhiteAttacking = !selectedPawn.Black && Black && this.Y + 1 == selectedPawn.Y &&
                                       (this.X - 1 == selectedPawn.X || this.X + 1 == selectedPawn.X);

                // если есть атака
                if (isBlackAttacking || isWhiteAttacking)
                {
                    // удаляем атакованную пешку с доски
                    MainWindow.mainWindow.gameBoard.Children.Remove(this.Figure);
                    MainWindow.mainWindow.Pawns.Remove(this);

                    // перемещаем атакующую пешку на место атакованной
                    Grid.SetColumn(selectedPawn.Figure, this.X);
                    Grid.SetRow(selectedPawn.Figure, this.Y);

                    // обновляем координаты пешки
                    selectedPawn.X = this.X;
                    selectedPawn.Y = this.Y;

                    // снимаем выделение с атакующей пешки
                    selectedPawn.SelectFigure(null, null);

                    // отмечаем что была атака
                    isAttacked = true;
                }
            }

            // если атаки от пешки не было, проверяем атаку от ладьи
            if (!isAttacked)
            {
                // ищем выделенную ладью
                Rook selectedRook = MainWindow.mainWindow.Rooks.Find(x => x.Select == true);

                // если нашли выделенную ладью
                if (selectedRook != null)
                {
                    // проверяем атаку по горизонтали
                    bool isHorizontalAttack = this.Y == selectedRook.Y && Math.Abs(this.X - selectedRook.X) >= 1;
                    // проверяем атаку по вертикали
                    bool isVerticalAttack = this.X == selectedRook.X && Math.Abs(this.Y - selectedRook.Y) >= 1;

                    // если есть атака и путь свободен
                    if ((isHorizontalAttack || isVerticalAttack) && selectedRook.IsPathClear(selectedRook.X, selectedRook.Y, this.X, this.Y) && selectedRook.Black != this.Black)
                    {
                        // удаляем атакованную пешку
                        MainWindow.mainWindow.gameBoard.Children.Remove(this.Figure);
                        MainWindow.mainWindow.Pawns.Remove(this);

                        // перемещаем ладью на место пешки
                        Grid.SetColumn(selectedRook.Figure, this.X);
                        Grid.SetRow(selectedRook.Figure, this.Y);

                        // обновляем координаты ладьи
                        selectedRook.X = this.X;
                        selectedRook.Y = this.Y;

                        // снимаем выделение с ладьи
                        selectedRook.SelectFigure(null, null);

                        // отмечаем атаку
                        isAttacked = true;
                    }
                    selectedRook.SelectFigure(null, null);
                }
            }

            // если атаки не было, выделяем пешку
            if (!isAttacked)
            {
                MainWindow.mainWindow.OnSelect(this);

                if (this.Select)
                {
                    // снимаем выделение
                    this.Figure.Background = Black ?
                        new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Pawn (black).png"))) :
                        new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Pawn.png")));
                    this.Select = false;
                }
                else
                {
                    // выделяем фигуру
                    this.Figure.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Pawn (select).png")));
                    this.Select = true;
                }
            }
        }

        // метод перемещения пешки
        public void Transform(int newX, int newY)
        {
            // направление движения (черные вниз, белые вверх)
            int direction = Black ? 1 : -1;

            // проверяем движение вперед
            if (newX == this.X)
            {
                // проверяем не занята ли клетка
                bool isCellOccupied = MainWindow.mainWindow.Pawns.Any(p => p.X == newX && p.Y == newY) ||
                                     MainWindow.mainWindow.Rooks.Any(r => r.X == newX && r.Y == newY);

                if (isCellOccupied)
                {
                    SelectFigure(null, null);
                    return;
                }

                // ход на одну клетку вперед
                if (newY == this.Y + direction)
                {
                    MoveTo(newX, newY);
                    return;
                }

                // ход на две клетки из начальной позиции
                if ((Black && this.Y == 1) || (!Black && this.Y == 6))
                {
                    if (newY == this.Y + 2 * direction)
                    {
                        // проверяем нет ли фигур на пути
                        bool isPathBlocked = MainWindow.mainWindow.Pawns.Any(p => p.X == newX && p.Y == this.Y + direction) ||
                                           MainWindow.mainWindow.Rooks.Any(r => r.X == newX && r.Y == this.Y + direction);

                        if (!isPathBlocked)
                        {
                            MoveTo(newX, newY);
                            return;
                        }
                    }
                }
            }
        }

        // метод перемещения на новую позицию
        private void MoveTo(int newX, int newY)
        {
            // меняем позицию на доске
            Grid.SetColumn(this.Figure, newX);
            Grid.SetRow(this.Figure, newY);

            // запоминаем новые координаты
            this.X = newX;
            this.Y = newY;

            // снимаем выделение
            SelectFigure(null, null);
        }
    }
}