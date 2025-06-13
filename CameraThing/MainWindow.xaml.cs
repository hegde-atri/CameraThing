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
        // Initialize toolbar position after window is loaded
        UpdateToolbarPosition();
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
        UpdateToolbarPosition();
        
        isResizing = false;
    }
    
    private void UpdateWindowClip()
    {
        double radius = Math.Min(this.Width, this.Height) / 2;
        double center = radius;
        
        // Update window clip
        WindowClip.Center = new Point(center, center);
        WindowClip.RadiusX = radius;
        WindowClip.RadiusY = radius;
        
        // Update video element clips
        var mediaClip = mediaUriElement.Clip as EllipseGeometry;
        if (mediaClip != null)
        {
            mediaClip.Center = new Point(center, center);
            mediaClip.RadiusX = radius;
            mediaClip.RadiusY = radius;
        }
        
        var cameraClip = cameraCaptureElement.Clip as EllipseGeometry;
        if (cameraClip != null)
        {
            cameraClip.Center = new Point(center, center);
            cameraClip.RadiusX = radius;
            cameraClip.RadiusY = radius;
        }
        
        // Update video element sizes to match window
        mediaUriElement.Width = this.Width;
        mediaUriElement.Height = this.Height;
        cameraCaptureElement.Width = this.Width;
        cameraCaptureElement.Height = this.Height;
    }
    
    private void UpdateToolbarPosition()
    {
        // Position toolbar above the circle
        double windowWidth = this.Width;
        double toolbarWidth = ToolbarPanel.ActualWidth;
        
        // Center the toolbar horizontally and position it above the circle
        Canvas.SetLeft(ToolbarPanel, (windowWidth - toolbarWidth) / 2);
        Canvas.SetTop(ToolbarPanel, -60); // 60 pixels above the window
    }
}