using System;
using System.Collections.Generic;
using System.Text;

namespace ComPanel.RawData
{
    public class HexDataFormater : DataFormater
    {
        public string ToString(byte[] data, int length)
        {
            return BitConverter.ToString(data, 0, length).Replace('-', ' ');
        }
    }
}
