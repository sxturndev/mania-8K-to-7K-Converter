using System;
using System.IO;
using System.Windows.Forms;

namespace osu_8K_to_7K_Converter
{
    internal class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Convert Keysounds?", "", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    Converter.convertSounds = true;
                }
                else if (dialogResult == DialogResult.No)
                {
                    Converter.convertSounds = false;
                }

                foreach (string file in args)
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
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
        }
    }
}