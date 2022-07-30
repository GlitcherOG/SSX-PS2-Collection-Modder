using System;
using System.IO;
using System.Windows.Forms;
using SSX_Modder.FileHandlers;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using System.Drawing.Imaging;
using SSX_Modder.Utilities;
using SSX_Modder.FileHandlers.MapEditor;
using SSX_Modder.ModSystem;
using System.Collections.Generic;

namespace SSX_Modder
{
    public partial class MainWindow : Form
    {
        public static Settings settings = new Settings();
        public static string workspacePath = Application.StartupPath + "\\disk\\SSX 3\\";
        public static string ToolsPath = Application.StartupPath + "\\Tools\\";
        public MainWindow()
        {
            InitializeComponent();
            settings = Settings.Load();
            Settings7ZipPath.Text = settings.ZipPath;
            SettingsImgBurn.Text = settings.ImgBurnPath;
            SettingsPCSX2Path.Text = settings.Pcsx2Path;
            GameType.SelectedIndex = settings.Game;
            SettingsExtractorArg.Text = settings.ExtractorArg;
            SettingsIsoBuilderArg.Text = settings.IsoArg;
            if (GameType.SelectedIndex == 0)
            {
                SettingsIsoPath.Text = settings.SSXISOPath;
            }
            if (GameType.SelectedIndex == 1)
            {
                SettingsIsoPath.Text = settings.SSX2ISOPath;
            }
            if (GameType.SelectedIndex == 2)
            {
                SettingsIsoPath.Text = settings.SSX3ISOPath;
            }
            if (GameType.SelectedIndex == 3)
            {
                SettingsIsoPath.Text = settings.SSX4ISOPath;
            }

            Directory.CreateDirectory(ToolsPath);

            if(File.Exists(ToolsPath+"\\chimp.exe"))
            {
                SettingsChimp.Checked = true;
            }
            if (File.Exists(ToolsPath + "\\ffmpeg.exe"))
            {
                Settingsffmpeg.Checked = true;
            }
            if (File.Exists(ToolsPath + "\\gx.exe"))
            {
                SettingsGX.Checked = true;
            }
            if (File.Exists(ToolsPath + "\\sx.exe"))
            {
                SettingsSX.Checked = true;
            }
        }
        private void ResetStatus(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
        }

        void SetStatus(string status)
        {
            toolStripStatusLabel1.Text = status;
        }
        private void MainWindow_Load(object sender, EventArgs e)
        {
            if (!Directory.Exists(workspacePath))
            {
                Directory.CreateDirectory(workspacePath);
            }

        }

        #region ToolStrip
        private void toolStripExtract_Click(object sender, EventArgs e)
        {
            if (File.Exists(settings.ZipPath))
            {
                //MessageBox.Show("Extractor Currently Disabled. Extract Iso Files To \n" + workspacePath);
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = "c:\\",
                    Filter = "Iso Image (*.iso)|*.iso|All files (*.*)|*.*",
                    FilterIndex = 1,
                    //RestoreDirectory = true
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ISOHandler.Extract(openFileDialog.FileName);
                }
            }
            else
            {
                MessageBox.Show("Extractor Not Detected");
            }
        }

        private void toolStripBuild_Click(object sender, EventArgs e)
        {
            if (File.Exists(settings.ImgBurnPath))
            {
                if (settings.Game != 0)
                {
                    SaveFileDialog openFileDialog = new SaveFileDialog
                    {
                        InitialDirectory = Application.StartupPath,
                        Filter = "Iso Image (*.iso)|*.iso|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = false
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        if(File.Exists(openFileDialog.FileName))
                        {
                            File.Delete(openFileDialog.FileName);
                            while(File.Exists(openFileDialog.FileName))
                            {
                                //Waits for file to be gone
                            }
                        }
                        ISOHandler.Build(openFileDialog.FileName, true);
                        MessageBox.Show("Iso Build Completed");
                    }
                }
                else
                {
                    MessageBox.Show("Warning Unable To Build Iso Of Game (SSX - ElfLdr Recommended)");
                }
            }
            else
            {
                MessageBox.Show("Iso Builder Not Detected");
            }
        }

        private void toolStripBuildRun_Click(object sender, EventArgs e)
        {
            if (File.Exists(settings.ImgBurnPath))
            {
                string path = "";
                if (MainWindow.settings.Game == 0)
                {
                    path = MainWindow.settings.SSXISOPath;
                }
                else if (MainWindow.settings.Game == 1)
                {
                    path = MainWindow.settings.SSX2ISOPath;
                }
                else if (MainWindow.settings.Game == 2)
                {
                    path = MainWindow.settings.SSX3ISOPath;
                }
                else if (MainWindow.settings.Game == 3)
                {
                    path = MainWindow.settings.SSX4ISOPath;
                }

                if(path!=""&&path!=null)
                {
                    File.Delete(path);
                    while (File.Exists(path))
                    {
                        //Waits for file to be gone
                    }
                    ISOHandler.Build(path, true);
                    if (File.Exists(settings.Pcsx2Path))
                    {
                        ISOHandler.Run();
                    }
                    else
                    {
                        MessageBox.Show("Emulator Path Not Detected");
                    }
                }
                else
                {
                    MessageBox.Show("Iso build path not set. Do this in settings or by building normally once");
                }
            }
            else
            {
                MessageBox.Show("Iso Builder Not Detected");
            }
        }

        private void toolStripWorkspace_Click(object sender, EventArgs e)
        {
            Process.Start(workspacePath);
        }
        private void toolStripGameStart_Click(object sender, EventArgs e)
        {
            if (File.Exists(settings.Pcsx2Path))
            {
                ISOHandler.Run();
            }
            else
            {
                MessageBox.Show("Emulator Not Detected");
            }
        }
        #endregion

