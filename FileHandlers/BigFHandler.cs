using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    class BigFHandler
    {
        public BIGFHeader bigHeader;
        public List<BIGFFiles> bigFiles;
        string bigPath;
        //bool BuildMode;
        public void LoadBig(string path)
        {
            //BuildMode = false;
            bigPath = path;
            bigHeader = new BIGFHeader();
            bigFiles = new List<BIGFFiles>();
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                bigHeader.MagicWords = StreamUtil.ReadString(stream, 4);
                //Figure out what any of these mean
                if(bigHeader.MagicWords!="BIGF")
                {
                    return;
                }

                bigHeader.fileSize = StreamUtil.ReadInt32(stream);

                bigHeader.ammount = StreamUtil.ReadInt32Big(stream);

                bigHeader.startOffset = StreamUtil.ReadInt32Big(stream);

                for (int i = 0; i < bigHeader.ammount; i++)
                {
                    BIGFFiles temp = new BIGFFiles();

                    temp.offset = StreamUtil.ReadInt32Big(stream);

                    temp.size = StreamUtil.ReadInt32Big(stream);

                    temp.path = StreamUtil.ReadNullEndString(stream);
                    bigFiles.Add(temp);
                    stream.Position += 1;
                }

                bigHeader.compression = StreamUtil.ReadString(stream, 4);

                bigHeader.footer = new byte[4];
                stream.Read(bigHeader.footer, 0, bigHeader.footer.Length);

                //Find UnCompressed Size
                if(bigHeader.compression== "L231")
                {
                    for (int i = 0; i < bigHeader.ammount; i++)
                    {
                        stream.Position = bigFiles[i].offset + 2;
                        BIGFFiles tempFile = bigFiles[i];

                        tempFile.UncompressedSize = StreamUtil.ReadInt24Big(stream);
                        bigFiles[i] = tempFile;
                    }
                }

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
                    try
                    {
                        var file = File.Create(path + "//" + bigFiles[i].path);
                        stream1.Position = 0;
                        stream1.CopyTo(file);
                        file.Close();
                    }
                    catch
                    {

                    }
                    stream1.Dispose();
                }
                stream.Dispose();
            }
        }

        public void LoadFolder(string path)
        {
            //BuildMode = true;
            bigPath = path;
            bigHeader = new BIGFHeader();
            bigFiles = new List<BIGFFiles>();
            string[] paths = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            bigHeader.ammount = paths.Length;
            int FileOffset = 16;
            for (int i = 0; i < paths.Length; i++)
            {
                BIGFFiles tempFile = new BIGFFiles();
                tempFile.path = paths[i].Remove(0, path.Length+1).Replace("//",@"\");
                FileOffset += tempFile.path.Length + 9;
                Stream stream = File.OpenRead(paths[i]);
                tempFile.size = (int)stream.Length;
                stream.Dispose();
                bigFiles.Add(tempFile);
            }
            FileOffset += 8;
            for (int i = 0; i < bigFiles.Count; i++)
            {
                BIGFFiles tempFile1 = bigFiles[i];
                tempFile1.offset = FileOffset;
                FileOffset += bigFiles[i].size;
                bigFiles[i] = tempFile1;
            }
        }

        public void BuildBig(string path)
        {
            LoadFolder(bigPath);
            Stream stream = new MemoryStream();
            bigHeader.MagicWords = "BIGF";
            byte[] tempByte = new byte[4];
            Encoding.ASCII.GetBytes(bigHeader.MagicWords).CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);

            // Set File Size Later
            tempByte = new byte[4];
            stream.Write(tempByte, 0, tempByte.Length);

            //Set Ammount
            tempByte = new byte[4];
            BitConverter.GetBytes(bigHeader.ammount).CopyTo(tempByte, 0);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempByte);
            stream.Write(tempByte, 0, tempByte.Length);

            //Set Blank Start of file offset
            tempByte = new byte[4];
            stream.Write(tempByte, 0, tempByte.Length);

            for (int i = 0; i < bigFiles.Count; i++)
            {
                //Write offset
                tempByte = new byte[4];
                BitConverter.GetBytes(bigFiles[i].offset).CopyTo(tempByte, 0);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(tempByte);
                stream.Write(tempByte, 0, tempByte.Length);

                //Write size
                tempByte = new byte[4];
                BitConverter.GetBytes(bigFiles[i].size).CopyTo(tempByte, 0);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(tempByte);
                stream.Write(tempByte, 0, tempByte.Length);

                //Write Path
                tempByte = new byte[bigFiles[i].path.Length + 1];
                Encoding.ASCII.GetBytes(bigFiles[i].path).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);
            }
            //Write Footer
            tempByte = new byte[8];
            Encoding.ASCII.GetBytes("L222").CopyTo(tempByte, 0);
            stream.Write(tempByte, 0, tempByte.Length);

            //Set File start offset
            long pos = stream.Position;
            stream.Position = 12;
            tempByte = new byte[4];
            BitConverter.GetBytes((int)pos).CopyTo(tempByte, 0);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(tempByte);
            stream.Write(tempByte, 0, tempByte.Length);

            stream.Position = stream.Length;
            //Write Files
            for (int i = 0; i < bigFiles.Count; i++)
            {
                using (Stream stream1 = File.Open(bigPath+ "\\" + bigFiles[i].path, FileMode.Open))
                {
                    tempByte = new byte[stream1.Length];
                    stream1.Read(tempByte, 0, tempByte.Length);
                    stream.Write(tempByte, 0, tempByte.Length);
                }
            }

            //Set filesize
            stream.Position = 4;
            tempByte = new byte[4];
            BitConverter.GetBytes((int)stream.Length).CopyTo(tempByte,0);
            stream.Write(tempByte, 0, tempByte.Length);

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
        public string compression;
        public byte[] footer;
    }

    struct BIGFFiles
    {
        public string path;
        public int size;
        public int offset;
        public int UncompressedSize;
    }
}
