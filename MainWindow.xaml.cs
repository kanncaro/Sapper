using saper1.Data;
using saper1.Entities;
using saper1.Extensions;
using saper1.IServices;
using saper1.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Application = System.Windows.Application;


namespace saper1
{
    public partial class MainWindow : Window
    {
        private readonly ISettingsService _settingsService;
        private readonly IThemeManager _themeManager;
        private readonly IGameTimer _gameTimer;
        private readonly IGameLogicController _gameLogic;
        private readonly IGridBuilder _gridBuilder;

        private int _gridSize;
        private int GridSquare => _gridSize * _gridSize;
        private int _minesNeeded;

        private readonly Dictionary<string, int[]> difficultyConfig = new()
        {
            { "Новачок", new int[] { 10, 15 } },
            { "Любитель", new int[]{ 15, 34 } },
            { "Професіонал", new int[]{ 20, 60 } }
        };

        private Cell[,] _cells;
        private List<Cell> MineMap => _cells != null ? [.. _cells.Where(x => x.IsMine)] : [];
        private GridBuilderOptions<Cell>? _gridOptions;

        private bool _isSettingsPanelOpen = false;

        public MainWindow(ISettingsService settingsService,
                          IThemeManager themeManager,
                          IGameTimer gameTimer,
                          IGridBuilder gridBuilder,
                          IGameLogicController gameLogic)
        {
            InitializeComponent();

            _settingsService = settingsService;
            _themeManager = themeManager;
            _gameTimer = gameTimer;
            _gridBuilder = gridBuilder;
            _gameLogic = gameLogic;


            ApplySettings();


            _gameTimer.TimeChanged += (min, sec) =>
            {
                Time.Text = string.Format("{0:00}:{1:00}", min, sec);
            };

            KeyDown += (s, e) => { if (e.Key == Key.Escape) Close(); };
            (Resources["MainAnimation"] as Storyboard)?.Begin(Main);
        }

        // Safe resource lookup helpers: try candidate keys in order and return a sensible fallback
        private Style GetStyleResource(params string[] keys)
        {
            foreach (var key in keys)
            {
                var res = TryFindResource(key);
                if (res is Style s) return s;
            }

            // Last-resort: return an empty style for Border so usage won't throw
            return new Style(typeof(Border));
        }

        private Brush GetBrushResource(string key, Brush fallback)
        {
            var res = TryFindResource(key);
            if (res is Brush b) return b;
            return fallback;
        }


