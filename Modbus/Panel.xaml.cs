using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Modbus
{
    /// <summary>
    /// Interaction logic for Panel.xaml
    /// </summary>
    public partial class Panel : UserControl
    {
        public int ScanRate;
        private Regex regex = new Regex("[^0-9]+");

        public Panel()
        {
            InitializeComponent();
        }

        private void ScanRateTextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = (sender as TextBox);
            textbox.Text = Regex.Replace(textbox.Text, "[^0-9]*", "");
            textbox.SelectionStart = textbox.Text.Length;
        }
    }
}
