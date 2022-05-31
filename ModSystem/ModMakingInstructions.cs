using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SSX_Modder.ModSystem
{
    public class ModMakingInstructions
    {
        public string ModPath = "";
        public List<Instruction> Instructions = new List<Instruction>();

        public void Save()
        {
            string paths = ModPath + "/ModInstructions.txt";
            string Main = "";
            for (int i = 0; i < Instructions.Count; i++)
            {
                Main += Instructions[i].Type + "," + Instructions[i].Source + "," + Instructions[i].Ouput;
                if(i!= Instructions.Count-1)
                {
                    Main += Environment.NewLine;
                }
            }
            File.WriteAllText(paths, Main);
        }

        public void Load(string path)
        {
            string paths = path + "//ModInstructions.txt";
            if (File.Exists(paths))
            {
                string[] Array = File.ReadAllLines(paths);
                for (int i = 0; i < Array.Length; i++)
                {
                    Instruction instruction = new Instruction();
                    string[] SplitLine = Array[i].Split(',');
                    instruction.Type = SplitLine[0];
                    instruction.Source = SplitLine[1];
                    instruction.Ouput = SplitLine[2];
                    Instructions.Add(instruction);
                }

                ModPath = path;
            }
            else
            {
                ModPath = path;
                Instructions = new List<Instruction>();
            }
        }
    }

    public struct Instruction
    {
        public string Type;
        public string Source;
        public string Ouput;
    }
}
