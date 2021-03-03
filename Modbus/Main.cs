using ComPanel.Modules;
using System.Windows.Controls;

namespace Modbus
{
    public class Main : ComPanel.Modules.Module
    {
        public SendRawDataHandler Send { get; set; }
        public string UniqueName { get; set; }

        private Panel _panel;

        private ModbusFrameSettings _modbusSettings;
        private byte[] _buffer;
        private int _length;

        public Main()
        {
            _panel = new Panel();
            _modbusSettings = new ModbusFrameSettings();

            var data = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            _panel.SendButton.Click += (sender, args) =>
            {
                Send(data, data.Length, null);
            };
        }

        public UserControl GetPanel()
        {
            return _panel;
        }

        public string GetName()
        {
            return "Modbus";
        }

    }
}
