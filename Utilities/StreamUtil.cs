using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SSX_Modder.Utilities
{
    class StreamUtil
    {
        public static string ReadNullEndString(Stream stream)
        {
            bool tillNull = false;
            int a = 0;
            while (!tillNull)
            {
                int temp1 = stream.ReadByte();
                if (temp1 == 0x00)
                {
                    tillNull = true;
                }
                else
                {
                    a++;
                }
            }
            stream.Position -= a + 1;
            byte[] FilePath = new byte[a];
            stream.Read(FilePath, 0, a);
            return Encoding.ASCII.GetString(FilePath);
        }

        public static string ReadString(Stream stream, int Length)
        {
            byte[] tempByte = new byte[Length];
            stream.Read(tempByte, 0, tempByte.Length);
            return Encoding.ASCII.GetString(tempByte);
        }

        public static int ReadInt16(Stream stream)
        {
            byte[] tempByte = new byte[2];
            stream.Read(tempByte, 0, tempByte.Length);
            return BitConverter.ToInt16(tempByte, 0);
        }

        public static int ReadInt12(Stream stream)
        {
            byte[] tempByte = new byte[2];
            stream.Read(tempByte, 0, tempByte.Length);
            return ByteUtil.BytesToBitConvert(tempByte,4,15);
        }

        public static int ReadInt24(Stream stream)
        {
            byte[] tempByte = new byte[4];
            stream.Read(tempByte, 0, 3);
            return BitConverter.ToInt32(tempByte, 0);
        }

        public static int ReadInt24Big(Stream stream)
        {
            byte[] tempByte = new byte[4];
            stream.Read(tempByte, 0, 3);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempByte);
            for (int bi = 1; bi < tempByte.Length; bi++)
            {
                tempByte[bi - 1] = tempByte[bi];
            }
            tempByte[3] = 0x00;
            return BitConverter.ToInt32(tempByte, 0);
        }

        public static int ReadInt32(Stream stream)
        {
            byte[] tempByte = new byte[4];
            stream.Read(tempByte, 0, tempByte.Length);
            return BitConverter.ToInt32(tempByte, 0);
        }

        public static int ReadInt32Big(Stream stream)
        {
            byte[] tempByte = new byte[4];
            stream.Read(tempByte, 0, tempByte.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempByte);
            return BitConverter.ToInt32(tempByte, 0);
        }

        public static Color ReadColour(Stream stream)
        {
            int R = stream.ReadByte();
            int G = stream.ReadByte();
            int B = stream.ReadByte();
            int A = stream.ReadByte() * 2 - 1;
            if (A < 0)
            {
                A = 0;
            }
            else if (A > 255)
            {
                A = 255;
            }
            return Color.FromArgb(A, R, G, B);
        }

        public static void WriteNullString(Stream stream, string String)
        {
            byte[] tempByte = new byte[String.Length + 1];
            Encoding.ASCII.GetBytes(String).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);
        }

        public static void WriteString(Stream stream, string String, int Length = 0)
        {
            int tempLength = String.Length;
            if (Length != 0)
            {
                tempLength = Length;
            }
            byte[] tempByte = new byte[tempLength];
            Encoding.ASCII.GetBytes(String).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);
        }

        public static void WriteInt8(Stream stream, int Int)
        {
            byte[] tempByte = new byte[4];
            BitConverter.GetBytes(Int).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, 1);
        }

        public static void WriteInt16(Stream stream, int Int)
        {
            byte[] tempByte = new byte[4];
            BitConverter.GetBytes(Int).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, 2);
        }

        public static void WriteInt24(Stream stream, int Int)
        {
            byte[] tempByte = new byte[4];
            BitConverter.GetBytes(Int).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, 3);
        }

        public static void WriteInt32(Stream stream, int Int)
        {
            byte[] tempByte = new byte[4];
            BitConverter.GetBytes(Int).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);
        }

        public static void WriteInt32Big(Stream stream, int Int)
        {
            byte[] tempByte = new byte[4];
            BitConverter.GetBytes(Int).CopyTo(tempByte, 0);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempByte);
            stream.Write(tempByte, 0, tempByte.Length);
        }
    }
}
