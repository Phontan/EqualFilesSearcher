using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace EqualFilesDetector
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnStartFindClick(object sender, EventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog()!= DialogResult.OK)
                return;

            var result = FileExplorer.GetEqualFiles(folderBrowserDialog.SelectedPath);

            foreach (var pair in result)
            {
                foreach (var path in pair.Value)
                {
                    rtbResults.AppendText(path+"\n");
                }
                rtbResults.AppendText("\n\n");
            }

        }
    }
}
