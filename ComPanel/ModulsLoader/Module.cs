using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace ComPanel.ModulesLoader
{
    public interface Module
    {
        public SendRawDataHandler Send { get; set; }

        public UserControl GetPanel();
    }

    public delegate void SendRawDataHandler(byte[] data, int length, Action<byte[], int> onDataRecieved = null);
}
