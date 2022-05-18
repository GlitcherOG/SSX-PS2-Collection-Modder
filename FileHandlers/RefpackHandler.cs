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
            //Ref_Ptr not working swap with byte[]?
            //Tried solution above but doesnt read correctly later on possible need to clear ref_ptr each time. or temp output to a temp byte[] before going to output
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

//std::vector<std::uint8_t> Decompress(const std::vector<std::uint8_t>& compressed)
//{
//    const std::uint8_t* in = compressed.data();

//    // Command variables
//    std::uint8_t first;
//    std::uint8_t second;
//    std::uint8_t third;
//    std::uint8_t fourth;

//    // output buffer
//    std::vector < std::uint8_t > out;

//    std::uint32_t proc_len;
//    std::uint32_t ref_run;
//    std::uint8_t* ref_ptr;

//    if (compressed.empty())
//        return { };

//    std::uint16_t signature = ((in[0] << 8) | in[1]);

//// a bit of robustness that was integrated into SSX Tricky's RefPack mechanism
//// but not integrated into TSO (which probably came from the EAC library directly like C&C)
//// is to check if the 0xFB magic exists before trying to decompress what could be
//// malformed data.
////
//// We do that. It's probably a good idea(TM).
//if (in[1] != 0xFB) {
//    return { };
//}

//		in += sizeof(std::uint16_t);

//// skip uint24 compressed size field
//if (signature & 0x0100)
//			in += sizeof(uint24_le_t);

//// read the uint24 decompressed size
//// TODO: see if we can use uint24_le_t directly!
//std::uint32_t decompressed_size = ((in[0] << 16) | (in[1] << 8) | in[2]);
//		in += sizeof(uint24_le_t);

//		// then resize output buffer

//		out.resize(decompressed_size);
//std::uint8_t* outptr = out.data();

//while (true)
//{
//    // Retrive the first command byte
//    first = *in++;

//    if (!(first & 0x80))
//    {
//        // 2-byte command: 0DDRRRPP DDDDDDDD
//        second = *in++;

//        proc_len = first & 0x03;

//        for (std::uint32_t i = 0; i < proc_len; i++)
//            *outptr++ = *in++;

//        ref_ptr = outptr - ((first & 0x60) << 3) - second - 1;
//        ref_run = ((first >> 2) & 0x07) + 3;

//        for (std::uint32_t i = 0; i < ref_run; ++i)
//            *outptr++ = *ref_ptr++;

//    }
//    else if (!(first & 0x40))
//    {
//        // 3-byte command: 10RRRRRR PPDDDDDD DDDDDDDD
//        second = *in++;
//        third = *in++;

//        proc_len = second >> 6;

//        for (std::uint32_t i = 0; i < proc_len; ++i)
//            *outptr++ = *in++;

//        ref_ptr = outptr - ((second & 0x3f) << 8) - third - 1;
//        ref_run = (first & 0x3f) + 4;

//        for (std::uint32_t i = 0; i < ref_run; ++i)
//            *outptr++ = *ref_ptr++;

//    }
//    else if (!(first & 0x20))
//    {
//        // 4-byte command: 110DRRPP DDDDDDDD DDDDDDDD RRRRRRRR
//        second = *in++;
//        third = *in++;
//        fourth = *in++;

//        proc_len = first & 0x03;

//        for (std::uint32_t i = 0; i < proc_len; ++i)
//            *outptr++ = *in++;

//        ref_ptr = outptr - ((first & 0x10) << 12) - (second << 8) - third - 1;
//        ref_run = ((first & 0x0c) << 6) + fourth + 5;

//        for (std::uint32_t i = 0; i < ref_run; ++i)
//            *outptr++ = *ref_ptr++;
//    }
//    else
//    {
//        // 1-byte command: 111PPPPP

//        proc_len = (first & 0x1f) * 4 + 4;

//        if (proc_len <= 0x70)
//        {
//            // no stop flag

//            for (std::uint32_t i = 0; i < proc_len; ++i)
//                *outptr++ = *in++;

//        }
//        else
//        {
//            // has a stop flag
//            proc_len = first & 0x3;

//            for (std::uint32_t i = 0; i < proc_len; ++i)
//                *outptr++ = *in++;

//            break;
//        }
//    }
//}

//return out;
//	}

//} // namespace bigfile::refpack
