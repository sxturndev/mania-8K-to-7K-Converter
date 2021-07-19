using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace osu_8K_to_7K_Converter
{
    public class Converter
    {
        private static string[] fileContents;
        public static bool convertSounds = false;

        private class Chart
        {
            internal List<string> events = new List<string>();
            internal List<string> timingPoints = new List<string>();
            internal List<string> notes = new List<string>();
            internal List<string> other = new List<string>(); // Lines of file not being altered.
        }

        public static void Convert(string file)
        {
            bool writeNotes = false;
            bool writeEvents = false;
            bool writeTimingPoints = false;

            Chart chart = new Chart();
            try
            {
                fileContents = File.ReadAllLines(file);

                // Separate file into lists.
                foreach (string line in fileContents)
                {
                    switch (line)
                    {
                        case string a when a.Contains("[Events]"):
                            writeEvents = true;
                            continue;
                        case string b when b.Contains("[TimingPoints]"):
                            writeEvents = false;
                            writeTimingPoints = true;
                            continue;
                        case string c when c.Contains("[HitObjects]"):
                            writeTimingPoints = false;
                            writeNotes = true;
                            continue;
                    }

                    if (writeEvents)
                    {
                        chart.events.Add(line);
                    }
                    else if (writeTimingPoints)
                    {
                        chart.timingPoints.Add(line);
                    }
                    else if (writeNotes)
                    {
                        chart.notes.Add(line);
                    }
                    else
                    {
                        chart.other.Add(line);
                    }
                }

                // Write new version name and set key count to 7.
                for (int i = 0; i < chart.other.Count; i++)
                {
                    if (chart.other[i].Contains("Version:"))
                    {
                        chart.other[i] = $"{chart.other[i]} converted";
                    }

                    if (chart.other[i].Contains("CircleSize:"))
                    {
                        chart.other[i] = "CircleSize: 7";
                        break;
                    }
                }

                #region Convert keysounds to storyboard if specified.

                if (convertSounds)
                {
                    for (int i = 0; i < chart.notes.Count; i++)
                    {
                        /* Hit object syntax: x,y,time,type,hitSound,objectParams,hitSample
                         * Hold syntax: x,y,time,type,hitSound,endTime: hitSample
                         * Hit sample syntax: normalSet:additionSet:index:volume:filename */

                        string[] note = chart.notes[i].Split(',');
                        string x = note[0];
                        string y = note[1];
                        string time = note[2];
                        string type = note[3];
                        string hitSound = note[4];
                        string hitSample = note[note.Length - 1];
                        string volume;
                        string filename;
                        bool isLongNote = false;

                        if (System.Convert.ToInt32(hitSample.Split(':')[0]) > 3)
                        {
                            isLongNote = true;
                        }

                        if (isLongNote == true)
                        {
                            string endTime = hitSample.Split(':')[0];
                            volume = hitSample.Split(':')[4];
                            filename = hitSample.Split(':')[5];
                            chart.events.Add($"5,{time},0,\"{filename}\",{volume}");
                            chart.notes[i] = $"{x},{y},{time},{type},{hitSound},{endTime}:0:0:0:0:";
                        }
                        else
                        {
                            volume = hitSample.Split(':')[3];
                            filename = hitSample.Split(':')[4];
                            chart.events.Add($"5,{time},0,\"{filename}\",{volume}");
                            chart.notes[i] = $"{x},{y},{time},{type},{hitSound},0:0:0:0:";
                        }
                    }
                }

                #endregion Convert keysounds to storyboard if specified.

                // Note Conversion.
                for (int i = 0; i < chart.notes.Count; i++)
                {
                    string[] note = chart.notes[i].Split(',');
                    Double.TryParse(note[0], out double xval);

                    // floor(x * columnNumber / 512)
                    int columnIndex = (int)Math.Floor(xval * 8 / 512);
                    string[] newColumn = {"36", "109", "182", "256", "329", "402", "475"};
                    if (columnIndex == 0) // Remove scratch note.
                    {
                        chart.notes.RemoveAt(i);
                        i--;
                    }
                    else if (columnIndex >= 1 && columnIndex <= 7)
                    {
                        note[0] = newColumn[columnIndex-1];
                        chart.notes[i] = string.Join(",", note);
                    }
                }

                // Write new file.
                string fileName = Path.GetFileName(file);
                using (StreamWriter output = new StreamWriter($"{Path.GetDirectoryName(file)}//[7K] {fileName}"))
                {
                    chart.other.ForEach(output.WriteLine);
                    output.WriteLine("[Events]");
                    chart.events.ForEach(output.WriteLine);
                    output.WriteLine("[TimingPoints]");
                    chart.timingPoints.ForEach(output.WriteLine);
                    output.WriteLine("[HitObjects]");
                    chart.notes.ForEach(output.WriteLine);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return;
            }
        }
    }
}
