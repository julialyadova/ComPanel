namespace ComPanel.RawData
{
    public interface Checksum
    {
        public void Write(byte[] data, int length);
        public bool Verify(byte[] data, int length);
    }
}
