using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFMediaKit.DirectShow.Controls;

namespace CameraThing;

public partial class MainWindow : Window
{
    private bool isResizing = false;

    public MainWindow()
    {
        InitializeComponent();

        if (MultimediaUtil.VideoInputDevices.Length > 0)
        {
            cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;
        }
        // Camera element is always visible now - black background shows when no source
        
        cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[0];
        // Set initial size to be square
        UpdateWindowClip();
    }

    private void cobVideoSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (cobVideoSource.SelectedIndex < 0)
        {
            // Stop any current camera capture
            cameraCaptureElement.VideoCaptureDevice = null;
            return;
        }
        
        // Set the selected camera device
        cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
    }    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.Left)
            DragMove();
    }
      private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Initialize window clip after loading
        UpdateWindowClip();
    }
      private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (isResizing) return;
        
        isResizing = true;
        
        // Force square aspect ratio
        double size = Math.Min(this.Width, this.Height);
        this.Width = size;
        this.Height = size;
        
        UpdateWindowClip();
        
        isResizing = false;
    }    private void UpdateWindowClip()
    {
        double radius = Math.Min(this.Width, this.Height) / 2;
        double center = radius;
        
        // Update window clip
        WindowClip.Center = new Point(center, center);
        WindowClip.RadiusX = radius;
        WindowClip.RadiusY = radius;
        
        // Update video element clips to match the window size
        MediaClip.Center = new Point(center, center);
        MediaClip.RadiusX = radius;
        MediaClip.RadiusY = radius;
        
        CameraClip.Center = new Point(center, center);
        CameraClip.RadiusX = radius;
        CameraClip.RadiusY = radius;
    }
}