using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertComplete
{
    public partial class Preferences : Form
    {
        SettingsHandler settingsHandler = SettingsHandler.Instance;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Preferences()
        {
            InitializeComponent();
            Color[] colors = settingsHandler.getColors();
            setColors(colors[0], colors[1], colors[2], colors[3], colors[4], colors[5]);
        }

        /// <summary>
        /// Sets the Colors for the current state.
        /// </summary>
        /// <param name="myBackColor">The background color</param>
        /// <param name="myForeColor">The text color</param>
        /// <param name="VariableColor">The JSON variable color</param>
        /// <param name="StringColor">The JSON string color</param>
        /// <param name="NumericColor">The JSON number color</param>
        /// <param name="SymbolColor">The JSON symbol color</param>
        public void setColors(Color myBackColor, Color myForeColor, Color VariableColor, Color StringColor, Color NumericColor, Color SymbolColor)
        {
            this.BackColor = myBackColor;
            this.ForeColor = myForeColor;

            // Set the Menu Colors
            this.menuStrip1.BackColor = myBackColor;
            this.menuStrip1.ForeColor = myForeColor;
            this.fileToolStripMenuItem.BackColor = myBackColor;
            this.fileToolStripMenuItem.ForeColor = myForeColor;
            this.saveToolStripMenuItem.BackColor = myBackColor;
            this.saveToolStripMenuItem.ForeColor = myForeColor;

            // Set the JSON Colors
            label4.ForeColor = VariableColor;
            label5.ForeColor = StringColor;
            label6.ForeColor = NumericColor;
            label7.ForeColor = SymbolColor;

            // Set the Group Box Label Colors
            groupBox1.ForeColor = myForeColor;
            groupBox2.ForeColor = myForeColor;

            // Preserve button label colors
            Dark_Mode.ForeColor = Color.Black;
            BackgroundColor.ForeColor = Color.Black;
            TextColor.ForeColor = Color.Black;
            JSONNumberColor.ForeColor = Color.Black;
            JSONStringColor.ForeColor = Color.Black;
            JSONSymbolColor.ForeColor = Color.Black;
            JSONVariableColor.ForeColor = Color.Black;
            Save.ForeColor = Color.Black;
        }

        /// <summary>
        /// Saves the colors to a config file.
        /// </summary>
        private async void saveColors()
        {
            Console.WriteLine(Environment.CurrentDirectory);
            dynamic json = new JObject();

            json.BackGroundColorA = SettingsHandler.myBackgroundColor.A;
            json.BackGroundColorR = SettingsHandler.myBackgroundColor.R;
            json.BackGroundColorG = SettingsHandler.myBackgroundColor.G;
            json.BackGroundColorB = SettingsHandler.myBackgroundColor.B;

            json.ForeColorA = SettingsHandler.myTextColor.A;
            json.ForeColorR = SettingsHandler.myTextColor.R;
            json.ForeColorG = SettingsHandler.myTextColor.G;
            json.ForeColorB = SettingsHandler.myTextColor.B;

            json.VariableColorA = SettingsHandler.VariableColor.A;
            json.VariableColorR = SettingsHandler.VariableColor.R;
            json.VariableColorG = SettingsHandler.VariableColor.G;
            json.VariableColorB = SettingsHandler.VariableColor.B;

            json.StringColorA = SettingsHandler.StringColor.A;
            json.StringColorR = SettingsHandler.StringColor.R;
            json.StringColorG = SettingsHandler.StringColor.G;
            json.StringColorB = SettingsHandler.StringColor.B;

            json.NumericColorA = SettingsHandler.NumericColor.A;
            json.NumericColorR = SettingsHandler.NumericColor.R;
            json.NumericColorG = SettingsHandler.NumericColor.G;
            json.NumericColorB = SettingsHandler.NumericColor.B;

            json.SymbolColorA = SettingsHandler.SymbolColor.A;
            json.SymbolColorR = SettingsHandler.SymbolColor.R;
            json.SymbolColorG = SettingsHandler.SymbolColor.G;
            json.SymbolColorB = SettingsHandler.SymbolColor.B;

            SettingsHandler.writeSettings(CertComplete.APPLICATION_PREFERENCES, json.ToString());
        }

        /// <summary>
        /// Launches a Color Picker dialog, then sets the background and saves the settings.
        /// </summary>
        private void BackgroundColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                this.BackColor = cd.Color;
                SettingsHandler.myBackgroundColor = cd.Color;
                Task.Run(() => saveColors());
            }
        }

        /// <summary>
        /// Lauches a Color Picker dialog, then sets the forecolor and saves the settings.
        /// </summary>
        private void TextColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                this.ForeColor = cd.Color;
                SettingsHandler.myTextColor = cd.Color;
                Task.Run(() => saveColors());
            }
        }

        /// <summary>
        /// Quick Setting for Dark Mode
        /// </summary>
        private void Dark_Mode_Click(object sender, EventArgs e)
        {
            // Change the button text
            if (Dark_Mode.Text == "Enable")
            {
                Dark_Mode.Text = "Disable";
                // Set the background color
                this.BackColor = Color.FromArgb(36, 36, 36);
                SettingsHandler.myBackgroundColor = this.BackColor;

                // Set the text color
                this.ForeColor = Color.FromArgb(250, 250, 250);
                SettingsHandler.myTextColor = this.ForeColor;

                // Set the menu colors
                this.menuStrip1.BackColor = SettingsHandler.myBackgroundColor;
                this.menuStrip1.ForeColor = SettingsHandler.myTextColor;
                this.fileToolStripMenuItem.BackColor = SettingsHandler.myBackgroundColor;
                this.fileToolStripMenuItem.ForeColor = SettingsHandler.myTextColor;
                this.saveToolStripMenuItem.BackColor = SettingsHandler.myBackgroundColor;
                this.saveToolStripMenuItem.ForeColor = SettingsHandler.myTextColor;

                // Set JSON Colors
                SettingsHandler.VariableColor = Color.FromArgb(255, 255, 128, 128);
                label4.ForeColor = SettingsHandler.VariableColor;
                SettingsHandler.StringColor = Color.FromArgb(255, 0, 128, 255);
                label5.ForeColor = SettingsHandler.StringColor;
                SettingsHandler.NumericColor = Color.FromArgb(255, 255, 128, 255);
                label6.ForeColor = SettingsHandler.NumericColor;
                SettingsHandler.SymbolColor = Color.FromArgb(255, 0, 255, 128);
                label7.ForeColor = SettingsHandler.SymbolColor;

                // Set the Group Box Label Colors
                groupBox1.ForeColor = Color.FromArgb(250, 250, 250);
                groupBox2.ForeColor = Color.FromArgb(250, 250, 250);

                // Preserve button label colors
                Dark_Mode.ForeColor = Color.Black;
                BackgroundColor.ForeColor = Color.Black;
                TextColor.ForeColor = Color.Black;
                JSONNumberColor.ForeColor = Color.Black;
                JSONStringColor.ForeColor = Color.Black;
                JSONSymbolColor.ForeColor = Color.Black;
                JSONVariableColor.ForeColor = Color.Black;
                Save.ForeColor = Color.Black;
            }
            else
            {
                Dark_Mode.Text = "Enable";
                this.BackColor = DefaultBackColor;
                SettingsHandler.myBackgroundColor = DefaultBackColor;
                this.ForeColor = DefaultForeColor;
                SettingsHandler.myTextColor = DefaultForeColor;

                SettingsHandler.VariableColor = Color.Black;
                label4.ForeColor = SettingsHandler.VariableColor;
                SettingsHandler.StringColor = Color.Black;
                label5.ForeColor = SettingsHandler.StringColor;
                SettingsHandler.NumericColor = Color.Black;
                label6.ForeColor = SettingsHandler.NumericColor;
                SettingsHandler.SymbolColor = Color.Black;
                label7.ForeColor = SettingsHandler.SymbolColor;

                // Set the Group Box Label Colors
                groupBox1.ForeColor = Color.Black;
                groupBox2.ForeColor = Color.Black;

                // Set the menu colors
                this.menuStrip1.BackColor = DefaultBackColor;
                this.menuStrip1.ForeColor = DefaultForeColor;
                this.fileToolStripMenuItem.BackColor = DefaultBackColor;
                this.fileToolStripMenuItem.ForeColor = DefaultForeColor;
                this.saveToolStripMenuItem.BackColor = DefaultBackColor;
                this.saveToolStripMenuItem.ForeColor = DefaultForeColor;

                // Preserve button label colors
                Dark_Mode.ForeColor = Color.Black;
                BackgroundColor.ForeColor = Color.Black;
                TextColor.ForeColor = Color.Black;
                JSONNumberColor.ForeColor = Color.Black;
                JSONStringColor.ForeColor = Color.Black;
                JSONSymbolColor.ForeColor = Color.Black;
                JSONVariableColor.ForeColor = Color.Black;
                Save.ForeColor = Color.Black;
            }

            // Save the settings
            Task.Run(() => saveColors());
        }

        private void JSONVariableColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                SettingsHandler.VariableColor = cd.Color;
                label4.ForeColor = cd.Color;
                Task.Run(() => saveColors());
            }
        }

        private void JSONStringColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                SettingsHandler.StringColor = cd.Color;
                label5.ForeColor = cd.Color;
                Task.Run(() => saveColors());
            }
        }

        private void JSONNumberColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                SettingsHandler.NumericColor = cd.Color;
                label6.ForeColor = cd.Color;
                Task.Run(() => saveColors());
            }
        }

        private void JSONSymbolColor_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                SettingsHandler.SymbolColor = cd.Color;
                label7.ForeColor = cd.Color;
                Task.Run(() => saveColors());
            }
        }

        private void Save_Click(object sender, EventArgs e)
        {
            saveColors();
            this.Close();
        }
    }
}
