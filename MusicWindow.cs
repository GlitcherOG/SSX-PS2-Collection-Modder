using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSX_Modder
{
    public partial class MusicWindow : Form
    {
        public MusicWindow()
        {
            InitializeComponent();
        }

        private void GeneralMPFToWav_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                //InitialDirectory = workspacePath,
                Filter = "MPF File (*.mpf)|*.mpf|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void GeneralWavToMPF_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                //InitialDirectory = workspacePath,
                Filter = "Wav File (*.Wav)|*.Wav|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = false
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {

            }
        }
    }
}
