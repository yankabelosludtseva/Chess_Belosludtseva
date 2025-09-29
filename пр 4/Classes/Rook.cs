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
    public class Rook
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

        // конструктор ладьи
        public Rook(int x, int y, bool black)
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

            // ищем выделенную ладью среди всех ладей
            Rook selectedRook = MainWindow.mainWindow.Rooks.Find(x => x.Select == true);

            // если нашли выделенную ладью и она вражеская
            if (selectedRook != null && selectedRook.Black != this.Black)
            {
                // проверяем атаку по горизонтали
                bool isHorizontalAttack = this.Y == selectedRook.Y && Math.Abs(this.X - selectedRook.X) >= 1;
                // проверяем атаку по вертикали
                bool isVerticalAttack = this.X == selectedRook.X && Math.Abs(this.Y - selectedRook.Y) >= 1;

                // если есть атака и путь свободен
                if ((isHorizontalAttack || isVerticalAttack) && IsPathClear(selectedRook.X, selectedRook.Y, this.X, this.Y))
                {
                    // удаляем атакованную ладью с доски
                    MainWindow.mainWindow.gameBoard.Children.Remove(this.Figure);
                    MainWindow.mainWindow.Rooks.Remove(this);

                    // перемещаем атакующую ладью на место атакованной
                    Grid.SetColumn(selectedRook.Figure, this.X);
                    Grid.SetRow(selectedRook.Figure, this.Y);

                    // обновляем координаты ладьи
                    selectedRook.X = this.X;
                    selectedRook.Y = this.Y;

                    // снимаем выделение с атакующей ладьи
                    selectedRook.SelectFigure(null, null);

                    // отмечаем что была атака
                    isAttacked = true;
                }
            }

            // ищем выделенную пешку
            Pawn selectedPawn = MainWindow.mainWindow.Pawns.Find(x => x.Select == true);

            // если нашли выделенную пешку
            if (selectedPawn != null)
            {
                // проверяем атаку черной пешки на белую ладью
                bool isBlackAttacking = selectedPawn.Black && !Black && this.Y - 1 == selectedPawn.Y &&
                                      (this.X - 1 == selectedPawn.X || this.X + 1 == selectedPawn.X);

                // проверяем атаку белой пешки на черную ладью
                bool isWhiteAttacking = !selectedPawn.Black && Black && this.Y + 1 == selectedPawn.Y &&
                                       (this.X - 1 == selectedPawn.X || this.X + 1 == selectedPawn.X);

                // если есть атака
                if (isBlackAttacking || isWhiteAttacking)
                {
                    // удаляем атакованную ладью с доски
                    MainWindow.mainWindow.gameBoard.Children.Remove(this.Figure);
                    MainWindow.mainWindow.Rooks.Remove(this);

                    // перемещаем пешку на место ладьи
                    Grid.SetColumn(selectedPawn.Figure, this.X);
                    Grid.SetRow(selectedPawn.Figure, this.Y);

                    // обновляем координаты пешки
                    selectedPawn.X = this.X;
                    selectedPawn.Y = this.Y;

                    // снимаем выделение с пешки
                    selectedPawn.SelectFigure(null, null);

                    // отмечаем что была атака
                    isAttacked = true;
                }
                selectedPawn.SelectFigure(null, null);
            }

            // если атаки не было, выделяем ладью
            if (!isAttacked)
            {
                MainWindow.mainWindow.OnSelectRook(this);

                if (this.Select)
                {
                    // снимаем выделение
                    this.Figure.Background = Black ?
                        new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Rook (black).png"))) :
                        new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Rook.png")));
                    this.Select = false;
                }
                else
                {
                    // выделяем фигуру
                    this.Figure.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Rook (select).png")));
                    this.Select = true;
                }
            }
        }

        // метод перемещения ладьи
        public void Transform(int newX, int newY)
        {
            // проверяем не стоит ли на клетке своя фигура
            bool isCellOccupiedByAlly = MainWindow.mainWindow.Pawns.Any(p => p.X == newX && p.Y == newY && p.Black == this.Black) ||
                                       MainWindow.mainWindow.Rooks.Any(r => r.X == newX && r.Y == newY && r.Black == this.Black && r == this);

            if (isCellOccupiedByAlly)
            {
                SelectFigure(null, null);
                return;
            }

            // ладья ходит только по горизонтали или вертикали
            bool isHorizontalMove = this.Y == newY && this.X != newX;
            bool isVerticalMove = this.X == newX && this.Y != newY;

            // если ход правильный и путь свободен
            if ((isHorizontalMove || isVerticalMove) && IsPathClear(this.X, this.Y, newX, newY))
            {
                // ищем вражескую пешку на целевой клетке
                var enemyPawn = MainWindow.mainWindow.Pawns.FirstOrDefault(p => p.X == newX && p.Y == newY && p.Black != this.Black);
                // ищем вражескую ладью на целевой клетке
                var enemyRook = MainWindow.mainWindow.Rooks.FirstOrDefault(r => r.X == newX && r.Y == newY && r.Black != this.Black);

                // если нашли вражескую пешку - удаляем ее
                if (enemyPawn != null)
                {
                    MainWindow.mainWindow.gameBoard.Children.Remove(enemyPawn.Figure);
                    MainWindow.mainWindow.Pawns.Remove(enemyPawn);
                }
                // если нашли вражескую ладью - удаляем ее
                else if (enemyRook != null)
                {
                    MainWindow.mainWindow.gameBoard.Children.Remove(enemyRook.Figure);
                    MainWindow.mainWindow.Rooks.Remove(enemyRook);
                }

                // перемещаем ладью на новую позицию
                Grid.SetColumn(this.Figure, newX);
                Grid.SetRow(this.Figure, newY);
                this.X = newX;
                this.Y = newY;
            }

            SelectFigure(null, null);
        }

        // проверка свободен ли путь для ладьи
        public bool IsPathClear(int startX, int startY, int endX, int endY)
        {
            // движение по вертикали
            if (startX == endX)
            {
                int minY = Math.Min(startY, endY);
                int maxY = Math.Max(startY, endY);

                // проверяем все клетки между начальной и конечной
                for (int y = minY + 1; y < maxY; y++)
                {
                    if (IsCellOccupied(startX, y))
                        return false;
                }
            }
            // движение по горизонтали
            else if (startY == endY)
            {
                int minX = Math.Min(startX, endX);
                int maxX = Math.Max(startX, endX);

                // проверяем все клетки между начальной и конечной
                for (int x = minX + 1; x < maxX; x++)
                {
                    if (IsCellOccupied(x, startY))
                        return false;
                }
            }

            return true;
        }

        // проверка занята ли клетка любой фигурой
        private bool IsCellOccupied(int x, int y)
        {
            return MainWindow.mainWindow.Pawns.Any(p => p.X == x && p.Y == y) ||
                   MainWindow.mainWindow.Rooks.Any(r => r.X == x && r.Y == y);
        }
    }
}