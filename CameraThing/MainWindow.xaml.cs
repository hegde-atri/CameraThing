using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Linq;
using Microsoft.Win32;
using System;
using System.Windows;
using WPFMediaKit.DirectShow.Controls;
using System.Linq;

namespace CameraThing;

 public partial class MainWindow : Window
    {
        private bool sliderDrag;
        private bool sliderMediaChange;

        public MainWindow()
        {
            InitializeComponent();

            this.Closing += MainWindow_Closing;

            if (MultimediaUtil.VideoInputDevices.Any())
            {
                cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;
            }
            SetCameraCaptureElementVisible(false);
        }

        private void SetCameraCaptureElementVisible(bool visible)
        {
            cameraCaptureElement.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            mediaUriElement.Visibility = !visible ? Visibility.Visible : Visibility.Collapsed;
            if (!visible)
            {
                cobVideoSource.SelectedIndex = -1;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mediaUriElement.Close();
        }

        private void cobVideoSource_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cobVideoSource.SelectedIndex < 0)
                return;
            SetCameraCaptureElementVisible(true);
            cameraCaptureElement.VideoCaptureDevice = MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex];
        }
    }