        private void InitializeCells()
        {
            _cells = new Cell[_gridSize, _gridSize];

            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    _cells[i, j] = new Cell(new Coordinates { X = i, Y = j })
                    {
                        IsOpen = false,
                        IsFlagged = false,
                        IsMine = false,
                        AdjacentMines = 0,
                        Border = null,
                    };
                }
            }
        }

        private void ApplySettings()
        {
            _settingsService.Load();

            _gridSize = difficultyConfig[_settingsService.SettingsData.Difficulty][0];
            _minesNeeded = difficultyConfig[_settingsService.SettingsData.Difficulty][1];

            difficultyComboBox.SelectedItem = difficultyComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _settingsService.SettingsData.Difficulty);

            themeComboBox.SelectedItem = themeComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _settingsService.SettingsData.Theme);

            _themeManager.ApplyTheme(_settingsService.SettingsData.Theme, Resources);

            InitializeCells();

            if (_cells != null)
            {
                foreach (var c in _cells.Where(x => x.IsOpen))
                {
                    c.Border.Background = _themeManager.OpenedCellBrush;
                }
            }

            BuildGrid();
        }

        private void BuildGrid()
        {
            float fontSize = (float)Math.Max(12, 500.0 / _gridSize * 0.4);

            _gridOptions = new GridBuilderOptions<Cell>(playField, _cells)
            {
                GridSize = _gridSize,
                CellStyle = GetStyleResource("Playfield", "ClosedPlayfield", "OpenPlayfield"),
                FlaggedStyle = GetStyleResource("selectedSquare"),
                TextColor = GetBrushResource("TextForeground", Brushes.Black),
                FontSize = fontSize
            };

            _gridBuilder.BuildGrid(_gridOptions);

            _cells.ForEach(cell =>
            {
                cell.Border.MouseLeftButtonDown += Cell_LeftClick;
                cell.Border.MouseRightButtonDown += Cell_RightClick;
            });
        }

        private bool _gameStarted;

        private void Cell_LeftClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border cell) return;
            int row = Grid.GetRow(cell), col = Grid.GetColumn(cell);

            var properCell = _cells[row, col];

            if (properCell.IsOpen || properCell.IsFlagged) return;

            if (!_gameStarted)
            {
                _gridBuilder.PlaceMines(_gridOptions, _minesNeeded, row, col);
                _gridBuilder.CountMines(_gridOptions);
                _gameTimer.Reset();
                _gameTimer.Start();
                RevealRecursive(row, col);
                _gameStarted = true;
                return;
            }

            var potentionalCell = _cells[row, col];

            bool IsMine = potentionalCell!.IsMine;

            if (IsMine)
            {
                if (potentionalCell.Border.Child is TextBlock block)
                {
                    block.Visibility = Visibility.Visible;
                    block.Background = Brushes.Red;
                    potentionalCell.Border.Background = _themeManager.OpenedCellBrush;
                    potentionalCell.IsOpen = true;
                }
                _gameLogic.RevealAllMines(MineMap);
                _gameTimer.Stop();
                MessageBox.Show("Game over!");
                Refresh();
            }
            else
            {
                RevealRecursive(row, col);

                if ((GridSquare - MineMap.Count) == _cells.Where(x => x.IsOpen).Count())
                {
                    _gameTimer.Stop();
                    MessageBox.Show("You win!");
                    Refresh();
                }
            }
        }

        private void Cell_RightClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is not Border cell) return;
            int row = Grid.GetRow(cell), col = Grid.GetColumn(cell);
            if (!_gameStarted || cell.Background == _themeManager.OpenedCellBrush) return;

            var current = _cells[row, col];
            if (current!.IsFlagged)
            {
                cell.Style = GetStyleResource("Playfield", "ClosedPlayfield", "OpenPlayfield");
                current.IsFlagged = false;
            }
            else
            {
                cell.Style = GetStyleResource("selectedSquare");
                current.IsFlagged = true;
            }
        }

        private void RevealRecursive(int row, int col)
        {
            if (row < 0 || col < 0 || row >= _gridSize || col >= _gridSize) return;

            var cell = _cells.FirstOrDefault(x => x.Coordinates?.X == row && x.Coordinates?.Y == col);
            if (cell == null || cell.IsOpen || cell.IsFlagged) return;

            cell.IsOpen = true;

            if (cell.Border.Child is TextBlock text)
            {
                cell.Border.Background = _themeManager.OpenedCellBrush;
                text.Visibility = Visibility.Visible;
                text.Text = cell.AdjacentMines == 0 ? " " : cell.AdjacentMines.ToString();
            }

            if (TryFindResource("RevealCellAnimation") is Storyboard revealAnim)
            {
                Storyboard.SetTarget(revealAnim, cell.Border);
                revealAnim.Begin();
            }

            if (cell.AdjacentMines == 0)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx != 0 || dy != 0)
                        {
                            RevealRecursive(row + dx, col + dy);
                        }
                    }
                }
            }
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void ExitPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_gameStarted) return;
            if (MessageBox.Show("Refresh game?", "Caution", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                Refresh();
        }

        private void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isSettingsPanelOpen) return;
            if (TryFindResource("OpenSettingsAnimation") is Storyboard openAnimation)
            {
                Overlay.Visibility = Visibility.Visible;
                openAnimation.Begin(this);
                _isSettingsPanelOpen = true;
            }
        }

        private void Refresh()
        {
            _gameStarted = false;
            _gameTimer.Reset();
            _gameTimer.Stop();

            _cells.ForEach(cell =>
            {
                cell.IsOpen = false;
                cell.IsFlagged = false;
                cell.IsMine = false;
                cell.AdjacentMines = 0;
            });

            BuildGrid();
        }

        private void CloseSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryFindResource("CloseSettingsAnimation") is Storyboard closeAnimation)
            {
                closeAnimation.Completed += (s, _) => Overlay.Visibility = Visibility.Collapsed;
                closeAnimation.Begin(this);
                _isSettingsPanelOpen = false;
            }
        }

        private void ConfirmSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var newDifficulty = (difficultyComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()!;
                var newTheme = (themeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString()!;

                _settingsService.Save(
                                new()
                                {
                                    Difficulty = newDifficulty,
                                    Theme = newTheme
                                });

                ApplySettings();
                Refresh();

                CloseSettingsButton_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
