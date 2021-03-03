using System;
using System.IO.Ports;

namespace ComPanel.Connection
{
    class SerialPortConnection
    {
        private SerialPort _port;
        private byte[] _buffer;

        public Action<string> LogError { get; set; }
        public Action<byte[], int> OnDataRecieved { get; set; }

        private bool TryOpenPort()
        {
            try
            {
                _port.Open();
            }
            catch (Exception e)
            {
                LogError(e.Message);
                return false;
            }
            return true;
        }

        public void Connect(PortSettings portSettings)
        {
            _port = new SerialPort
            (
                portName:   portSettings.PortName,
                baudRate:   portSettings.BaudRate,
                parity:     portSettings.Parity,
                dataBits:   portSettings.DataBits,
                stopBits:   portSettings.StopBits
            );

            _port.ReadTimeout = portSettings.TimeOutMS;
            _port.WriteTimeout = portSettings.TimeOutMS;

            _buffer = new byte[_port.ReadBufferSize];

            if (!TryOpenPort())
                return;

            _port.DataReceived += DataRecievedEventHandler;
        }

        public void Disonnect()
        {
            _port.Close();
        }

        public void Send(byte[] buffer, int length)
        {
            if (_port != null && _port.IsOpen)
                _port.Write(buffer, 0, length);
        }

        private void DataRecievedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var length = _port.BytesToRead;
            _port.Read(_buffer, 0, length);
            OnDataRecieved?.Invoke(_buffer, length);
        }
    }
}
