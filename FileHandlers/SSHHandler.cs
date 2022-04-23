using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace SSX_Modder.FileHandlers
{
    class SSHHandler
    {
        public byte[] MagicWord;
        public int Size;
        public int Ammount;
        public string format;
        public List<SSHImage> sshImages = new List<SSHImage>();
        public void LoadSSH(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                byte[] tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                MagicWord = tempByte;

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                Size = BitConverter.ToInt32(tempByte,0);

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                Ammount = BitConverter.ToInt32(tempByte, 0);

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                format = Encoding.ASCII.GetString(tempByte);

                for (int i = 0; i < Ammount; i++)
                {
                    SSHImage tempImage = new SSHImage();

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImage.shortname = Encoding.ASCII.GetString(tempByte);

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImage.offset = BitConverter.ToInt32(tempByte, 0);

                    sshImages.Add(tempImage);
                }

                for (int i = 0; i < sshImages.Count; i++)
                {
                    SSHImage tempImage = sshImages[i];
                    SSHImageHeader tempImageHeader = new SSHImageHeader();

                    stream.Position = tempImage.offset;

                    tempByte = new byte[1];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImageHeader.Unknown = (sbyte)tempByte[0];

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, 3);
                    tempImageHeader.Size = BitConverter.ToInt32(tempByte,0);

                    tempByte = new byte[2];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImageHeader.Width = BitConverter.ToInt16(tempByte, 0);

                    tempByte = new byte[2];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImageHeader.Height = BitConverter.ToInt16(tempByte, 0);

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImageHeader.Unused = BitConverter.ToInt32(tempByte, 0);

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImageHeader.Format = BitConverter.ToInt32(tempByte, 0);

                    tempByte = new byte[tempImageHeader.Width * tempImageHeader.Height];
                    stream.Read(tempByte, 0, tempByte.Length);
                    tempImage.Matrix = tempByte;

                    stream.Position += 16;
                    tempImage.colorTable = new List<Color>();
                    for (int a = 0; a < 256; a++)
                    {
                        int R = stream.ReadByte();
                        int G = stream.ReadByte();
                        int B = stream.ReadByte();
                        int A = stream.ReadByte()*2-1;
                        if (A < 0)
                        {
                            A = 0;
                        }
                        else if(A > 255)
                        {
                            A = 255;
                        }

                        tempImage.colorTable.Add(Color.FromArgb(A, R, G, B));
                    }

                    tempImage.bitmap = new Bitmap(tempImageHeader.Width, tempImageHeader.Height, PixelFormat.Format32bppArgb);
                    int post = 0;
                    for (int y = 0; y < tempImageHeader.Height; y++)
                    {
                        for (int x = 0; x < tempImageHeader.Width; x++)
                        {
                            int colorPos = tempImage.Matrix[post];
                            tempImage.bitmap.SetPixel(x, y, tempImage.colorTable[simulateSwitching4th5thBit(colorPos)]);
                            post++;
                        }
                    }

                    sshImages[i] = tempImage;
                }
            }
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

        //Add check for 256 colours
        void Test()
        {
            Bitmap bitmap = new Bitmap(215, 215, PixelFormat.Format8bppIndexed);
            BitmapData bitData = bitmap.LockBits(new Rectangle(0, 0,bitmap.Width,bitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            ColorPalette pal = bitmap.Palette;
            //bitmap.Palette.Entries[0] = 1;
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = Color.FromArgb(0, 0, 0, 0);
            }

        }
    }

    struct SSHImage
    {
        public string shortname;
        public int offset;
        public SSHImageHeader sshHeader;
        public byte[] Matrix;
        public List<Color> colorTable;
        public Bitmap bitmap;
    }
    struct SSHImageHeader
    {
        public sbyte Unknown;
        public int Size;
        public int Width;
        public int Height;
        public int Unused;
        public int Format;
    }

    //[StructLayout(LayoutKind.Sequential)]
    //public struct UInt24
    //{
    //    private Byte _b0;
    //    private Byte _b1;
    //    private Byte _b2;

    //    public UInt24(UInt32 value)
    //    {
    //        _b0 = (byte)((value) & 0xFF);
    //        _b1 = (byte)((value >> 8) & 0xFF);
    //        _b2 = (byte)((value >> 16) & 0xFF);
    //    }

    //    public unsafe Byte* Byte0 { get { return (byte*)_b0; } }

    //    public UInt32 Value { get { return (uint)(_b0 | (_b1 << 8) | (_b2 << 16)); } }
    //}
}
