using ComPanel.Resources;
using ComPanel.Resources.Controls;
using System;
using System.Windows;

//TODO
//добавить кнопку обновления списка портов
//сохранять последние натстройки

namespace ComPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Action ConnectClick;
        public Action DisconnectClick;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TopBarGrid_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if (OptionsStackPanel.IsEnabled)
            {
                ConnectButton.Content = "Disconnect";
                OptionsStackPanel.IsEnabled = false;
                ConnectClick?.Invoke();
            }
            else
            {
                DisconnectClick?.Invoke();
                ConnectButton.Content = "Connect";
                OptionsStackPanel.IsEnabled = true;
            }
        }
    }
}
