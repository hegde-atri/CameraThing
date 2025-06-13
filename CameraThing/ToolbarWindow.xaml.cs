using System.Windows;
using System.Windows.Controls;
using WPFMediaKit.DirectShow.Controls;

namespace CameraThing
{
    public partial class ToolbarWindow : Window
    {
        private MainWindow _mainWindow;

        public ToolbarWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            
            // Populate camera sources
            if (MultimediaUtil.VideoInputDevices.Length > 0)
            {
                cobVideoSource.ItemsSource = MultimediaUtil.VideoInputNames;
            }
        }        private void cobVideoSource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cobVideoSource.SelectedIndex < 0)
            {
                // Stop any current camera capture
                _mainWindow.SetCameraDevice(null);
                return;
            }

            // Set the selected camera device
            _mainWindow.SetCameraDevice(MultimediaUtil.VideoInputDevices[cobVideoSource.SelectedIndex]);
        }

        public void UpdateDeeperColorBinding()
        {
            // Update the deeper color binding from main window
            chkDeeperColor.IsChecked = _mainWindow.GetDeeperColorValue();
        }

        private void chkDeeperColor_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _mainWindow.SetDeeperColorValue(chkDeeperColor.IsChecked == true);
        }
    }
}
