using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSX_Modder.FileHandlers
{
    class BigFHandler
    {
        public BIGFHeader bigHeader;
        public List<BIGFFiles> bigFiles;
        string bigPath;
        bool BuildMode;
        public void LoadBig(string path)
        {
            BuildMode = false;
            bigPath = path;
            bigHeader = new BIGFHeader();
            bigFiles = new List<BIGFFiles>();
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                byte[] tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                bigHeader.MagicWords = Encoding.ASCII.GetString(tempByte);
                //Figure out what any of these mean
                if(bigHeader.MagicWords!="BIGF")
                {
                    return;
                }

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                bigHeader.fileSize = BitConverter.ToInt32(tempByte, 0);

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(tempByte);
                bigHeader.ammount = BitConverter.ToInt32(tempByte, 0);

                tempByte = new byte[4];
                stream.Read(tempByte, 0, tempByte.Length);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(tempByte);
                bigHeader.startOffset = BitConverter.ToInt32(tempByte, 0);

                //
                for (int i = 0; i < bigHeader.ammount; i++)
                {
                    BIGFFiles temp = new BIGFFiles();
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(tempByte);
                    temp.offset = BitConverter.ToInt32(tempByte, 0);

                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    if (BitConverter.IsLittleEndian)
                        Array.Reverse(tempByte);
                    temp.size = BitConverter.ToInt32(tempByte, 0);

                    bool tillNull = false;
                    int a = 0;
                    while (!tillNull)
                    {
                        int temp1 = stream.ReadByte();
                        if (temp1==0x00)
                        {
                            tillNull = true;
                        }
                        else
                        {
                            a++;
                        }
                    }
                    stream.Position -= a+1;
                    byte[] FilePath = new byte[a];
                    stream.Read(FilePath, 0, a);
                    temp.path = Encoding.ASCII.GetString(FilePath);
                    bigFiles.Add(temp);
                    stream.Position += 1;
                }
                bigHeader.footer = new byte[8];
                stream.Read(bigHeader.footer, 0, bigHeader.footer.Length);
                stream.Dispose();
            }
        }

        public void ExtractBig(string path = null)
        {
            using (Stream stream = File.Open(bigPath, FileMode.Open))
            {
                for (int i = 0; i < bigFiles.Count; i++)
                {
                    Stream stream1 = new MemoryStream();
                    byte[] temp = new byte[bigFiles[i].size];
                    stream.Position = bigFiles[i].offset;
                    stream.Read(temp, 0, temp.Length);
                    stream1.Write(temp, 0, temp.Length);

                    Directory.CreateDirectory(Path.GetDirectoryName(path + "//" + bigFiles[i].path));
                    var file = File.Create(path + "//" + bigFiles[i].path);
                    stream1.Position = 0;
                    stream1.CopyTo(file);
                    file.Close();
                    stream1.Dispose();
                }
                stream.Dispose();
            }
        }
    }

//00-03 - Magic Word
//04-07 - file size (little endian)
//08-0b - Ammount
//0c-0f - Start of file Offset
//Start of file listings
//10-13 - offset
//14-17 - Size
//17 - file path
//80 ish blank bytes after each file (Done to make the file an even number?)

    struct BIGFHeader
    {
        public string MagicWords;
        public int fileSize;
        public int ammount;
        public int startOffset;
        public byte[] footer;
    }

    struct BIGFFiles
    {
        public string path;
        public int size;
        public int offset;
    }
}
