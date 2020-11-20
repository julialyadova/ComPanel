namespace ComPanel.RawData
{
    interface DataFormater
    {
        public string ToString(byte[] data, int length);
    }
}
