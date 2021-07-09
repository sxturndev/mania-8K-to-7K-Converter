using System;
using System.IO;
using System.Windows.Forms;

namespace osu_8K_to_7K_Converter
{
    public partial class Form1 : Form
    {
        private string[] filePaths;

        public Form1()
        {
            InitializeComponent();
            openfd.FileName = "";
            //openfd.Filter = ".osu files|*.osu";
            openfd.Multiselect = true;
            textBox.Text = string.Empty;
        }

        private void convertKeysounds_CheckedChanged(object sender, EventArgs e)
        {
            if (this.convertKeysounds.Checked)
            {
                Converter.convertSounds = true;
            }
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            openfd.ShowDialog();
            textBox.Text = openfd.FileName;
            filePaths = openfd.FileNames;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            if (textBox.Text == string.Empty)
            {
                MessageBox.Show("You need to select a file!");
                return;
            }

            foreach (string file in filePaths)
            {
                if (Path.GetExtension(file) != ".osu")
                {
                    MessageBox.Show($"Can't convert: {file}");
                }
                else
                {
                    Converter.Convert(file);
                }
            }

            MessageBox.Show("Done!");
        }

        private void githubLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/sxturndev/mania-8K-to-7K-Converter/");
        }
    }
}