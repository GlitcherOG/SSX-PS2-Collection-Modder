﻿using System;
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
                StreamUtil.WriteString(stream, charDBs[i].LongName, 32);

                StreamUtil.WriteString(stream, charDBs[i].FirstName, 16);

                StreamUtil.WriteString(stream, charDBs[i].NickName, 16);

                StreamUtil.WriteInt32(stream, charDBs[i].Unkown1);

                StreamUtil.WriteInt32(stream, charDBs[i].Stance);

                StreamUtil.WriteInt32(stream, charDBs[i].ModelSize);

                StreamUtil.WriteString(stream, charDBs[i].BloodType, 16);

                StreamUtil.WriteInt32(stream, charDBs[i].Gender);

                StreamUtil.WriteInt32(stream, charDBs[i].Age);

                StreamUtil.WriteString(stream, charDBs[i].Height, 16);

                StreamUtil.WriteString(stream, charDBs[i].Nationality, 16);

                StreamUtil.WriteInt32(stream, charDBs[i].Position);
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
