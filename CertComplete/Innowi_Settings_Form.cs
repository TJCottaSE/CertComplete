using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertComplete
{
    public partial class Innowi_Settings_Form : Form
    {
        string configFilePath = "";
        SettingsHandler settingsHandler = SettingsHandler.Instance;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Innowi_Settings_Form()
        {
            InitializeComponent();
            setColors();
        }

        /// <summary>
        /// Alternate Constructor
        /// </summary>
        /// <param name="configFilePath">File path to the configuration file.</param>
        public Innowi_Settings_Form(string configFilePath)
        {
            InitializeComponent();
            this.configFilePath = configFilePath;
            readSettings();
            setColors();
        }

        /// <summary>
        /// Sets the foreground and background colors of the window.
        /// </summary>
        private void setColors()
        {
            this.BackColor = SettingsHandler.myBackgroundColor;
            this.ForeColor = SettingsHandler.myTextColor;
            save.ForeColor = Color.Black;
        }

        /// <summary>
        /// Reads in the Configuration file settings and sets them to display.
        /// </summary>
        private void readSettings()
        {
            try
            {
                // Read in config file
                string settingsString = System.IO.File.ReadAllText(configFilePath);

                // Parse out as JSON components
                Newtonsoft.Json.Linq.JObject settings = Newtonsoft.Json.Linq.JObject.Parse(settingsString);
                Newtonsoft.Json.Linq.JToken DestinationIP = settings.GetValue("DestinationIP");
                Newtonsoft.Json.Linq.JToken DestinationPort = settings.GetValue("DestinationPort");

                // Update the display fields
                destinationIP.Text = DestinationIP.ToString();
                destinationPort.Text = DestinationPort.ToString();
            }
            catch (Exception e)
            {
                // Error reading file
                Console.WriteLine("Error reading file {0}", configFilePath);
            }
        }

        /// <summary>
        /// Saves the settings to the configuration file.
        /// </summary>
        private void save_Click(object sender, EventArgs e)
        {
            bool deleted = false;
            // Delete existing settings file
            System.IO.FileInfo fi = new System.IO.FileInfo(configFilePath);
            try
            {
                fi.Delete();
                deleted = true;
            }
            catch (System.IO.IOException ex)
            {
                Console.WriteLine(ex.Message);
            }

            // If successful file deletion, write the new settings
            if (deleted)
            {
                string settings = "{\n\t\"DestinationIP\":\"" + destinationIP.Text + "\",\n\t";
                settings += "\"DestinationPort\":\"" + destinationPort.Text + "\"\n}";
                try
                {
                    System.IO.File.WriteAllText(configFilePath, settings);
                    this.Close();
                }
                catch (Exception exc)
                {
                    Console.WriteLine("Error writing settings file");
                }
            }
        }
    }
}
