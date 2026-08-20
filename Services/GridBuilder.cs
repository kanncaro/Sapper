using saper1.Entities;
using saper1.IServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace saper1.Services
{
    public class GridBuilderOptions<T>
    {
        public Grid TargetGrid { get; }
        public T[,] Content;

        public int GridSize { get; set; } = 10;
        public Style CellStyle { get; set; } = null!;
        public Style FlaggedStyle { get; set; } = null!;
        public Brush TextColor { get; set; } = Brushes.Black;
        public float FontSize { get; set; } = 12f;

        public GridBuilderOptions(Grid target, T[,] content)
        {
            TargetGrid = target;
            Content = content;
        }
    }

    public class GridBuilder : IGridBuilder
    {
        private readonly IMineCounter _mineCounter;
        private readonly IMinePlacer _minePlacer;

        public GridBuilder(IMinePlacer minePlacer, IMineCounter mineCounter)
        {
            _minePlacer = minePlacer;
            _mineCounter = mineCounter;
        }

        public void BuildGrid(GridBuilderOptions<Cell> options)
        {
            BuildGridVisuals(options);
        }

        public void BuildGridVisuals(GridBuilderOptions<Cell> options)
        {
            var targetGrid = options.TargetGrid;
            var gridSize = options.GridSize;
            var cellStyle = options.CellStyle;
            var textColor = options.TextColor;
            var fontSize = options.FontSize;

            targetGrid.Children.Clear();
            targetGrid.RowDefinitions.Clear();
            targetGrid.ColumnDefinitions.Clear();

            for (int i = 0; i < gridSize; i++)
            {
                targetGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                targetGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int row = 0; row < gridSize; row++)
            {
                for (int col = 0; col < gridSize; col++)
                {
                    var text = new TextBlock
                    {
                        Text = " ",
                        Visibility = Visibility.Collapsed,
                        Foreground = textColor,
                        FontSize = fontSize,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    var cell = new Border
                    {
                        Name = $"Cell_{row}_{col}",
                        Style = cellStyle,
                        Child = text
                    };

                    Grid.SetRow(cell, row);
                    Grid.SetColumn(cell, col);
                    targetGrid.Children.Add(cell);

                    var target = options.Content[row, col];

                    target.Coordinates.X = row;
                    target.Coordinates.Y = col;
                    target.Border = cell;
                }
            }
        }

        public void PlaceMines(GridBuilderOptions<Cell> options, int mineCount, int safeRow, int safeCol)
        {
            _minePlacer.PlaceMines(options.GridSize, mineCount, safeRow, safeCol, ref options.Content);
        }

        public void CountMines(GridBuilderOptions<Cell> options)
        {
            _mineCounter.CountAllMines(ref options.Content);
        }
    }
}
