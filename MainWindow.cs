using System;
using System.IO;
using System.Windows.Forms;
using SSX_Modder.FileHandlers;
using System.Diagnostics;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Drawing;
using System.Drawing.Imaging;
using SSX_Modder.Utilities;

namespace SSX_Modder
{
    public partial class MainWindow : Form
    {
        string workspacePath = Application.StartupPath + "\\disk\\SSX3\\";
        string ImgBurnPath = @"C:\Program Files (x86)\ImgBurn\ImgBurn.exe";
        string ZipPath = @"C:\Program Files\7-Zip\7z.exe";
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
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ZipPath;
                string test = Path.Combine(workspacePath, "");
                startInfo.Arguments = "x \"" + openFileDialog.FileName +"\" *.* -o\"" + test + "\" -r -y";
                Process.Start(startInfo);
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
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = ImgBurnPath;
                string test = Path.Combine(workspacePath, "");
                startInfo.Arguments = "/MODE BUILD /BUILDINPUTMODE IMAGEFILE /SRC \"" + test + "\" /DEST \"" + openFileDialog.FileName + "\" /FILESYSTEM \"ISO9660 + UDF\" /UDFREVISION \"1.02\" /VOLUMELABEL \"SSX3\" /ERASE /OVERWITE YES /START /NOIMAGEDETAILS /ROOTFOLDER";
                Process.Start(startInfo);
            }
        }
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Process.Start(workspacePath);
        }

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
            else if (a != LocBox1.Items.Count-1)
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

        #region BIGF
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
                if(bigfHandler.bigHeader.compression)
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
                BigCompressed.Text = "Null";
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
                    SSHlistBox1.Items.Add(sshHandler.sshImages[i].shortname+ "."+sshHandler.sshImages[i].longname);
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

                if (SSHColourByteSwapped.Checked)
                {
                    temp.sshTable.Format = 2;
                }
                else
                {
                    temp.sshTable.Format = 0;
                }

                if(SSHImageByteSwapped.Checked)
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
            }
        }
        #endregion

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
    }
}
