using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WPFMediaKit.DirectShow.Controls;

namespace CameraThing;

public partial class MainWindow : Window
{
    private bool isResizing;
    private ToolbarWindow _toolbarWindow;
    private bool _isToolbarVisible = false;    public MainWindow()
    {
        InitializeComponent();

        // Initialize toolbar window
        _toolbarWindow = new ToolbarWindow(this);

        if (MultimediaUtil.VideoInputDevices.Length > 0)
        {
            // Set initial camera device
            cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[0];
        }
        
        // Set initial size to be square
        UpdateWindowClip();
    }

    protected override void OnClosed(EventArgs e)
    {
        // Hide and close the toolbar window when main window closes
        if (_toolbarWindow != null)
        {
            _toolbarWindow.Close();
        }
        base.OnClosed(e);
    }private void cobVideoSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // This method is no longer needed as it's handled in ToolbarWindow
    }    public void SetCameraDevice(DirectShowLib.DsDevice? device)
    {
        cameraCaptureElement.VideoCaptureDevice = device;
    }

    public bool GetDeeperColorValue()
    {
        // Return the deeper color value - you may need to adjust this based on your implementation
        return false; // placeholder
    }

    public void SetDeeperColorValue(bool value)
    {
        // Set the deeper color value - you may need to adjust this based on your implementation
        // This should update the cameraCaptureElement.DeeperColor property
    }    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }

    protected override void OnLocationChanged(EventArgs e)
    {
        base.OnLocationChanged(e);
        UpdateToolbarPosition();
    }

    private void UpdateToolbarPosition()
    {
        if (_isToolbarVisible && _toolbarWindow != null)
        {
            // Position toolbar above the main window
            var mainWindowRect = new Rect(Left, Top, Width, Height);
            _toolbarWindow.Left = mainWindowRect.Left + (mainWindowRect.Width - _toolbarWindow.Width) / 2;
            _toolbarWindow.Top = mainWindowRect.Top - _toolbarWindow.Height - 10;
        }
    }    private void SettingsButton_Click(object sender, RoutedEventArgs e)
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
    }    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (isResizing) return;

        isResizing = true;

        // Force square aspect ratio
        var size = Math.Min(Width, Height);
        Width = size;
        Height = size;

        UpdateWindowClip();
        UpdateToolbarPosition();

        isResizing = false;
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