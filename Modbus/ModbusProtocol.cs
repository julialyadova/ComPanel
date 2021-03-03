using System;

namespace Modbus
{
    public interface Protocol
    {
        int FormRequestFrame(byte[] buffer);
    }

    public class ModbusFrameSettings
    {
        public byte DeviceAddress = 1;
        public ModbusFunction Function = ModbusFunction.InputRegisters;
        public ushort LogicalStartRegister = 1;
        public ushort RegistersCount = 1;
    }

    public class ModbusProtocol : Protocol
    {
        private CRC16 CRC16;
        private ModbusFrameSettings settings;

        public ModbusProtocol(ModbusFrameSettings settings)
        {
            CRC16 = new CRC16();
            this.settings = settings;
        }

        public int FormRequestFrame(byte[] buffer)
        {
            if (settings.Function == ModbusFunction.InputRegisters)
                return FormReadInputRegistersFrame(buffer);
            else if (settings.Function == ModbusFunction.HoldingRegisters)
                return FormReadHoldingRegistersFrame(buffer);
            else
                return 0;
        }

        public static int ExtractData(byte[] buffer, byte[] frame)
        {
            if (frame.Length < 4)
                return 0;

            if (frame[1] == 0x03 || frame[1] == 0x04)
                return ExtractInputOrHoldingRegistersData(buffer, frame);
            else
                return 0;
        }

        private int FormReadInputRegistersFrame(byte[] buffer)
        {
            buffer[0] = settings.DeviceAddress;
            buffer[1] = 0x04;
            buffer[2] = (byte)((settings.LogicalStartRegister - 1) >> 8);
            buffer[3] = (byte)((settings.LogicalStartRegister - 1) & 0xff);
            buffer[4] = (byte)(settings.RegistersCount >> 8);
            buffer[5] = (byte)(settings.RegistersCount & 0xff);
            CRC16.WriteFrameChecksum(buffer, 8);
            return 8;
        }

        private int FormReadHoldingRegistersFrame(byte[] buffer)
        {
            buffer[0] = settings.DeviceAddress;
            buffer[1] = 0x03;
            buffer[2] = (byte)((settings.LogicalStartRegister - 1) >> 8);
            buffer[3] = (byte)((settings.LogicalStartRegister - 1) & 0xff);
            buffer[4] = (byte)(settings.RegistersCount >> 8);
            buffer[5] = (byte)(settings.RegistersCount & 0xff);
            CRC16.WriteFrameChecksum(buffer, 8);
            return 8;
        }

        private static int ExtractInputOrHoldingRegistersData(byte[] buffer, byte[] frame)
        {
            int dataLength = frame[2];
            for (int i = 0; i < Math.Min(dataLength, buffer.Length); i++)
            {
                buffer[i] = frame[3 + i];
            }
            return dataLength;
        }
    }

    public enum ModbusFunction
    {
        InputRegisters,
        HoldingRegisters
    }
}
