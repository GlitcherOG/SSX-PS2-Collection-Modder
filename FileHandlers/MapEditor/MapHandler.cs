using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSX_Modder.FileHandlers.MapEditor
{
    public class MapHandler
    {
        public List<LinkerItem> Models;
        public List<LinkerItem> particelModels;
        public List<LinkerItem> Patchs;
        public List<LinkerItem> InternalInstances;
        public List<LinkerItem> PlayerStarts; //Unused
        public List<LinkerItem> ParticleInstances;
        public List<LinkerItem> Splines;
        public List<LinkerItem> Lights;
        public List<LinkerItem> Materials;
        public List<LinkerItem> ContextBlocks;
        public List<LinkerItem> Cameras;
        public List<LinkerItem> Textures;
        public List<LinkerItem> Lightmaps;
        public void Load(string path)
        {
            string[] Lines = File.ReadAllLines(path);

            int LinePos = 23;

            Models=new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0,82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Models.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            particelModels = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                particelModels.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            Patchs = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Patchs.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            InternalInstances = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                InternalInstances.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            PlayerStarts = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                PlayerStarts.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            ParticleInstances = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                ParticleInstances.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            Splines = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Splines.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            Lights = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Lights.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            Materials = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Materials.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            ContextBlocks = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                ContextBlocks.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 9;
            Cameras = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Cameras.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 8;
            Textures = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "" || Lines[LinePos].Length == 0)
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Textures.Add(LinkerItem);
                LinePos++;
            }

            LinePos += 8;
            Lightmaps = new List<LinkerItem>();
            while (true)
            {
                if (Lines[LinePos] == "")
                {
                    break;
                }
                var LinkerItem = new LinkerItem();
                LinkerItem.Name = Lines[LinePos].Substring(0, 82);
                LinkerItem.UID = Int32.Parse(Lines[LinePos].Substring(82, 10).Replace(" ", ""));
                LinkerItem.Ref = Int32.Parse(Lines[LinePos].Substring(92, 10).Replace(" ", ""));
                LinkerItem.Hashvalue = Lines[LinePos].Substring(102, 10);
                Lightmaps.Add(LinkerItem);
                LinePos++;
            }
        }
    }

    public struct LinkerItem
    {
        public string Name;
        public int UID;
        public int Ref;
        public string Hashvalue;
    }
}
