using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    byte[] tempByte = new byte[32];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.LongName = Encoding.ASCII.GetString(tempByte);
                    tempByte = new byte[16];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.FirstName = Encoding.ASCII.GetString(tempByte);
                    tempByte = new byte[16];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.NickName = Encoding.ASCII.GetString(tempByte);
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Unkown1 = BitConverter.ToInt32(tempByte, 0);
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Stance = BitConverter.ToInt32(tempByte, 0);
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.ModelSize = BitConverter.ToInt32(tempByte, 0);
                    tempByte = new byte[16];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.BloodType = Encoding.ASCII.GetString(tempByte);
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Gender = BitConverter.ToInt32(tempByte, 0);
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Age = BitConverter.ToInt32(tempByte, 0);
                    tempByte = new byte[16];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Height = Encoding.ASCII.GetString(tempByte);
                    tempByte = new byte[16];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Nationality = Encoding.ASCII.GetString(tempByte);
                    tempByte = new byte[4];
                    stream.Read(tempByte, 0, tempByte.Length);
                    temp.Position = BitConverter.ToInt32(tempByte, 0);
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
