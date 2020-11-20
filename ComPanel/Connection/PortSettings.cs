using System.Collections.Generic;
using System.IO.Ports;
using System;

namespace ComPanel.Connection
{
    public class PortSettings
    {
        public string PortName = "COM1";
        public int BaudRate = 9600;
        public Parity Parity = Parity.None;
        public int DataBits = 8;
        public StopBits StopBits = StopBits.One;
        public int TimeOutMS = 1000;

        public static IEnumerable<string> GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public static IEnumerable<int> GetCommonBaudRates()
        {
            return new int[] { 9600, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000 };
        }
    }
}
