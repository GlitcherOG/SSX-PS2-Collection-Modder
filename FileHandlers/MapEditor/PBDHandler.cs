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
        public int ParticleInstancesOffset;
        public int UnknownOffset5;
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
        public int ModelDataOffset;
        public int Unknown34;
        public int Unknown35;

        public List<Patch> Patches;
        public List<Model> models;

        public void loadandsave(string path)
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
                ParticleInstancesOffset = StreamUtil.ReadInt32(stream);
                UnknownOffset5 = StreamUtil.ReadInt32(stream);
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
                ModelDataOffset = StreamUtil.ReadInt32(stream);
                Unknown34 = StreamUtil.ReadInt32(stream);
                Unknown35 = StreamUtil.ReadInt32(stream);

                stream.Position = PatchOffset;
                Patches = new List<Patch>();
                for (int i = 0; i < PatchCount; i++)
                {
                    Patch patch = LoadPatch(stream);
                    Patches.Add(patch);
                }

                stream.Position = ModelDataOffset;
                models = new List<Model>();
                Model model = new Model();
                model.staticMeshes = new List<StaticMesh>();
                int count = 0;
                while (true)
                {
                    var temp = ReadMesh(stream);
                    if (temp.Equals(new StaticMesh()))
                    {
                        break;
                    }
                    count++;
                    model.staticMeshes.Add(GenerateFaces(temp));
                    stream.Position += 31;
                    if(StreamUtil.ReadByte(stream)==0x6C)
                    {
                        stream.Position -= 32;
                    }
                    else
                    {
                        stream.Position += 48;
                        models.Add(model);
                        model = new Model();
                        model.staticMeshes = new List<StaticMesh>();
                    }
                }
            }
        }


        public StaticMesh ReadMesh(Stream stream)
        {
            var ModelData = new StaticMesh();

            if(stream.Position>=stream.Length)
            {
                return new StaticMesh();
            }

            stream.Position += 48;

            ModelData.StripCount = StreamUtil.ReadInt32(stream);
            ModelData.Unknown1 = StreamUtil.ReadInt32(stream);
            ModelData.VertexCount = StreamUtil.ReadInt32(stream);
            ModelData.Unknown3 = StreamUtil.ReadInt32(stream);

            stream.Position += 16;

            //Load Strip Count
            List<int> TempStrips = new List<int>();
            for (int a = 0; a < ModelData.StripCount; a++)
            {
                TempStrips.Add(StreamUtil.ReadInt16(stream));
            }
            StreamUtil.AlignBy16(stream);

            stream.Position += 16;
            ModelData.Strips = TempStrips;


            List<UV> UVs = new List<UV>();
            //Read UV Texture Points
            stream.Position += 48;
            for (int a = 0; a < ModelData.VertexCount; a++)
            {
                UV uv = new UV();
                uv.X = StreamUtil.ReadInt16(stream);
                uv.Y = StreamUtil.ReadInt16(stream);
                //uv.XU = StreamUtil.ReadInt16(stream);
                //uv.YU = StreamUtil.ReadInt16(stream);
                UVs.Add(uv);
            }
            StreamUtil.AlignBy16(stream);
            stream.Position += 16;

            //Everything Above is Correct

            ModelData.uv = UVs;

            List<UVNormal> Normals = new List<UVNormal>();
            //Read Normals
            stream.Position += 32;
            for (int a = 0; a < ModelData.VertexCount; a++)
            {
                UVNormal normal = new UVNormal();
                normal.X = StreamUtil.ReadInt16(stream);
                normal.Y = StreamUtil.ReadInt16(stream);
                normal.Z = StreamUtil.ReadInt16(stream);
                Normals.Add(normal);
            }
            StreamUtil.AlignBy16(stream);
            ModelData.uvNormals = Normals;

            List<Vertex3> vertices = new List<Vertex3>();
            stream.Position += 16;
            //Load Vertex
            for (int a = 0; a < ModelData.VertexCount; a++)
            {
                Vertex3 vertex = new Vertex3();
                //Float 16's
                vertex.X = (float)StreamUtil.ReadInt16(stream) / 4096f;
                vertex.Y = (float)StreamUtil.ReadInt16(stream) / 4096f;
                vertex.Z = (float)StreamUtil.ReadInt16(stream) / 4096f;
                vertices.Add(vertex);
            }
            StreamUtil.AlignBy16(stream);
            ModelData.vertices = vertices;
            stream.Position += 16;

            return ModelData;
        }

        #region Standard Mesh Stuff
        public StaticMesh GenerateFaces(StaticMesh models)
        {
            var ModelData = models;
            //Increment Strips
            List<int> strip2 = new List<int>();
            strip2.Add(0);
            foreach (var item in ModelData.Strips)
            {
                strip2.Add(strip2[strip2.Count - 1] + item);
            }
            ModelData.Strips = strip2;

            //Make Faces
            ModelData.faces = new List<Face>();
            int localIndex = 0;
            int Rotation = 0;
            for (int b = 0; b < ModelData.vertices.Count; b++)
            {
                if (InsideSplits(b, ModelData.Strips))
                {
                    Rotation = 0;
                    localIndex = 1;
                    continue;
                }
                if (localIndex < 2)
                {
                    localIndex++;
                    continue;
                }

                ModelData.faces.Add(CreateFaces(b, ModelData, Rotation));
                Rotation++;
                if (Rotation == 2)
                {
                    Rotation = 0;
                }
                localIndex++;
            }

            return ModelData;
        }
        public bool InsideSplits(int Number, List<int> splits)
        {
            foreach (var item in splits)
            {
                if (item == Number)
                {
                    return true;
                }
            }
            return false;
        }
        public Face CreateFaces(int Index, StaticMesh ModelData, int roatation)
        {
            Face face = new Face();
            int Index1 = 0;
            int Index2 = 0;
            int Index3 = 0;
            //Fixes the Rotation For Exporting
            //Swap When Exporting to other formats
            //1-Clockwise
            //0-Counter Clocwise
            if (roatation == 1)
            {
                Index1 = Index;
                Index2 = Index - 1;
                Index3 = Index - 2;
            }
            if (roatation == 0)
            {
                Index1 = Index;
                Index2 = Index - 2;
                Index3 = Index - 1;
            }
            face.V1 = ModelData.vertices[Index1];
            face.V2 = ModelData.vertices[Index2];
            face.V3 = ModelData.vertices[Index3];

            face.V1Pos = Index1;
            face.V2Pos = Index2;
            face.V3Pos = Index3;

            if (ModelData.uv.Count != 0)
            {
                face.UV1 = ModelData.uv[Index1];
                face.UV2 = ModelData.uv[Index2];
                face.UV3 = ModelData.uv[Index3];

                face.UV1Pos = Index1;
                face.UV2Pos = Index2;
                face.UV3Pos = Index3;

                face.Normal1 = ModelData.uvNormals[Index1];
                face.Normal2 = ModelData.uvNormals[Index2];
                face.Normal3 = ModelData.uvNormals[Index3];

                face.Normal1Pos = Index1;
                face.Normal2Pos = Index2;
                face.Normal3Pos = Index3;
            }

            return face;
        }
        #endregion

        public Patch LoadPatch(Stream stream)
        {
            Patch face = new Patch();

            face.ScalePoint = ReadVertices(stream, true);

            face.UVPoint1 = ReadVertices(stream, true);
            face.UVPoint2 = ReadVertices(stream, true);
            face.UVPoint3 = ReadVertices(stream, true);
            face.UVPoint4 = ReadVertices(stream, true);

            face.UnknownPoint1 = ReadVertices(stream, true);
            face.UnknownPoint2 = ReadVertices(stream, true);
            face.UnknownPoint3 = ReadVertices(stream, true);
            face.UnknownPoint4 = ReadVertices(stream, true);
            face.UnknownPoint5 = ReadVertices(stream, true);
            face.UnknownPoint6 = ReadVertices(stream, true);
            face.UnknownPoint7 = ReadVertices(stream, true);
            face.UnknownPoint8 = ReadVertices(stream, true);
            face.UnknownPoint9 = ReadVertices(stream, true);
            face.UnknownPoint10 = ReadVertices(stream, true);
            face.UnknownPoint11 = ReadVertices(stream, true);
            face.UnknownPoint12 = ReadVertices(stream, true);
            face.UnknownPoint13 = ReadVertices(stream, true);
            face.UnknownPoint14 = ReadVertices(stream, true);
            face.UnknownPoint15 = ReadVertices(stream, true);
            face.CenterPoint = ReadVertices(stream, true);

            face.LowestXYZ = ReadVertices(stream);
            face.HighestXYZ = ReadVertices(stream);

            face.Point1 = ReadVertices(stream, true);
            face.Point2 = ReadVertices(stream, true);
            face.Point3 = ReadVertices(stream, true);
            face.Point4 = ReadVertices(stream, true);

            face.PatchStyle = StreamUtil.ReadInt32(stream);
            face.Unknown2 = StreamUtil.ReadInt32(stream); //Material/Lighting
            face.TextureAssigment = StreamUtil.ReadInt16(stream);
            face.Unknown3 = StreamUtil.ReadInt16(stream);

            //Always the same
            face.Unknown4 = StreamUtil.ReadInt32(stream); //Negitive one
            face.Unknown5 = StreamUtil.ReadInt32(stream);
            face.Unknown6 = StreamUtil.ReadInt32(stream);

            return face;
        }

        public Vertex3 ReadVertices(Stream stream, bool w = false)
        {
            Vertex3 vertex = new Vertex3();
            vertex.X = StreamUtil.ReadFloat(stream);
            vertex.Y = StreamUtil.ReadFloat(stream);
            vertex.Z = StreamUtil.ReadFloat(stream);
            if (w)
            {
                vertex.W = StreamUtil.ReadFloat(stream);
            }
            return vertex;
        }

        public void SaveModel(string path)
        {
            glstHandler.SavePDBModelglTF(path, this);
        }
    }

    public struct Model
    {
        public List<StaticMesh> staticMeshes;
    }

    public struct StaticMesh
    {
        public int ChunkID;

        public int StripCount;
        public int Unknown1;
        public int VertexCount;
        public int Unknown3;
        public List<int> Strips;

        public List<UV> uv;
        public List<Vertex3> vertices;
        public List<Face> faces;
        public List<UVNormal> uvNormals;
    }

    //Since there both int 16's They need to be divided by 4096
    public struct UV
    {
        public int X;
        public int Y;
        public int XU;
        public int YU;
    }

    public struct UVNormal
    {
        public int X;
        public int Y;
        public int Z;
    }

    public struct Face
    {
        public Vertex3 V1;
        public Vertex3 V2;
        public Vertex3 V3;

        public int V1Pos;
        public int V2Pos;
        public int V3Pos;

        public UV UV1;
        public UV UV2;
        public UV UV3;

        public int UV1Pos;
        public int UV2Pos;
        public int UV3Pos;

        public UVNormal Normal1;
        public UVNormal Normal2;
        public UVNormal Normal3;

        public int Normal1Pos;
        public int Normal2Pos;
        public int Normal3Pos;
    }

    public struct Patch
    {
        public Vertex3 ScalePoint; 

        public Vertex3 UVPoint1;  
        public Vertex3 UVPoint2; 
        public Vertex3 UVPoint3; 
        public Vertex3 UVPoint4; 

        public Vertex3 UnknownPoint1; 
        public Vertex3 UnknownPoint2; 
        public Vertex3 UnknownPoint3; 
        public Vertex3 UnknownPoint4; 
        public Vertex3 UnknownPoint5;
        public Vertex3 UnknownPoint6; 
        public Vertex3 UnknownPoint7;
        public Vertex3 UnknownPoint8;
        public Vertex3 UnknownPoint9; 
        public Vertex3 UnknownPoint10;
        public Vertex3 UnknownPoint11;
        public Vertex3 UnknownPoint12;
        public Vertex3 UnknownPoint13; 
        public Vertex3 UnknownPoint14;
        public Vertex3 UnknownPoint15;

        public Vertex3 CenterPoint;
        //1-Unknown
        //2- UV Point

        public Vertex3 LowestXYZ;
        public Vertex3 HighestXYZ;

        //Main Corners After all the maths is done to them
        //Probably just used for Rendering
        public Vertex3 Point1;
        public Vertex3 Point2;
        public Vertex3 Point3;
        public Vertex3 Point4;

        //0 - Reset
        //1 - Standard Snow
        //2 - Standard Off Track?
        //3 - Powered Snow
        //4 - Slow Powered Snow
        //5 - Ice Standard
        //6 - Bounce/Unskiiable //
        //7 - Ice/Water No Trail
        //8 - Glidy(Lots Of snow particels) //
        //9 - Rock 
        //10 - Wall
        //11 - No Trail, Ice Crunch Sound Effect//
        //12 - No Sound, No Trail, Small particle Wake//
        //13 - Off Track Metal (Slidly Slow, Metal Grinding sounds, Sparks)
        //14 - Speed, Grinding Sound//
        //15 - Standard?//
        //16 - Standard Sand
        //17 - ?//
        //18 - Show Off Ramp/Metal
        public int PatchStyle; //Type

        public int Unknown2; // Some Kind of material Assignment Or Lighting
        public int TextureAssigment; // Texture Assigment 
        public int Unknown3;
        public int Unknown4; //Negative one
        public int Unknown5; //Same
        public int Unknown6; //Same
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
    [System.Serializable]
    public struct Vertex3
    {
        public float X;
        public float Y;
        public float Z;

        public float W;
    }
    enum Styles
    {

    }
}
