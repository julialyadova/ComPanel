using System;

namespace ComPanel.Connection
{
    [Serializable]
    public class PortSettingsValues
    {
        public int[] BaudRateValues = new int[] {9600, 2400, 4800, 9600, 14400, 19200, 38400, 57600, 115200, 128000, 256000};
        public int[] DataBitsValues = new int[] { 5, 6, 7, 8};
        public int[] TimeoutValues = new int[] { 100, 200, 500, 1000 };
    }
}
