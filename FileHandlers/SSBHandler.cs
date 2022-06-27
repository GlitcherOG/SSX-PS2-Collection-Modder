using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSX_Modder.Utilities;

namespace SSX_Modder.FileHandlers
{
    public class SSBHandler
    {
        public List<SSBEntry> entries = new List<SSBEntry>();
        public void load(string path)
        {
            entries = new List<SSBEntry>();
            using (Stream stream = File.Open(path, FileMode.Open))
            {
                bool done = false;
                while (!done)
                {
                    SSBEntry entry = new SSBEntry();
                    entry.Magicword = StreamUtil.ReadString(stream, 4);
                    entry.Size = StreamUtil.ReadInt32(stream);
                    entry.File = StreamUtil.ReadBytes(stream, entry.Size-8);
                    RefpackHandler refpackHandler = new RefpackHandler();
                    entry.File = refpackHandler.Decompress(entry.File);
                    entries.Add(entry);
                    if (stream.Position == stream.Length)
                    {
                        done = true;
                    }
                }
            }
        }

        public void Save(string path)
        {
            Stream stream = new MemoryStream();

            for (int i = 0; i < 1; i++)
            {
                StreamUtil.WriteString(stream, entries[i].Magicword, 4);
                StreamUtil.WriteInt32(stream, entries[i].File.Length+8);
                StreamUtil.WriteBytes(stream, entries[i].File);
            }

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var file = File.Create(path);
            stream.Position = 0;
            stream.CopyTo(file);
            stream.Dispose();
            file.Close();
        }
    }
}

public struct SSBEntry
{
    public string Magicword;
    public int Size;
    public byte[] File;
}
