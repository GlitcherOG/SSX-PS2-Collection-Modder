using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    internal class RefpackHandler
    {
        byte[] Signature = new byte[2];
        public int DecompressSize;
        public int CompressSize;

        public byte[] Decompress(byte[] Matrix)
        {
            Stream stream = new MemoryStream(Matrix);

            int first;
            int second;
            int third;
            int fourth;

            byte[] Output;
            int pos = 0;

            int proc_len;
            int ref_run;
            byte[] ref_ptr;

            if(Matrix.Length==0)
            {
                return null;
            }

            stream.Read(Signature, 0, 2);

            if (Signature[1]!=0xFB)
            {
                return null;
            }

            if (Signature[0] == 0x01 && Signature[1] == 0x00)
            {
                stream.Position+=3;
            }

            CompressSize = (int)stream.Length;
            DecompressSize = StreamUtil.ReadInt24Big(stream);

            Output = new byte[DecompressSize];

            while(true)
            {
                first = stream.ReadByte();

                if ((first & 0x80)==0) //Best Guess
                {
                    second = stream.ReadByte();

                    proc_len = first & 0x03;

                    for (int i = 0; i < proc_len; i++)
                    {
                        Output[pos] = (byte)stream.ReadByte();
                        pos++;
                    }

                    ref_ptr = Output;
                    int TempPos;
                    TempPos = pos - ((first & 0x60) << 3) - second - 1;

                    ref_run = ((first >> 2) & 0x07) + 3;
                    for (int i = 0; i < ref_run; i++)
                    {
                        Output[pos] = ref_ptr[TempPos+i];
                        pos++;
                    }

                }
                else if ((first & 0x40)==0)
                {
                    second = stream.ReadByte();
                    third = stream.ReadByte();

                    proc_len = second >> 6;
                    for (int i = 0; i < proc_len; i++)
                    {
                        Output[pos] = (byte)stream.ReadByte();
                        pos++;
                    }

                    ref_ptr = Output;
                    int TempPos;
                    TempPos = pos - ((second & 0x3f) << 8) - third - 1;
                    ref_run = (first & 0x3f) + 4;

                    for (int i = 0; i < ref_run; i++)
                    {
                        Output[pos] = ref_ptr[TempPos+i];
                        pos++;
                    }

                }
                else if((first & 0x20)==0)
                {
                    second = stream.ReadByte();
                    third = stream.ReadByte();
                    fourth = stream.ReadByte();

                    proc_len = first & 0x03;

                    for (int i = 0; i < proc_len; i++)
                    {
                        Output[pos] = (byte)stream.ReadByte();
                        pos++;
                    }

                    ref_ptr = Output;
                    int TempPos;
                    TempPos = pos - ((first & 0x10) << 12) - (second << 8) - third - 1;
                    ref_run = ((first & 0x0c) << 6) + fourth + 5;

                    for (int i = 0; i < ref_run; i++)
                    {
                        Output[pos] = ref_ptr[TempPos+i];
                        pos++;
                    }
                }
                else
                {
                    proc_len = (first & 0x1f) * 4 + 4;

                    if (proc_len <= 0x70)
                    {
                        // no stop flag

                        for (int i = 0; i < proc_len; i++)
                        {
                            Output[pos] = (byte)stream.ReadByte();
                            pos++;
                        }

                    }
                    else
                    {
                        // has a stop flag
                        proc_len = first & 0x3;

                        for (int i = 0; i < proc_len; i++)
                        {
                            Output[pos] = (byte)stream.ReadByte();
                            pos++;
                        }

                        break;
                    }
                }

            }

            stream.Dispose();
            stream.Close();
            return Output;
        }
    }
}
