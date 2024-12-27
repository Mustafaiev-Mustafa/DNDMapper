using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

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
        private int _hexRadius = 8;
        private MapInfoModel _mapInfoModel = new MapInfoModel();
        private BiDictionary<HexCell, Polygon> _cellToPolygonMap = new BiDictionary<HexCell, Polygon>();
        private bool _isDragging = false;
        private string _startColor = null;
        public MainWindow()
        {
            InitializeComponent();
            SetupTransforms();
            //left click
            HexCanvas.MouseDown += OnMouseDownChangeColor;
            HexCanvas.MouseMove += OnMouseMoveChangeColor;
            HexCanvas.MouseUp += OnMouseUpChangeColorDrag;
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
            //
        }
        private void SetupTransforms()
        {
            TransformGroup transformGroup = new TransformGroup();
            transformGroup.Children.Add(_translateTransform);
            transformGroup.Children.Add(_scaleTransform);
            HexCanvas.RenderTransform = transformGroup;
        }
        //Left click
        private void OnMouseMoveChangeColor(object sender, MouseEventArgs e)
        {
            if (_isDragging && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPosition = e.GetPosition(HexCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(HexCanvas, currentPosition);

                if (hitTestResult?.VisualHit is Polygon hex && _cellToPolygonMap.TryGetBySecond(hex, out var cell))
                {
                    if (cell.Color == _startColor)
                    {
                        HexHelper.ChangeHexColor(hex, cell);
                    }
                }
            }
        }
        private void OnMouseDownChangeColor(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                Point clickPosition = e.GetPosition(HexCanvas);
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(HexCanvas, clickPosition);

                if (hitTestResult?.VisualHit is Polygon hex && _cellToPolygonMap.TryGetBySecond(hex, out var cell))
                {
                    _startColor = cell.Color;
                    HexHelper.ChangeHexColor(hex, cell);
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
        //Window buttons
        private void OnGenerateContinentsClick(object sender, RoutedEventArgs e)
        {
            HexMapGenerator.GenerateMap(_mapInfoModel);
            HexHelper.UpdateHexColors(_cellToPolygonMap, _mapInfoModel);
        }
        private void OnCreateMapClick(object sender, RoutedEventArgs e)
        {
            CreateMapDialog dialog = new CreateMapDialog();
            if (dialog.ShowDialog() == true)
            {
                string name = dialog.MapName;
                int xSize = dialog.MapXSize;
                int ySize = dialog.MapYSize;

                var mapToCreate = new MapInfoModel(name, xSize, ySize);
                if (mapToCreate != null)
                {
                    _mapInfoModel = mapToCreate;
                    HexHelper.DrawHexMap(HexCanvas, _mapInfoModel, _cellToPolygonMap, _hexRadius);
                }
            }
        }
        private void OnSaveMapClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _mapInfoModel.EditedAt = DateTime.Now;
                HexFileLoader.SaveMapToFile($"{_mapInfoModel.Name}\\{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json", _mapInfoModel);
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
                    MapInfoModel mapToOpen = HexFileLoader.LoadMapFromFile(filePath);
                    if (mapToOpen != null)
                    {
                        _mapInfoModel = mapToOpen;
                        HexHelper.DrawHexMap(HexCanvas, _mapInfoModel, _cellToPolygonMap, _hexRadius);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}