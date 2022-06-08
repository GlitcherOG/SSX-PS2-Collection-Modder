using SSX_Modder.FileHandlers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSX_Modder.ModSystem
{
    public class ModApplication
    {
        public ModInfo modInfo = new ModInfo();
        public Image image;
        public ModMakingInstructions modInstructions = new ModMakingInstructions();

        public void LoadMod(string path)
        {
            if (image != null)
            {
                image.Dispose();
            }

            if(Directory.Exists(Application.StartupPath + "//Temp"))
            {
                Directory.Delete(Application.StartupPath + "//Temp", true);
            }

            ZipFile.ExtractToDirectory(path, Application.StartupPath + "//Temp");

            modInfo = ModInfo.Load(Application.StartupPath + "//Temp");

            if (modInfo.ModPackVersion <= 1)
            {
                modInstructions.Load(Application.StartupPath + "//Temp");

                if (File.Exists(Application.StartupPath + "//Temp//Icon.png"))
                {
                    image = Image.FromFile(Application.StartupPath + "//Temp//Icon.png");
                }
                else
                {
                    image = null;
                }
            }
            else
            {
                MessageBox.Show("Error Mod is using Newer Version Of Modpack System.");
            }
        }

//Copy
//Delete
//Big Extract
//BigF Make
//BigC0FB Make
//Config Insert
        public void ApplyMod()
        {
            if(modInfo!=new ModInfo())
            {
                var Instructions = modInstructions.Instructions;
                bool Valid = false;
                for (int i = 0; i < Instructions.Count(); i++)
                {
                    //Load Source and Output
                    string Source = Instructions[i].Source;
                    string Output = Instructions[i].Ouput;

                    if(Source.StartsWith("Game\\"))
                    {
                        Source = Source.Replace("Game\\", MainWindow.workspacePath + "//");
                    }

                    if (Source.StartsWith("Mod\\"))
                    {
                        Source = Source.Replace("Mod\\", Application.StartupPath + "//Temp//");
                    }

                    if (Output.StartsWith("Game\\"))
                    {
                        Output = Output.Replace("Game\\", MainWindow.workspacePath + "//");
                    }

                    if (Output.StartsWith("Mod\\"))
                    {
                        Output = Output.Replace("Mod\\", Application.StartupPath + "//Temp//");
                    }

                    if (Output != "")
                    {
                        Output = Path.GetFullPath(Output);
                    }

                    Source = Path.GetFullPath(Source);

                    //Check Source Is Valid
                    if (File.Exists(Source))
                    {
                        Valid = true;
                    }
                    else if (Directory.Exists(Source))
                    {
                        Valid = true;
                    }

                    if(!Valid)
                    {
                        break;
                    }

                    //Working
                    if (Instructions[i].Type=="Copy")
                    {
                        if (File.Exists(Source))
                        {
                            if(File.Exists(Output))
                            {
                                File.Delete(Output);
                            }
                            File.Copy(Source, Output);
                        }
                        else if(Directory.Exists(Source))
                        {
                            CopyDirectory(Source, Output,true);
                        }
                    }
                    //Working
                    else if(Instructions[i].Type == "Delete")
                    {
                        if (File.Exists(Source))
                        {
                            File.Delete(Source);
                        }
                        else if (Directory.Exists(Source))
                        {
                            Directory.Delete(Source, true);
                        }
                    }
                    //Working
                    else if (Instructions[i].Type == "Big Extract")
                    {
                        if (File.Exists(Source))
                        {
                            BigHandler tempHeader = new BigHandler();
                            tempHeader.LoadBig(Source);
                            tempHeader.ExtractBig(Output);
                        }
                    }
                    else if (Instructions[i].Type == "BigF Make")
                    {
                        if (Directory.Exists(Source))
                        {
                            BigHandler tempHeader = new BigHandler();
                            tempHeader.LoadFolder(Source);
                            tempHeader.bigType = BigType.BIGF;
                            tempHeader.BuildBig(Output);
                        }
                    }
                    //Working
                    else if (Instructions[i].Type == "BigC0FB Make")
                    {
                        if (Directory.Exists(Source))
                        {
                            BigHandler tempHeader = new BigHandler();
                            tempHeader.LoadFolder(Source);
                            tempHeader.bigType = BigType.C0FB;
                            tempHeader.BuildBig(Output);
                        }
                    }
                    else if (Instructions[i].Type == "Big4 Make")
                    {
                        if (Directory.Exists(Source))
                        {
                            BigHandler tempHeader = new BigHandler();
                            tempHeader.LoadFolder(Source);
                            tempHeader.bigType = BigType.BIG4;
                            tempHeader.BuildBig(Output);
                        }
                    }
                    else if (Instructions[i].Type == "Txt Insert")
                    {
                        if (File.Exists(Source))
                        {
                            var String = File.ReadAllText(Source);
                            var String1 = File.ReadAllText(Output);
                            String = String + "\n" + String1;
                            File.WriteAllText(Output, String);
                        }
                    }
                }

                if (Valid)
                {
                    if(File.Exists(MainWindow.workspacePath + "//ModList.txt"))
                    {
                        var String = File.ReadAllText(MainWindow.workspacePath + "//ModList.txt");
                        String += "\n" + modInfo.Name + " (" + modInfo.Version + ")";
                        File.WriteAllText(MainWindow.workspacePath + "//ModList.txt", String);
                    }
                    else
                    {
                        File.WriteAllText(MainWindow.workspacePath + "//ModList.txt", modInfo.Name + " (" + modInfo.Version + ")");
                    }
                    MessageBox.Show("Mod Applied");
                }
                else
                {
                    MessageBox.Show("Instructions Source Path Invalid. Are you using the correct game?");
                }
            }
            else
            {
                MessageBox.Show("No Mod Loaded");
            }
        }

        void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
        {
            // Get information about the source directory
            var dir = new DirectoryInfo(sourceDir);

            // Check if the source directory exists
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            // Cache directories before we start copying
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Create the destination directory
            Directory.CreateDirectory(destinationDir);

            // Get the files in the source directory and copy to the destination directory
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destinationDir, file.Name);
                if(File.Exists(targetFilePath))
                {
                    File.Delete(targetFilePath);
                }
                file.CopyTo(targetFilePath);
            }

            // If recursive and copying subdirectories, recursively call this method
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }
}
