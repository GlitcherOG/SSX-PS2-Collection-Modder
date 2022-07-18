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
                    modelHeader.BoneWeightOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.Unknown1 = StreamUtil.ReadInt32(stream);
                    modelHeader.MeshDataOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.Unknown2 = StreamUtil.ReadInt32(stream);
                    modelHeader.BoneDataOffset2 = StreamUtil.ReadInt32(stream);
                    modelHeader.NumberListOffset = StreamUtil.ReadInt32(stream);
                    modelHeader.BoneWeightOffset2 = StreamUtil.ReadInt32(stream);
                    modelHeader.Unknown3 = StreamUtil.ReadInt32(stream);

                    modelHeader.UnknownCount = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount2 = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount3 = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount4 = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount5 = StreamUtil.ReadInt16(stream);
                    modelHeader.UnknownCount6 = StreamUtil.ReadInt16(stream);
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



        }



        public struct MPFModelHeader
        {
            //Main Header
            public string FileName;
            public int DataOffset;
            public int EntrySize;
            public int BoneDataOffset;
            public int BoneWeightOffset;
            public int Unknown1;
            public int MeshDataOffset;
            public int Unknown2;
            public int BoneDataOffset2;
            public int NumberListOffset;
            public int BoneWeightOffset2;
            public int Unknown3;

            //Counts
            public int UnknownCount;
            public int UnknownCount2;
            public int UnknownCount3;
            public int UnknownCount4;
            public int UnknownCount5;
            public int UnknownCount6;
            public int UnknownCount7;
            public int UnknownCount8;

            public byte[] Matrix;
        }

    }
}
