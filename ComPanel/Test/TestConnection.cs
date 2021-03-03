using System;
using System.Threading;

namespace ComPanel.Test
{
    class TestConnection
    {
        public Action<string> LogError { get; set; }
        public Action<byte[], int> OnDataRecieved { get; set; }

        private byte[] _testRecievedData = new byte[] { 0x00, 0x01, 0x02, 0x03 };
        private bool _started;

        public void Send(byte[] buffer, int length)
        {
            Thread.Sleep(new Random().Next(10, 200));
            OnDataRecieved?.Invoke(_testRecievedData, _testRecievedData.Length);
        }

        public void Connect()
        {
            if (_started)
                throw new Exception("TestConnection.Start() : Connection was already started");
            _started = true;
        }

        public void Disonnect()
        {
            if (!_started)
                throw new Exception("TestConnection.Stop() : Connection was not started");
            _started = false;
        }
    }
}
