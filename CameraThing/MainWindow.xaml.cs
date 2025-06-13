using System.Windows;
using System.Windows.Input;
using DirectShowLib;

namespace CameraThing;

public partial class MainWindow : Window
{
    private readonly ToolbarWindow _toolbarWindow;
    private bool _isResizeMode;
    private bool _isResizing;
    private bool _isToolbarVisible;
    private Point _resizeModeStartPoint;
    private Size _resizeModeStartSize;

    public MainWindow()
    {
        InitializeComponent();

        // Initialise toolbar window
        _toolbarWindow = new ToolbarWindow(this);

        // if (MultimediaUtil.VideoInputDevices.Length > 0)
        //     // Set initial camera device
        //     CameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[0];


        // Set initial size to be square
        UpdateWindowClip();
    }

    protected override void OnClosed(EventArgs e)
    {
        // Hide and close the toolbar window when main window closes
        if (_toolbarWindow != null)
            try
            {
                _toolbarWindow.Close();
            }
            catch
            {
                // Ignore exceptions if toolbar is already closing/closed
            }

        base.OnClosed(e);
    }

    public void SetCameraDevice(DsDevice? device)
    {
        CameraCaptureElement.VideoCaptureDevice = device;
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
        {
            if (_isResizeMode)
            {
                // Exit resize mode on left click
                _isResizeMode = false;
                Cursor = Cursors.Arrow;
            }
            else
            {
                DragMove();
            }
        }
    }

    private void Window_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_isResizeMode)
        {
            // Exit resize mode if already in it
            _isResizeMode = false;
            Cursor = Cursors.Arrow;
        }
        else
        {
            // Enter resize mode
            _isResizeMode = true;
            _resizeModeStartPoint = e.GetPosition(this);
            _resizeModeStartSize = new Size(Width, Height);
            Cursor = Cursors.SizeNWSE;
        }
    }

    private void Window_MouseMove(object sender, MouseEventArgs e)
    {
        if (!_isResizeMode) return;

        var currentPoint = e.GetPosition(this);
        var deltaX = currentPoint.X - _resizeModeStartPoint.X;
        var deltaY = currentPoint.Y - _resizeModeStartPoint.Y;

        // Calculate new size based on the larger delta (to maintain square aspect ratio)
        var delta = Math.Max(Math.Abs(deltaX), Math.Abs(deltaY));

        // Determine if we're increasing or decreasing size
        var isIncreasing = deltaX + deltaY > 0;
        var newSize = isIncreasing ? _resizeModeStartSize.Width + delta : _resizeModeStartSize.Width - delta;

        // Enforce minimum size
        newSize = Math.Max(newSize, MinWidth);

        _isResizing = true;
        Width = newSize;
        Height = newSize;
        _isResizing = false;

        UpdateWindowClip();
        UpdateToolbarPosition();
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);
        UpdateToolbarPosition();
    }

    private void UpdateToolbarPosition()
    {
        if (_isToolbarVisible)
        {
            // Position toolbar above the main window
            var mainWindowRect = new Rect(Left, Top, Width, Height);
            _toolbarWindow.Left = mainWindowRect.Left + (mainWindowRect.Width - _toolbarWindow.Width) / 2;
            _toolbarWindow.Top = mainWindowRect.Top - _toolbarWindow.Height - 10;
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isToolbarVisible)
        {
            _toolbarWindow.Hide();
            _isToolbarVisible = false;
        }
        else
        {
            _toolbarWindow.Show();
            _isToolbarVisible = true;
            UpdateToolbarPosition();
        }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Initialize window clip after loading
        UpdateWindowClip();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isResizing || _isResizeMode) return;

        _isResizing = true;

        // Force square aspect ratio
        var size = Math.Min(Width, Height);
        Width = size;
        Height = size;

        UpdateWindowClip();
        UpdateToolbarPosition();

        _isResizing = false;
    }

    private void UpdateWindowClip()
    {
        var radius = Math.Min(Width, Height) / 2;
        var center = radius;

        // Update window clip
        WindowClip.Center = new Point(center, center);
        WindowClip.RadiusX = radius;
        WindowClip.RadiusY = radius;

        // Update video element clips to match the window size
        CameraClip.Center = new Point(center, center);
        CameraClip.RadiusX = radius;
        CameraClip.RadiusY = radius;
    }
}