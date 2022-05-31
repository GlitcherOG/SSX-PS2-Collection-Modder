using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SSX_Modder.ModSystem
{
    [XmlRoot("ModInfo")]
    public class ModInfo
    {
        public int ModPackVersion;
        public string Name;
        public string Game;
        public string Description;
        public string Version;
        public string Author;

        //Region Compatablity
        public bool PAL;
        public bool NTSC;
        public bool NTSCJ;
        public bool NTSCK;

        public bool PALDemo;
        public bool NTSCDemo;
        public bool NTSCJDemo;
        public bool NTSCKDemo;

        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(ModInfo));
            string paths = path + "/ModInfo.XML";
            var stream = new FileStream(paths, FileMode.Create);
            serializer.Serialize(stream, this);
            stream.Close();
        }

        public static ModInfo Load(string path)
        {
            string paths = path + "/ModInfo.XML";
            if (File.Exists(paths))
            {
                var serializer = new XmlSerializer(typeof(ModInfo));
                var stream = new FileStream(paths, FileMode.Open);
                var container = serializer.Deserialize(stream) as ModInfo;
                stream.Close();
                return container;
            }
            else
            {
                return new ModInfo();
            }
        }
    }
}
