using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace PMCEditor
{
    public partial class ProfileEditor : Form
    {
        private string _profilePath = "";
        private dynamic _jsonObject;

        // Max/Min skill points
        int _numericMax = 5100;
        int _numericMin = 0;

        public ProfileEditor()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Make sure the title is set
            this.Text = "SPT-AKI Profile Editor";

            // Disable the entire group box while no file is loaded
            skillGroupBox.Enabled = false;
            userGroupBox.Enabled = false;

            // Disable the save button until a file is open
            saveToolStripMenuItem.Enabled = false;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.Cancel) return;
            
            try
            {
                // Set our path to the profile
                _profilePath = openFileDialog1.FileName;

                try
                {
                    // If there is already a backup made, delete it
                    string backupFile = _profilePath + ".backup";
                    if(File.Exists( backupFile ))
                    {
                        File.Delete(backupFile);
                    }

                    // Make a backup copy 
                    File.Copy(_profilePath, backupFile);
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message}");
                }

                // Unlock our groupboxes for editing
                userGroupBox.Enabled = true;
                skillGroupBox.Enabled = true;

                // Read and Deserialize data
                string jsonContent = File.ReadAllText(_profilePath);
                dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonContent);
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

                saveToolStripMenuItem.Enabled = true;
            }
            catch (FileNotFoundException)
            {
                throw new Exception("File not found.");
            }
            catch(Exception ex) 
            {
                throw new Exception($"{ex.Message}");
            }
        }

        private void saveToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            // characters->pmc->Skills->Common[]->Id, Progress
            dynamic commonSkills = _jsonObject.characters.pmc.Skills.Common;

            // Iterate over each skill in the Common array, skipping the first two elements (bot things idk what they do)
            for (int i = 2; i < commonSkills.Count; i++)
            {
                dynamic skill = commonSkills[i];
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

        // Max all values
        private void maxAllSkillPointsButton_Click(object sender, EventArgs e)
        {
            for (int i = 2; i < 54; i++)
            {
                NumericUpDown numericUpDown = Controls.Find("numericUpDown" + (i - 1), true).FirstOrDefault() as NumericUpDown;

                if (numericUpDown != null)
                {
                    numericUpDown.Minimum = _numericMin;
                    numericUpDown.Maximum = _numericMax;
                    numericUpDown.Value = _numericMax;
                }
            }
        }

        // Min all values
        private void zeroAllSkillPointsButton_Click(object sender, EventArgs e)
        {
            for (int i = 2; i < 54; i++)
            {
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
