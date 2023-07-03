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
using Newtonsoft.Json;

/* TODO LIST
 * Refactor anything hardcoded
 * Make an actual Save function
 * Break code into classes
 * ACTUAL ERROR CHECKING!
 * Make Backups of profiles
 * Actually Design the UI
 * Remove all placeholder text
 * Refactor the max/min buttons
 */

namespace PMCEditor
{
    public partial class Form1 : Form
    {
        private string _profilePath = "";
        private dynamic _jsonObject;

        // Max/Min skill points
        int _numericMax = 5100;
        int _numericMin = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "SPT-AKI Profile Editor v1.0.0 - By BrandoTheDev";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            try
            {
                // Set our path to the profile
                _profilePath = openFileDialog1.FileName;

                // Read and Deserialize data
                string jsonContent = File.ReadAllText(_profilePath);
                dynamic jsonObject = JsonConvert.DeserializeObject(jsonContent);
                _jsonObject = jsonObject;

                // info section
                _profileName.Text = jsonObject.info.username;
                _gameEdition.Text = jsonObject.info.edition;

                // characters->pmc->Skills->Common[]->Id, Progress
                dynamic commonSkills = _jsonObject.characters.pmc.Skills.Common;

                // Iterate over each skill in the Common array, skipping the first two elements (bot things idk what they do)
                for (int i = 2; i < commonSkills.Count; i++)
                {
                    dynamic skill = commonSkills[i];

                    // Access the Id and Progress properties dynamically
                    string skillId = skill.Id;
                    int skillProgress = skill.Progress;

                    // Assign values to the corresponding labels and numeric up-down controls
                    Label label = Controls.Find("label" + (i - 1), true).FirstOrDefault() as Label;
                    NumericUpDown numericUpDown = Controls.Find("numericUpDown" + (i - 1), true).FirstOrDefault() as NumericUpDown;

                    if (label != null)
                        label.Text = skillId;

                    if (numericUpDown != null)
                    {
                        numericUpDown.Minimum = _numericMin;
                        numericUpDown.Maximum = _numericMax;
                        numericUpDown.Value = skillProgress;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                // TODO LOL
            }
            catch(Exception ex) 
            {
                // TODO LOL
            }
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            dynamic commonSkills = _jsonObject.characters.pmc.Skills.Common;

            // Iterate over each skill in the Common array, skipping the first two elements (bot things idk what they do)
            for (int i = 2; i < commonSkills.Count; i++)
            {
                dynamic skill = commonSkills[i];

                // Assign values to the corresponding numeric up-down controls
                NumericUpDown numericUpDown = Controls.Find("numericUpDown" + (i - 1), true).FirstOrDefault() as NumericUpDown;

                if (numericUpDown != null)
                {
                    skill.Progress = numericUpDown.Value;
                }
            }

            // Serialize the modified data back to JSON
            string modifiedJsonContent = JsonConvert.SerializeObject(_jsonObject, Formatting.Indented);

            // Write the modified JSON back to the file
            File.WriteAllText(_profilePath, modifiedJsonContent);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 2; i < 54; i++)
            {
                // Assign values to the corresponding numeric up-down controls
                NumericUpDown numericUpDown = Controls.Find("numericUpDown" + (i - 1), true).FirstOrDefault() as NumericUpDown;

                if (numericUpDown != null)
                {
                    numericUpDown.Minimum = _numericMin;
                    numericUpDown.Maximum = _numericMax;
                    numericUpDown.Value = _numericMax;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            for (int i = 2; i < 54; i++)
            {
                // Assign values to the corresponding numeric up-down controls
                NumericUpDown numericUpDown = Controls.Find("numericUpDown" + (i - 1), true).FirstOrDefault() as NumericUpDown;

                if (numericUpDown != null)
                {
                    numericUpDown.Minimum = _numericMin;
                    numericUpDown.Maximum = _numericMax;
                    numericUpDown.Value = _numericMin;
                }
            }
        }
    }
}
