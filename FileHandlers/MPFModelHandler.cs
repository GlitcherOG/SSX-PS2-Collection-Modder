using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    public class MPFModelHandler
    {
        public List<MPFModelHeader> ModelList = new List<MPFModelHeader>();

        public void load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                //Load Headers
                while (true)
                {
                    MPFModelHeader modelHeader = new MPFModelHeader();

                    modelHeader.U1 = StreamUtil.ReadInt32(stream);
                    modelHeader.U2 = StreamUtil.ReadInt32(stream); //File Uncompressed
                    modelHeader.FileStart = StreamUtil.ReadInt32(stream);

                    int Test = StreamUtil.ReadInt32(stream);
                    if (Test == 0)
                    {
                        ModelList.Add(modelHeader);
                        break;
                    }
                    else
                    {
                        stream.Position -= 4;
                    }

                    modelHeader.ModelName = StreamUtil.ReadString(stream, 16).Replace("\0", "");
                    modelHeader.Offset = StreamUtil.ReadInt32(stream);
                    modelHeader.EntrySize = StreamUtil.ReadInt32(stream);
                    modelHeader.HeaderEnd = StreamUtil.ReadInt32(stream);
                    modelHeader.U7 = StreamUtil.ReadInt32(stream);
                    modelHeader.U8 = StreamUtil.ReadInt32(stream);
                    modelHeader.U9 = StreamUtil.ReadInt32(stream);
                    modelHeader.U10 = StreamUtil.ReadInt32(stream);
                    modelHeader.U11 = StreamUtil.ReadInt32(stream);
                    modelHeader.U12 = StreamUtil.ReadInt32(stream);
                    modelHeader.U13 = StreamUtil.ReadInt32(stream);
                    modelHeader.U14 = StreamUtil.ReadInt32(stream);
                    modelHeader.U15 = StreamUtil.ReadInt32(stream);
                    modelHeader.U16 = StreamUtil.ReadInt32(stream);
                    modelHeader.U17 = StreamUtil.ReadInt32(stream);
                    modelHeader.U18 = StreamUtil.ReadInt32(stream);
                    modelHeader.U19 = StreamUtil.ReadInt32(stream);
                    modelHeader.U20 = StreamUtil.ReadInt32(stream);
                    ModelList.Add(modelHeader);
                }

                //Read Matrix
                int StartPos = ModelList[0].FileStart;
                for (int i = 0; i < ModelList.Count-1; i++)
                {
                    stream.Position = StartPos + ModelList[i].Offset;
                    int EndPos = 0;
                    if(i == ModelList.Count - 2)
                    {
                        EndPos = ((int)stream.Length - StartPos) - ModelList[i].Offset;
                    }
                    else
                    {
                        EndPos = ModelList[i + 1].Offset - ModelList[i].Offset;
                    }

                    MPFModelHeader modelHandler = ModelList[i];
                    modelHandler.Matrix = StreamUtil.ReadBytes(stream, EndPos);
                    RefpackHandler refpackHandler = new RefpackHandler();
                    modelHandler.Matrix = refpackHandler.Decompress(modelHandler.Matrix);
                    ModelList[i] = modelHandler;
                }
            }
        }

        public void Save(string path)
        {
            Stream stream = new MemoryStream();
            List<long> StreamPos = new List<long>();
            for (int i = 0; i < ModelList.Count; i++)
            {
                StreamUtil.WriteInt32(stream, ModelList[i].U1);
                StreamUtil.WriteInt32(stream, ModelList[i].U2);
                StreamUtil.WriteInt32(stream, 0);

                if (ModelList[i].ModelName == null)
                {
                    byte[] bytes = new byte[4];
                    StreamUtil.WriteBytes(stream, bytes);
                    break;
                }
                StreamUtil.WriteString(stream, ModelList[i].ModelName, 16);

                StreamPos.Add(stream.Position);
                StreamUtil.WriteInt32(stream, 0);

                StreamUtil.WriteInt32(stream, ModelList[i].EntrySize);
                StreamUtil.WriteInt32(stream, ModelList[i].HeaderEnd);
                StreamUtil.WriteInt32(stream, ModelList[i].U7); //Vertex Points? Start
                StreamUtil.WriteInt32(stream, ModelList[i].U8); //Vertex Connect?
                StreamUtil.WriteInt32(stream, ModelList[i].U9);
                StreamUtil.WriteInt32(stream, ModelList[i].U10);
                StreamUtil.WriteInt32(stream, ModelList[i].U11);
                StreamUtil.WriteInt32(stream, ModelList[i].U12);
                StreamUtil.WriteInt32(stream, ModelList[i].U13);
                StreamUtil.WriteInt32(stream, ModelList[i].U14);
                StreamUtil.WriteInt32(stream, ModelList[i].U15);
                StreamUtil.WriteInt32(stream, ModelList[i].U16);
                StreamUtil.WriteInt32(stream, ModelList[i].U17);
                StreamUtil.WriteInt32(stream, ModelList[i].U18);
                StreamUtil.WriteInt32(stream, ModelList[i].U19);
                StreamUtil.WriteInt32(stream, ModelList[i].U20);
            }

            stream.Position = 8;
            int FileStart = (int)stream.Length;
            StreamUtil.WriteInt32(stream, FileStart);
            stream.Position = stream.Length;

            for (int i = 0; i < ModelList.Count-1; i++)
            {
                //Save current pos go back and set start pos
                long CurPos = stream.Position - FileStart;
                stream.Position = StreamPos[i];
                StreamUtil.WriteInt32(stream, (int)CurPos);
                stream.Position = CurPos + FileStart;
            //Write Matrix
                StreamUtil.WriteBytes(stream, ModelList[i].Matrix);
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


        public struct MPFModelHeader
        {
            public int U1;
            public int U2; //Possibly 2 16ints
            public int FileStart;
            public string ModelName;
            public int Offset;
            public int EntrySize;
            public int HeaderEnd; //Offset Start Of something
            public int U7; //Offset Start Of something
            public int U8; //Offset Start Of something
            public int U9; //Offset Start Of something
            public int U10; //Blank Guessing Also Offset Start
            public int U11; //?
            public int U12; 
            public int U13;
            public int U14;
            public int U15;
            public int U16;
            public int U17; //Rest of File might be int 16
            public int U18;
            public int U19; //Subobjects?
            public int U20;

            public byte[] Matrix;
        }
    }
}
