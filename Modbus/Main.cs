using ComPanel.ModulesLoader;
using System.Windows.Controls;

namespace Modbus
{
    public class Main : ComPanel.ModulesLoader.Module
    {
        public SendRawDataHandler Send { get; set; }

        private Panel _panel;        

        public Main()
        {
            _panel = new Panel();

            var data = new byte[] { 0x01, 0x00, 0xF1, 0xFF };
            _panel.SendButton.Click += (sender, args) =>
            {
                Send(data, data.Length, null);
            };
        }

        public UserControl GetPanel()
        {
            return _panel;
        }

    }
}
