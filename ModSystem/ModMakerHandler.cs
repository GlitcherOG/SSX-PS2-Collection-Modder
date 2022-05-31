using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SSX_Modder.ModSystem
{
    public class ModMakerHandler
    {
        public string ModFolder;
        public Image Icon;
        public ModInfo ModInfo;

        public void PackMod(string ModLocation)
        {
            if(File.Exists(ModLocation))
            {
                File.Delete(ModLocation);
            }
            ZipFile.CreateFromDirectory(ModFolder, ModLocation);
        }
    }
}