        #region Tools
        private void ToolsColours_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "Png Image (*.Png)|*.Png|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string strCmdText = "/C magick convert \"" + openFileDialog.FileName + "\" -colors " + (int)NumToolsColour.Value + " \"" + openFileDialog.FileName + "\"";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
            }
        }

        private void ToolsPadding_Click(object sender, EventArgs e)
        {
            File.Delete(workspacePath + "//PAD0.000");
            File.Delete(workspacePath + "//PAD1.000");
        }

        private void GameType_Click(object sender, EventArgs e)
        {
            if (GameType.SelectedIndex == 0)
            {
                SettingsIsoPath.Text = settings.SSXISOPath;
                workspacePath = Application.StartupPath + "\\disk\\SSX\\";
                if (!Directory.Exists(workspacePath))
                {
                    Directory.CreateDirectory(workspacePath);
                }
            }
            if (GameType.SelectedIndex == 1)
            {
                SettingsIsoPath.Text = settings.SSX2ISOPath;
                workspacePath = Application.StartupPath + "\\disk\\SSX Tricky\\";
                if (!Directory.Exists(workspacePath))
                {
                    Directory.CreateDirectory(workspacePath);
                }
            }
            if (GameType.SelectedIndex == 2)
            {
                SettingsIsoPath.Text = settings.SSX3ISOPath;
                workspacePath = Application.StartupPath + "\\disk\\SSX 3\\";
                if (!Directory.Exists(workspacePath))
                {
                    Directory.CreateDirectory(workspacePath);
                }
            }
            if (GameType.SelectedIndex == 3)
            {
                SettingsIsoPath.Text = settings.SSX4ISOPath;
                workspacePath = Application.StartupPath + "\\disk\\SSX On Tour\\";
                if (!Directory.Exists(workspacePath))
                {
                    Directory.CreateDirectory(workspacePath);
                }
            }
            settings.Game = GameType.SelectedIndex;
            settings.Save();
        }

        private void SettingsIsoSet_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Boot File (*.iso, *.elf)|*.iso;*.elf|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string path = openFileDialog.FileName;
                SettingsIsoPath.Text = path;

                if (settings.Game == 0)
                {
                    settings.SSXISOPath = openFileDialog.FileName;
                }
                else if (settings.Game == 1)
                {
                    settings.SSX2ISOPath = openFileDialog.FileName;
                }
                else if (settings.Game == 2)
                {
                    settings.SSX3ISOPath = openFileDialog.FileName;
                }
                else if (settings.Game == 3)
                {
                    settings.SSX4ISOPath = openFileDialog.FileName;
                }

                settings.Save();
            }
        }
        #region Image Tools
        private void ToolBrighten_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "png File (*.png)|*.png|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageUtil.Brighten(openFileDialog.FileName);
            }
        }

        private void ToolDarken_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "png File (*.png)|*.png|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                ImageUtil.Darken(openFileDialog.FileName);
            }
        }

        private void ToolBrightenFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImageUtil.BrightenFolder(openFileDialog.FileName);
            }
        }

        private void ToolDarkenFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ImageUtil.DarkenFolder(openFileDialog.FileName);
            }
        }
        #endregion

        #region Backup Tools
        private void ToolsBackupSSX_Click(object sender, EventArgs e)
        {
            string Path = Application.StartupPath + "\\disk\\SSX\\";
            string Path2 = Application.StartupPath + "\\backup\\SSX\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX backed up");
        }

        private void ToolsBackupSSXTricky_Click(object sender, EventArgs e)
        {
            string Path = Application.StartupPath + "\\disk\\SSX Tricky\\";
            string Path2 = Application.StartupPath + "\\backup\\SSX Tricky\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX Tricky backed up");
        }

        private void ToolsBackupSSX3_Click(object sender, EventArgs e)
        {
            string Path = Application.StartupPath + "\\disk\\SSX 3\\";
            string Path2 = Application.StartupPath + "\\backup\\SSX 3\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX 3 backed up");
        }

        private void ToolsRestoreSSX_Click(object sender, EventArgs e)
        {
            string Path2 = Application.StartupPath + "\\disk\\SSX\\";
            string Path = Application.StartupPath + "\\backup\\SSX\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX Workspace Restored");
        }

        private void ToolsRestoreSSXTricky_Click(object sender, EventArgs e)
        {
            string Path2 = Application.StartupPath + "\\disk\\SSX Tricky\\";
            string Path = Application.StartupPath + "\\backup\\SSX Tricky\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX Tricky Workspace Restored");
        }

        private void ToolsRestoreSSX3_Click(object sender, EventArgs e)
        {
            string Path2 = Application.StartupPath + "\\disk\\SSX 3\\";
            string Path = Application.StartupPath + "\\backup\\SSX 3\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX 3 Workspace Restored");
        }

        private void ToolsBackupSSXOnTour_Click(object sender, EventArgs e)
        {
            string Path = Application.StartupPath + "\\disk\\SSX On Tour\\";
            string Path2 = Application.StartupPath + "\\backup\\SSX On Tour\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX On Tour Backed up");
        }

        private void ToolsRestoreSSXOnTour_Click(object sender, EventArgs e)
        {
            string Path2 = Application.StartupPath + "\\disk\\SSX On Tour\\";
            string Path = Application.StartupPath + "\\backup\\SSX On Tour\\";
            if (!Directory.Exists(Path2))
            {
                Directory.CreateDirectory(Path2);
            }
            else
            {
                Directory.Delete(Path2, true);
                Directory.CreateDirectory(Path2);
            }
            CopyDirectory(Path, Path2, true);
            MessageBox.Show("SSX On Tour Workspace Restored");
        }
        #endregion

        static void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
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
        #endregion

        #region Settings
        private void Settings7ZipButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "7z (7z.exe)|7z.exe|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                settings.ZipPath = openFileDialog.FileName;
                Settings7ZipPath.Text = settings.ZipPath;
                settings.Save();
            }
        }

        private void SettingsImgBurnButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "ImgBurn (ImgBurn.exe)|ImgBurn.exe|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                settings.ImgBurnPath = openFileDialog.FileName;
                SettingsImgBurn.Text = settings.ImgBurnPath;
                settings.Save();
            }
        }

        private void SettingsPCSX2Button_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Exe Application (*.exe)|*.exe|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                settings.Pcsx2Path = openFileDialog.FileName;
                SettingsPCSX2Path.Text = settings.Pcsx2Path;
                settings.Save();
            }
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://github.com/GlitcherOG/SSX-PS2-Collection-Modder");
        }

        private void SettingsSave_Click(object sender, EventArgs e)
        {
            settings.ExtractorArg = SettingsExtractorArg.Text;
            settings.IsoArg = SettingsIsoBuilderArg.Text;
            settings.Save();
        }

        #endregion

        #region Loc File
        LOCHandler locHandler = new LOCHandler();
        private void LocBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LocBox1.SelectedIndex != -1)
            {
                toolStripStatusLabel1.Text = "";
                LocrichTextBox1.Text = locHandler.textList[LocBox1.SelectedIndex];
                LocrichTextBox2.Text = locHandler.byteListString[LocBox1.SelectedIndex];
            }
        }
        private void LocLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "English Loc File (*AMER.loc)|*AMER.loc|Loc File (*.loc)|*.loc|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false,
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SetStatus("Loading LOC File...");
                ClearLod();
                locHandler.ReadLodFile(openFileDialog.FileName);
                for (int i = 0; i < locHandler.textList.Count; i++)
                {
                    LocBox1.Items.Add(locHandler.textList[i]);
                }

            }
        }
        void ClearLod()
        {
            locHandler = new LOCHandler();
            LocBox1.Items.Clear();
            LocrichTextBox1.Text = "";
            LocrichTextBox2.Text = "";
        }

        private void LocrichTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (LocBox1.SelectedIndex != -1)
            {
                int pos = LocrichTextBox1.SelectionStart;
                toolStripStatusLabel1.Text = "";
                locHandler.textList[LocBox1.SelectedIndex] = LocrichTextBox1.Text;
                LocBox1.Items[LocBox1.SelectedIndex] = LocrichTextBox1.Text;
                locHandler.UpdatedByteList(LocBox1.SelectedIndex);
                LocrichTextBox2.Text = locHandler.byteListString[LocBox1.SelectedIndex];
                LocrichTextBox1.SelectionStart = pos;
            }
        }

        private void LocSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath + "/DATA/LOCALE",
                Filter = "Loc File (*.loc)|*.loc|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            //openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SetStatus("Saving LOC File...");
                locHandler.SaveLodFile(openFileDialog.FileName);
                SetStatus("File Saved");
            }
        }

        private void LOCSave_Click(object sender, EventArgs e)
        {
            locHandler.SaveLodFile();
            SetStatus("File Saved");
        }

        private void LocSearchNext_Click(object sender, EventArgs e)
        {
            int a = LocBox1.SelectedIndex;
            if (LocBox1.SelectedIndex == -1)
            {
                a = 0;
            }
            else if (a != LocBox1.Items.Count - 1)
            {
                a++;
            }
            for (int i = a; i < locHandler.textList.Count; i++)
            {
                if (locHandler.textList[i].ToLower().Contains(LocSearchText.Text.ToLower()))
                {
                    LocBox1.SelectedIndex = i;
                    break;
                }
            }
        }

        private void LocSearchBack_Click(object sender, EventArgs e)
        {
            int a = LocBox1.SelectedIndex;
            if (a == -1)
            {
                a = LocBox1.Items.Count - 1;
            }
            else if (a != 0)
            {
                a--;
            }
            for (int i = a; i > -1; i--)
            {
                if (locHandler.textList[i].ToLower().Contains(LocSearchText.Text.ToLower()))
                {
                    LocBox1.SelectedIndex = i;
                    break;
                }
            }
        }
        #endregion

        #region CharDB File
        CHARDBLHandler charHandler = new CHARDBLHandler();
        private void charLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath + "DATA\\BE",
                Filter = "Character DBL|CHARDB.DBL|DataBase List (*.DBL)|*.DBL|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            //openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                charBox1.Items.Clear();
                charHandler.LoadCharFile(openFileDialog.FileName);
                for (int i = 0; i < charHandler.charDBs.Count; i++)
                {
                    charBox1.Items.Add(charHandler.charDBs[i].LongName);
                }
            }
        }

        private void CharApply(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            CharDB temp = new CharDB
            {
                LongName = chartextBox1.Text,
                FirstName = chartextBox2.Text,
                NickName = chartextBox3.Text,
                BloodType = chartextBox4.Text,
                Height = chartextBox5.Text,
                Nationality = chartextBox6.Text,
                Unkown1 = (int)charnumericUpDown1.Value,
                Stance = (int)charnumericUpDown2.Value,
                ModelSize = (int)charnumericUpDown3.Value,
                Gender = (int)charnumericUpDown4.Value,
                Age = (int)charnumericUpDown5.Value,
                Position = (int)charnumericUpDown6.Value
            };
            charHandler.charDBs[charBox1.SelectedIndex] = temp;
            int temp1 = charBox1.SelectedIndex;
            charBox1.Items[temp1] = temp.LongName;
        }

        private void charBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (charBox1.SelectedIndex != -1)
            {
                CharDB temp = charHandler.charDBs[charBox1.SelectedIndex];
                chartextBox1.Text = temp.LongName;
                chartextBox2.Text = temp.FirstName;
                chartextBox3.Text = temp.NickName;
                chartextBox4.Text = temp.BloodType;
                chartextBox5.Text = temp.Height;
                chartextBox6.Text = temp.Nationality;
                charnumericUpDown1.Value = temp.Unkown1;
                charnumericUpDown2.Value = temp.Stance;
                charnumericUpDown3.Value = temp.ModelSize;
                charnumericUpDown4.Value = temp.Gender;
                charnumericUpDown5.Value = temp.Age;
                charnumericUpDown6.Value = temp.Position;
            }
        }

        private void charSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath + "DATA\\BE",
                Filter = "Data Base List (*.DBL)|*.DBL|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                charHandler.SaveCharFile(openFileDialog.FileName);
            }
        }

        private void charSave_Click(object sender, EventArgs e)
        {
            charHandler.SaveCharFile();
        }
        #endregion

        #region MusicINF File
        MusicINFHandler musicINFHandler = new MusicINFHandler();

        private void MusicLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath + "DATA\\CONFIG",
                Filter = "Music Config |MUSIC.INF|Config File (*.INF)|*.INF|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                musiclistBox1.Items.Clear();
                musicINFHandler.musPath = openFileDialog.FileName;
                musicINFHandler.LoadMusFile(openFileDialog.FileName);
                for (int i = 0; i < musicINFHandler.musFileSongs.Count; i++)
                {
                    musiclistBox1.Items.Add(musicINFHandler.musFileSongs[i].ID);
                }
            }
        }
        bool musHold;
        private void musiclistBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            musHold = true;
            if (musiclistBox1.SelectedIndex != -1)
            {
                MusicID.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].ID;
                MusicTitleBox.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Title;
                MusicArtistBox.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Artist;
                MusicAlbumBox.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Album;
                MusicPathDataBox.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].PathData;
                MusicDataBox.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].MusicData;
                MusicLoopData.Text = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].loopData;
                MusINFnumericUpDown1.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].BeatsPerMeasure;
                MusINFnumericUpDown2.Value = (decimal)musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].BPM;
                MusINFnumericUpDown3.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].SEDValue;
                MusINFnumericUpDown4.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Lowpass;
                MusINFnumericUpDown5.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Preview;
                MusINFnumericUpDown6.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].MeasuresPerBar;
                MusINFnumericUpDown7.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].PhrasesPerBank;
                MusINFnumericUpDown8.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].BeatsPerMeasure;
                MusINFnumericUpDown9.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].BeatsPerPhrase;
                MusINFnumericUpDown10.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].AsyncLevel;
                MusINFnumericUpDown11.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].SongBig;
                MusINFnumericUpDown12.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].DuckToLoops;
                MusnumericUpDown0.Value = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].AADTOFE;
                MuscheckBox1.Checked = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Category0;
                MuscheckBox2.Checked = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Category1;
                MuscheckBox3.Checked = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Category2;
                MuscheckBox4.Checked = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Category3;
                MuscheckBox5.Checked = musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex].Category4;
            }
            musHold = false;
        }

        private void MusUpdate(object sender, EventArgs e)
        {
            if (musiclistBox1.SelectedIndex != -1 && !musHold)
            {
                MusFileSong temp = new MusFileSong();
                temp.ID = MusicID.Text;
                temp.Title = MusicTitleBox.Text;
                temp.Artist = MusicArtistBox.Text;
                temp.Album = MusicAlbumBox.Text;
                temp.PathData = MusicPathDataBox.Text;
                temp.MusicData = MusicDataBox.Text;
                temp.loopData = MusicLoopData.Text;
                temp.BeatsPerMeasure = (int)MusINFnumericUpDown1.Value;
                temp.BPM = (int)MusINFnumericUpDown2.Value;
                temp.SEDValue = (int)MusINFnumericUpDown3.Value;
                temp.Lowpass = (int)MusINFnumericUpDown4.Value;
                temp.Preview = (int)MusINFnumericUpDown5.Value;
                temp.MeasuresPerBar = (int)MusINFnumericUpDown6.Value;
                temp.PhrasesPerBank = (int)MusINFnumericUpDown7.Value;
                temp.BeatsPerMeasure = (int)MusINFnumericUpDown8.Value;
                temp.BeatsPerPhrase = (int)MusINFnumericUpDown9.Value;
                temp.AsyncLevel = (int)MusINFnumericUpDown10.Value;
                temp.SongBig = (int)MusINFnumericUpDown11.Value;
                temp.DuckToLoops = (int)MusINFnumericUpDown12.Value;
                temp.AADTOFE = (int)MusnumericUpDown0.Value;
                temp.Category0 = MuscheckBox1.Checked;
                temp.Category1 = MuscheckBox2.Checked;
                temp.Category2 = MuscheckBox3.Checked;
                temp.Category3 = MuscheckBox4.Checked;
                temp.Category4 = MuscheckBox5.Checked;

                musicINFHandler.musFileSongs[musiclistBox1.SelectedIndex] = temp;

            }
        }

        private void MusSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath + "DATA\\CONFIG",
                Filter = "Config File (*.INF)|*.INF|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                musicINFHandler.SaveMusFile(openFileDialog.FileName);
            }
        }

        private void MusAdd_Click(object sender, EventArgs e)
        {
            MusFileSong song = new MusFileSong();
            song.ID = "[Null]";
            musicINFHandler.musFileSongs.Add(song);
            musiclistBox1.Items.Clear();
            for (int i = 0; i < musicINFHandler.musFileSongs.Count; i++)
            {
                musiclistBox1.Items.Add(musicINFHandler.musFileSongs[i].ID);
            }
        }

        private void MusRemove_Click(object sender, EventArgs e)
        {
            if (musiclistBox1.SelectedIndex != -1)
            {
                musicINFHandler.musFileSongs.RemoveAt(musiclistBox1.SelectedIndex);
                musiclistBox1.Items.Clear();
                for (int i = 0; i < musicINFHandler.musFileSongs.Count; i++)
                {
                    musiclistBox1.Items.Add(musicINFHandler.musFileSongs[i].ID);
                }
            }
        }
        #endregion

        #region BIG
        BigHandler bigfHandler = new BigHandler();
        private void BigLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "BIG File (*.BIG)|*.BIG|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                BigBox1.Items.Clear();
                bigfHandler.LoadBig(openFileDialog.FileName);
                BigExtract.Enabled = true;
                BuildBigButton.Enabled = false;
                BigFType.Text = bigfHandler.bigType.ToString();
                if (bigfHandler.bigHeader.compression)
                {
                    BigCompressed.Text = "Yes";
                }
                else
                {
                    BigCompressed.Text = "No";
                }
                if (bigfHandler.bigFiles.Count == 0)
                {
                    MessageBox.Show("Error loading file");
                }
                for (int i = 0; i < bigfHandler.bigFiles.Count; i++)
                {
                    BigBox1.Items.Add(bigfHandler.bigFiles[i].path);
                }
            }
        }

        private void BigBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (BigBox1.SelectedIndex != -1)
            {
                int i = BigBox1.SelectedIndex;
                BigPathLabel.Text = bigfHandler.bigFiles[i].path;
                byte[] temp = BitConverter.GetBytes(bigfHandler.bigFiles[i].offset);
                if (bigfHandler.bigFiles[i].Compressed)
                {
                    BigCompressed.Text = "Yes";
                }
                else
                {
                    BigCompressed.Text = "No";
                }
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(temp);
                BigOffsetLabel.Text = "0x" + BitConverter.ToString(temp).Replace("-", "");
                BigSizeLabel.Text = bigfHandler.bigFiles[i].size.ToString();
                bigUncompressed.Text = bigfHandler.bigFiles[i].UncompressedSize.ToString();
            }
        }

        private void BigExtract_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                bigfHandler.ExtractBig(openFileDialog.FileName);
                GC.Collect();
                Process.Start(openFileDialog.FileName);
            }
        }

        private void BigLoadFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                BigBox1.Items.Clear();
                BigExtract.Enabled = false;
                BuildBigButton.Enabled = true;
                BigCompressed.Text = "Null";
                BigFType.SelectedIndex = ((int)bigfHandler.bigType);
                bigfHandler.LoadFolder(openFileDialog.FileName);
                for (int i = 0; i < bigfHandler.bigFiles.Count; i++)
                {
                    BigBox1.Items.Add(bigfHandler.bigFiles[i].path);
                }
            }
        }

        private void BuildBigButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "BIG File (*.BIG)|*.BIG|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Enum.TryParse(BigFType.Text, out bigfHandler.bigType);
                bigfHandler.BuildBig(openFileDialog.FileName);
                MessageBox.Show("Building Done");
                GC.Collect();
            }
        }
        #endregion

        #region SSH
        SSHHandler sshHandler = new SSHHandler();
        private void SSHLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "SSH File (*.SSH)|*.SSH|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SSHlistBox1.Items.Clear();
                sshHandler.LoadSSH(openFileDialog.FileName);
                for (int i = 0; i < sshHandler.sshImages.Count; i++)
                {
                    SSHlistBox1.Items.Add(sshHandler.sshImages[i].shortname + "." + sshHandler.sshImages[i].longname);
                    SSHFileFormat.Text = sshHandler.format;
                    SSHFileNameLabel.Text = openFileDialog.SafeFileName;
                }
            }
        }

        private void SSHlistBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1 && !sshHold)
            {
                sshHold = true;
                SSHColourAmmount.Text = sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshTable.Total.ToString();
                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshTable.Total > 256)
                {
                    SSHColourAmmount.BackColor = Color.Red;
                }
                else
                {
                    SSHColourAmmount.BackColor = Color.White;
                }
                SSHpictureBox1.Image = sshHandler.sshImages[SSHlistBox1.SelectedIndex].bitmap;
                SSHImageName.Text = sshHandler.sshImages[SSHlistBox1.SelectedIndex].longname;
                SSHImageShortName.Text = sshHandler.sshImages[SSHlistBox1.SelectedIndex].shortname;
                SSHXAxis.Value = sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshHeader.Xaxis;
                SSHYAxis.Value = sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshHeader.Yaxis;
                SSHAlphaMetal.Checked = sshHandler.sshImages[SSHlistBox1.SelectedIndex].MetalBin;
                bool tempBool = sshHandler.sshImages[SSHlistBox1.SelectedIndex].MetalBin;
                SSHImageSize.Text = sshHandler.sshImages[SSHlistBox1.SelectedIndex].bitmap.Width + " Width x " + sshHandler.sshImages[SSHlistBox1.SelectedIndex].bitmap.Height + " Height";
                sshHandler.SSHColorCalculate(SSHlistBox1.SelectedIndex);
                SSHMetalExtract.Enabled = tempBool;
                SSHMetalLoad.Enabled = tempBool;
                SSHBothExtract.Enabled = tempBool;
                SSHBothImport.Enabled = tempBool;
                SSHAlphaFix.Checked = sshHandler.sshImages[SSHlistBox1.SelectedIndex].AlphaFix;

                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshHeader.LXPos == 0)
                {
                    SSHImageByteSwapped.Checked = false;
                }
                else
                {
                    SSHImageByteSwapped.Checked = true;
                }

                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshTable.Format == 0)
                {
                    SSHColourByteSwapped.Checked = false;
                }
                else
                {
                    SSHColourByteSwapped.Checked = true;
                }

                for (int i = 0; i < SSHMatrixType.Items.Count; i++)
                {
                    if (SSHMatrixType.Items[i].ToString().Contains(sshHandler.sshImages[SSHlistBox1.SelectedIndex].sshHeader.MatrixFormat.ToString() + " "))
                    {
                        SSHMatrixType.SelectedIndex = i;
                        break;
                    }
                    else
                    {
                        SSHMatrixType.SelectedIndex = SSHMatrixType.Items.Count - 1;
                    }
                }
                sshHold = false;
            }
        }
        bool sshHold;
        private void SSH_TextChanged(object sender, EventArgs e)
        {
            sshHandler.format = SSHFileFormat.Text;
            if (SSHlistBox1.SelectedIndex != -1 && !sshHold)
            {
                sshHold = true;

                var temp = sshHandler.sshImages[SSHlistBox1.SelectedIndex];
                var tempHeader = temp.sshHeader;
                temp.longname = SSHImageName.Text;
                temp.shortname = SSHImageShortName.Text;
                temp.MetalBin = SSHAlphaMetal.Checked;
                SSHFileFormat.Text = sshHandler.format;
                tempHeader.Xaxis = (int)SSHXAxis.Value;
                tempHeader.Yaxis = (int)SSHYAxis.Value;
                string tempString = SSHMatrixType.Text;
                string[] tempAString = tempString.Split(' ');
                tempString = tempAString[0];
                int indexInt = Int32.Parse(tempString);
                tempHeader.MatrixFormat = (byte)indexInt;
                SSHlistBox1.Items[SSHlistBox1.SelectedIndex] = temp.shortname + "." + temp.longname;
                temp.AlphaFix = SSHAlphaFix.Checked;

                if (SSHColourByteSwapped.Checked)
                {
                    temp.sshTable.Format = 2;
                }
                else
                {
                    temp.sshTable.Format = 0;
                }

                if (SSHImageByteSwapped.Checked)
                {
                    tempHeader.LXPos = 2;
                }
                else
                {
                    tempHeader.LXPos = 0;
                }

                bool tempBool = temp.MetalBin;
                SSHMetalExtract.Enabled = tempBool;
                SSHMetalLoad.Enabled = tempBool;
                SSHBothExtract.Enabled = tempBool;
                SSHBothImport.Enabled = tempBool;
                sshHandler.SSHColorCalculate(SSHlistBox1.SelectedIndex);

                temp.sshHeader = tempHeader;
                sshHandler.sshImages[SSHlistBox1.SelectedIndex] = temp;
                sshHold = false;
            }
        }

        private void SSHbmpExport_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SetStatus("Extracting Please Wait....");
                sshHandler.BMPExtract(openFileDialog.FileName);
                SetStatus("");
                GC.Collect();
                Process.Start(openFileDialog.FileName);
            }
        }

        private void SSHDisplayMode_CheckedChanged(object sender, EventArgs e)
        {
            if (SSHDisplayMode.Checked)
            {
                SSHpictureBox1.BackColor = Color.Black;
            }
            else
            {
                SSHpictureBox1.BackColor = Color.White;
            }
        }

        private void SSHLoadFolder_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SSHlistBox1.Items.Clear();
                sshHandler.LoadFolder(openFileDialog.FileName);
                for (int i = 0; i < sshHandler.sshImages.Count; i++)
                {
                    SSHlistBox1.Items.Add(sshHandler.sshImages[i].shortname + "." + sshHandler.sshImages[i].longname);
                    SSHFileFormat.Text = sshHandler.format;
                }
            }
        }

        private void SSHSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath + "DATA",
                Filter = "SSH File (*.ssh)|*.ssh|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                sshHandler.SaveSSH(openFileDialog.FileName);
            }
        }

        private void SSHReplace_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = workspacePath + "DATA",
                    Filter = "Png File (*.png)|*.png|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    sshHandler.LoadSingle(openFileDialog.FileName, SSHlistBox1.SelectedIndex);
                    SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                    SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
                }
            }
        }

        private void SSHExportOne_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                SaveFileDialog openFileDialog = new SaveFileDialog
                {
                    InitialDirectory = workspacePath,
                    Filter = "Png File (*.png)|*.png|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                openFileDialog.FileName = sshHandler.sshImages[SSHlistBox1.SelectedIndex].shortname + "." + sshHandler.sshImages[SSHlistBox1.SelectedIndex].longname + ".png";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    sshHandler.BMPOneExtract(openFileDialog.FileName, SSHlistBox1.SelectedIndex);
                    GC.Collect();
                }
            }
        }

        private void SSHMetalExtract_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].MetalBin)
                {
                    SaveFileDialog openFileDialog = new SaveFileDialog
                    {
                        InitialDirectory = workspacePath,
                        Filter = "Png File (*.png)|*.png|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = false
                    };
                    openFileDialog.FileName = sshHandler.sshImages[SSHlistBox1.SelectedIndex].shortname + "." + sshHandler.sshImages[SSHlistBox1.SelectedIndex].longname + ".Metal.png";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sshHandler.BMPOneExtractMetal(openFileDialog.FileName, SSHlistBox1.SelectedIndex);
                        GC.Collect();
                    }
                }
            }
        }

        private void SSHMetalLoad_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].MetalBin)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = workspacePath,
                        Filter = "Png File (*.png)|*.png|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = false
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sshHandler.LoadSingleMetal(openFileDialog.FileName, SSHlistBox1.SelectedIndex);
                        GC.Collect();
                    }
                }
            }
        }

        private void SSHBothExtract_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].MetalBin)
                {
                    SaveFileDialog openFileDialog = new SaveFileDialog
                    {
                        InitialDirectory = workspacePath,
                        Filter = "Png File (*.png)|*.png|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = false
                    };
                    openFileDialog.FileName = sshHandler.sshImages[SSHlistBox1.SelectedIndex].shortname + "." + sshHandler.sshImages[SSHlistBox1.SelectedIndex].longname + ".Both.png";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sshHandler.BMPOneBothExtract(openFileDialog.FileName, SSHlistBox1.SelectedIndex);
                        GC.Collect();
                    }
                }
            }
        }

        private void SSHBothImport_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                if (sshHandler.sshImages[SSHlistBox1.SelectedIndex].MetalBin)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        InitialDirectory = workspacePath,
                        Filter = "Png File (*.png)|*.png|All files (*.*)|*.*",
                        FilterIndex = 1,
                        RestoreDirectory = false
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        sshHandler.LoadSingleBoth(openFileDialog.FileName, SSHlistBox1.SelectedIndex);
                        SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                        SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
                        GC.Collect();
                    }
                }
            }
        }

        private void SSHRefreshColor_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                sshHandler.SSHColorCalculate(SSHlistBox1.SelectedIndex);
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
            }
        }

        private void sshDColour_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                sshHandler.BrightenBitmap(SSHlistBox1.SelectedIndex);
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
            }
        }

        private void sshHColour_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                sshHandler.DarkenImage(SSHlistBox1.SelectedIndex);
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
            }
        }

        private void sshDAlpha_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                sshHandler.DoubleAlphaImage(SSHlistBox1.SelectedIndex);
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
            }
        }

        private void sshHAlpha_Click(object sender, EventArgs e)
        {
            if (SSHlistBox1.SelectedIndex != -1)
            {
                sshHandler.HalfAlphaImage(SSHlistBox1.SelectedIndex);
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex - 1;
                SSHlistBox1.SelectedIndex = SSHlistBox1.SelectedIndex + 1;
            }
        }
        #endregion

        #region ModMaker Info
        ModMakerHandler modMaker = new ModMakerHandler();
        private void ModMakerLoad_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                modMaker.ModFolder = openFileDialog.FileName;
                modMaker.ModInfo = ModInfo.Load(openFileDialog.FileName);

                if (File.Exists(openFileDialog.FileName + "//Icon.png"))
                {
                    modMaker.Icon = Image.FromFile(openFileDialog.FileName + "//Icon.png");
                    ModMakerIcon.Image = modMaker.Icon;
                }
                else
                {
                    ModMakerIcon.Image = null;
                }

                ModMakerName.Text = modMaker.ModInfo.Name;
                ModMakerVersion.Text = modMaker.ModInfo.Version;
                ModMakerAuthor.Text = modMaker.ModInfo.Author;
                ModMakerDescription.Text = modMaker.ModInfo.Description;
                ModMakerGame.Text = modMaker.ModInfo.Game;

                ModMakerPal.Checked = modMaker.ModInfo.PAL;
                ModMakerNTSC.Checked = modMaker.ModInfo.NTSC;
                ModMakerNTSCJ.Checked = modMaker.ModInfo.NTSCJ;
                ModMakerNTSCK.Checked = modMaker.ModInfo.NTSCK;

                ModMakerPalDemo.Checked = modMaker.ModInfo.PALDemo;
                ModMakerNTSCDemo.Checked = modMaker.ModInfo.NTSCDemo;
                ModMakerNTSCJDemo.Checked = modMaker.ModInfo.NTSCJDemo;
                ModMakerNTSCKDemo.Checked = modMaker.ModInfo.NTSCKDemo;

                modMakerSSXElfLdr.Checked = modMaker.ModInfo.SSXElfLdr;

                ModMakerList.Items.Clear();
                if (File.Exists(openFileDialog.FileName + "//ModInstructions.txt"))
                {
                    var tempString = File.ReadAllLines(openFileDialog.FileName + "//ModInstructions.txt");

                    for (int i = 0; i < tempString.Length; i++)
                    {
                        ModMakerList.Items.Add(tempString[i]);
                    }
                }
            }
        }

        private void ModMakerSave_Click(object sender, EventArgs e)
        {
            if (modMaker.ModFolder != "")
            {
                modMaker.ModInfo.Name = ModMakerName.Text;
                modMaker.ModInfo.Version = ModMakerVersion.Text;
                modMaker.ModInfo.Author = ModMakerAuthor.Text;
                modMaker.ModInfo.Description = ModMakerDescription.Text;
                modMaker.ModInfo.Game = ModMakerGame.Text;
                modMaker.ModInfo.ModPackVersion = 1;

                modMaker.ModInfo.PAL = ModMakerPal.Checked;
                modMaker.ModInfo.NTSC = ModMakerNTSC.Checked;
                modMaker.ModInfo.NTSCJ = ModMakerNTSCJ.Checked;
                modMaker.ModInfo.NTSCK = ModMakerNTSCK.Checked;
                modMaker.ModInfo.PALDemo = ModMakerPalDemo.Checked;
                modMaker.ModInfo.NTSCDemo = ModMakerNTSCDemo.Checked;
                modMaker.ModInfo.NTSCJDemo = ModMakerNTSCJDemo.Checked;
                modMaker.ModInfo.NTSCKDemo = ModMakerNTSCKDemo.Checked;

                modMaker.ModInfo.SSXElfLdr = modMakerSSXElfLdr.Checked;

                modMaker.ModInfo.Save(modMaker.ModFolder);
            }
        }

        private void ModMakerPack_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "SSX Mod File (*.zip)|*.zip|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                modMaker.PackMod(openFileDialog.FileName);
            }
        }
        #endregion

        #region Instructions
        ModMakingInstructions makingInstructions = new ModMakingInstructions();

        public void UpdateInstructionList()
        {
            ModInstructionListbox.Items.Clear();
            for (int i = 0; i < makingInstructions.Instructions.Count; i++)
            {
                ModInstructionListbox.Items.Add(makingInstructions.Instructions[i].Type + ", " + makingInstructions.Instructions[i].Source + ", " + makingInstructions.Instructions[i].Ouput);
            }
        }
        private void ModInstructionAdd_Click(object sender, EventArgs e)
        {
            if (makingInstructions.ModPath != "")
            {
                makingInstructions.Instructions.Add(new Instruction() { Type = "Copy", Source = "Mod\\", Ouput = "Game\\" });
                UpdateInstructionList();
            }
        }

        private void ModInstructionRemove_Click(object sender, EventArgs e)
        {
            if (ModInstructionListbox.SelectedIndex != -1)
            {
                makingInstructions.Instructions.RemoveAt(ModInstructionListbox.SelectedIndex);
            }
            UpdateInstructionList();
        }

        private void ModInstructionMoveUp_Click(object sender, EventArgs e)
        {
            if (ModInstructionListbox.SelectedIndex > 0)
            {
                var temp = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex - 1];
                makingInstructions.Instructions[ModInstructionListbox.SelectedIndex - 1] = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex];
                makingInstructions.Instructions[ModInstructionListbox.SelectedIndex] = temp;
            }
            UpdateInstructionList();
        }

        private void ModInstructionMoveDown_Click(object sender, EventArgs e)
        {
            if (ModInstructionListbox.SelectedIndex != makingInstructions.Instructions.Count - 1)
            {
                var temp = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex + 1];
                makingInstructions.Instructions[ModInstructionListbox.SelectedIndex + 1] = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex];
                makingInstructions.Instructions[ModInstructionListbox.SelectedIndex] = temp;
            }
            UpdateInstructionList();
        }

        private void ModInstructionLoad_Click(object sender, EventArgs e)
        {
            CommonOpenFileDialog openFileDialog = new CommonOpenFileDialog
            {
                InitialDirectory = workspacePath,
                IsFolderPicker = true,
            };
            if (openFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                makingInstructions.Load(openFileDialog.FileName);
                ModInstructionGame.Items.Clear();
                var tempGamePaths = Directory.GetFileSystemEntries(workspacePath, "*", SearchOption.AllDirectories);
                for (int i = 0; i < tempGamePaths.Length; i++)
                {
                    ModInstructionGame.Items.Add("Game\\" + tempGamePaths[i].Substring(workspacePath.Length));
                }

                ModInstructionMod.Items.Clear();
                var tempModPaths = Directory.GetFileSystemEntries(openFileDialog.FileName, "*", SearchOption.AllDirectories);
                for (int i = 0; i < tempModPaths.Length; i++)
                {
                    ModInstructionMod.Items.Add("Mod" + tempModPaths[i].Substring(openFileDialog.FileName.Length));
                }
            }
            UpdateInstructionList();
        }

        private void ModInstructionSave_Click(object sender, EventArgs e)
        {
            if (makingInstructions.ModPath != "")
            {
                makingInstructions.Save();
            }
        }
        bool modHold;
        private void ModInstructionListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModInstructionListbox.SelectedIndex != -1 && !modHold)
            {
                modHold = true;
                ModInstructionType.Text = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex].Type;
                ModInstructionSource.Text = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex].Source;
                ModInstructionOutput.Text = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex].Ouput;
                modHold = false;
            }
        }

        private void ModInstructionChange(object sender, EventArgs e)
        {
            if (ModInstructionListbox.SelectedIndex != -1 && !modHold)
            {
                int temp1 = ModInstructionListbox.SelectedIndex;
                modHold = true;
                var temp = makingInstructions.Instructions[ModInstructionListbox.SelectedIndex];
                temp.Source = ModInstructionSource.Text;
                temp.Ouput = ModInstructionOutput.Text;
                temp.Type = ModInstructionType.Text;
                makingInstructions.Instructions[ModInstructionListbox.SelectedIndex] = temp;
                UpdateInstructionList();
                ModInstructionListbox.SelectedIndex = temp1;
                modHold = false;
            }
        }

        private void ModInstructionGame_MouseDown(object sender, MouseEventArgs e)
        {
            ModInstructionGame.SelectedIndex = ModInstructionGame.IndexFromPoint(e.X, e.Y);
            if (ModInstructionGame.SelectedIndex != -1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ModInstructionSource.Text = ModInstructionGame.Items[ModInstructionGame.SelectedIndex].ToString();
                }
                if (e.Button == MouseButtons.Right)
                {
                    ModInstructionOutput.Text = ModInstructionGame.Items[ModInstructionGame.SelectedIndex].ToString();
                }
            }
        }

        private void ModInstructionMod_MouseDown(object sender, MouseEventArgs e)
        {
            ModInstructionMod.SelectedIndex = ModInstructionMod.IndexFromPoint(e.X, e.Y);
            if (ModInstructionMod.SelectedIndex != -1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ModInstructionSource.Text = ModInstructionMod.Items[ModInstructionMod.SelectedIndex].ToString();
                }
                if (e.Button == MouseButtons.Right)
                {
                    ModInstructionOutput.Text = ModInstructionMod.Items[ModInstructionMod.SelectedIndex].ToString();
                }
            }
        }

        #endregion

        #region Mod
        ModApplication modApplication = new ModApplication();
        private void ModLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "SSX Mod File (*.zip)|*.zip|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                modApplication.LoadMod(openFileDialog.FileName);
                ModAuthor.Text = modApplication.modInfo.Author;
                ModLabel.Text = modApplication.modInfo.Name;
                ModVersion.Text = modApplication.modInfo.Version;
                ModDescription.Text = modApplication.modInfo.Description;
                ModGame.Text = modApplication.modInfo.Game;
                ModPicture.Image = modApplication.image;

                ModPal.Checked = modApplication.modInfo.PAL;
                ModNTSC.Checked = modApplication.modInfo.NTSC;
                ModNTSCJ.Checked = modApplication.modInfo.NTSCJ;
                ModNTSCK.Checked = modApplication.modInfo.NTSCK;

                ModPalDemo.Checked = modApplication.modInfo.PALDemo;
                ModNTSCDemo.Checked = modApplication.modInfo.NTSCDemo;
                ModNTSCJDemo.Checked = modApplication.modInfo.NTSCJDemo;
                ModNTSCKDemo.Checked = modApplication.modInfo.NTSCKDemo;

                modSSXElfLdr.Checked = modApplication.modInfo.SSXElfLdr;

                ModList.Items.Clear();
                for (int i = 0; i < modApplication.modInstructions.Instructions.Count; i++)
                {
                    ModList.Items.Add(modApplication.modInstructions.Instructions[i].Type + ", " + modApplication.modInstructions.Instructions[i].Source + ", " + modApplication.modInstructions.Instructions[i].Ouput);
                }
            }
        }

        private void ModApply_Click(object sender, EventArgs e)
        {
            bool Valid = false;
            if (modApplication.modInfo.Game == "SSX (2000)" && settings.Game == 0)
            {
                Valid = true;
            }
            if (modApplication.modInfo.Game == "SSX Tricky" && settings.Game == 1)
            {
                Valid = true;
            }
            if (modApplication.modInfo.Game == "SSX 3" && settings.Game == 2)
            {
                Valid = true;
            }
            if (modApplication.modInfo.Game == "SSX On Tour" && settings.Game == 3)
            {
                Valid = true;
            }

            if (Valid)
            {
                modApplication.ApplyMod();
            }
            else
            {
                MessageBox.Show("Wrong Game Selected");
            }
        }
        #endregion

        #region BoltPS2 Items
        BoltPS2Handler boltPS2 = new BoltPS2Handler();
        bool loaded = false;
        private void BoltLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "BOLTPS2 File (*.dat)|*.dat|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                boltPS2 = new BoltPS2Handler();
                boltPS2.load(openFileDialog.FileName);
                loaded = true;
                BoltCharacter.SelectedIndex = 0;
                GenerateTreeview();
            }
        }

        List<bool> Parented = new List<bool>();
        int pos = 0;
        void GenerateTreeview()
        {
            BoltPS2TreeView.Nodes.Clear();

            var temp = boltPS2.characters[BoltCharacter.SelectedIndex];
            Parented = new List<bool>();

            for (int i = 0; i < temp.entries.Count; i++)
            {
                Parented.Add(false);
            }
            bool test = false;
            int Testing = 0;
            while (!test)
            {
                for (int i = 0; i < temp.entries.Count; i++)
                {
                    Testing++;
                    if (!Parented[i])
                    {
                        pos = i;
                        //Check Parent Node
                        if (temp.entries[i].ParentID == -1)
                        {
                            Parented[i] = true;
                            BoltPS2TreeView.Nodes.Add(temp.entries[i].ItemID.ToString(), temp.entries[i].ItemID.ToString() + " - " + temp.entries[i].itemName);
                        }
                        else
                        {
                            for (int a = 0; a < BoltPS2TreeView.Nodes.Count; a++)
                            {
                                if (BoltPS2TreeView.Nodes[a].Name == temp.entries[i].ParentID.ToString())
                                {
                                    Parented[i] = true;
                                    BoltPS2TreeView.Nodes[a].Nodes.Add(temp.entries[i].ItemID.ToString(), temp.entries[i].ItemID.ToString() + " - " + temp.entries[i].itemName);
                                    break;
                                }
                                else
                                {
                                    var temp1 = BoltPS2TreeView.Nodes[a];
                                    CheckChildNode(temp1, temp.entries[i]);
                                }
                            }
                        }
                    }
                }


                //Check if list has been ordered
                test = true;
                for (int i = 0; i < Parented.Count; i++)
                {
                    if(!Parented[i])
                    {
                        test = false;
                        break;
                    }
                }
            }
        }

        void CheckChildNode(TreeNode Parent, ItemEntries item)
        {
            for (int i = 0; i < Parent.Nodes.Count; i++)
            {
                if (Parent.Nodes[i].Name == item.ParentID.ToString())
                {
                    Parented[pos] = true;
                    Parent.Nodes[i].Nodes.Add(item.ItemID.ToString(), item.ItemID.ToString() + " - " + item.itemName);
                    //return Parent;
                }
                else
                {
                    CheckChildNode(Parent.Nodes[i], item);
                }
            }
            //return Parent;
        }

        private void BoltSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "BIG File (*.dat)|*.dat|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                boltPS2.Save(openFileDialog.FileName);
            }
        }

        private void BoltDupe_Click(object sender, EventArgs e)
        {
            if (BoltPS2TreeView.SelectedNode != null)
            {
                int CharIndex = BoltCharacter.SelectedIndex;
                int Index = -1;

                for (int i = 0; i < boltPS2.characters[CharIndex].entries.Count; i++)
                {
                    if (BoltPS2TreeView.SelectedNode.Name == boltPS2.characters[CharIndex].entries[i].ItemID.ToString())
                    {
                        Index = i;
                        break;
                    }
                }

                var temp1 = boltPS2.characters[CharIndex];
                var temp = temp1.entries[Index];

                temp1.entries.Add(temp);

                boltPS2.characters[CharIndex] = temp1;
            }
        }

        private void BoltApply_Click(object sender, EventArgs e)
        {
            if (BoltPS2TreeView.SelectedNode != null && loaded)
            {
                int Index1 = BoltCharacter.SelectedIndex;
                int Index = 0;

                for (int i = 0; i < boltPS2.characters[Index1].entries.Count; i++)
                {
                    if (BoltPS2TreeView.SelectedNode.Name == boltPS2.characters[Index1].entries[i].ItemID.ToString())
                    {
                        Index = i;
                        break;
                    }
                }
                var Char = boltPS2.characters[Index1];
                var tempEntry = Char.entries[Index];

                tempEntry.unkownInt1 = (int)BoltUnkownOne.Value;
                tempEntry.Unlock = (int)BoltUnlock.Value;
                tempEntry.unkownInt2 = (int)BoltUnkownTwo.Value;
                tempEntry.ItemID = (int)BoltUnkownThree.Value;
                tempEntry.ParentID = (int)BoltUnkownFour.Value;
                tempEntry.category= (int)BoltCat.Value;
                tempEntry.buyable = (int)BoltBuy.Value;
                tempEntry.menuOrder = (int)BoltMenuOrder.Value;
                tempEntry.unkownInt5 = (int)BoltUnkown7.Value;
                tempEntry.weight = (int)BoltFillBar.Value;
                tempEntry.cost = (int)BoltCost.Value;
                tempEntry.unkownInt8 = (int)BoltUnkown8.Value;

                tempEntry.SpecialID = (int)BoltSpecialOne.Value;
                tempEntry.SpecialID2 = (int)BoltSpecialTwo.Value;
                tempEntry.SpecialID3 = (int)BoltSpecialThree.Value;

                tempEntry.itemName = BoltName.Text;
                tempEntry.ModelID = BoltModelID.Text;
                tempEntry.ModelID2 = BoltModelIDTwo.Text;
                tempEntry.ModelID3 = BoltModelIDThree.Text;
                tempEntry.ModelID4 = BoltModelIDFour.Text;
                tempEntry.ModelPath = BoltModelPath.Text;
                tempEntry.TexturePath = BoltTexturePath.Text;
                tempEntry.SmallIcon = BoltIconPath.Text;

                tempEntry.unkownInt6 = (int)BoltUnkown9.Value;


                Char.entries[Index] = tempEntry;
                boltPS2.characters[Index1] = Char;

                GenerateTreeview();
            }
        }

        private void BoltCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(BoltCharacter.SelectedIndex!=-1 && loaded)
            {
                GenerateTreeview();
            }
        }

        private void BoltPS2TreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (BoltPS2TreeView.SelectedNode != null && loaded)
            {
                int Index1 = BoltCharacter.SelectedIndex;
                int Index = 0;

                for (int i = 0; i < boltPS2.characters[Index1].entries.Count; i++)
                {
                    if (BoltPS2TreeView.SelectedNode.Name == boltPS2.characters[Index1].entries[i].ItemID.ToString())
                    {
                        Index = i;
                        break;
                    }
                }


                BoltUnkownOne.Value = boltPS2.characters[Index1].entries[Index].unkownInt1;
                BoltUnlock.Value = boltPS2.characters[Index1].entries[Index].Unlock;
                BoltUnkownTwo.Value = boltPS2.characters[Index1].entries[Index].unkownInt2;
                BoltUnkownThree.Value = boltPS2.characters[Index1].entries[Index].ItemID;
                BoltUnkownFour.Value = boltPS2.characters[Index1].entries[Index].ParentID;
                BoltCat.Value = boltPS2.characters[Index1].entries[Index].category;
                BoltBuy.Value = boltPS2.characters[Index1].entries[Index].buyable;
                BoltMenuOrder.Value = boltPS2.characters[Index1].entries[Index].menuOrder;
                BoltUnkown7.Value = boltPS2.characters[Index1].entries[Index].unkownInt5;
                BoltFillBar.Value = boltPS2.characters[Index1].entries[Index].weight;
                BoltCost.Value = boltPS2.characters[Index1].entries[Index].cost;
                BoltUnkown8.Value = boltPS2.characters[Index1].entries[Index].unkownInt8;

                BoltSpecialOne.Value = boltPS2.characters[Index1].entries[Index].SpecialID;
                BoltSpecialTwo.Value = boltPS2.characters[Index1].entries[Index].SpecialID2;
                BoltSpecialThree.Value = boltPS2.characters[Index1].entries[Index].SpecialID3;

                BoltName.Text = boltPS2.characters[Index1].entries[Index].itemName;
                BoltModelID.Text = boltPS2.characters[Index1].entries[Index].ModelID;
                BoltModelIDTwo.Text = boltPS2.characters[Index1].entries[Index].ModelID2;
                BoltModelIDThree.Text = boltPS2.characters[Index1].entries[Index].ModelID3;
                BoltModelIDFour.Text = boltPS2.characters[Index1].entries[Index].ModelID4;
                BoltModelPath.Text = boltPS2.characters[Index1].entries[Index].ModelPath;
                BoltTexturePath.Text = boltPS2.characters[Index1].entries[Index].TexturePath;
                BoltIconPath.Text = boltPS2.characters[Index1].entries[Index].SmallIcon;

                BoltUnkown9.Value = boltPS2.characters[Index1].entries[Index].unkownInt6;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < boltPS2.unkown2.Count; i++)
            {
                var temp = boltPS2.unkown2[i];
                temp.UnkownInt4 = 0;
                boltPS2.unkown2[i] = temp;
            }
        }

        #endregion

        #region Model Handler
        SSXMPFModelHandler modelHandler = new SSXMPFModelHandler();
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "Model File (*.mpf)|*.mpf|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                modelHandler = new SSXMPFModelHandler();
                modelHandler.load(openFileDialog.FileName);
                MPFList.Items.Clear();
                for (int i = 0; i < modelHandler.ModelList.Count; i++)
                {
                    MPFList.Items.Add(modelHandler.ModelList[i].FileName);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //SaveFileDialog openFileDialog = new SaveFileDialog
            //{
            //    InitialDirectory = workspacePath,
            //    Filter = "Model File (*.mpf)|*.mpf|All files (*.*)|*.*",
            //    FilterIndex = 1,
            //    RestoreDirectory = false
            //};
            //if (openFileDialog.ShowDialog() == DialogResult.OK)
            //{
            //    modelHandler.Save(openFileDialog.FileName);
            //}
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (MPFList.SelectedIndex != -1)
            {
                SaveFileDialog openFileDialog = new SaveFileDialog
                {
                    InitialDirectory = workspacePath,
                    Filter = "gltf File (*.gltf)|*.gltf|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    modelHandler.SaveModel(openFileDialog.FileName, MPFList.SelectedIndex);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (MPFList.SelectedIndex != -1)
            {
                var temp = modelHandler.ModelList[MPFList.SelectedIndex].staticMesh[MPFModelList.SelectedIndex].stripHeader;
                //SaveFileDialog openFileDialog = new SaveFileDialog
                //{
                //    InitialDirectory = workspacePath,
                //    Filter = "Obj File (*.obj)|*.obj|All files (*.*)|*.*",
                //    FilterIndex = 1,
                //    RestoreDirectory = false
                //};
                //if (openFileDialog.ShowDialog() == DialogResult.OK)
                //{
                //    //modelHandler.TestSave(openFileDialog.FileName, MPFList.SelectedIndex);
                //}
            }
        }
        #endregion

        private void MPFList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(MPFList.SelectedIndex!=-1)
            {
                ChunkSize.Text = modelHandler.ModelList[MPFList.SelectedIndex].ChunksCount.ToString();
                MeshCount.Text = modelHandler.ModelList[MPFList.SelectedIndex].MeshCount.ToString();
                SkinMeshCount.Text = modelHandler.ModelList[MPFList.SelectedIndex].FlexMeshCount.ToString();

                MPFModelList.Items.Clear();
                for (int i = 0; i < modelHandler.ModelList[MPFList.SelectedIndex].staticMesh.Count; i++)
                {
                    MPFModelList.Items.Add(i);
                }
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            MusicWindow newwindow = new MusicWindow();
            newwindow.Show();
        }

        private void ToolsVideoToMPC_Click(object sender, EventArgs e)
        {
            string Input = "";
            string Output = "";
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                //InitialDirectory = workspacePath,
                Filter = "All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                Input = openFileDialog.FileName;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                //InitialDirectory = workspacePath,
                Filter = "MPC File (*.mpc)|*.mpc|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (Input!="")
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Output = openFileDialog.FileName;
                }
            }

            if(Output!="")
            {
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "TempVideo"));
                Directory.CreateDirectory(Path.Combine(Application.StartupPath, "TempVideo\\Frame"));

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ToolsPath + "\\ffmpeg.exe";
                Input = Path.Combine(Input, "");
                string Arguments = "-i \"" + Input + "\" \"" + Path.Combine(Application.StartupPath , "TempVideo\\Temp_Audio.wav") +"\"";
                startInfo.Arguments = Arguments;
                var temp = Process.Start(startInfo);
                temp.WaitForExit();

                startInfo = new ProcessStartInfo();
                startInfo.FileName = ToolsPath + "\\ffmpeg.exe";
                Input = Path.Combine(Input, "");
                Arguments = "-i \"" + Input + "\" \"" + Path.Combine(Application.StartupPath, "TempVideo\\Frame\\%1d.png") + "\"";
                startInfo.Arguments = Arguments;
                //temp = Process.Start(startInfo);
                //temp.WaitForExit();

                //Run Through CMD
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();

                cmd.StandardInput.WriteLine(ToolsPath.Substring(0,2));
                cmd.StandardInput.WriteLine("cd " + ToolsPath);
                cmd.StandardInput.WriteLine("sx.exe -v3 -fps29.97 -ps2stream -eaxa_blk \"" + Path.Combine(Application.StartupPath, "TempVideo\\Temp_Audio.wav") + "\" = \"" + Path.Combine(Application.StartupPath, "TempVideo\\Temp_Audio.asf") + "\"");
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();
                cmd.WaitForExit();

            }

            //Directory.Delete(Path.Combine(Application.StartupPath, "TempVideo"), true);
            //while (Directory.Exists(Path.Combine(Application.StartupPath, "TempVideo")))
            {

            }
        }

        TrickyMPFModelHandler trickyMPF = new TrickyMPFModelHandler();
        private void Mpf2Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "Model File (*.mpf)|*.mpf|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                trickyMPF = new TrickyMPFModelHandler();
                trickyMPF.load(openFileDialog.FileName);
                for (int i = 0; i < trickyMPF.ModelList.Count; i++)
                {
                    listBox1.Items.Add(trickyMPF.ModelList[i].FileName);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {
                SaveFileDialog openFileDialog = new SaveFileDialog
                {
                    InitialDirectory = workspacePath,
                    Filter = "gltf File (*.gltf)|*.gltf|All files (*.*)|*.*",
                    FilterIndex = 1,
                    RestoreDirectory = false
                };
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    trickyMPF.SaveModel(openFileDialog.FileName);
                }
        }

        private void MPFModelList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        PBDHandler pBDHandler = new PBDHandler();
        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "Model File (*.pbd)|*.pbd|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pBDHandler = new PBDHandler();
                pBDHandler.load(openFileDialog.FileName);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SaveFileDialog openFileDialog = new SaveFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "gltf File (*.gltf)|*.gltf|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pBDHandler.SaveModel(openFileDialog.FileName);
            }
        }
    }
}
