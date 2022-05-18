using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSX_Modder.Utilities
{
    public class ByteUtil
    {
        public static int ByteToBitConvert(byte Byte, int Start = 0, int End = 3)
        {
            byte[] array = new byte[1] { Byte };
            var bits = new BitArray(array);
            int Point = 1;
            int Number = 0;
            for (int i = Start; i < End + 1; i++)
            {
                Number += (bits[i] ? 1 : 0) * Point;
                Point = Point * 2;
            }
            return Number;
        }

        public static int BitConbineConvert(byte OneByte, byte TwoByte, int StartPoint = 0, int Length = 4, int Inset=4)
        {
            byte[] arrayOne = new byte[1] { OneByte };
            var bitsOne = new BitArray(arrayOne);
            arrayOne = new byte[1] { TwoByte };
            var bitsTwo = new BitArray(arrayOne);

            for (int i = StartPoint; i < Length; i++)
            {
                bitsOne[Inset + i] = bitsTwo[i];
            }


            int Point = 1;
            int Number = 0;

            for (int i = 0; i < 8; i++)
            {
                Number += (bitsOne[i] ? 1 : 0) * Point;
                Point = Point * 2;
            }

            return Number;
        }

        public static long FindPosition(Stream stream, byte[] byteSequence, long Start = -1, long End = -1)
        {
            int b;
            long i = 0;
            if (Start != -1)
            {
                stream.Position = Start;
            }
            while ((b = stream.ReadByte()) != -1)
            {
                if (End != -1)
                {
                    if (End <= stream.Position)
                    {
                        return -1;
                    }
                }
                if (b == byteSequence[i++])
                {
                    if (i == byteSequence.Length)
                        return stream.Position - byteSequence.Length;
                }
                else
                    i = b == byteSequence[0] ? 1 : 0;
            }
            return -1;
        }

        public static int simulateSwitching4th5thBit(int nr)
        {
            bool bit4 = (nr % 16) / 8 >= 1;
            bool bit5 = (nr % 32) / 16 >= 1;
            if (bit4 && !bit5)
            {
                return nr + 8;
            }
            if (!bit4 && bit5)
            {
                return nr - 8;
            }
            else
            {
                return nr;
            }
        }

        public static int ByteBitSwitch(int Byte, int Bit1 = 3, int Bit2 = 4)
        {
            byte[] array = new byte[1] { (byte)Byte };
            var bits = new BitArray(array);
            bool temp1 = bits[Bit2];
            bits[Bit2] = bits[Bit1];
            bits[Bit1] = temp1;
            int Number = 0;
            int Point = 1;
            for (int i = 0; i < 8; i++)
            {
                Number += (bits[i] ? 1 : 0) * Point;
                Point = Point * 2;
            }
            return Number;
        }

        public static int BytesBitSwitch(byte[] Bytes, int Bit1 = 3, int Bit2 = 4)
        {
            byte[] array = Bytes;
            var bits = new BitArray(array);
            bool temp1 = bits[Bit2];
            bits[Bit2] = bits[Bit1];
            bits[Bit1] = temp1;
            int Number = 0;
            int Point = 1;
            for (int i = 0; i < bits.Length; i++)
            {
                Number += (bits[i] ? 1 : 0) * Point;
                Point = Point * 2;
            }
            return Number;
        }
    }
}

/*Point decodePixelLocation(Dimension imageDimensions, Dimension imageBlockDimensions, Point pxlLocation)
{
    final int pixelLocation = pxlLocation.x * imageDimensions.width + pxlLocation.y;

    int result = pixelLocation;
    if (!rowBit1EqualsRowBit2(imageDimensions, result))
    {
        result = toggleBit(result, 2);  // info += " -neq r1,r2 ? ^c2-> " + asBin(result); // 4,0 -> 4,16 (^b2 <<2)
    }
    result = swapRowBit0Bit1(imageDimensions, result); // info += " -Sw Ro 0,1-> " + asBin(result); // 1,0 -> 2,0

    result = rotateColumnBlockBitsLeft(imageBlockDimensions, result);  // info += " -RL1 c0_c3-> " + asBin(result); // 0,32 -> 32,0

    result = rotateColumnWithOneBitOfRowLeft(imageDimensions, result);  // info += " -RL1 c0_r1-> " + asBin(result); // 0,32 -> 32,0
    return new Point(result / imageDimensions.width, result % imageDimensions.width);
}
*/