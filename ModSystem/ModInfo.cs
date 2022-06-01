using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SSX_Modder.ModSystem
{
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
            var serializer = JsonConvert.SerializeObject(this);
            string paths = path + "/ModInfo.json";
            File.WriteAllText(paths, serializer);
        }

        public static ModInfo Load(string path)
        {
            string paths = path + "/ModInfo.json";
            if (File.Exists(paths))
            {
                var stream = File.ReadAllText(paths);
                var container = JsonConvert.DeserializeObject<ModInfo>(stream);
                return container;
            }
            else
            {
                return new ModInfo();
            }
        }
    }
}
