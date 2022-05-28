using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSX_Modder.Utilities
{
    internal class ImageUtil
    {
        public static void Brighten(string path)
        {
            Stream stream = File.Open(path, FileMode.Open);
            var ImageTemp = Image.FromStream(stream);
            stream.Close();
            stream.Dispose();
            var bitmap = (Bitmap)ImageTemp;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    int A = color.A;
                    int R = color.R * 2 - 1;
                    if (R < 0)
                    {
                        R = 0;
                    }
                    else if (R > 255)
                    {
                        R = 255;
                    }
                    int G = color.G * 2 - 1;
                    if (G < 0)
                    {
                        G = 0;
                    }
                    else if (G > 255)
                    {
                        G = 255;
                    }
                    int B = color.B * 2 - 1;
                    if (B < 0)
                    {
                        B = 0;
                    }
                    else if (B > 255)
                    {
                        B = 255;
                    }

                    color = Color.FromArgb(A, R, G, B);
                    bitmap.SetPixel(x, y, color);
                }
            }

            bitmap.Save(path);
        }

        public static void Darken(string path)
        {
            Stream stream = File.Open(path, FileMode.Open);
            var ImageTemp = Image.FromStream(stream);
            stream.Close();
            stream.Dispose();
            var bitmap = (Bitmap)ImageTemp;

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    int A = color.A;
                    int R = color.R;
                    int G = color.G;
                    int B = color.B;

                    R = (R + 1) / 2;
                    G = (G + 1) / 2;
                    B = (B + 1) / 2;

                    color = Color.FromArgb(A, R, G, B);
                    bitmap.SetPixel(x, y, color);
                }
            }

            bitmap.Save(path);
        }
    }
}
