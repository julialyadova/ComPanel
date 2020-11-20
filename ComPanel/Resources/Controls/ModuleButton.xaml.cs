using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ComPanel.Resources.Controls
{
    /// <summary>
    /// Interaction logic for ModuleButton.xaml
    /// </summary>
    public partial class ModuleButton : UserControl
    {
        public RoutedEventHandler Click;

        private Brush baseBrush;
        public ModuleButton(string name, bool isFirst = false)
        {
            InitializeComponent();

            Name.Content = name;
            baseBrush = CenterPart.Fill;

            if (isFirst)
                Shadow.Visibility = Visibility.Hidden;

            Name.Click += (sender, args) => Click?.Invoke(sender,args);
        }

        public ModuleButton()
        {
            InitializeComponent();
        }

        public void Highlight()
        {
            LeftPart.Fill = Brushes.White;
            RightPart.Fill = Brushes.White;
            CenterPart.Fill = Brushes.White;
        }

        public void Darken()
        {
            LeftPart.Fill = baseBrush;
            RightPart.Fill = baseBrush;
            CenterPart.Fill = baseBrush;
        }
    }
}
