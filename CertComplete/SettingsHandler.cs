using System;
using System.Drawing;
using System.Threading;

namespace CertComplete
{
    public sealed class SettingsHandler
    {
        private static SettingsHandler instance = null;
        private static readonly object padlock = new object();
        static ReaderWriterLock rwl = new ReaderWriterLock();

        public static Color VariableColor = new Color();
        public static Color StringColor = new Color();
        public static Color NumericColor = new Color();
        public static Color SymbolColor = new Color();
        public static Color myBackgroundColor = new Color();
        public static Color myTextColor = new Color();

        /// <summary>
        /// Private Singleton Constructor
        /// </summary>
        private SettingsHandler()
        {
            readSettings();
        }

        /// <summary>
        /// Thread Safe Singleton get instance method.
        /// </summary>
        public static SettingsHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SettingsHandler();
                    }
                    return instance;
                }
            }
        }

        /// <summary>
        /// Reads the Application Preferences Settings
        /// </summary>
        public static void readSettings()
        {
            try
            {
                // Acquiure Read Lock
                rwl.AcquireReaderLock(Timeout.Infinite);
                // Perform Read Action
                string settingsString = System.IO.File.ReadAllText(CertComplete.APPLICATION_PREFERENCES);

                Newtonsoft.Json.Linq.JObject settings = Newtonsoft.Json.Linq.JObject.Parse(settingsString);
                // Release Read Lock
                rwl.ReleaseReaderLock();

                // Set Colors
                int myBackgroundColorA = Int32.Parse(settings.GetValue("BackGroundColorA").ToString());
                int myBackgroundColorR = Int32.Parse(settings.GetValue("BackGroundColorR").ToString());
                int myBackgroundColorG = Int32.Parse(settings.GetValue("BackGroundColorG").ToString());
                int myBackgroundColorB = Int32.Parse(settings.GetValue("BackGroundColorB").ToString());
                myBackgroundColor = Color.FromArgb(myBackgroundColorA, myBackgroundColorR, myBackgroundColorG, myBackgroundColorB);

                int ForeColorA = Int32.Parse(settings.GetValue("ForeColorA").ToString());
                int ForeColorR = Int32.Parse(settings.GetValue("ForeColorR").ToString());
                int ForeColorG = Int32.Parse(settings.GetValue("ForeColorG").ToString());
                int ForeColorB = Int32.Parse(settings.GetValue("ForeColorB").ToString());
                myTextColor = Color.FromArgb(ForeColorA, ForeColorR, ForeColorG, ForeColorB);

                int VariableColorA = Int32.Parse(settings.GetValue("VariableColorA").ToString());
                int VariableColorR = Int32.Parse(settings.GetValue("VariableColorR").ToString());
                int VariableColorG = Int32.Parse(settings.GetValue("VariableColorG").ToString());
                int VariableColorB = Int32.Parse(settings.GetValue("VariableColorB").ToString());
                VariableColor = Color.FromArgb(VariableColorA, VariableColorR, VariableColorG, VariableColorB);

                int StringColorA = Int32.Parse(settings.GetValue("StringColorA").ToString());
                int StringColorR = Int32.Parse(settings.GetValue("StringColorR").ToString());
                int StringColorG = Int32.Parse(settings.GetValue("StringColorG").ToString());
                int StringColorB = Int32.Parse(settings.GetValue("StringColorB").ToString());
                StringColor = Color.FromArgb(StringColorA, StringColorR, StringColorG, StringColorB);

                int NumericColorA = Int32.Parse(settings.GetValue("NumericColorA").ToString());
                int NumericColorR = Int32.Parse(settings.GetValue("NumericColorR").ToString());
                int NumericColorG = Int32.Parse(settings.GetValue("NumericColorG").ToString());
                int NumericColorB = Int32.Parse(settings.GetValue("NumericColorB").ToString());
                NumericColor = Color.FromArgb(NumericColorA, NumericColorR, NumericColorG, NumericColorB);

                int SymbolColorA = Int32.Parse(settings.GetValue("SymbolColorA").ToString());
                int SymbolColorR = Int32.Parse(settings.GetValue("SymbolColorR").ToString());
                int SymbolColorG = Int32.Parse(settings.GetValue("SymbolColorG").ToString());
                int SymbolColorB = Int32.Parse(settings.GetValue("SymbolColorB").ToString());
                SymbolColor = Color.FromArgb(SymbolColorA, SymbolColorR, SymbolColorG, SymbolColorB);
            } catch (Exception e)
            {
                Console.WriteLine("Error reading properties");
            }
            
        }

        /// <summary>
        /// Writes the specified settings to a settings file.
        /// </summary>
        /// <param name="path">The file path of the settings file with file name included.</param>
        /// <param name="jsonData">The JSON structured settings data.</param>
        public static void writeSettings(string path, string jsonData)
        {
            // Acquire Write Lock
            /*
            if (rwl.IsReaderLockHeld)
            {
                rwl.UpgradeToWriterLock(Timeout.Infinite);
            }
            else
            {
                rwl.AcquireWriterLock(1000);
            }
            */

            // Perform Write Action
            bool deleted = false;
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            Console.WriteLine(fi.FullName);
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
                try
                {
                    System.IO.File.WriteAllText(path, jsonData);
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Error writing settings file");
                    //rwl.ReleaseWriterLock();
                }
            }

            // Release Write Lock
            //rwl.ReleaseWriterLock();
        }

        /// <summary>
        /// Gets the background and foreground colors
        /// </summary>
        /// <returns>An array of the colors in the following order: Background, Foreground, JSON_Variables, JSON_Strings, JSON_Numbers, JSON_Symbols.</returns>
        public Color[] getColors()
        {
            readSettings();
            Color[] colors = { myBackgroundColor, myTextColor, VariableColor, StringColor, NumericColor, SymbolColor };
            return colors;
        }
    }
}
