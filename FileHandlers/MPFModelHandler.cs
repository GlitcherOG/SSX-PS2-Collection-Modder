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
        public int HeaderCount;
        public int HeaderSize;
        public List<MPFModelHeader> ModelList = new List<MPFModelHeader>();

        public void load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                stream.Position += 4;
                HeaderCount = StreamUtil.ReadInt16(stream);
                stream.Position = 0;
                //Load Headers
                for (int i = 0; i < HeaderCount; i++)
                {
                    MPFModelHeader modelHeader = new MPFModelHeader();

                    modelHeader.U1 = StreamUtil.ReadInt32(stream);
                    modelHeader.SubHeaders = StreamUtil.ReadInt16(stream);
                    modelHeader.HeaderSize = StreamUtil.ReadInt16(stream);
                    modelHeader.FileStart = StreamUtil.ReadInt32(stream);

                    modelHeader.FileName = StreamUtil.ReadString(stream, 16).Replace("\0", "");
                    modelHeader.DataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.EntrySize = StreamUtil.ReadInt32(stream);
                    modelHeader.ModelsOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.RotationInfo = StreamUtil.ReadInt32(stream);
                    modelHeader.ChunkOffsets = StreamUtil.ReadInt32(stream);
                    modelHeader.DataStart = StreamUtil.ReadInt32(stream);

                    modelHeader.ChunksCount = StreamUtil.ReadInt16(stream);
                    modelHeader.ModelCount = StreamUtil.ReadInt16(stream);
                    modelHeader.U22 = StreamUtil.ReadInt16(stream);
                    modelHeader.BodyObjectsCount = StreamUtil.ReadByte(stream);
                    modelHeader.U23 = StreamUtil.ReadByte(stream);
                    stream.Position += 4;
                    ModelList.Add(modelHeader);
                }

                //Read Matrix
                int StartPos = ModelList[0].FileStart;
                for (int i = 0; i < ModelList.Count; i++)
                {
                    stream.Position = StartPos + ModelList[i].DataOffset;
                    MPFModelHeader modelHandler = ModelList[i];
                    modelHandler.Matrix = StreamUtil.ReadBytes(stream, ModelList[i].EntrySize);
                    RefpackHandler refpackHandler = new RefpackHandler();
                    modelHandler.Matrix = refpackHandler.Decompress(modelHandler.Matrix);
                    ModelList[i] = modelHandler;
                }
            }

            //Read Matrix Data
            for (int i = 0; i < ModelList.Count; i++)
            {
                Stream streamMatrix = new MemoryStream();
                var Model = ModelList[i];
                streamMatrix.Write(ModelList[i].Matrix, 0, ModelList[i].Matrix.Length);

                //Read BodyObjects
                Model.bodyObjectsList = new List<BodyObjects>();
                streamMatrix.Position = 0;
                for (int b = 0; b < Model.BodyObjectsCount; b++)
                {
                    BodyObjects body = new BodyObjects();
                    body.Name = StreamUtil.ReadString(streamMatrix, 4);
                    body.Float1 = StreamUtil.ReadFloat(streamMatrix);
                    body.Float2 = StreamUtil.ReadFloat(streamMatrix);
                    body.Float3 = StreamUtil.ReadFloat(streamMatrix);
                    Model.bodyObjectsList.Add(body);
                }

                //Read ModelData
                streamMatrix.Position = Model.ModelsOffset;
                var models = new List<Models>();
                for (int b = 0; b < Model.ModelCount; b++)
                {
                    Models modelData = new Models();
                    modelData.modelName = StreamUtil.ReadString(streamMatrix, 16);
                    modelData.Int1 = StreamUtil.ReadInt32(streamMatrix);
                    modelData.Float2 = StreamUtil.ReadFloat(streamMatrix);
                    modelData.Float3 = StreamUtil.ReadFloat(streamMatrix);
                    modelData.Float4 = StreamUtil.ReadFloat(streamMatrix);
                    models.Add(modelData);
                }
                Model.models = models;

                //Read RotationData?
                streamMatrix.Position = Model.RotationInfo;
                var Verties = new List<Vertex3>();
                for (int b = 0; b < Model.U23; b++)
                {
                    Vertex3 v = new Vertex3();
                    v.X = StreamUtil.ReadFloat(streamMatrix);
                    v.Y = StreamUtil.ReadFloat(streamMatrix);
                    v.Z = StreamUtil.ReadFloat(streamMatrix);
                    streamMatrix.Position += 4;
                    Verties.Add(v);
                }
                //Model.Unkown = Verties;

                //Read Chunk Offsets
                streamMatrix.Position = Model.ChunkOffsets;
                Model.chunks = new List<Chunk>();
                for (int b = 0; b < Model.ChunksCount; b++)
                {
                    Chunk chunk = new Chunk();
                    chunk.ID = StreamUtil.ReadInt32(streamMatrix);
                    chunk.ChunkID = StreamUtil.ReadInt32(streamMatrix);
                    streamMatrix.Position += 4;
                    chunk.ModelDataOffsetStart = StreamUtil.ReadInt32(streamMatrix);
                    chunk.ModelDataOffsetEnd = StreamUtil.ReadInt32(streamMatrix);
                    chunk.OffsetStart = StreamUtil.ReadInt32(streamMatrix);
                    chunk.OffsetEnd = StreamUtil.ReadInt32(streamMatrix);
                    Model.chunks.Add(chunk);
                }
                Model.modelsData = new List<ModelData>();
                streamMatrix.Position = Model.DataStart;
                //Read Model Data
                for (int n = 0; n < Model.ChunksCount; n++)
                {
                    int ModelPos = 0;
                    streamMatrix.Position = Model.chunks[n].ModelDataOffsetStart;
                    while (true)
                    {
                        bool found = false;
                        var ModelData = new ModelData();
                        ModelData.vertices = new List<Vertex3>();
                        ModelData.Strips = new List<int>();
                        ModelData.uv = new List<UV>();
                        if (ModelPos < Model.modelsData.Count)
                        {
                            found = true;
                            ModelData = Model.modelsData[ModelPos];
                        }

                        //Load Main Model Data Header
                        streamMatrix.Position += 48;
                        if (streamMatrix.Position >= Model.chunks[n].ModelDataOffsetEnd)
                        {
                            break;
                        }
                        ModelData.StripCount = StreamUtil.ReadInt32(streamMatrix);
                        ModelData.EdgeCount = StreamUtil.ReadInt32(streamMatrix);
                        ModelData.NormalCount = StreamUtil.ReadInt32(streamMatrix);
                        ModelData.VertexCount = StreamUtil.ReadInt32(streamMatrix);

                        //Load Strip Count
                        List<int> TempStrips = ModelData.Strips;
                        for (int d = 0; d < ModelData.StripCount; d++)
                        {
                            TempStrips.Add(StreamUtil.ReadInt32(streamMatrix));
                            streamMatrix.Position += 12;
                        }
                        ModelData.Strips = TempStrips;
                        streamMatrix.Position += 16;

                        //Read UV Texture Points
                        if (ModelData.NormalCount != 0)
                        {
                            streamMatrix.Position += 48;
                            List<UV> UVs = ModelData.uv;
                            for (int c = 0; c < ModelData.VertexCount; c++)
                            {
                                UV uv = new UV();
                                uv.X = StreamUtil.ReadInt16(streamMatrix);
                                uv.Y = StreamUtil.ReadInt16(streamMatrix);
                                //streamMatrix.Position += 4;
                                UVs.Add(uv);
                            }
                            ModelData.uv = UVs;
                            StreamUtil.AlignBy16(streamMatrix);
                        }

                        //Something todo with normals
                        if (ModelData.NormalCount != 0)
                        {
                            streamMatrix.Position += 48;
                            List<UV> UVs = ModelData.uv;
                            for (int c = 0; c < ModelData.VertexCount; c++)
                            {
                                streamMatrix.Position += 6;
                            }
                            ModelData.uv = UVs;
                            StreamUtil.AlignBy16(streamMatrix);
                        }

                        //Load Vertex
                        if (ModelData.VertexCount != 0)
                        {
                            streamMatrix.Position += 48;
                            ModelData.vertices = ReadVertex(ModelData.VertexCount, streamMatrix, ModelData.vertices);
                            StreamUtil.AlignBy16(streamMatrix);
                        }
                        streamMatrix.Position += 16;


                        if (!found)
                        {
                            Model.modelsData.Add(ModelData);
                        }
                        else
                        {
                            Model.modelsData[ModelPos] = ModelData;
                        }
                        //ModelPos++;
                    }
                }

                for (int z = 0; z < Model.modelsData.Count; z++)
                {
                    Model.modelsData[z] = GenerateFaces(Model.modelsData[z]);
                }
                ModelList[i] = Model;
                streamMatrix.Dispose();
                streamMatrix.Close();
            }
        }

        public List<Vertex3> ReadVertex(int count, Stream stream, List<Vertex3> vertices)
        {
            for (int a = 0; a < count; a++)
            {
                Vertex3 vertex = new Vertex3();
                vertex.X = StreamUtil.ReadFloat(stream);
                vertex.Y = StreamUtil.ReadFloat(stream);
                vertex.Z = StreamUtil.ReadFloat(stream);
                vertices.Add(vertex);
            }
            return vertices;
        }

        public ModelData GenerateFaces(ModelData models)
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
            for (int b = 0; b < ModelData.vertices.Count; b++)
            {
                if (InsideSplits(b, ModelData.Strips))
                {
                    localIndex = 1;
                    continue;
                }
                if (localIndex < 2)
                {
                    localIndex++;
                    continue;
                }

                ModelData.faces.Add(CreateFaces(b, ModelData));
                localIndex++;
            }

            return ModelData;
        }

        public bool InsideSplits(int Number, List<int> splits)
        {
            foreach (var item in splits)
            {
                if(item==Number)
                {
                    return true;
                }
            }
            return false;
        }

        public Face CreateFaces(int Index, ModelData ModelData)
        {
            Face face = new Face();
            face.V1 = ModelData.vertices[Index];
            face.V2 = ModelData.vertices[Index - 1];
            face.V3 = ModelData.vertices[Index - 2];

            face.V1Pos = Index;
            face.V2Pos = Index - 1;
            face.V3Pos = Index - 2;

            if (ModelData.uv.Count != 0)
            {
                face.UV1 = ModelData.uv[Index];
                face.UV2 = ModelData.uv[Index - 1];
                face.UV3 = ModelData.uv[Index - 2];

                face.UV1Pos = Index;
                face.UV2Pos = Index - 1;
                face.UV3Pos = Index - 2;
            }

            return face;
        }

        public void SaveModel(string path, int pos = 0, int ChunkPos = 0)
        {
            string output = "";
            var Model = ModelList[pos];
            int b = ChunkPos;
            var ModelData = Model.modelsData[b];
            output += "o " + Model.FileName + b.ToString() + "\n";

            //Conevert Vertices into List
            List<Vertex3> vertices = new List<Vertex3>();
            for (int i = 0; i < ModelData.faces.Count; i++)
            {
                var Face = ModelData.faces[i];
                if (!vertices.Contains(Face.V1))
                {
                    vertices.Add(Face.V1);
                }
                Face.V1Pos = vertices.IndexOf(Face.V1);

                if (!vertices.Contains(Face.V2))
                {
                    vertices.Add(Face.V2);
                }
                Face.V2Pos = vertices.IndexOf(Face.V2);

                if (!vertices.Contains(Face.V3))
                {
                    vertices.Add(Face.V3);
                }
                Face.V3Pos = vertices.IndexOf(Face.V3);

                ModelData.faces[i] = Face;
            }

            //Convert UV Points Into List
            List<UV> UV = new List<UV>();
            if (ModelData.uv.Count != 0)
            {
                for (int i = 0; i < ModelData.faces.Count; i++)
                {
                    var Face = ModelData.faces[i];
                    if (!UV.Contains(Face.UV1))
                    {
                        UV.Add(Face.UV1);
                    }
                    Face.UV1Pos = UV.IndexOf(Face.UV1);

                    if (!UV.Contains(Face.UV2))
                    {
                        UV.Add(Face.UV2);
                    }
                    Face.UV2Pos = UV.IndexOf(Face.UV2);

                    if (!UV.Contains(Face.UV3))
                    {
                        UV.Add(Face.UV3);
                    }
                    Face.UV3Pos = UV.IndexOf(Face.UV3);

                    ModelData.faces[i] = Face;
                }
            }

            for (int i = 0; i < vertices.Count; i++)
            {
                output += "v " + vertices[i].X + " " + vertices[i].Y + " " + vertices[i].Z + "\n";
            }
            //While Math Works its wrong
            for (int i = 0; i < UV.Count; i++)
            {
                output += "vt " + ( ((float)UV[i].X) / 4096 ) + " " + ( ((float)UV[i].Y) / 4096) + "\n";
            }

            if (ModelData.uv.Count != 0)
            {
                for (int i = 0; i < ModelData.faces.Count; i++)
                {
                    var Face = ModelData.faces[i];
                    output += "f " + (Face.V1Pos + 1).ToString() + "/" + (Face.UV1Pos + 1).ToString() + " " + (Face.V2Pos + 1).ToString() + "/" + (Face.UV2Pos + 1).ToString() + " " + (Face.V3Pos + 1).ToString() + "/" + (Face.UV3Pos + 1).ToString() + " " + "\n";
                }
            }
            else
            {
                for (int i = 0; i < ModelData.faces.Count; i++)
                {
                    var Face = ModelData.faces[i];
                    output += "f " + (Face.V1Pos + 1).ToString() + " " + (Face.V2Pos + 1).ToString() + " " + (Face.V3Pos + 1).ToString() + " " + "\n";
                }
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.WriteAllText(path, output);
        }

        public struct MPFModelHeader
        {
            //GlobalHeader
            public int U1;
            public int SubHeaders;
            public int HeaderSize;
            public int FileStart;
            //Main Header
            public string FileName;
            public int DataOffset;
            public int EntrySize;
            public int ModelsOffset; 
            public int RotationInfo; 
            public int ChunkOffsets;
            public int DataStart;
            //Counts
            public int ChunksCount;
            public int ModelCount; 
            public int BodyObjectsCount; 
            public int U22;
            public int U23;

            public byte[] Matrix;
            //public List<Vertex3> Unkown;
            //Matrix Data
            public List<Chunk> chunks;
            public List<BodyObjects> bodyObjectsList;
            public List<ModelData> modelsData;
            public List<Models> models;
            //
        }

        public struct ModelData
        {
            public int StripCount;
            public int EdgeCount;
            public int NormalCount;
            public int VertexCount;

            public List<UV> uv;
            public List<Vertex3> vertices;
            public List<Face> faces;
            public List<int> Strips;
        }

        public struct Models
        {
            public string modelName;
            public float Int1;
            public float Float2;
            public float Float3;
            public float Float4;
        }

        public struct Chunk
        {
            public int ID;
            public int ChunkID;
            public int ModelDataOffsetStart;
            public int ModelDataOffsetEnd;
            public int OffsetStart;
            public int OffsetEnd;
        }


        public struct Vertex3
        {
            public float X;
            public float Y;
            public float Z;
        }

        public struct UV
        {
            public int X;
            public int Y;
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
        }

        public struct BodyObjects
        {
            public string Name;
            public float Float1;
            public float Float2;
            public float Float3;
        }

        public struct MPFUnkownArray1
        {
            //Header
            public int Count;
            public int StartOffset; 
            public int EndOffset; //Sometimes Used Sometimes Not
            public List<int> IntList;
        }
    }
}
