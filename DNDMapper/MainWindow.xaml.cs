using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DNDMapper.Core;
using DNDMapper.Core.Enums;
using DNDMapper.Helpers;
using DNDMapper.Infrastructure;
using DNDMapper.Models;

namespace DNDMapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScaleTransform _scaleTransform = new ScaleTransform();
        private TranslateTransform _translateTransform = new TranslateTransform();
        private Point _lastMousePosition;
        private bool _isDragging = false;
        private ColorEnum? _startColor = ColorEnum.Blue; 
        private List<HexCell> _selectedCells = new List<HexCell>();
        private bool _isSelectingRegion = false;
        private MapModel _mapModel;
        public MainWindow()
        {
            InitializeComponent();
            SetupTransforms();
            PopulateLayerSelector();
            //left click
            HexCanvas.MouseDown += OnMouseDownChangeColor;
            HexCanvas.MouseDown += OnHexCellClickRegion;
            HexCanvas.MouseMove += OnMouseMoveChangeColor;
            HexCanvas.MouseUp += OnHexCellClickRegion;
            //
            //wheel click
            HexCanvas.MouseDown += OnWheelDown;
            HexCanvas.MouseUp += OnWheelUp;
            HexCanvas.MouseMove += OnMouseMove;
            //
            //wheel rolling
            HexCanvas.MouseWheel += OnMouseWheel;
            //
            //right click
            HexCanvas.MouseDown += OnRightMouseClick;
            //
        }
        private void SetupTransforms()
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(_translateTransform);
            transformGroup.Children.Add(_scaleTransform);
            HexCanvas.RenderTransform = transformGroup;
        }

        private void PopulateLayerSelector()
        {
            foreach (var layer in Enum.GetValues(typeof(MapLayerEnum)))
            {
                LayerSelector.Items.Add(new ComboBoxItem
                {
                    Content = layer.ToString(),
                    Tag = layer
                });
            }

            LayerSelector.SelectedIndex = 0;
        }
        //Left click
        private void OnMouseMoveChangeColor(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed && !_isSelectingRegion)
            {
                Point currentPosition = e.GetPosition(HexCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(HexCanvas, currentPosition);

                if (hitTestResult?.VisualHit is Polygon hex && _mapModel.CellToPolygonMap.TryGetBySecond(hex, out var cell))
                {
                    if (cell.Color == _startColor)
                    {
                        _mapModel.MapHelper.ChangeHexColor(hex, cell);
                    }
                }
            }
        }
        private void OnMouseDownChangeColor(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !_isSelectingRegion)
            {
                _isDragging = true;
                Point clickPosition = e.GetPosition(HexCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(HexCanvas, clickPosition);

                if (hitTestResult?.VisualHit is Polygon hex && _mapModel.CellToPolygonMap.TryGetBySecond(hex, out var cell))
                {
                    _startColor = cell.Color;
                    _mapModel.MapHelper.ChangeHexColor(hex, cell);
                }
            }
        } 
        private void OnMouseUpChangeColorDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                _isDragging = false;
                _startColor = null;
            }
        }
        private void OnHexCellClickRegion(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && _isSelectingRegion)
            {
                Point clickPosition = e.GetPosition(HexCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(HexCanvas, clickPosition);

                if (hitTestResult?.VisualHit is Polygon hex && _mapModel.CellToPolygonMap.TryGetBySecond(hex, out var cell))
                {
                    if (cell.Color == ColorEnum.Green || cell.Color == ColorEnum.Brown)
                    {
                        if (!_selectedCells.Contains(cell))
                        {
                            _selectedCells.Add(cell);
                            hex.Stroke = Brushes.Yellow;
                            hex.StrokeThickness = 3;
                        }
                        else
                        {
                            _selectedCells.Remove(cell);
                            hex.Stroke = Brushes.Black;
                            hex.StrokeThickness = 1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("You can't choose sea.");
                    }
                }
            }
        }
        //Wheel
        private void OnWheelDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _lastMousePosition = e.GetPosition(this);
                HexCanvas.CaptureMouse();
            }
        }
        private void OnWheelUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
            {
                _lastMousePosition = e.GetPosition(this);
                HexCanvas.ReleaseMouseCapture();
            }
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(this);
                Vector delta = currentPosition - _lastMousePosition;

                _translateTransform.X += delta.X;
                _translateTransform.Y += delta.Y;

                _lastMousePosition = currentPosition;
            }
        }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double zoom = e.Delta > 0 ? 1.1 : 1 / 1.1;
            Point mousePosition = e.GetPosition(HexCanvas);

            double newScaleX = _scaleTransform.ScaleX * zoom;
            double newScaleY = _scaleTransform.ScaleY * zoom;

            if (newScaleX < 0.5 || newScaleX > 2.0 || newScaleY < 0.5 || newScaleY > 2.0)
            {
                return;
            }

            double contentX = (mousePosition.X - _translateTransform.X) / _scaleTransform.ScaleX;
            double contentY = (mousePosition.Y - _translateTransform.Y) / _scaleTransform.ScaleY;

            _scaleTransform.ScaleX = newScaleX;
            _scaleTransform.ScaleY = newScaleY;

            _translateTransform.X -= (contentX * zoom - contentX) * _scaleTransform.ScaleX;
            _translateTransform.Y -= (contentY * zoom - contentY) * _scaleTransform.ScaleY;
        }
        //Right click
        private void OnRightMouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed && _isSelectingRegion)
            {
                _isSelectingRegion = false;

                if (_selectedCells.Count == 0)
                {
                    MessageBox.Show("You didn't choose cells.");
                    return;
                }

                var dialog = new CreateRegionDialog();
                if (dialog.ShowDialog() == true)
                {
                    var region = new Region(dialog.RegionName, null, dialog.RegionColor, new List<HexCell>(_selectedCells));
                    _mapModel.MapInfo.Regions.Add(region);

                    MessageBox.Show($"Region '{region.Name}' created succesfuly!");
                }

                foreach (var cell in _selectedCells)
                {
                    if (_mapModel.CellToPolygonMap.TryGetByFirst(cell, out var hex))
                    {
                        hex.Stroke = Brushes.Black;
                        hex.StrokeThickness = 1;
                    }
                }

                _selectedCells.Clear();
            }
        }
        //Window buttons
        private void OnGenerateContinentsClick(object sender, RoutedEventArgs e)
        {
            GenerateHelper.GenerateMap(_mapModel.MapInfo);
            _mapModel.MapHelper.ShowLayer();
        }
        private void OnCreateMapClick(object sender, RoutedEventArgs e)
        {
            CreateMapDialog dialog = new CreateMapDialog();
            if (dialog.ShowDialog() == true)
            {
                string name = dialog.MapName;
                int xSize = dialog.MapXSize;
                int ySize = dialog.MapYSize;

                var mapToCreate = new MapInfo(name, xSize, ySize);
                if (mapToCreate != null)
                {
                    mapToCreate.InitializeMap();
                    if (_mapModel != null)
                        _mapModel.SetMapInfo(HexCanvas, mapToCreate);
                    else
                        _mapModel = new MapModel(HexCanvas, mapToCreate);
                }
            }
        }
        private void OnSaveMapClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _mapModel.MapInfo.EditedAt = DateTime.Now;
                FileHelper.SaveMapToFile($"{_mapModel.MapInfo.Name}\\{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json", _mapModel.MapInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("File saved succesfuly");
        }
        private void OnOpenMapClick(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                Title = "Choose map file"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string filePath = openFileDialog.FileName;
                    MapInfo mapToOpen = FileHelper.LoadMapFromFile(filePath);
                    if (mapToOpen != null)
                    {
                        if(_mapModel != null)
                            _mapModel.SetMapInfo(HexCanvas, mapToOpen);
                        else
                            _mapModel = new MapModel(HexCanvas, mapToOpen);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void OnStartRegionSelectionClick(object sender, RoutedEventArgs e)
        {
            _isSelectingRegion = true;
            _selectedCells.Clear();
            MessageBox.Show("Chose cells to create region from. Click right button to confirm creating region from selected cells.");
        }
        private void OnLayerSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LayerSelector.SelectedItem is ComboBoxItem selectedItem && Enum.TryParse(selectedItem.Tag.ToString(), out MapLayerEnum selectedLayer))
            {
                if(_mapModel != null)
                    _mapModel.MapHelper.ShowLayer(selectedLayer);
            }
        }
    }
}