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
        public int NumPlayerStarts;
        public int NumPatches;
        public int NumInstances;
        public int NumParticleInstances;
        public int NumMaterials;
        public int NumMaterialBlocks;
        public int NumLights;
        public int NumSplines;
        public int NumSplineSegments;
        public int NumTextureFlips;
        public int NumModels;
        public int Unknown;
        public int NumTextures; 

        public int NumCameras;
        public int LightMapSize;

        public int PlayerStartOffset;
        public int PatchOffset;
        public int InstanceOffset;
        public int Unknown2;
        public int MaterialOffset;
        public int MaterialBlocksOffset;
        public int LightsOffset;
        public int SplineOffset;
        public int SplineSegmentOffset;
        public int TextureFlipOffset;
        public int ModelPointerOffset;
        public int ModelsOffset;


        public int ParticleModelPointerOffset;
        public int ParticleModelsOffset;
        public int CameraPointerOffset;
        public int CamerasOffset;
        public int HashOffset;
        public int ModelDataOffset;
        public int Unknown34;
        public int Unknown35;

        public List<Patch> Patches;
        public List<Model> models;
        public List<Spline> splines;
        public List<SplinesSegments> splinesSegments;

        public void loadandsave(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                MagicBytes = StreamUtil.ReadBytes(stream, 4);
                NumPlayerStarts = StreamUtil.ReadInt32(stream);
                NumPatches = StreamUtil.ReadInt32(stream);
                NumInstances = StreamUtil.ReadInt32(stream);
                NumParticleInstances = StreamUtil.ReadInt32(stream);
                NumMaterials = StreamUtil.ReadInt32(stream);
                NumMaterialBlocks = StreamUtil.ReadInt32(stream);
                NumLights = StreamUtil.ReadInt32(stream);
                NumSplines = StreamUtil.ReadInt32(stream);
                NumSplineSegments = StreamUtil.ReadInt32(stream);
                NumTextureFlips = StreamUtil.ReadInt32(stream);
                NumModels = StreamUtil.ReadInt32(stream);
                Unknown = StreamUtil.ReadInt32(stream);
                NumTextures = StreamUtil.ReadInt32(stream);
                NumCameras = StreamUtil.ReadInt32(stream);
                LightMapSize = StreamUtil.ReadInt32(stream);

                PlayerStartOffset = StreamUtil.ReadInt32(stream);
                PatchOffset = StreamUtil.ReadInt32(stream);
                InstanceOffset = StreamUtil.ReadInt32(stream);
                Unknown2 = StreamUtil.ReadInt32(stream);
                MaterialOffset = StreamUtil.ReadInt32(stream);
                MaterialBlocksOffset = StreamUtil.ReadInt32(stream);
                LightsOffset = StreamUtil.ReadInt32(stream);
                SplineOffset = StreamUtil.ReadInt32(stream);
                SplineSegmentOffset = StreamUtil.ReadInt32(stream);
                TextureFlipOffset = StreamUtil.ReadInt32(stream);
                ModelPointerOffset = StreamUtil.ReadInt32(stream);
                ModelsOffset = StreamUtil.ReadInt32(stream);

                ParticleModelPointerOffset = StreamUtil.ReadInt32(stream);
                ParticleModelsOffset = StreamUtil.ReadInt32(stream);
                CameraPointerOffset = StreamUtil.ReadInt32(stream);
                CamerasOffset = StreamUtil.ReadInt32(stream);
                HashOffset = StreamUtil.ReadInt32(stream);
                ModelDataOffset = StreamUtil.ReadInt32(stream);
                Unknown34 = StreamUtil.ReadInt32(stream);
                Unknown35 = StreamUtil.ReadInt32(stream);

                //Patch Loading
                stream.Position = PatchOffset;
                Patches = new List<Patch>();
                for (int i = 0; i < NumPatches; i++)
                {
                    Patch patch = LoadPatch(stream);
                    Patches.Add(patch);
                }

                //Spline Data
                stream.Position = SplineOffset;
                splines = new List<Spline>();
                for (int i = 0; i < NumSplines; i++)
                {
                    Spline spline = new Spline();
                    spline.X1 = StreamUtil.ReadFloat(stream);
                    spline.Y1 = StreamUtil.ReadFloat(stream);
                    spline.Z1 = StreamUtil.ReadFloat(stream);
                    spline.X2 = StreamUtil.ReadFloat(stream);
                    spline.Y2 = StreamUtil.ReadFloat(stream);
                    spline.Z2 = StreamUtil.ReadFloat(stream);
                    spline.Unknown7 = StreamUtil.ReadInt32(stream);
                    spline.Unknown8 = StreamUtil.ReadInt32(stream);
                    spline.Unknown9 = StreamUtil.ReadInt32(stream);
                    spline.Unknown10 = StreamUtil.ReadInt32(stream);
                    splines.Add(spline);
                }

                //Spline Segments
                stream.Position = SplineSegmentOffset;
                splinesSegments = new List<SplinesSegments>();
                for (int i = 0; i < NumSplineSegments; i++)
                {
                    SplinesSegments splinesSegment = new SplinesSegments();
                    splinesSegment.Unknown1 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown2 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown3 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown4 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown5 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown6 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown7 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown8 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown9 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown10 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown11 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown12 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown13 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown14 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown15 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown16 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown17 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown18 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown19 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown20 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown21 = StreamUtil.ReadInt32(stream);
                    splinesSegment.Unknown22 = StreamUtil.ReadInt32(stream);
                    splinesSegment.Unknown23 = StreamUtil.ReadInt32(stream);
                    splinesSegment.Unknown24 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown25 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown26 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown27 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown28 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown29 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown30 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown31 = StreamUtil.ReadFloat(stream);
                    splinesSegment.Unknown32 = StreamUtil.ReadInt32(stream);
                    splinesSegments.Add(splinesSegment);
                }

                //ModelData
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

        #region Standard Mesh Stuff
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

            face.R4C4 = ReadVertices(stream, true);
            face.R4C3 = ReadVertices(stream, true);
            face.R4C2 = ReadVertices(stream, true);
            face.R4C1 = ReadVertices(stream, true);
            face.R3C4 = ReadVertices(stream, true);
            face.R3C3 = ReadVertices(stream, true);
            face.R3C2 = ReadVertices(stream, true);
            face.R3C1 = ReadVertices(stream, true);
            face.R2C4 = ReadVertices(stream, true);
            face.R2C3 = ReadVertices(stream, true);
            face.R2C2 = ReadVertices(stream, true);
            face.R2C1 = ReadVertices(stream, true);
            face.R1C4 = ReadVertices(stream, true);
            face.R1C3 = ReadVertices(stream, true);
            face.R1C2 = ReadVertices(stream, true);
            face.R1C1 = ReadVertices(stream, true);

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

    public struct Spline
    {
        public float X1;
        public float Y1;
        public float Z1;
        public float X2;
        public float Y2;
        public float Z2;
        public int Unknown7;
        public int Unknown8;
        public int Unknown9;
        public int Unknown10;
    }

    public struct SplinesSegments
    {
        public float Unknown1;
        public float Unknown2;
        public float Unknown3;
        public float Unknown4;
        public float Unknown5;
        public float Unknown6;
        public float Unknown7;
        public float Unknown8;
        public float Unknown9;
        public float Unknown10;
        public float Unknown11;
        public float Unknown12;
        public float Unknown13;
        public float Unknown14;
        public float Unknown15;
        public float Unknown16;
        public float Unknown17;
        public float Unknown18;
        public float Unknown19;
        public float Unknown20;
        public int Unknown21;
        public int Unknown22;
        public int Unknown23;
        public float Unknown24;
        public float Unknown25;
        public float Unknown26;
        public float Unknown27;
        public float Unknown28;
        public float Unknown29;
        public float Unknown30;
        public float Unknown31;
        public int Unknown32;
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

        public Vertex3 R4C4; 
        public Vertex3 R4C3; 
        public Vertex3 R4C2; 
        public Vertex3 R4C1; 
        public Vertex3 R3C4;
        public Vertex3 R3C3; 
        public Vertex3 R3C2;
        public Vertex3 R3C1;
        public Vertex3 R2C4; 
        public Vertex3 R2C3;
        public Vertex3 R2C2;
        public Vertex3 R2C1;
        public Vertex3 R1C4; 
        public Vertex3 R1C3;
        public Vertex3 R1C2;
        public Vertex3 R1C1;

        public Vertex3 LowestXYZ;
        public Vertex3 HighestXYZ;

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

    [System.Serializable]
    public struct Vertex3
    {
        public float X;
        public float Y;
        public float Z;

        public float W;
    }
}
