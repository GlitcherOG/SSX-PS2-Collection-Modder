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
                long pos = FindPosition(stream, new byte[] { 0x53, 0x48, 0x50, 0x53 }); //SHPS
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
                        MessageBox.Show("Error reading File " + MagicWord + " "+ format + " " + sshImages[0].Matrix);
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
                tempImageHeader.Size = BitConverter.ToInt32(tempByte, 0);

                tempByte = new byte[2];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImageHeader.Width = BitConverter.ToInt16(tempByte, 0);

                tempByte = new byte[2];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImageHeader.Height = BitConverter.ToInt16(tempByte, 0);

                tempByte = new byte[2];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImageHeader.Xaxis = BitConverter.ToInt16(tempByte, 0);

                tempByte = new byte[2];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImageHeader.Yaxis = BitConverter.ToInt16(tempByte, 0);

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImageHeader.Format = BitConverter.ToInt32(tempByte, 0);


                int RealSize;
                
                if(tempImageHeader.Size !=0)
                {
                    RealSize = tempImageHeader.Size-16;
                }
                else
                {
                    RealSize = tempImageHeader.Width * tempImageHeader.Height;
                    if (tempImageHeader.MatrixFormat == 5)
                    {
                        RealSize = RealSize * 4;
                    }
                }

                tempByte = new byte[RealSize];
                stream.Read(tempByte, 0, tempByte.Length);
                tempImage.Matrix = tempByte;

                //stream.Position += Size-RealSize;
                //INDEXED COLOUR
                if (tempImageHeader.MatrixFormat == 2 || tempImageHeader.MatrixFormat == 1)
                {
                    int Spos = (int)stream.Position;
                    bool find = false;
                    while (!find)
                    {
                        if (stream.ReadByte() == 0x21)
                        {
                            find = true;
                        }
                    }
                    SSHColourTable sshTable = new SSHColourTable();

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length-1);
                    sshTable.Size = BitConverter.ToInt32(tempByte, 0);

                    tempByte = new byte[2];
                    stream.Read(tempByte, 0, tempByte.Length);
                    sshTable.Width = BitConverter.ToInt16(tempByte, 0);

                    tempByte = new byte[2];
                    stream.Read(tempByte, 0, tempByte.Length);
                    sshTable.Height = BitConverter.ToInt16(tempByte, 0);

                    tempByte = new byte[2];
                    stream.Read(tempByte, 0, tempByte.Length);
                    sshTable.Total = BitConverter.ToInt16(tempByte, 0);

                    stream.Position += 6;
                    sshTable.colorTable = new List<Color>();
                    //for (int c = 0; c < 256; c++)
                    //{
                    //    sshTable.colorTable.Add(Color.FromArgb(255, 160, 32, 240));
                    //}
                    int tempSize = (sshTable.Size / 4) - 4;
                    if (sshTable.Size==0)
                    {
                        tempSize = sshTable.Total;
                    }

                    for (int a = 0; a < tempSize; a++)
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
                        sshTable.colorTable.Add(Color.FromArgb(A, R, G, B));
                    }
                    tempImage.sshTable = sshTable;
                }
                int test = (int)stream.Position;
                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                if (tempByte[0] == 0x70)
                {
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
                }
                tempImage.bitmap = new Bitmap(tempImageHeader.Width, tempImageHeader.Height, PixelFormat.Format32bppArgb);
                int post = 0;
                //if (tempImageHeader.MatrixFormat == 0 /*1*/)
                //{
                //    for (int y = 0; y < tempImageHeader.Height; y++)
                //    {
                //        for (int x = 0; x < tempImageHeader.Width; x++)
                //        {
                //            int colorPos = tempImage.Matrix[post];
                //            //colorPos = simulateSwitching4th5thBit(colorPos);
                //            tempImage.bitmap.SetPixel(x, y, tempImage.sshTable.colorTable[colorPos]);
                //            post++;
                //        }
                //    }
                //}
                //else
                if (tempImageHeader.MatrixFormat == 2)
                {
                    for (int y = 0; y < tempImageHeader.Height; y++)
                    {
                        for (int x = 0; x < tempImageHeader.Width; x++)
                        {
                            int colorPos = tempImage.Matrix[post];
                            colorPos = simulateSwitching4th5thBit(colorPos);
                            tempImage.bitmap.SetPixel(x, y, tempImage.sshTable.colorTable[colorPos]);
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
                            int A = tempImage.Matrix[post];
                            post++;
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
                            //post++;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error reading File " + MagicWord + " " + format + "- Matrix " + tempImageHeader.MatrixFormat.ToString());
                    break;
                }
                tempImage.sshHeader = tempImageHeader;
                sshImages[i] = tempImage;
            }
            stream.Dispose();
        }

        public void BMPExtract(string path)
        {
            string index = "";
            for (int i = 0; i < sshImages.Count; i++)
            {
                index += sshImages[i].shortname + "." + sshImages[i].longname + ".png" +Environment.NewLine;
                sshImages[i].bitmap.Save(path+"\\"+ sshImages[i].shortname+"."+ sshImages[i].longname +".png",ImageFormat.Png);
            }

            //File.Create(path + "\\Index.txt");
            File.WriteAllText(path + "\\Index.txt", index);
        }

        public void LoadFolder(string path)
        {
            MagicWord = "";
            Size = 0;
            format = "G???";
            group = "";
            endingstring = "";
            sshImages = new List<SSHImage>();
            string[] paths = File.ReadAllLines(path + "\\Index.txt");
            Ammount = paths.Length;
            for (int i = 0; i < paths.Length; i++)
            {
                paths[i] = path + "\\" + paths[i];
            }

            for (int i = 0; i < paths.Length; i++)
            {
                SSHImage tempImage = new SSHImage();
                SSHImageHeader imageHeader = new SSHImageHeader();
                var tempImage1 = //Image.FromFile(paths[i]);
                //tempImage1.Save(path + "/Temp.bmp");
                tempImage.bitmap = (Bitmap)Image.FromFile(paths[i]);

                imageHeader.MatrixFormat = 2;

                string name = Path.GetFileName(paths[i].Replace(".png", ""));
                string[] NameList = name.Split('.');

                tempImage.longname = NameList[1];
                tempImage.shortname = NameList[0];
                imageHeader.Width = tempImage.bitmap.Width;
                imageHeader.Height = tempImage.bitmap.Height;

                tempImage.sshHeader = imageHeader;
                sshImages.Add(tempImage);
            }
            File.Delete(path + "/Temp.bmp");
        }

        public void SaveSSH(string path)
        {
            Stream stream = new MemoryStream();
            MagicWord = "SHPS";
            byte[] tempByte = new byte[4];
            Encoding.ASCII.GetBytes(MagicWord).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);

            tempByte = new byte[4];
            stream.Write(tempByte, 0, tempByte.Length);

            tempByte = new byte[4];
            BitConverter.GetBytes(sshImages.Count).CopyTo(tempByte,0);
            stream.Write(tempByte, 0, tempByte.Length);

            tempByte = new byte[4];
            Encoding.ASCII.GetBytes(format).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);

            List<int> intPos = new List<int>();

            for (int i = 0; i < sshImages.Count; i++)
            {
                tempByte = new byte[4];
                Encoding.ASCII.GetBytes(sshImages[i].shortname).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);
                intPos.Add((int)stream.Position);
                tempByte = new byte[4];
                stream.Write(tempByte, 0, tempByte.Length);
            }

            tempByte = new byte[16];
            Encoding.ASCII.GetBytes("Buy ERTS").CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);

            for (int i = 0; i < sshImages.Count; i++)
            {
                int temp = (int)stream.Position;
                stream.Position = intPos[i];

                //Set Start Offset

                tempByte = new byte[4];
                BitConverter.GetBytes(temp).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                stream.Position = temp;

                tempByte = new byte[4];
                BitConverter.GetBytes(sshImages[i].sshHeader.MatrixFormat).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, 1);


                //Set SSH Header Info
                if (sshImages[i].sshHeader.MatrixFormat == 2)
                {
                    tempByte = new byte[4];
                    BitConverter.GetBytes(sshImages[i].sshHeader.Width* sshImages[i].sshHeader.Height+16).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 3);
                }
                else if (sshImages[i].sshHeader.MatrixFormat == 5)
                {
                    tempByte = new byte[4];
                    BitConverter.GetBytes((sshImages[i].sshHeader.Width * sshImages[i].sshHeader.Height*4) + 16).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 3);
                }

                tempByte = new byte[4];
                BitConverter.GetBytes(sshImages[i].sshHeader.Width).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, 2);

                tempByte = new byte[4];
                BitConverter.GetBytes(sshImages[i].sshHeader.Height).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, 2);

                tempByte = new byte[4];
                BitConverter.GetBytes(sshImages[i].sshHeader.Xaxis).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, 2);

                tempByte = new byte[4];
                BitConverter.GetBytes(sshImages[i].sshHeader.Yaxis).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, 2);

                tempByte = new byte[4];
                BitConverter.GetBytes(sshImages[i].sshHeader.Format).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                if (sshImages[i].sshHeader.MatrixFormat == 2)
                {
                    SSHColourTable colourTable = new SSHColourTable();
                    colourTable.colorTable = new List<Color>();
                    //colourTable.colorTable.Add(Color.FromArgb(0, 0, 0, 0));
                    for (int y = 0; y < sshImages[i].bitmap.Height; y++)
                    {
                        for (int x = 0; x < sshImages[i].bitmap.Width; x++)
                        {
                            Color color = sshImages[i].bitmap.GetPixel(x, y);
                            //color.A = (color.A - 1) / 2;
                            if(colourTable.colorTable.Contains(color))
                            {
                                int index = colourTable.colorTable.IndexOf(color);
                                index = simulateSwitching4th5thBit(index);
                                tempByte = new byte[4];
                                BitConverter.GetBytes(index).CopyTo(tempByte, 0);
                                stream.Write(tempByte, 0, 1);
                            }
                            else
                            {
                                colourTable.colorTable.Add(color);
                                int index = colourTable.colorTable.Count - 1;
                                index = simulateSwitching4th5thBit(index);
                                tempByte = new byte[4];
                                BitConverter.GetBytes(index).CopyTo(tempByte, 0);
                                stream.Write(tempByte, 0, 1);
                            }
                        }
                    }

                    //Colour Table
                    tempByte = new byte[4];
                    BitConverter.GetBytes(0x21).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 1);

                    tempByte = new byte[4];
                    BitConverter.GetBytes((colourTable.colorTable.Count*4)+16).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 3);

                    tempByte = new byte[4];
                    BitConverter.GetBytes(colourTable.colorTable.Count).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 2);

                    tempByte = new byte[4];
                    BitConverter.GetBytes(1).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 2);

                    tempByte = new byte[4];
                    BitConverter.GetBytes(colourTable.colorTable.Count).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, 2);

                    tempByte = new byte[6] { 0x00, 0x00, 0x00, 0x20, 0x00, 0x00 };
                    //BitConverter.GetBytes(colourTable.colorTable.Count).CopyTo(tempByte, 0);
                    stream.Write(tempByte, 0, tempByte.Length);

                    for (int a = 0; a < colourTable.colorTable.Count; a++)
                    {
                        tempByte = new byte[4];
                        int R = colourTable.colorTable[a].R;
                        int G = colourTable.colorTable[a].G;
                        int B = colourTable.colorTable[a].B;
                        int A = (colourTable.colorTable[a].A+1)/2;
                        BitConverter.GetBytes(R).CopyTo(tempByte, 0);
                        stream.Write(tempByte, 0, 1);
                        tempByte = new byte[4];
                        BitConverter.GetBytes(G).CopyTo(tempByte, 0);
                        stream.Write(tempByte, 0, 1);
                        tempByte = new byte[4];
                        BitConverter.GetBytes(B).CopyTo(tempByte, 0);
                        stream.Write(tempByte, 0, 1);
                        tempByte = new byte[4];
                        BitConverter.GetBytes(A).CopyTo(tempByte, 0);
                        stream.Write(tempByte, 0, 1);
                    }
                }

                //ending
                tempByte = new byte[4] { 0x70, 0x00,0x00,0x00};
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[sshImages[i].longname.Length+1];
                Encoding.ASCII.GetBytes(sshImages[i].longname).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[9];
                Encoding.ASCII.GetBytes("Buy ERTS").CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var file = File.Create(path);
            stream.Position = 0;
            stream.CopyTo(file);
            stream.Dispose();
            file.Close();
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
    }


    struct SSHImage
    {
        public string shortname;
        public string longname;
        public int unknownEnd;
        public int offset;
        public SSHImageHeader sshHeader;
        public byte[] Matrix;
        public SSHColourTable sshTable;
        public Bitmap bitmap;
    }
    struct SSHImageHeader
    {
        public byte MatrixFormat;
        public int Size;
        public int Width;
        public int Height;
        public int Xaxis;
        public int Yaxis;
        public int Format;
    }

    struct SSHColourTable
    {
        public int Size;
        public int Width;
        public int Height;
        public int Total;
        public List<Color> colorTable;
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
