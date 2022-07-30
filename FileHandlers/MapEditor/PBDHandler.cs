using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers.MapEditor
{
    public class PBDHandler
    {
        public byte[] MagicBytes;
        public int Unknown1;
        public int PatchCount;
        public int UnknownCount1;
        public int Unknown4;
        public int Unknown5;
        public int Unknown6;
        public int Unknown7;
        public int Unknown8;
        public int Unknown9;
        public int Unknown10;
        public int Unknown11;
        public int Unknown12;
        public int Unknown13;
        public int Unknown14;
        public int Unknown15;

        public int Unknown17;
        public int PatchOffset;

        public int Unknown18;
        public int Unknown19;
        public int Unknown20;
        public int Unknown21;
        public int Unknown22;
        public int Unknown23;
        public int Unknown24;
        public int Unknown25;
        public int Unknown26;
        public int Unknown27;
        public int Unknown28;
        public int Unknown29;

        public int Unknown30;
        public int Unknown31;
        public int Unknown32;
        public int Unknown33;
        public int Unknown34;
        public int Unknown35;

        public List<Face> Faces;
        public void load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                MagicBytes = StreamUtil.ReadBytes(stream, 4);
                Unknown1 = StreamUtil.ReadInt32(stream);
                PatchCount = StreamUtil.ReadInt32(stream);
                UnknownCount1 = StreamUtil.ReadInt32(stream);
                Unknown4 = StreamUtil.ReadInt32(stream);
                Unknown5 = StreamUtil.ReadInt32(stream);
                Unknown6 = StreamUtil.ReadInt32(stream);
                Unknown7 = StreamUtil.ReadInt32(stream);
                Unknown8 = StreamUtil.ReadInt32(stream);
                Unknown9 = StreamUtil.ReadInt32(stream);
                Unknown10 = StreamUtil.ReadInt32(stream);
                Unknown11 = StreamUtil.ReadInt32(stream);
                Unknown12 = StreamUtil.ReadInt32(stream);
                Unknown13 = StreamUtil.ReadInt32(stream);
                Unknown14 = StreamUtil.ReadInt32(stream);
                Unknown15 = StreamUtil.ReadInt32(stream);
                Unknown17 = StreamUtil.ReadInt32(stream);
                PatchOffset = StreamUtil.ReadInt32(stream);
                Unknown18 = StreamUtil.ReadInt32(stream);
                Unknown19 = StreamUtil.ReadInt32(stream);
                Unknown20 = StreamUtil.ReadInt32(stream);
                Unknown21 = StreamUtil.ReadInt32(stream);
                Unknown22 = StreamUtil.ReadInt32(stream);
                Unknown23 = StreamUtil.ReadInt32(stream);
                Unknown24 = StreamUtil.ReadInt32(stream);
                Unknown25 = StreamUtil.ReadInt32(stream);
                Unknown26 = StreamUtil.ReadInt32(stream);
                Unknown27 = StreamUtil.ReadInt32(stream);
                Unknown28 = StreamUtil.ReadInt32(stream);
                Unknown29 = StreamUtil.ReadInt32(stream);
                Unknown30 = StreamUtil.ReadInt32(stream);
                Unknown31 = StreamUtil.ReadInt32(stream);
                Unknown32 = StreamUtil.ReadInt32(stream);
                Unknown33 = StreamUtil.ReadInt32(stream);
                Unknown34 = StreamUtil.ReadInt32(stream);
                Unknown35 = StreamUtil.ReadInt32(stream);

                stream.Position = PatchOffset;
                Faces = new List<Face>();
                for (int i = 0; i < PatchCount; i++)
                {
                    Face name = new Face();
                    name.X = StreamUtil.ReadFloat(stream);
                    name.Y = StreamUtil.ReadFloat(stream);
                    name.Z = StreamUtil.ReadFloat(stream);
                    name.vertex3s = new List<Vertex3>();
                    stream.Position += 348;
                    for (int a = 0; a < 4; a++)
                    {
                        Vertex3 vertex3 = new Vertex3();
                        vertex3.X = StreamUtil.ReadFloat(stream)/100f;
                        vertex3.Y = StreamUtil.ReadFloat(stream)/100f;
                        vertex3.Z = StreamUtil.ReadFloat(stream)/100f;
                        vertex3.Unknown = StreamUtil.ReadFloat(stream);
                        name.vertex3s.Add(vertex3);
                    }
                    Faces.Add(name);
                    stream.Position += 24;
                }
            }
        }

        public void SaveModel(string path)
        {
            glstHandler.SavePBDglTF(path, this);
        }
    }

    public struct Face
    {
        public float X;
        public float Y;
        public float Z;

        public List<Vertex3> vertex3s;
    }

    public struct Vertex3
    {
        public float X;
        public float Y;
        public float Z;

        public float Unknown;
    }

}
