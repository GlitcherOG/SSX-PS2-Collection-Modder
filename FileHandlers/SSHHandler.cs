using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Forms;

namespace SSX_Modder.FileHandlers
{
    class SSHHandler
    {
        public string MagicWord;
        public int Size;
        public int Ammount;
        public string format;
        public string group;
        public string endingstring;
        public List<SSHImage> sshImages = new List<SSHImage>();
        public void LoadSSH(string path)
        {
            sshImages = new List<SSHImage>();
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                byte[] tempByte = new byte[4];
                long pos = FindPosition(stream, new byte[] { 0x53, 0x48, 0x50, 0x53 });
                stream.Position = 0;
                if (pos != -1)
                {
                    stream.Position = pos;
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    MagicWord = Encoding.ASCII.GetString(tempByte);
                }
                if (MagicWord == "SHPS")
                {
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    Size = BitConverter.ToInt32(tempByte, 0);

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    Ammount = BitConverter.ToInt32(tempByte, 0);

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    format = Encoding.ASCII.GetString(tempByte);

                    try
                    {
                        StandardToBitmap(stream, (int)stream.Position);
                    }
                    catch
                    {
                        sshImages = new List<SSHImage>();
                        MessageBox.Show("Error reading File " + MagicWord + " "+ format);
                    }
                }
            }
        }

        public void StandardToBitmap(Stream stream, int offset)
        {
            stream.Position = offset;
            byte[] tempByte;
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

            tempByte = new byte[4];
            stream.Read(tempByte, 0, tempByte.Length);
            group = Encoding.ASCII.GetString(tempByte);

            tempByte = new byte[4];
            stream.Read(tempByte, 0, tempByte.Length);
            endingstring = Encoding.ASCII.GetString(tempByte);

            for (int i = 0; i < sshImages.Count; i++)
            {
                SSHImage tempImage = sshImages[i];
                SSHImageHeader tempImageHeader = new SSHImageHeader();

                stream.Position = tempImage.offset;

                tempByte = new byte[1];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImageHeader.MatrixFormat = tempByte[0];

                tempByte = new byte[4];
                stream.Read(tempByte, 0, 3);
                tempImageHeader.Size = BitConverter.ToInt32(tempByte, 0)-16;

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

                tempByte = new byte[tempImageHeader.Size];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImage.Matrix = tempByte;

                //stream.Position += 10;
                //INDEXED COLOUR
                if (tempImageHeader.MatrixFormat == 2)
                {
                    stream.Position += 16;
                    tempImage.colorTable = new List<Color>();
                    for (int a = 0; a < 256; a++)
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
                        //Replace with proper check
                        if (R == 112 && G == 0 && B == 0 && A == 0)
                        {
                            stream.Position -= 4;
                            break;
                        }
                        tempImage.colorTable.Add(Color.FromArgb(A, R, G, B));
                    }
                }

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImage.unknownEnd = BitConverter.ToInt32(tempByte, 0);

                bool tillNull = false;
                int t = 0;
                while (!tillNull)
                {
                    int temp1 = stream.ReadByte();
                    if (temp1 == 0x00)
                    {
                        tillNull = true;
                    }
                    else
                    {
                        t++;
                    }
                }
                stream.Position -= t + 1;
                tempByte = new byte[t];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImage.longname = Encoding.ASCII.GetString(tempByte);

                tempImage.bitmap = new Bitmap(tempImageHeader.Width, tempImageHeader.Height, PixelFormat.Format32bppArgb);
                int post = 0;
                if (tempImageHeader.MatrixFormat == 2)
                {
                    for (int y = 0; y < tempImageHeader.Height; y++)
                    {
                        for (int x = 0; x < tempImageHeader.Width; x++)
                        {
                            int colorPos = tempImage.Matrix[post];
                            colorPos = simulateSwitching4th5thBit(colorPos);
                            tempImage.bitmap.SetPixel(x, y, tempImage.colorTable[colorPos]);
                            post++;
                        }
                    }
                }
                else
                if (tempImageHeader.MatrixFormat == 5)
                {
                    for (int y = 0; y < tempImageHeader.Height; y++)
                    {
                        for (int x = 0; x < tempImageHeader.Width; x++)
                        {
                            int R = tempImage.Matrix[post];
                            post++;
                            int G = tempImage.Matrix[post];
                            post++;
                            int B = tempImage.Matrix[post];
                            post++;
                            int A = 255;//tempImage.Matrix[post] * 2 - 1;
                            //post++;
                            if (A < 0)
                            {
                                A = 0;
                            }
                            else if (A > 255)
                            {
                                A = 255;
                            }
                            //tempImage.colorTable.Add(Color.FromArgb(A, R, G, B));
                            tempImage.bitmap.SetPixel(x, y, Color.FromArgb(A, R, G, B));
                            post++;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error reading File " + MagicWord + " " + format + "- Matrix " + tempImageHeader.MatrixFormat.ToString());
                }
                tempImage.sshHeader = tempImageHeader;
                sshImages[i] = tempImage;
            }
        }

        public void BMPExtract(string path)
        {
            for (int i = 0; i < sshImages.Count; i++)
            {
                sshImages[i].bitmap.Save(path+"\\"+ sshImages[i].shortname+"."+ sshImages[i].longname +".bmp");
            }
        }

        public static long FindPosition(Stream stream, byte[] byteSequence)
        {
            int b;
            long i = 0;
            while ((b = stream.ReadByte()) != -1)
            {
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
        public string longname;
        public int unknownEnd;
        public int offset;
        public SSHImageHeader sshHeader;
        public byte[] Matrix;
        public List<Color> colorTable;
        public Bitmap bitmap;
    }
    struct SSHImageHeader
    {
        public byte MatrixFormat;
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
