using System;
using System.IO.Ports;

namespace ComPanel.Connection
{
    class SerialPortConnection : Connection
    {
        private SerialPort _port;
        private byte[] _buffer;

        public Action<string> LogError { get; set; }
        public Action<byte[], int> OnDataRecieved { get; set; }

        public SerialPortConnection(PortSettings portSettings)
        {
            _port = new SerialPort(
                portName: portSettings.PortName,
                baudRate: portSettings.BaudRate,
                parity: portSettings.Parity,
                dataBits: portSettings.DataBits,
                stopBits: portSettings.StopBits);
            _port.ReadTimeout = portSettings.TimeOutMS;
            _port.WriteTimeout = portSettings.TimeOutMS;
            _buffer = new byte[_port.ReadBufferSize];
        }

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

        public void Start()
        {
            if (!TryOpenPort())
                return;

            if (OnDataRecieved != null)
                _port.DataReceived += DataRecievedEventHandler;
        }

        public void Stop()
        {
            _port.Close();
        }

        public void Send(byte[] buffer, int length)
        {
            if (_port.IsOpen)
                _port.Write(buffer, 0, length);
        }

        private void DataRecievedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var length = _port.BytesToRead;
            _port.Read(_buffer, 0, length);
            OnDataRecieved(_buffer, length);
        }
    }
}
