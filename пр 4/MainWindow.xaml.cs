using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace пр_4
{
    /// <summary>
    /// логика взаимодействия для mainwindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mainWindow;
        public List<Classes.Pawn> Pawns = new List<Classes.Pawn>();
        public List<Classes.Rook> Rooks = new List<Classes.Rook>();
        public MainWindow()
        {
            InitializeComponent();
            MainWindow.mainWindow = this;

            // добавляем белые пешки
            Pawns.Add(new Classes.Pawn(0, 6, false));
            Pawns.Add(new Classes.Pawn(1, 6, false));
            Pawns.Add(new Classes.Pawn(2, 6, false));
            Pawns.Add(new Classes.Pawn(3, 6, false));
            Pawns.Add(new Classes.Pawn(4, 6, false));
            Pawns.Add(new Classes.Pawn(5, 6, false));
            Pawns.Add(new Classes.Pawn(6, 6, false));
            Pawns.Add(new Classes.Pawn(7, 6, false));
            // добавляем черные пешки
            Pawns.Add(new Classes.Pawn(0, 1, true));
            Pawns.Add(new Classes.Pawn(1, 1, true));
            Pawns.Add(new Classes.Pawn(2, 1, true));
            Pawns.Add(new Classes.Pawn(3, 1, true));
            Pawns.Add(new Classes.Pawn(4, 1, true));
            Pawns.Add(new Classes.Pawn(5, 1, true));
            Pawns.Add(new Classes.Pawn(6, 1, true));
            Pawns.Add(new Classes.Pawn(7, 1, true));
            // белые ладьи
            Rooks.Add(new Classes.Rook(0, 7, false));
            Rooks.Add(new Classes.Rook(7, 7, false));
            // черные ладьи
            Rooks.Add(new Classes.Rook(0, 0, true));
            Rooks.Add(new Classes.Rook(7, 0, true));

            CreateFigure();
            CreateRooks();
        }
        /// <summary>
        /// метод создания фигур на доске
        /// </summary>
        public void CreateFigure()
        {
            // перебираем все пешки в списке
            foreach (Classes.Pawn pawn in Pawns)
            {
                // создаем grid для отображения фигуры
                pawn.Figure = new Grid()
                {
                    Width = 50,
                    Height = 50
                };

                // выбираем картинку в зависимости от цвета пешки
                if (pawn.Black)
                    pawn.Figure.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Pawn (black).png")));
                else
                    pawn.Figure.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Pawn.png")));

                // ставим пешку на начальную позицию
                Grid.SetColumn(pawn.Figure, pawn.X);
                Grid.SetRow(pawn.Figure, pawn.Y);

                // добавляем обработчик клика по фигуре
                pawn.Figure.MouseDown += pawn.SelectFigure;

                // добавляем фигуру на игровую доску
                gameBoard.Children.Add(pawn.Figure);
            }
        }

        /// <summary>
        /// метод для снятия выделения с других пешек
        /// </summary>
        public void OnSelect(Classes.Pawn selectedPawn)
        {
            // перебираем все пешки
            foreach (Classes.Pawn pawn in Pawns)
            {
                // пропускаем пустые элементы
                if (pawn == null) continue;

                // если это не выбранная пешка
                if (pawn != selectedPawn)
                {
                    // и если она выделена - снимаем выделение
                    if (pawn.Select)
                    {
                        pawn.SelectFigure(null, null);
                    }
                }
            }
        }

        /// <summary>
        /// обработчик клика по клетке доски
        /// </summary>
        private void SelectTile(object sender, MouseButtonEventArgs e)
        {
            if (sender is Grid tile)
            {
                int x = Grid.GetColumn(tile);
                int y = Grid.GetRow(tile);

                // ищем выделенную пешку
                Classes.Pawn selectPawn = Pawns.Find(pawn => pawn.Select == true);
                if (selectPawn != null)
                {
                    // перемещаем пешку на выбранную клетку
                    selectPawn.Transform(x, y);
                    return;
                }

                // ищем выделенную ладью
                Classes.Rook selectRook = Rooks.Find(rook => rook.Select == true);
                if (selectRook != null)
                {
                    // перемещаем ладью на выбранную клетку
                    selectRook.Transform(x, y);
                }
            }
        }

        /// <summary>
        /// метод создания ладей на доске
        /// </summary>
        public void CreateRooks()
        {
            foreach (Classes.Rook rook in Rooks)
            {
                // создаем grid для ладьи
                rook.Figure = new Grid()
                {
                    Width = 50,
                    Height = 50
                };

                // выбираем картинку в зависимости от цвета
                if (rook.Black)
                    rook.Figure.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Rook (black).png")));
                else
                    rook.Figure.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/Images/Rook.png")));

                // ставим ладью на начальную позицию
                Grid.SetColumn(rook.Figure, rook.X);
                Grid.SetRow(rook.Figure, rook.Y);

                // добавляем обработчик клика
                rook.Figure.MouseDown += rook.SelectFigure;

                // добавляем ладью на доску
                gameBoard.Children.Add(rook.Figure);
            }
        }

        /// <summary>
        /// метод для снятия выделения с других ладей
        /// </summary>
        public void OnSelectRook(Classes.Rook selectedRook)
        {
            foreach (Classes.Rook rook in Rooks)
            {
                if (rook == null) continue;
                // если ладья не текущая и выделена - снимаем выделение
                if (rook != selectedRook && rook.Select)
                {
                    rook.SelectFigure(null, null);
                }
            }
        }

        /// <summary>
        /// проверяет, есть ли на клетке вражеская фигура
        /// </summary>
        public bool IsEnemy(int x, int y, bool isBlack)
        {
            return Pawns.Any(p => p.X == x && p.Y == y && p.Black != isBlack) ||
                   Rooks.Any(r => r.X == x && r.Y == y && r.Black != isBlack);
        }

        /// <summary>
        /// проверяет, есть ли на клетке своя фигура
        /// </summary>
        public bool IsAlly(int x, int y, bool isBlack)
        {
            return Pawns.Any(p => p.X == x && p.Y == y && p.Black == isBlack) ||
                   Rooks.Any(r => r.X == x && r.Y == y && r.Black == isBlack);
        }

        /// <summary>
        /// проверяет, занята ли клетка любой фигурой
        /// </summary>
        public bool IsCellOccupied(int x, int y)
        {
            return Pawns.Any(p => p.X == x && p.Y == y) || Rooks.Any(r => r.X == x && r.Y == y);
        }
    }
}