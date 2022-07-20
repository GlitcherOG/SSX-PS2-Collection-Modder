using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    /// <summary>
    /// NEEDS MASSIVE FIXES AND IS JUST A COPY PASTE OF THE SSX FILE
    /// </summary>
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
                    modelHeader.ChunkOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.MeshDataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.Unknown2 = StreamUtil.ReadInt32(stream);
                    modelHeader.BoneDataOffset2 = StreamUtil.ReadInt32(stream);
                    modelHeader.NumberListOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.BoneWeightOffset2 = StreamUtil.ReadInt32(stream);
                    modelHeader.Unknown3 = StreamUtil.ReadInt32(stream);

                    modelHeader.UnknownCount = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount2 = StreamUtil.ReadInt16(stream);
                    modelHeader.ChunkCount = StreamUtil.ReadInt16(stream);
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

                //Chunk Data
                streamMatrix.Position = Model.ChunkOffset;
                Model.ChunkDatas = new List<ChunkData>();

                for (int a = 0; a < Model.ChunkCount; a++)
                {
                    var TempChunkData = new ChunkData();
                    TempChunkData.ID = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.MaterialID = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.Unknown = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.LinkOffsetCount = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.LinkOffset = StreamUtil.ReadInt32(streamMatrix);

                    int TempPos = (int)streamMatrix.Position;
                    streamMatrix.Position = TempChunkData.LinkOffset;

                    TempChunkData.LinkOffset2 = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.LinkOffsetCount2 = StreamUtil.ReadInt32(streamMatrix);

                    streamMatrix.Position = TempChunkData.LinkOffset2;
                    TempChunkData.ModelOffset = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.Unknown2 = StreamUtil.ReadInt32(streamMatrix);
                    TempChunkData.Unknown3 = StreamUtil.ReadInt32(streamMatrix);

                    streamMatrix.Position = TempPos;

                    Model.ChunkDatas.Add(TempChunkData);
                }

                ModelList[i] = Model;
            }

        }


        public struct MPFModelHeader
        {
            //Main Header
            public string FileName;
            public int DataOffset;
            public int EntrySize;
            public int BoneDataOffset;
            public int IKPointOffset;
            public int ChunkOffset;
            public int MeshDataOffset;
            public int Unknown2;
            public int BoneDataOffset2;
            public int NumberListOffset;
            public int BoneWeightOffset2;
            public int Unknown3;

            //Counts
            public int UnknownCount;
            public int UnknownCount2;
            public int ChunkCount;
            public int BoneDataCount;
            public int MaterialCount;
            public int IKCount;
            public int UnknownCount7;
            public int UnknownCount8;

            public byte[] Matrix;

            public List<MaterialData> materialDatas;
            public List<BoneData> boneDatas;
            public List<IKPoint> iKPoints;
            public List<ChunkData> ChunkDatas;
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

        public struct ChunkData
        {
            public int ID;
            public int MaterialID;
            public int Unknown;
            public int LinkOffsetCount;
            public int LinkOffset;

            public int LinkOffset2;
            public int LinkOffsetCount2;

            public int ModelOffset;
            public int Unknown2;
            public int Unknown3;
        }
    }
}
