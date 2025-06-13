using System.Windows;
using WPFMediaKit.DirectShow.Controls;

namespace CameraThing;

 public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            if (MultimediaUtil.VideoInputDevices.Length > 0)
            {
                cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;
            }
            SetCameraCaptureElementVisible(false);
        }

        private void SetCameraCaptureElementVisible(bool visible)
        {
            cameraCaptureElement.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            if (!visible)
            {
                cobVideoSource.SelectedIndex = -1;
            }
        }

        private void cobVideoSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cobVideoSource.SelectedIndex < 0)
                return;
            SetCameraCaptureElementVisible(true);
            cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
        }
    }