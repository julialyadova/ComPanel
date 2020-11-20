using System;

namespace ComPanel.Connection
{
    public interface Connection
    {
        public Action<string> LogError { get; set; }
        public Action<byte[], int> OnDataRecieved { get; set; }

        public void Start();
        public void Stop();
        public void Send(byte[] buffer, int length);
    }
}
