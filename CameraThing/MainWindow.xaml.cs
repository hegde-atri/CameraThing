using System.Windows;
using System.Windows.Input;
using WPFMediaKit.DirectShow.Controls;

namespace CameraThing;

 public partial class MainWindow : Window
    {        public MainWindow()
        {
            InitializeComponent();

            if (MultimediaUtil.VideoInputDevices.Length > 0)
            {
                cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;            }
            // Camera element is always visible now - black background shows when no source
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
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                DragMove();
        }
    }