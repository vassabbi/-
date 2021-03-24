using System;
using System.Collections.Generic;
using System.Text;

namespace Tracert
{
    public class ICMP
    {
        public byte Type;
        public byte Code;
        public UInt16 CheckSum;
        public int MessageSize;
        public byte[] Message = new byte[1024];
        public const int indentType = 20;
        public const int indentCode = 21;
        public const int indentCheckSum = 22;
        public const int indentData = 24;

        public ICMP()
        {
        }
        public ICMP(byte[] data, int size)
        {
            Type = data[indentType];
            Code = data[indentCode];
            CheckSum = BitConverter.ToUInt16(data, indentCheckSum);
            MessageSize = size - indentData;
            Buffer.BlockCopy(data, indentData, Message, 0, MessageSize);
        }

        public byte[] getBytes()
        {
            byte[] data = new byte[MessageSize + 9];
            Buffer.BlockCopy(BitConverter.GetBytes(Type), 0, data, 0, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(Code), 0, data, 1, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(CheckSum), 0, data, 2, 2);
            Buffer.BlockCopy(Message, 0, data, 4, MessageSize);
            return data;
        }
        public UInt16 getChecksum()
        {
            UInt32 chcksm = 0;
            byte[] data = getBytes();
            int packetsize = MessageSize + 8;
            int index = 0;

            while (index < packetsize)
            {
                chcksm += Convert.ToUInt32(BitConverter.ToUInt16(data, index));
                index += 2;
            }
            chcksm = (chcksm >> 16) + (chcksm & 0xffff);
            chcksm += (chcksm >> 16);
            return (UInt16)(~chcksm);
        }
    }
}
