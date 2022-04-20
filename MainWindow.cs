using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SSX_Modder.FileHandlers;
using DiscUtils;
using DiscUtils.Iso9660;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace SSX_Modder
{
    public partial class MainWindow : Form
    {
        string workspacePath = Application.StartupPath + "\\disk\\SSX3\\";
        public MainWindow()
        {
            InitializeComponent();
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
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = "c:\\",
                Filter = "Iso Image (*.iso)|*.iso|All files (*.*)|*.*",
                FilterIndex = 1,
                //RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                SetStatus("Starting Extraction... (Program may freeze)");
                ExtractISO(openFileDialog.FileName, workspacePath);
                MessageBox.Show("Extraction Done");
                SetStatus("");
            }

        }

        void ExtractISO(string ISOName, string ExtractionPath)
        {
            using (FileStream ISOStream = File.Open(ISOName, FileMode.Open))
            {
                CDReader Reader = new CDReader(ISOStream, true, true);
                if(Directory.Exists(ExtractionPath))
                {
                    Directory.Delete(ExtractionPath,true);
                }
                ExtractDirectory(Reader.Root, ExtractionPath + "\\", "");
                Reader.Dispose();
            }
        }
        void ExtractDirectory(DiscDirectoryInfo Dinfo, string RootPath, string PathinISO)
        {
            if (!string.IsNullOrWhiteSpace(PathinISO))
            {
                PathinISO += "\\" + Dinfo.Name;
            }
            RootPath += "\\" + Dinfo.Name;
            AppendDirectory(RootPath);
            foreach (DiscDirectoryInfo dinfo in Dinfo.GetDirectories())
            {
                ExtractDirectory(dinfo, RootPath, PathinISO);
            }
            foreach (DiscFileInfo finfo in Dinfo.GetFiles())
            {
                using (Stream FileStr = finfo.OpenRead())
                {
                    using (FileStream Fs = File.Create(RootPath + "\\" + finfo.Name)) // Here you can Set the BufferSize Also e.g. File.Create(RootPath + "\\" + finfo.Name, 4 * 1024)
                    {
                        FileStr.CopyTo(Fs, 4 * 1024); // Buffer Size is 4 * 1024 but you can modify it in your code as per your need
                    }
                }
            }
        }
        static void AppendDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (DirectoryNotFoundException Ex)
            {
                AppendDirectory(Path.GetDirectoryName(path));
            }
            catch (PathTooLongException Exx)
            {
                AppendDirectory(Path.GetDirectoryName(path));
            }
        }
        private void toolStripButton2_Click(object sender, EventArgs e)
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
                SetStatus("Starting Building...");
                //CreateIsoImage(Path.GetFullPath(workspacePath + "/disk/" + Path.GetFileNameWithoutExtension("SSX3")), openFileDialog.FileName, "SSX3");
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Program Files (x86)\ImgBurn\ImgBurn.exe";
                string test = Path.Combine(workspacePath, "");
                startInfo.Arguments = "/MODE BUILD /BUILDINPUTMODE IMAGEFILE /SRC \"" + test + "\" /DEST \"" + openFileDialog.FileName + "\" /FILESYSTEM \"ISO9660 + UDF\" /UDFREVISION \"1.02\" /VOLUMELABEL \"SSX3\" /ERASE /OVERWITE YES /START /CLOSESUCCESS /NOIMAGEDETAILS /ROOTFOLDER";
                Process.Start(startInfo);
            }
        }
        public string CreateIsoImage(string sourceDrive, string targetIso, string volumeName)
        {
            try
            {
                var srcFiles = Directory.GetFiles(sourceDrive, "*", SearchOption.AllDirectories);
                var iso = new CDBuilder
                {
                    UseJoliet = false,
                    VolumeIdentifier = volumeName
                };
                foreach (var file in srcFiles)
                {
                    var fi = new FileInfo(file);
                    if (fi.Directory.FullName == sourceDrive)
                    {
                        iso.AddFile($"{fi.Name}", fi.FullName);
                        continue;
                    }
                    var srcDir = fi.Directory.FullName.Replace(sourceDrive, "").TrimEnd('\\');
                    iso.AddDirectory(srcDir);
                    iso.AddFile($"{srcDir}\\{fi.Name}", fi.FullName);
                }
                iso.Build(targetIso);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #region Loc File
        LOCHandler locHandler = new LOCHandler();
        private void LocBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "";
            LocrichTextBox1.Text = locHandler.textList[LocBox1.SelectedIndex];
            LocrichTextBox2.Text = locHandler.byteListString[LocBox1.SelectedIndex];
        }
        private void LocLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = workspacePath,
                Filter = "Loc File (*.loc)|*.loc|All files (*.*)|*.*",
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
                toolStripStatusLabel1.Text = "";
                locHandler.textList[LocBox1.SelectedIndex] = LocrichTextBox1.Text;
                LocBox1.Items[LocBox1.SelectedIndex] = LocrichTextBox1.Text;
                locHandler.UpdatedByteList(LocBox1.SelectedIndex);
                LocrichTextBox2.Text = locHandler.byteListString[LocBox1.SelectedIndex];
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
        #endregion

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Process.Start(workspacePath);
        }

        BigFHandler bigfHandler = new BigFHandler();
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
                if (bigfHandler.bigFiles.Count==0)
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
            if(BigBox1.SelectedIndex!=-1)
            {
                int i = BigBox1.SelectedIndex;
                BigPathLabel.Text = bigfHandler.bigFiles[i].path;
                byte[] temp = BitConverter.GetBytes(bigfHandler.bigFiles[i].offset);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(temp);
                BigOffsetLabel.Text = "0x"+BitConverter.ToString(temp).Replace("-","");
                BigSizeLabel.Text = bigfHandler.bigFiles[i].size.ToString();
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
                SetStatus("Extracting Please Wait....");
                bigfHandler.ExtractBig(openFileDialog.FileName);
                SetStatus("");
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
                bigfHandler.BuildBig(openFileDialog.FileName);
                MessageBox.Show("Building Done");
                GC.Collect();
            }
        }
    }
}
