﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    public class TrickyMPFModelHandler
    {
        public int U1;
        public int HeaderCount;
        public int HeaderSize;
        public int FileStart;
        public List<MPFModelHeader> ModelList = new List<MPFModelHeader>();

        public void load(string path)
        {
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                U1 = StreamUtil.ReadInt32(stream);
                HeaderCount = StreamUtil.ReadInt16(stream);
                HeaderSize = StreamUtil.ReadInt16(stream);
                FileStart = StreamUtil.ReadInt32(stream);
                //Load Headers
                for (int i = 0; i < HeaderCount; i++)
                {
                    MPFModelHeader modelHeader = new MPFModelHeader();

                    modelHeader.FileName = StreamUtil.ReadString(stream, 16);
                    modelHeader.DataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.EntrySize = StreamUtil.ReadInt32(stream);
                    modelHeader.BoneDataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.IKPointOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.MeshGroupOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.MeshDataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.MaterialOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.NumberListOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.BoneWeightOffet = StreamUtil.ReadInt32(stream);

                    modelHeader.Unused1 = StreamUtil.ReadInt32(stream);
                    modelHeader.Unused2 = StreamUtil.ReadInt32(stream);

                    modelHeader.BoneWeightCount = StreamUtil.ReadInt16(stream);
                    modelHeader.NumberListCount = StreamUtil.ReadInt16(stream);
                    modelHeader.MeshGroupCount = StreamUtil.ReadInt16(stream);
                    modelHeader.BoneDataCount = StreamUtil.ReadInt16(stream);
                    modelHeader.MaterialCount = StreamUtil.ReadInt16(stream);
                    modelHeader.IKCount = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount7 = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount8 = StreamUtil.ReadInt16(stream);
                    stream.Position += 4;

                    ModelList.Add(modelHeader);
                }

                //Read Matrix
                int StartPos = FileStart;
                for (int i = 0; i < ModelList.Count; i++)
                {
                    stream.Position = StartPos + ModelList[i].DataOffset;
                    MPFModelHeader modelHandler = ModelList[i];
                    modelHandler.Matrix = StreamUtil.ReadBytes(stream, ModelList[i].EntrySize);
                    RefpackHandler refpackHandler = new RefpackHandler();
                    //modelHandler.Matrix = refpackHandler.Decompress(modelHandler.Matrix);
                    ModelList[i] = modelHandler;
                }
            }

            for (int i = 0; i < ModelList.Count; i++)
            {
                Stream streamMatrix = new MemoryStream();
                var Model = ModelList[i];
                streamMatrix.Write(ModelList[i].Matrix, 0, ModelList[i].Matrix.Length);
                streamMatrix.Position = 0;

                Model.materialDatas = new List<MaterialData>();
                for (int a = 0; a < Model.MaterialCount; a++)
                {
                    var TempMat = new MaterialData();
                    TempMat.MainTexture = StreamUtil.ReadString(streamMatrix, 4);
                    TempMat.Texture1 = StreamUtil.ReadString(streamMatrix, 4);
                    TempMat.Texture2 = StreamUtil.ReadString(streamMatrix, 4);
                    TempMat.Texture3 = StreamUtil.ReadString(streamMatrix, 4);
                    TempMat.Texture4 = StreamUtil.ReadString(streamMatrix, 4);

                    TempMat.R = StreamUtil.ReadFloat(streamMatrix);
                    TempMat.G = StreamUtil.ReadFloat(streamMatrix);
                    TempMat.B = StreamUtil.ReadFloat(streamMatrix);
                    Model.materialDatas.Add(TempMat);
                }

                streamMatrix.Position = Model.BoneDataOffset;
                Model.boneDatas = new List<BoneData>();
                for (int a = 0; a < Model.BoneDataCount; a++)
                {
                    var TempBoneData = new BoneData();
                    TempBoneData.BoneName = StreamUtil.ReadString(streamMatrix, 16);
                    TempBoneData.Unknown = StreamUtil.ReadInt16(streamMatrix);
                    TempBoneData.ParentBone = StreamUtil.ReadInt16(streamMatrix);
                    TempBoneData.Unknown2 = StreamUtil.ReadInt16(streamMatrix);
                    TempBoneData.BoneID = StreamUtil.ReadInt16(streamMatrix);
                    TempBoneData.XLocation = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.YLocation = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.ZLocation = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.XRadian = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.YRadian = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.ZRadian = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.XRadian2 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.YRadian2 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.ZRadian2 = StreamUtil.ReadFloat(streamMatrix);

                    TempBoneData.UnknownFloat1 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.UnknownFloat2 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.UnknownFloat3 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.UnknownFloat4 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.UnknownFloat5 = StreamUtil.ReadFloat(streamMatrix);
                    TempBoneData.UnknownFloat6 = StreamUtil.ReadFloat(streamMatrix);
                    Model.boneDatas.Add(TempBoneData);
                }

                streamMatrix.Position = Model.IKPointOffset;
                Model.iKPoints = new List<IKPoint>();
                for (int a = 0; a < Model.IKCount; a++)
                {
                    var TempIKPoint = new IKPoint();
                    TempIKPoint.X = StreamUtil.ReadFloat(streamMatrix);
                    TempIKPoint.Y = StreamUtil.ReadFloat(streamMatrix);
                    TempIKPoint.Z = StreamUtil.ReadFloat(streamMatrix);
                    streamMatrix.Position += 4;
                    Model.iKPoints.Add(TempIKPoint);
                }

                //Mesh Group Data
                streamMatrix.Position = Model.MeshGroupOffset;
                Model.MeshGroups = new List<GroupMainHeader>();

                for (int a = 0; a < Model.MeshGroupCount; a++)
                {
                    var TempChunkData = new GroupMainHeader();
                    TempChunkData.ID = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.MaterialID = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.Unknown = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.LinkCount = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.LinkOffset = StreamUtil.ReadInt32(streamMatrix);

                    int TempPos = (int)streamMatrix.Position;
                    streamMatrix.Position = TempChunkData.LinkOffset;
                    TempChunkData.meshGroupSubs = new List<GroupSubHeader>();
                    for (int b = 0; b < TempChunkData.LinkCount; b++)
                    {
                        var TempSubHeader = new GroupSubHeader();
                        TempSubHeader.LinkOffset = StreamUtil.ReadInt32(streamMatrix);
                        TempSubHeader.LinkCount = StreamUtil.ReadInt32(streamMatrix);
                        int TempPos1 = (int)streamMatrix.Position;
                        TempSubHeader.MeshGroupHeaders = new List<MeshGroupHeader>();
                        streamMatrix.Position = TempSubHeader.LinkOffset;
                        for (int c = 0; c < TempSubHeader.LinkCount; c++)
                        {
                            var TempMeshGroupHeader = new MeshGroupHeader();
                            TempMeshGroupHeader.ModelOffset = StreamUtil.ReadInt32(streamMatrix);
                            TempMeshGroupHeader.Unknown2 = StreamUtil.ReadInt32(streamMatrix);
                            TempMeshGroupHeader.Unknown3 = StreamUtil.ReadInt32(streamMatrix);
                            TempSubHeader.MeshGroupHeaders.Add(TempMeshGroupHeader);
                        }
                        streamMatrix.Position = TempPos1;
                        TempChunkData.meshGroupSubs.Add(TempSubHeader);
                    }

                    streamMatrix.Position = TempPos;

                    Model.MeshGroups.Add(TempChunkData);
                }

                //Bone Weight
                streamMatrix.Position = Model.BoneWeightOffet;
                Model.boneWeightHeader = new List<BoneWeightHeader>();

                for (int b = 0; b < Model.BoneWeightCount; b++)
                {
                    var BoneWeight = new BoneWeightHeader();

                    BoneWeight.length = StreamUtil.ReadInt32(streamMatrix);
                    BoneWeight.WeightListOffset = StreamUtil.ReadInt32(streamMatrix);
                    BoneWeight.unknown = StreamUtil.ReadInt32(streamMatrix);
                    BoneWeight.boneWeights = new List<BoneWeight>();
                    int TempPos = (int)streamMatrix.Position;
                    streamMatrix.Position = BoneWeight.WeightListOffset;
                    for (int a = 0; a < BoneWeight.length; a++)
                    {
                        var boneWeight = new BoneWeight();
                        boneWeight.weight = StreamUtil.ReadInt16(streamMatrix);
                        boneWeight.ID = StreamUtil.ReadByte(streamMatrix);
                        boneWeight.unknown = StreamUtil.ReadByte(streamMatrix);
                        BoneWeight.boneWeights.Add(boneWeight);
                    }
                    streamMatrix.Position = TempPos;
                    Model.boneWeightHeader.Add(BoneWeight);
                }

                //Number List Ref
                streamMatrix.Position = Model.NumberListOffset;
                Model.numberListRefs = new List<NumberListRef>();
                for (int b = 0; b < Model.NumberListCount; b++)
                {
                    var NumberListRef = new NumberListRef();
                    NumberListRef.SubCount = StreamUtil.ReadInt32(streamMatrix);
                    NumberListRef.Offset = StreamUtil.ReadInt32(streamMatrix);
                    NumberListRef.subNumberRefs = new List<SubNumberRef>();

                    int TempPos = (int)streamMatrix.Position;
                    streamMatrix.Position = NumberListRef.Offset;
                    for (int c = 0; c < NumberListRef.SubCount; c++)
                    {
                        var SubNumberRef = new SubNumberRef();
                        SubNumberRef.Unknown = StreamUtil.ReadInt32(streamMatrix);
                        NumberListRef.subNumberRefs.Add(SubNumberRef);
                    }
                    streamMatrix.Position = TempPos;
                    Model.numberListRefs.Add(NumberListRef);
                }


                Model.staticMesh = new List<StaticMesh>();

                for (int ax = 0; ax < Model.MeshGroupCount; ax++)
                {
                    var GroupHeader = Model.MeshGroups[ax];
                    for (int bx = 0; bx < GroupHeader.meshGroupSubs.Count; bx++)
                    {
                        var SubGroupHeader = GroupHeader.meshGroupSubs[bx];
                        for (int cx = 0; cx < SubGroupHeader.MeshGroupHeaders.Count; cx++)
                        {
                            var SubSubGroupHeader = SubGroupHeader.MeshGroupHeaders[cx];
                            streamMatrix.Position = SubSubGroupHeader.ModelOffset;
                            int z = 0;
                            while (true)
                            {
                                streamMatrix.Position += 31;
                                byte Temp = StreamUtil.ReadByte(streamMatrix);
                                if (Temp != 0x6C)
                                {
                                    break;
                                }
                                streamMatrix.Position += 16;
                                var ModelData = new StaticMesh();
                                ModelData.ChunkID = ax;
                                ModelData.GroupID = bx;
                                ModelData.SubGroupID = cx;
                                ModelData.SubGroupPosition = z;
                                z++;

                                ModelData.StripCount = StreamUtil.ReadInt32(streamMatrix);
                                ModelData.EdgeCount = StreamUtil.ReadInt32(streamMatrix);
                                ModelData.NormalCount = StreamUtil.ReadInt32(streamMatrix);
                                ModelData.VertexCount = StreamUtil.ReadInt32(streamMatrix);

                                //Load Strip Count
                                List<int> TempStrips = new List<int>();
                                for (int a = 0; a < ModelData.StripCount; a++)
                                {
                                    TempStrips.Add(StreamUtil.ReadInt32(streamMatrix));
                                    streamMatrix.Position += 12;
                                }
                                streamMatrix.Position += 16;
                                ModelData.Strips = TempStrips;

                                List<UV> UVs = new List<UV>();
                                //Read UV Texture Points
                                if (ModelData.NormalCount != 0)
                                {
                                    streamMatrix.Position += 48;
                                    for (int a = 0; a < ModelData.VertexCount; a++)
                                    {
                                        UV uv = new UV();
                                        uv.X = StreamUtil.ReadInt16(streamMatrix);
                                        uv.Y = StreamUtil.ReadInt16(streamMatrix);
                                        uv.XU = StreamUtil.ReadInt16(streamMatrix);
                                        uv.YU = StreamUtil.ReadInt16(streamMatrix);
                                        UVs.Add(uv);
                                    }
                                    StreamUtil.AlignBy16(streamMatrix);
                                }
                                ModelData.uv = UVs;

                                List<UVNormal> Normals = new List<UVNormal>();
                                //Read Normals
                                if (ModelData.NormalCount != 0)
                                {
                                    streamMatrix.Position += 48;
                                    for (int a = 0; a < ModelData.VertexCount; a++)
                                    {
                                        UVNormal normal = new UVNormal();
                                        normal.X = StreamUtil.ReadInt16(streamMatrix);
                                        normal.Y = StreamUtil.ReadInt16(streamMatrix);
                                        normal.Z = StreamUtil.ReadInt16(streamMatrix);
                                        Normals.Add(normal);
                                    }
                                    StreamUtil.AlignBy16(streamMatrix);
                                }
                                ModelData.uvNormals = Normals;

                                List<Vertex3> vertices = new List<Vertex3>();
                                //Load Vertex
                                if (ModelData.VertexCount != 0)
                                {
                                    streamMatrix.Position += 47;
                                    //Can also make it use normal but this seems safer
                                    if (StreamUtil.ReadByte(streamMatrix) != 0x6C)
                                    {
                                        for (int a = 0; a < ModelData.VertexCount; a++)
                                        {
                                            Vertex3 vertex = new Vertex3();
                                            vertex.X = StreamUtil.ReadFloat(streamMatrix);
                                            vertex.Y = StreamUtil.ReadFloat(streamMatrix);
                                            vertex.Z = StreamUtil.ReadFloat(streamMatrix);
                                            vertices.Add(vertex);
                                        }
                                        StreamUtil.AlignBy16(streamMatrix);
                                    }
                                    else
                                    {
                                        for (int a = 0; a < ModelData.VertexCount; a++)
                                        {
                                            Vertex3 vertex = new Vertex3();
                                            vertex.X = StreamUtil.ReadFloat(streamMatrix);
                                            vertex.Y = StreamUtil.ReadFloat(streamMatrix);
                                            vertex.Z = StreamUtil.ReadFloat(streamMatrix);
                                            vertex.Unknown = StreamUtil.ReadFloat(streamMatrix);
                                            vertices.Add(vertex);
                                        }
                                        StreamUtil.AlignBy16(streamMatrix);
                                    }
                                }
                                ModelData.vertices = vertices;

                                streamMatrix.Position += 16 * 2;
                                Model.staticMesh.Add(ModelData);
                            } 
                        }
                    }
                }


                for (int b = 0; b < Model.staticMesh.Count; b++)
                {
                    Model.staticMesh[b] = GenerateFaces(Model.staticMesh[b]);
                }


                ModelList[i] = Model;
            }

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

        public void SaveModel(string path)
        {
            glstHandler.SaveTrickyglTF(path, this);
        }


        public struct MPFModelHeader
        {
            //Main Header
            public string FileName;
            public int DataOffset;
            public int EntrySize;
            public int BoneDataOffset;
            public int IKPointOffset;
            public int MeshGroupOffset;
            public int MeshDataOffset;
            public int MaterialOffset;
            public int NumberListOffset;
            public int BoneWeightOffet;
            public int Unused1;
            public int Unused2;

            //Counts
            public int BoneWeightCount;
            public int NumberListCount;
            public int MeshGroupCount;
            public int BoneDataCount;
            public int MaterialCount;
            public int IKCount;
            public int UnknownCount7;
            public int UnknownCount8;

            public byte[] Matrix;

            public List<MaterialData> materialDatas;
            public List<BoneData> boneDatas;
            public List<IKPoint> iKPoints;
            public List<GroupMainHeader> MeshGroups;
            public List<BoneWeightHeader> boneWeightHeader;
            public List<StaticMesh> staticMesh;
            public List<NumberListRef> numberListRefs;
        }

        public struct NumberListRef
        {
            public int SubCount;
            public int Offset;

            public List<SubNumberRef> subNumberRefs;
        }
        public struct SubNumberRef
        {
            public int Unknown;
        }

        public struct MaterialData
        {
            public string MainTexture;
            public string Texture1;
            public string Texture2;
            public string Texture3;
            public string Texture4;

            public float R;
            public float G;
            public float B;
        }

        public struct BoneData
        {
            public string BoneName;
            public int Unknown;
            public int ParentBone;
            public int Unknown2;
            public int BoneID;
            public float XLocation;
            public float YLocation;
            public float ZLocation;

            public float XRadian;
            public float YRadian;
            public float ZRadian;
            public float XRadian2;
            public float YRadian2;
            public float ZRadian2;

            public float UnknownFloat1;
            public float UnknownFloat2;
            public float UnknownFloat3;
            public float UnknownFloat4;
            public float UnknownFloat5;
            public float UnknownFloat6;

        }

        public struct IKPoint
        {
            public float X;
            public float Y;
            public float Z;
        }

        public struct GroupMainHeader
        {
            public int ID;
            public int MaterialID;
            public int Unknown;
            public int LinkCount;
            public int LinkOffset;

            public List<GroupSubHeader> meshGroupSubs;
        }

        public struct GroupSubHeader
        {
            public int LinkOffset;
            public int LinkCount;

            public List<MeshGroupHeader> MeshGroupHeaders;
        }

        public struct MeshGroupHeader
        {
            public int ModelOffset;
            public int Unknown2;
            public int Unknown3;
        }

        public struct BoneWeightHeader
        {
            public int length;
            public int WeightListOffset;
            public int unknown;

            public List<BoneWeight> boneWeights;
        }

        public struct BoneWeight
        {
            public int weight;
            public int ID;
            public int unknown;
        }

        public struct StaticMesh
        {
            public int ChunkID;
            public int GroupID;
            public int SubGroupID;
            public int SubGroupPosition;

            public int StripCount;
            public int EdgeCount;
            public int NormalCount;
            public int VertexCount;
            public List<int> Strips;

            public List<UV> uv;
            public List<Vertex3> vertices;
            public List<Face> faces;
            public List<UVNormal> uvNormals;
        }

        public struct Vertex3
        {
            public float X;
            public float Y;
            public float Z;

            public float Unknown;
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
    }
}
