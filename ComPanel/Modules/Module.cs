using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ComPanel.Modules
{
    public interface Module
    {
        public string UniqueName { get; set; }
        public SendRawDataHandler Send { get; set; }
        public UserControl GetPanel();
        public string GetName();
    }

    public delegate void SendRawDataHandler(byte[] data, int length, Action<byte[], int> onDataRecieved = null);
}
