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
        public int InternalInstancesCount;
        public int ParticleInstancesCount;
        public int Unknown5;
        public int ModelCount; //Model And Context Count Could be the wrong way around
        public int LightCount;
        public int SplinesCount;
        public int Unknown9;
        public int Unknown10;
        public int ContextCount;
        public int ParticleModelCount;
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
                InternalInstancesCount = StreamUtil.ReadInt32(stream);
                ParticleInstancesCount = StreamUtil.ReadInt32(stream);
                Unknown5 = StreamUtil.ReadInt32(stream);
                ModelCount = StreamUtil.ReadInt32(stream);
                LightCount = StreamUtil.ReadInt32(stream);
                SplinesCount = StreamUtil.ReadInt32(stream);
                Unknown9 = StreamUtil.ReadInt32(stream);
                Unknown10 = StreamUtil.ReadInt32(stream);
                ContextCount = StreamUtil.ReadInt32(stream);
                ParticleModelCount = StreamUtil.ReadInt32(stream);
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
                    Face face = new Face();
                    face.Points = new List<Vertex3>();
                    for (int a = 0; a < 21; a++)
                    {
                        Vertex3 vertex3 = new Vertex3();
                        vertex3.X = StreamUtil.ReadFloat(stream);
                        vertex3.Y = StreamUtil.ReadFloat(stream);
                        vertex3.Z = StreamUtil.ReadFloat(stream); 
                        vertex3.W = StreamUtil.ReadFloat(stream);
                        face.Points.Add(vertex3);
                    }

                    for (int a = 0; a < 2; a++)
                    {
                        Vertex3 vertex3 = new Vertex3();
                        vertex3.X = StreamUtil.ReadFloat(stream);
                        vertex3.Y = StreamUtil.ReadFloat(stream);
                        vertex3.Z = StreamUtil.ReadFloat(stream);
                        face.Points.Add(vertex3);
                    }

                    face.vertex3s = new List<Vertex3>();
                    for (int a = 0; a < 4; a++)
                    {
                        Vertex3 vertex3 = new Vertex3();
                        vertex3.X = StreamUtil.ReadFloat(stream);
                        vertex3.Y = StreamUtil.ReadFloat(stream);
                        vertex3.Z = StreamUtil.ReadFloat(stream);
                        vertex3.W = StreamUtil.ReadFloat(stream);
                        face.vertex3s.Add(vertex3);
                    }

                    face.Unknown1 = StreamUtil.ReadInt32(stream); //Surface Type?
                    face.Unknown2 = StreamUtil.ReadInt32(stream); //TrailS omething?

                    //Counting Up Possible Thing for pathing?
                    face.Unknown3 = StreamUtil.ReadInt32(stream);

                    //Always the same
                    face.Unknown4 = StreamUtil.ReadInt32(stream);
                    face.Unknown5 = StreamUtil.ReadInt32(stream);
                    face.Unknown6 = StreamUtil.ReadInt32(stream);

                    Faces.Add(face);
                   
                }
                Console.Beep();
            }
        }

        public void SaveModel(string path)
        {
            glstHandler.SavePBDglTF(path, this);
        }
    }

    public struct Face
    {
        public List<Vertex3> Points;
        public List<Vertex3> vertex3s;

        public int Unknown1;
        public int Unknown2;
        public int Unknown3;
        public int Unknown4;//Negative one
        public int Unknown5;
        public int Unknown6;
    }



//0 - Centre Point?/Start Point?

//Possibly normal Makes a square

//1 - All Basically 0,0,0
//2 - All Basically 0,0,0
//3 - All Basically 0,0,0
//4 - All Basically 0,0,0


    //5 - Unknown
    //6 - Unknown
    //7 - Unknown
    //8 - Unknown

    //9 - Unknown
    //10 - Unknown
    //11 - Unknown
    //12 - Unknown

    //13 - Unknown
    //14 - Unknown
    //15 - Unknown
    //16 - Unknown

    //17 - Unknown
    //18 - Unknown
    //19 - Unknown
    //20 - Point on Model

    public struct Vertex3
    {
        public float X;
        public float Y;
        public float Z;

        public float W;
    }

}
