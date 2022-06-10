using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSX_Modder.FileHandlers
{
    public class CSVHandler
    {
        public string[,] StringArray;

        public void SetSize(int x, int y)
        {
            StringArray = new string[x, y];
        }

        public void SetPosition(string var, int x, int y)
        {
            StringArray[x, y] = var;
        }

        public string ReadPosition(int x, int y)
        {
            return StringArray[x, y];
        }

        public int ReadPositionInt(int x, int y)
        {
            return int.Parse(StringArray[x, y]);
        }

        public void Save(string path)
        {

        }

        public void Load(string path)
        {

        }
    }
}
