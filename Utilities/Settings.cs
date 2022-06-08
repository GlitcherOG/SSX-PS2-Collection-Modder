using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SSX_Modder.Utilities
{
    [XmlRoot("Settings")]
    public class Settings
    {
        public string ImgBurnPath = @"C:\Program Files (x86)\ImgBurn\ImgBurn.exe";
        public string ZipPath = @"C:\Program Files\7-Zip\7z.exe";
        public string Pcsx2Path = "";
        public string SSXISOPath = "";
        public string SSX2ISOPath = "";
        public string SSX3ISOPath = "";
        public string SSX4ISOPath = "";
        public string ExtractorArg = "x \"#Source\" *.* -o\"#Output\" -r -y";
        public string IsoArg = "/MODE BUILD /BUILDINPUTMODE IMAGEFILE /SRC \"#Source\" /DEST \"#Output\" /FILESYSTEM \"ISO9660 + UDF\" /UDFREVISION \"1.02\" /VOLUMELABEL \"SSX\" /ERASE /OVERWITE YES /START /NOIMAGEDETAILS /ROOTFOLDER /CLOSE";
        public int Game = 2;
        public void Save()
        {
            var serializer = new XmlSerializer(typeof(Settings));
            string paths = Directory.GetCurrentDirectory() + "/Config.XML";
            var stream = new FileStream(paths, FileMode.Create);
            serializer.Serialize(stream, this);
            stream.Close();
        }

        public static Settings Load()
        {
            string paths = Directory.GetCurrentDirectory() + "/Config.XML";
            if (File.Exists(paths))
            {
                var serializer = new XmlSerializer(typeof(Settings));
                var stream = new FileStream(paths, FileMode.Open);
                var container = serializer.Deserialize(stream) as Settings;
                stream.Close();
                return container;
            }
            else
            {
                return new Settings();
            }
        }
    }
}
