using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    class CHARDBLHandler
    {
        public List<CharDB> charDBs = new List<CharDB>();
        string charPath;

        public void LoadCharFile(string path)
        {
            charPath = path;
            charDBs = new List<CharDB>();
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                while (stream.Position != stream.Length)
                {
                    CharDB temp = new CharDB();

                    temp.LongName = StreamUtil.ReadString(stream, 32);

                    temp.FirstName = StreamUtil.ReadString(stream, 16);

                    temp.NickName = StreamUtil.ReadString(stream, 16);

                    temp.Unkown1 = StreamUtil.ReadInt32(stream);

                    temp.Stance = StreamUtil.ReadInt32(stream);

                    temp.ModelSize = StreamUtil.ReadInt32(stream);

                    temp.BloodType = StreamUtil.ReadString(stream, 16);

                    temp.Gender = StreamUtil.ReadInt32(stream);

                    temp.Age = StreamUtil.ReadInt32(stream);

                    temp.Height = StreamUtil.ReadString(stream, 16);

                    temp.Nationality = StreamUtil.ReadString(stream, 16);

                    temp.Position = StreamUtil.ReadInt32(stream);
                    charDBs.Add(temp);
                }
            }
        }

        public void SaveCharFile(string path = null)
        {
            if(path == null)
            {
                path = charPath;
            }
            Stream stream = new MemoryStream();
            for (int i = 0; i < charDBs.Count; i++)
            {
                byte[] tempByte = new byte[32];
                Encoding.ASCII.GetBytes(charDBs[i].LongName).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[16];
                Encoding.ASCII.GetBytes(charDBs[i].FirstName).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[16];
                Encoding.ASCII.GetBytes(charDBs[i].NickName).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[4];
                tempByte = BitConverter.GetBytes(charDBs[i].Unkown1);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[4];
                tempByte = BitConverter.GetBytes(charDBs[i].Stance);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[4];
                tempByte = BitConverter.GetBytes(charDBs[i].ModelSize);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[16];
                Encoding.ASCII.GetBytes(charDBs[i].BloodType).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[4];
                tempByte = BitConverter.GetBytes(charDBs[i].Gender);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[4];
                tempByte = BitConverter.GetBytes(charDBs[i].Age);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[16];
                Encoding.ASCII.GetBytes(charDBs[i].Height).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[16];
                Encoding.ASCII.GetBytes(charDBs[i].Nationality).CopyTo(tempByte, 0);
                stream.Write(tempByte, 0, tempByte.Length);

                tempByte = new byte[4];
                tempByte = BitConverter.GetBytes(charDBs[i].Position);
                stream.Write(tempByte, 0, tempByte.Length);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var file = File.Create(path);
            stream.Position = 0;
            stream.CopyTo(file);
            file.Close();
        }
    }
    struct CharDB
    {
        public string LongName;
        public string FirstName;
        public string NickName;
        public int Unkown1;
        public int Stance;
        public int ModelSize;
        public string BloodType;
        public int Gender;
        public int Age;
        public string Height;
        public string Nationality;
        public int Position;
    }
}
