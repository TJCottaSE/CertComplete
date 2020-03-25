using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;

namespace CertComplete
{
    public partial class Test_Creation_Form : Form
    {
        SettingsHandler settingsHandler = SettingsHandler.Instance;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Test_Creation_Form()
        {
            InitializeComponent();
            setColors();
        }

        /// <summary>
        /// Sets the forground and background colors of the window.
        /// </summary>
        private void setColors()
        {
            // Set the background color
            this.BackColor = SettingsHandler.myBackgroundColor;
            // Set the text color
            this.ForeColor = SettingsHandler.myTextColor;
            // Set the file toolstrip colors
            this.menuStrip1.ForeColor = SettingsHandler.myTextColor;
            this.menuStrip1.BackColor = SettingsHandler.myBackgroundColor;
            this.openToolStripMenuItem.ForeColor = SettingsHandler.myTextColor;
            this.openToolStripMenuItem.BackColor = SettingsHandler.myBackgroundColor;
            this.saveToolStripMenuItem.ForeColor = SettingsHandler.myTextColor;
            this.saveToolStripMenuItem.BackColor = SettingsHandler.myBackgroundColor;
            this.exitToolStripMenuItem.ForeColor = SettingsHandler.myTextColor;
            this.exitToolStripMenuItem.BackColor = SettingsHandler.myBackgroundColor;
            // Set the groupbox label colors
            Device_Type.ForeColor = SettingsHandler.myTextColor;
            groupBox1.ForeColor = SettingsHandler.myTextColor;
            groupBox2.ForeColor = SettingsHandler.myTextColor;
            groupBox3.ForeColor = SettingsHandler.myTextColor;
            // Set the Save Button Text Color
            Save.ForeColor = Color.Black;
        }

        /// <summary>
        /// Closes the Test Creation Window
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Saves the new test to a json file. Performs error checking and form validation 
        /// before writing to file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do Form Validation
            string errors = DoFormValidation();

            // Report Errors
            if (errors != null)
            {
                MessageBox.Show(errors);
            }
            else
            {
                // Build and Save the File
                string testJSON = buildTest();
                Console.WriteLine(testJSON);
                saveFile(testJSON);
                this.Close();
            }
        }

        /// <summary>
        /// Saves the JSON file to disk.
        /// </summary>
        /// <param name="data">string representation of the data to be saved.</param>
        private void saveFile(string data)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\lib\\tests\\";
            saveDialog.Filter = "Test Files (*.json)|*.json|All files (*.*)|*.*";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string name = saveDialog.FileName;
                File.WriteAllText(name, data);
            }
        }

        /// <summary>
        /// Performs form validation for building a test in JSON from the GUI.
        /// </summary>
        /// <returns>A string list of form errors for correction, or null if no errors.</returns>
        private string DoFormValidation()
        {
            string errorsList = "";
            if (PAX_S300.Checked)
            {
                errorsList = doValidation();
            }
            else if (Innowi_ChecOut_M.Checked)
            {
                errorsList = doValidation();
            }
            else if (true)
            {
                // Reserved for Additional Devices
            }

            if (errorsList != "")
            {
                return errorsList;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Validates form data for PAX S300 and Innowi ChecOut M Devices.
        /// </summary>
        /// <returns>A string of errors in the form data.</returns>
        private string doValidation()
        {
            string errorsList = "";
            if (Test_Number.Text == "")
            {
                errorsList += "The Test Number field cannot be blank." + Environment.NewLine;
            }
            else
            {
                if (!numbersOnly(Test_Number.Text))
                {
                    errorsList += "The Test Number field must be an integer number." + Environment.NewLine;
                }
            }
            if (Test_Name.Text == "")
            {
                errorsList += "The Test Name cannot be blank" + Environment.NewLine;
            }
            if (Sequence1.Text == "")
            {
                errorsList += "You must select a value for Test Sequence 1." + Environment.NewLine;
            }
            if (Card_Type.Text == "")
            {
                errorsList += "Part 1: The Card Type must be selected" + Environment.NewLine;
            }
            if (Primary_Amount.Text == "")
            {
                errorsList += "Part 1: The Primary Amount field cannot be blank." + Environment.NewLine;
            }
            else
            {
                if (!isAnAmount(Primary_Amount.Text))
                {
                    errorsList += "Part 1: The Primary Amount must be specifed as a proper amount. Ex. 4.00" + Environment.NewLine;
                }
            }
            if (!isAnAmount(Tip_Amount.Text))
            {
                errorsList += "Part 1: The Tip Amount must be specified as a proper amount. Ex. 1.99" + Environment.NewLine;
            }
            if (!isAnAmount(Tax_Amount.Text))
            {
                errorsList += "Part 1: The Tax Amount must be specified as a proper amount. Ex. 0.99" + Environment.NewLine;
            }
            if (!isAnAmount(CashBack.Text))
            {
                errorsList += "Part1: The Cash Back Amount must be specified as a proper amount. Ex. 0.99" + Environment.NewLine;
            }
            if (!numbersOnly(Clerk.Text) || Clerk.Text.Length > 5)
            {
                errorsList += "Part 1: The Clerk field must be a number and less than 5 digits in length." + Environment.NewLine;
            }
            if (!numbersOnly(Invoice.Text) || Invoice.Text.Length > 10)
            {
                errorsList += "Part 1: The Invoice field must be a number and less than 10 digits in length." + Environment.NewLine;
            }
            // Street Address not validated
            if (!numbersOnly(Billing_Zip_Code.Text) || Billing_Zip_Code.Text.Length > 9)
            {
                errorsList += "Part 1: The Billing Zip Code must be a number and less than 9 digits in length." + Environment.NewLine;
            }
            if (!numbersOnly(Destination_Zip_Code.Text) || Destination_Zip_Code.Text.Length > 9)
            {
                errorsList += "Part 1: The Destination Zip Code must be a number and less than 9 digits in length." + Environment.NewLine;
            }
            // Customer First Name not validated
            // Customer Last Name not validated
            if (Customer_Reference.Text.Length > 25)
            {
                errorsList += "Part 1: Customer Reference must be less than 25 characters in length." + Environment.NewLine;
            }
            if (Product_Descriptor.Text.Length > 40)
            {
                errorsList += "Part 1: Product Descriptor cannot be greater than 40 characters." + Environment.NewLine;
            }
            // Check Sequence 2 Values
            if (Sequence2.Text != "" && Card_Type2.Text == "")
            {
                errorsList += "Part 2: The Card Type must be selected" + Environment.NewLine;
            }
            if (Sequence2.Text != "" && Card_Type.Text != Card_Type2.Text)
            {
                errorsList += "Part 2: The Card Type in Part 1 and Part 2 must match" + Environment.NewLine;
            }
            if (Sequence2.Text != "")
            {
                if (Primary_Amount2.Text != "" && !isAnAmount(Primary_Amount2.Text))
                {
                    errorsList += "Part 2: The Primary Amount must be specifed as a proper amount. Ex. 4.00" + Environment.NewLine;
                }
                if (Tip_Amount2.Text != "" && !isAnAmount(Tip_Amount2.Text))
                {
                    errorsList += "Part 2: The Tip Amount must be specified as a proper amount. Ex. 1.99" + Environment.NewLine;
                }
                if (Tax_Amount2.Text != "" && !isAnAmount(Tax_Amount2.Text))
                {
                    errorsList += "Part 2: The Tax Amount must be specified as a proper amount. Ex. 0.99" + Environment.NewLine;
                }
                if (CashBack2.Text != "" && !isAnAmount(CashBack2.Text))
                {
                    errorsList += "Part 2: The Cash Back Amount must be specified as a proper amount. Ex. 1.99" + Environment.NewLine;
                }
                if (Clerk2.Text != "" && !numbersOnly(Clerk2.Text))
                {
                    errorsList += "Part 2: The Clerk field must be a number and less than 5 digits in length." + Environment.NewLine;
                }
                if (Invoice2.Text != "" && !numbersOnly(Invoice2.Text))
                {
                    errorsList += "Part 2: The Invoice field must be a number and less than 10 digits in length." + Environment.NewLine;
                }
                // Street Address not validated
                if (Billing_Zip_Code2.Text != "" && !numbersOnly(Billing_Zip_Code2.Text))
                {
                    errorsList += "Part 2: The Billing Zip Code must be a number and less than 9 digits in length." + Environment.NewLine;
                }
                if (Destination_Zip_Code2.Text != "" && !numbersOnly(Destination_Zip_Code2.Text))
                {
                    errorsList += "Part 2: The Destination Zip Code must be a number and less than 9 digits in length." + Environment.NewLine;
                }
                // Customer First Name not validated
                // Customer Last Name not validated
                if (Customer_Reference2.Text != "" && Customer_Reference2.Text.Length > 25)
                {
                    errorsList += "Part 2: Customer Reference must be less than 25 characters in length." + Environment.NewLine;
                }
                if (Product_Descriptor2.Text != "" && Product_Descriptor2.Text.Length > 40)
                {
                    errorsList += "Part 2: Product Descriptor cannot be greater than 40 characters." + Environment.NewLine;
                }
            }
            // Check Sequence 3 Values
            if (Sequence3.Text != "")
            {
                if (Primary_Amount3.Text != "" && !isAnAmount(Primary_Amount3.Text))
                {
                    errorsList += "Part 3: The Primary Amount must be specifed as a proper amount. Ex. 4.00" + Environment.NewLine;
                }
                if (Tip_Amount3.Text != "" && !isAnAmount(Tip_Amount3.Text))
                {
                    errorsList += "Part 3: The Tip Amount must be specified as a proper amount. Ex. 1.99" + Environment.NewLine;
                }
                if (Tax_Amount3.Text != "" && !isAnAmount(Tax_Amount3.Text))
                {
                    errorsList += "Part 3: The Tax Amount must be specified as a proper amount. Ex. 0.99" + Environment.NewLine;
                }
                if (CashBack3.Text != "" && !isAnAmount(CashBack3.Text))
                {
                    errorsList += "Part 3: The Cash Back Amount must be specified as a proper amount. Ex. 1.99" + Environment.NewLine;
                }
                if (Clerk3.Text != "" && !numbersOnly(Clerk3.Text))
                {
                    errorsList += "Part 3: The Clerk field must be a number and less than 5 digits in length." + Environment.NewLine;
                }
                if (Invoice3.Text != "" && !numbersOnly(Invoice3.Text))
                {
                    errorsList += "Part 3: The Invoice field must be a number and less than 10 digits in length." + Environment.NewLine;
                }
                // Street Address not validated
                if (Billing_Zip_Code3.Text != "" && !numbersOnly(Billing_Zip_Code3.Text))
                {
                    errorsList += "Part 3: The Billing Zip Code must be a number and less than 9 digits in length." + Environment.NewLine;
                }
                if (Destination_Zip_Code3.Text != "" && !numbersOnly(Destination_Zip_Code3.Text))
                {
                    errorsList += "Part 2: The Destination Zip Code must be a number and less than 9 digits in length." + Environment.NewLine;
                }
                // Customer First Name not validated
                // Customer Last Name not validated
                if (Customer_Reference3.Text != "" && Customer_Reference3.Text.Length > 25)
                {
                    errorsList += "Part 3: Customer Reference must be less than 25 characters in length." + Environment.NewLine;
                }
                if (Product_Descriptor3.Text != "" && Product_Descriptor3.Text.Length > 40)
                {
                    errorsList += "Part 3: Product Descriptor cannot be greater than 40 characters." + Environment.NewLine;
                }
            }
            return errorsList;
        }

        /// <summary>
        /// Performs a REGEX search to check if the string only contains number 0-9.
        /// </summary>
        /// <param name="input">The string to be searched</param>
        /// <returns>True if only number, false otherwise</returns>
        private bool numbersOnly(string input)
        {
            if (input == "") { return true; }
            string pattern = @"^[0-9]+$";
            Match result = Regex.Match(input, pattern);
            return result.Success;
        }

        /// <summary>
        /// Performs a REGEX search to check if the string matches a valid money pattern. XXX.XX
        /// </summary>
        /// <param name="testAmount">The amount to check.</param>
        /// <returns>True if is valid money pattern, false otherwise.</returns>
        private bool isAnAmount(string testAmount)
        {
            if (testAmount == "") { return true; }
            string pattern = @"^[0-9]*(\.[0-9]{2})?$";
            Match result = Regex.Match(testAmount, pattern);
            return result.Success;
        }

        /// <summary>
        /// Builds a JSON structured test sequence.
        /// </summary>
        /// <returns>JSON formated string of a sequence of tests.</returns>
        private string buildTest()
        {
            // Build Base Object Data
            dynamic json = new JObject();
            json.TestNumber = Test_Number.Text;
            json.TestName = Test_Name.Text;
            if (PAX_S300.Checked)
            {
                json.DeviceType = "PAX S300";
            }
            else if (Innowi_ChecOut_M.Checked)
            {
                json.DeviceType = "Innowi ChecOut M";
            }

            // Builds the Test Sequence Array
            if (Sequence2.Text == "" && Sequence3.Text == "")
            {
                json.TestSequence = new JArray(Sequence1.Text);
            }
            else if (Sequence2.Text != "" && Sequence3.Text == "")
            {
                json.TestSequence = new JArray(Sequence1.Text, Sequence2.Text);
            }
            else
            {
                json.TestSequence = new JArray(Sequence1.Text, Sequence2.Text, Sequence3.Text);
            }

            // Build Part 1 Data
            dynamic testPart1 = new JObject();
            if (Card_Type.Text != "") { testPart1.CardType = Card_Type.Text; }
            if (Sequence1.Text != "")
            {
                if (Sequence1.Text == "Auth" || Sequence1.Text == "Sale" || Sequence1.Text == "Void")
                {
                    testPart1.TransactionType = Sequence1.Text.ToUpper();
                }
                else if (Sequence1.Text == "Manual Auth" || Sequence1.Text == "Manual Sale")
                {
                    // Not supported by PAX at this time. 8/6/19
                    if (Innowi_ChecOut_M.Checked)
                    {
                        testPart1.TransactionType = Sequence1.Text.Replace(" ", string.Empty).ToUpper();
                    }
                }
                else if (Sequence1.Text == "Refund")
                {
                    if (PAX_S300.Checked)
                        testPart1.TransactionType = "RETURN";
                    if (Innowi_ChecOut_M.Checked)
                        testPart1.TransactionType = "STANDALONEREFUND";
                }
                else if (Sequence1.Text == "Capture")
                {
                    if (PAX_S300.Checked)
                        testPart1.TransactionType = "POSTAUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart1.TransactionType = "CAPTURE";
                }
                else if (Sequence1.Text == "Incremental Auth")
                {
                    testPart1.TransactionType = "INCREMENTALAUTH";
                }

            }
            if (Primary_Amount.Text != "") { testPart1.PrimaryAmount = Double.Parse(Primary_Amount.Text); }
            if (Tip_Amount.Text != "") { testPart1.SecondaryAmount = Double.Parse(Tip_Amount.Text); }
            if (Tax_Amount.Text != "") { testPart1.TaxAmount = Double.Parse(Tax_Amount.Text); }
            if (CashBack.Text != "") { testPart1.CashBack = Double.Parse(CashBack.Text); }
            if (Clerk.Text != "") { testPart1.Clerk = Int32.Parse(Clerk.Text); }
            if (Invoice.Text != "") { testPart1.Invoice = Int32.Parse(Invoice.Text); }
            if (Street_Address.Text != "") { testPart1.StreetAddress = Street_Address.Text; }
            if (Billing_Zip_Code.Text != "") { testPart1.BillingZipCode = Int32.Parse(Billing_Zip_Code.Text); }
            if (Destination_Zip_Code.Text != "") { testPart1.DestinationZipCode = Int32.Parse(Destination_Zip_Code.Text); }
            if (Customer_First_Name.Text != "") { testPart1.CustomerFirstName = Customer_First_Name.Text; }
            if (Customer_Last_Name.Text != "") { testPart1.CustomerLastName = Customer_Last_Name.Text; }
            if (Customer_Reference.Text != "") { testPart1.CustomerReference = Customer_Reference.Text; }
            if (Product_Descriptor.Text != "") { testPart1.ProductDescriptor = Product_Descriptor.Text; }
            if (AuthCode.Text != "") { testPart1.AuthCode = AuthCode.Text; }
            if (APIOptions.Text != "")
            {
                List<string> opt = APIOptions.Text.Replace(" ","").Split(',').ToList();
                JArray opts = new JArray();
                for (int i = 0; i < opt.Count; i++)
                {
                    opts.Add(opt[i]);
                }
                testPart1.APIOptions = opts;
            }

            // Build Part 2 Data
            dynamic testPart2 = new JObject();
            if (Sequence2.Text != "")
            {
                // Handle Subsequent Request Types
                testPart2.CardType = Card_Type.Text;
                if (Sequence1.Text == "Auth" && Sequence2.Text == "Capture")
                {
                    if (PAX_S300.Checked)
                        testPart2.TransacitonType = "POSTAUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart2.TransacitonType = "CAPTURE";
                }
                else if (Sequence1.Text == "Auth" && Sequence2.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart2.TransactionType = "VOID AUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart2.TransactionType = "VOID";
                }
                else if (Sequence1.Text == "Auth" && Sequence2.Text == "Incremental Auth")
                {
                    testPart2.TransactionType = "INCREMENTALAUTH";
                }
                else if (Sequence1.Text == "Sale" && Sequence2.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart2.TransactionType = "VOID SALE";
                    if (Innowi_ChecOut_M.Checked)
                        testPart2.TransactionType = "VOID";
                }
                else if (Sequence1.Text == "Sale" && Sequence2.Text == "Refund")
                {
                    // What happened to PAX here???

                    if (Innowi_ChecOut_M.Checked)
                        testPart2.TransactionType = "REFUND";
                }
                else if (Sequence1.Text == "Refund" && Sequence2.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart2.TransactionType = "VOID RETURN";
                    if (Innowi_ChecOut_M.Checked)
                        testPart2.TransactionType = "VOID";
                }
                else if (Sequence1.Text == "Capture" && Sequence2.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart2.TransactionType = "VOID POSTAUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart2.TransactionType = "VOID";
                }
                else if (Sequence2.Text == "Manual Auth" || Sequence2.Text == "Manual Sale")
                {
                    // Not supported by PAX at this time. 8/6/19
                    if (Innowi_ChecOut_M.Checked)
                    {
                        testPart2.TransactionType = Sequence2.Text.Replace(" ", string.Empty).ToUpper();
                    }
                }

                // Add form data
                if (Primary_Amount2.Text != "") { testPart2.PrimaryAmount = Double.Parse(Primary_Amount2.Text); }
                if (Tip_Amount2.Text != "") { testPart2.SecondaryAmount = Double.Parse(Tip_Amount2.Text); }
                if (Tax_Amount2.Text != "") { testPart2.TaxAmount = Double.Parse(Tax_Amount2.Text); }
                if (CashBack2.Text != "") { testPart2.CashBack = Double.Parse(CashBack2.Text); }
                if (Clerk2.Text != "") { testPart2.Clerk = Int32.Parse(Clerk2.Text); }
                if (Invoice2.Text != "") { testPart2.Invoice = Int32.Parse(Invoice2.Text); }
                if (Street_Address2.Text != "") { testPart2.StreetAddress = Street_Address2.Text; }
                if (Billing_Zip_Code2.Text != "") { testPart2.BillingZipCode = Int32.Parse(Billing_Zip_Code2.Text); }
                if (Destination_Zip_Code2.Text != "") { testPart2.DestinationZipCode = Int32.Parse(Destination_Zip_Code2.Text); }
                if (Customer_First_Name2.Text != "") { testPart2.CustomerFirstName = Customer_First_Name2.Text; }
                if (Customer_Last_Name2.Text != "") { testPart2.CustomerLastName = Customer_Last_Name2.Text; }
                if (Customer_Reference2.Text != "") { testPart2.CustomerReference = Customer_Reference2.Text; }
                if (Product_Descriptor2.Text != "") { testPart2.ProductDescriptor = Product_Descriptor2.Text; }
                if (AuthCode2.Text != "") { testPart2.AuthCode = AuthCode2.Text; }
                if (APIOptions2.Text != "")
                {
                    List<string> opt = APIOptions2.Text.Replace(" ", "").Split(',').ToList();
                    JArray opts = new JArray();
                    for (int i = 0; i < opt.Count; i++)
                    {
                        opts.Add(opt[i]);
                    }
                    testPart2.APIOptions = opts;
                }
            }

            // Build Part 3 Data
            dynamic testPart3 = new JObject();
            if (Card_Type2.Text != "") { testPart2.CardType = Card_Type2.Text; }
            if (Sequence3.Text != "")
            {
                // Handle Subsequent Request Type
                testPart3.CardType = Card_Type.Text;
                if (Sequence2.Text == "Auth" && Sequence3.Text == "Capture")
                {
                    if (PAX_S300.Checked)
                        testPart3.TransacitonType = "POSTAUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart3.TransactionType = "CAPTURE";
                }
                else if (Sequence2.Text == "Auth" && Sequence3.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart3.TransactionType = "VOID AUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart3.TransactionType = "VOID";
                }
                else if (Sequence2.Text == "Auth" && Sequence3.Text == "Incremental Auth")
                {
                    testPart3.TransactionType = "INCREMENTALAUTH";
                }
                else if (Sequence2.Text == "Incremental Auth" && Sequence3.Text == "Capture")
                {
                    if (PAX_S300.Checked)
                        testPart3.TransactionType = "POSTAUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart3.TransactionType = "CAPTURE";
                }
                else if (Sequence2.Text == "Sale" && Sequence3.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart3.TransactionType = "VOID SALE";
                    if (Innowi_ChecOut_M.Checked)
                        testPart3.TransactionType = "VOID";
                }
                else if (Sequence2.Text == "Refund" && Sequence3.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart3.TransactionType = "VOID RETURN";
                    if (Innowi_ChecOut_M.Checked)
                        testPart3.TransactionType = "VOID";
                }
                else if (Sequence2.Text == "Capture" && Sequence3.Text == "Void")
                {
                    if (PAX_S300.Checked)
                        testPart3.TransactionType = "VOID POSTAUTH";
                    if (Innowi_ChecOut_M.Checked)
                        testPart3.TransactionType = "VOID";
                }
                else if (Sequence3.Text == "Manual Auth" || Sequence3.Text == "Manual Sale")
                {
                    // Not supported by PAX at this time. 8/6/19
                    if (Innowi_ChecOut_M.Checked)
                    {
                        testPart3.TransactionType = Sequence3.Text.Replace(" ", string.Empty).ToUpper();
                    }
                }

                // Add form data
                if (Primary_Amount3.Text != "") { testPart3.PrimaryAmount = Double.Parse(Primary_Amount3.Text); }
                if (Tip_Amount3.Text != "") { testPart3.SecondaryAmount = Double.Parse(Tip_Amount3.Text); }
                if (Tax_Amount3.Text != "") { testPart3.TaxAmount = Double.Parse(Tax_Amount3.Text); }
                if (CashBack3.Text != "") { testPart3.CashBack = Double.Parse(CashBack3.Text); }
                if (Clerk3.Text != "") { testPart3.Clerk = Int32.Parse(Clerk3.Text); }
                if (Invoice3.Text != "") { testPart3.Invoice = Int32.Parse(Invoice3.Text); }
                if (Street_Address3.Text != "") { testPart3.StreetAddress = Street_Address3.Text; }
                if (Billing_Zip_Code3.Text != "") { testPart3.BillingZipCode = Int32.Parse(Billing_Zip_Code3.Text); }
                if (Destination_Zip_Code3.Text != "") { testPart3.DestinationZipCode = Int32.Parse(Destination_Zip_Code3.Text); }
                if (Customer_First_Name3.Text != "") { testPart3.CustomerFirstName = Customer_First_Name3.Text; }
                if (Customer_Last_Name3.Text != "") { testPart3.CustomerLastName = Customer_Last_Name3.Text; }
                if (Customer_Reference3.Text != "") { testPart3.CustomerReference = Customer_Reference3.Text; }
                if (Product_Descriptor3.Text != "") { testPart3.ProductDescriptor = Product_Descriptor3.Text; }
                if (AuthCode3.Text != "") { testPart3.AuthCode = AuthCode3.Text; }
                if (APIOptions3.Text != "")
                {
                    List<string> opt = APIOptions3.Text.Replace(" ", "").Split(',').ToList();
                    JArray opts = new JArray();
                    for (int i = 0; i < opt.Count; i++)
                    {
                        opts.Add(opt[i]);
                    }
                    testPart3.APIOptions = opts;
                }
            }

            // Add the tests to the Test Sequence Data
            if (Sequence1.Text != "" && Sequence2.Text != "" && Sequence3.Text != "")
            {
                json.TestSequenceData = new JArray(testPart1, testPart2, testPart3);
            }
            else if (Sequence1.Text != "" && Sequence2.Text != "")
            {
                json.TestSequenceData = new JArray(testPart1, testPart2);
            }
            else if (Sequence1.Text != "")
            {
                json.TestSequenceData = new JArray(testPart1);
            }
            
            return json.ToString();
        }

        /// <summary>
        /// Sets the Card Type to be the same for subsequent transactions.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Card_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            Card_Type2.Text = Card_Type.Text;
            Card_Type3.Text = Card_Type2.Text;
        }

        /// <summary>
        /// Opens the windows explorer to find a file to open and read in the values
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\lib\\tests\\";
            openDialog.Filter = "Test Files (*.json)|*.json|All files (*.*)|*.*";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string filepath = openDialog.FileName;
                string data = "";
                // Read in the file
                try
                {
                    data = System.IO.File.ReadAllText(filepath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error reading the file");
                }
                // Parse and set data for field values
                parseAndSet(data);
            }
        }

        /// <summary>
        /// Parses in a given test and sets the test builder fields to what is currently in the file.
        /// </summary>
        /// <param name="data">The current test file read as a JSON string.</param>
        private void parseAndSet(string data)
        {
            try
            {
                // Get JSON Tokens
                Newtonsoft.Json.Linq.JObject parameters = Newtonsoft.Json.Linq.JObject.Parse(data);
                Test_Number.Text = parameters.GetValue("TestNumber").ToString();
                Test_Name.Text = parameters.GetValue("TestName").ToString();
                Device_Type.Text = parameters.GetValue("DeviceType").ToString();
                Newtonsoft.Json.Linq.JArray testSequence = Newtonsoft.Json.Linq.JArray.Parse(parameters.GetValue("TestSequence").ToString());
                Newtonsoft.Json.Linq.JArray testSequenceData = Newtonsoft.Json.Linq.JArray.Parse(parameters.GetValue("TestSequenceData").ToString());

                // Parse out nested Test Sequence Headers
                try
                {
                    Sequence1.Text = testSequence[0].ToString();
                    Sequence2.Text = testSequence[1].ToString();
                    Sequence3.Text = testSequence[2].ToString();
                }
                catch (Exception e) { /* Do nothing additional as the array isn't full */ }

                // Parse out 
                try
                {
                    // Set Column 1
                    Newtonsoft.Json.Linq.JToken card_type = testSequenceData[0].SelectToken("CardType", false);
                    Newtonsoft.Json.Linq.JToken primary_amount = testSequenceData[0].SelectToken("PrimaryAmount", false);
                    Newtonsoft.Json.Linq.JToken tip_amount = testSequenceData[0].SelectToken("SecondaryAmount", false);
                    Newtonsoft.Json.Linq.JToken tax_amount = testSequenceData[0].SelectToken("TaxAmount", false);
                    Newtonsoft.Json.Linq.JToken cash_back_amount = testSequenceData[0].SelectToken("CashBack", false);
                    Newtonsoft.Json.Linq.JToken clerk = testSequenceData[0].SelectToken("Clerk", false);
                    Newtonsoft.Json.Linq.JToken invoice = testSequenceData[0].SelectToken("Invoice", false);
                    Newtonsoft.Json.Linq.JToken street_address = testSequenceData[0].SelectToken("StreetAddress", false);
                    Newtonsoft.Json.Linq.JToken billing_zip_code = testSequenceData[0].SelectToken("BillingZipCode", false);
                    Newtonsoft.Json.Linq.JToken destination_zip_code = testSequenceData[0].SelectToken("DestinationZipCode", false);
                    Newtonsoft.Json.Linq.JToken customer_first_name = testSequenceData[0].SelectToken("CustomerFirstName", false);
                    Newtonsoft.Json.Linq.JToken customer_last_name = testSequenceData[0].SelectToken("CustomerLastName", false);
                    Newtonsoft.Json.Linq.JToken customer_reference = testSequenceData[0].SelectToken("CustomerReference", false);
                    Newtonsoft.Json.Linq.JToken product_descriptor = testSequenceData[0].SelectToken("ProductDescriptor", false);
                    if (card_type != null) { Card_Type.Text = card_type.ToString(); }
                    if (primary_amount != null) { Primary_Amount.Text = primary_amount.ToString(); }
                    if (tip_amount != null) { Tip_Amount.Text = tip_amount.ToString(); }
                    if (tax_amount != null) { Tax_Amount.Text = tax_amount.ToString(); }
                    if (cash_back_amount != null) { CashBack.Text = cash_back_amount.ToString(); }
                    if (clerk != null) { Clerk.Text = clerk.ToString(); }
                    if (invoice != null) { Invoice.Text = invoice.ToString(); }
                    if (street_address != null) { Street_Address.Text = street_address.ToString(); }
                    if (billing_zip_code != null) { Billing_Zip_Code.Text = billing_zip_code.ToString(); }
                    if (destination_zip_code != null) { Destination_Zip_Code.Text = destination_zip_code.ToString(); }
                    if (customer_first_name != null) { Customer_First_Name.Text = customer_first_name.ToString(); }
                    if (customer_last_name != null) { Customer_Last_Name.Text = customer_last_name.ToString(); }
                    if (customer_reference != null) { Customer_Reference.Text = customer_reference.ToString(); }
                    if (product_descriptor != null) { Product_Descriptor.Text = product_descriptor.ToString(); }

                    // Set Column 2
                    Newtonsoft.Json.Linq.JToken card_type2 = testSequenceData[1].SelectToken("CardType", false);
                    Newtonsoft.Json.Linq.JToken primary_amount2 = testSequenceData[1].SelectToken("PrimaryAmount", false);
                    Newtonsoft.Json.Linq.JToken tip_amount2 = testSequenceData[1].SelectToken("SecondaryAmount", false);
                    Newtonsoft.Json.Linq.JToken tax_amount2 = testSequenceData[1].SelectToken("TaxAmount", false);
                    Newtonsoft.Json.Linq.JToken cash_back_amount2 = testSequenceData[1].SelectToken("CashBack", false);
                    Newtonsoft.Json.Linq.JToken clerk2 = testSequenceData[1].SelectToken("Clerk", false);
                    Newtonsoft.Json.Linq.JToken invoice2 = testSequenceData[1].SelectToken("Invoice", false);
                    Newtonsoft.Json.Linq.JToken street_address2 = testSequenceData[1].SelectToken("StreetAddress", false);
                    Newtonsoft.Json.Linq.JToken billing_zip_code2 = testSequenceData[1].SelectToken("BillingZipCode", false);
                    Newtonsoft.Json.Linq.JToken destination_zip_code2 = testSequenceData[1].SelectToken("DestinationZipCode", false);
                    Newtonsoft.Json.Linq.JToken customer_first_name2 = testSequenceData[1].SelectToken("CustomerFirstName", false);
                    Newtonsoft.Json.Linq.JToken customer_last_name2 = testSequenceData[1].SelectToken("CustomerLastName", false);
                    Newtonsoft.Json.Linq.JToken customer_reference2 = testSequenceData[1].SelectToken("CustomerReference", false);
                    Newtonsoft.Json.Linq.JToken product_descriptor2 = testSequenceData[1].SelectToken("ProductDescriptor", false);
                    if (card_type2 != null) { Card_Type2.Text = card_type2.ToString(); }
                    if (primary_amount2 != null) { Primary_Amount2.Text = primary_amount2.ToString(); }
                    if (tip_amount2 != null) { Tip_Amount2.Text = tip_amount2.ToString(); }
                    if (tax_amount2 != null) { Tax_Amount2.Text = tax_amount2.ToString(); }
                    if (cash_back_amount2 != null) { CashBack2.Text = cash_back_amount2.ToString(); }
                    if (clerk2 != null) { Clerk2.Text = clerk2.ToString(); }
                    if (invoice2 != null) { Invoice2.Text = invoice2.ToString(); }
                    if (street_address2 != null) { Street_Address2.Text = street_address2.ToString(); }
                    if (billing_zip_code2 != null) { Billing_Zip_Code2.Text = billing_zip_code2.ToString(); }
                    if (destination_zip_code2 != null) { Destination_Zip_Code2.Text = destination_zip_code2.ToString(); }
                    if (customer_first_name2 != null) { Customer_First_Name2.Text = customer_first_name2.ToString(); }
                    if (customer_last_name2 != null) { Customer_Last_Name2.Text = customer_last_name2.ToString(); }
                    if (customer_reference2 != null) { Customer_Reference2.Text = customer_reference2.ToString(); }
                    if (product_descriptor2 != null) { Product_Descriptor2.Text = product_descriptor2.ToString(); }

                    // Set Column 3
                    Newtonsoft.Json.Linq.JToken card_type3 = testSequenceData[2].SelectToken("CardType", false);
                    Newtonsoft.Json.Linq.JToken primary_amount3 = testSequenceData[2].SelectToken("PrimaryAmount", false);
                    Newtonsoft.Json.Linq.JToken tip_amount3 = testSequenceData[2].SelectToken("SecondaryAmount", false);
                    Newtonsoft.Json.Linq.JToken tax_amount3 = testSequenceData[2].SelectToken("TaxAmount", false);
                    Newtonsoft.Json.Linq.JToken cash_back_amount3 = testSequenceData[2].SelectToken("CashBack", false);
                    Newtonsoft.Json.Linq.JToken clerk3 = testSequenceData[2].SelectToken("Clerk", false);
                    Newtonsoft.Json.Linq.JToken invoice3 = testSequenceData[2].SelectToken("Invoice", false);
                    Newtonsoft.Json.Linq.JToken street_address3 = testSequenceData[2].SelectToken("StreetAddress", false);
                    Newtonsoft.Json.Linq.JToken billing_zip_code3 = testSequenceData[2].SelectToken("BillingZipCode", false);
                    Newtonsoft.Json.Linq.JToken destination_zip_code3 = testSequenceData[2].SelectToken("DestinationZipCode", false);
                    Newtonsoft.Json.Linq.JToken customer_first_name3 = testSequenceData[2].SelectToken("CustomerFirstName", false);
                    Newtonsoft.Json.Linq.JToken customer_last_name3 = testSequenceData[2].SelectToken("CustomerLastName", false);
                    Newtonsoft.Json.Linq.JToken customer_reference3 = testSequenceData[2].SelectToken("CustomerReference", false);
                    Newtonsoft.Json.Linq.JToken product_descriptor3 = testSequenceData[2].SelectToken("ProductDescriptor", false);
                    if (card_type3 != null) { Card_Type3.Text = card_type3.ToString(); }
                    if (primary_amount3 != null) { Primary_Amount3.Text = primary_amount3.ToString(); }
                    if (tip_amount3 != null) { Tip_Amount3.Text = tip_amount3.ToString(); }
                    if (tax_amount3 != null) { Tax_Amount3.Text = tax_amount3.ToString(); }
                    if (cash_back_amount3 != null) { CashBack3.Text = cash_back_amount3.ToString(); }
                    if (clerk3 != null) { Clerk3.Text = clerk3.ToString(); }
                    if (invoice3 != null) { Invoice3.Text = invoice3.ToString(); }
                    if (street_address3 != null) { Street_Address3.Text = street_address3.ToString(); }
                    if (billing_zip_code3 != null) { Billing_Zip_Code3.Text = billing_zip_code3.ToString(); }
                    if (destination_zip_code3 != null) { Destination_Zip_Code3.Text = destination_zip_code3.ToString(); }
                    if (customer_first_name3 != null) { Customer_First_Name3.Text = customer_first_name3.ToString(); }
                    if (customer_last_name3 != null) { Customer_Last_Name3.Text = customer_last_name3.ToString(); }
                    if (customer_reference3 != null) { Customer_Reference3.Text = customer_reference3.ToString(); }
                    if (product_descriptor3 != null) { Product_Descriptor3.Text = product_descriptor3.ToString(); }
                }
                catch (Exception e)
                {
                    // Not sure this is needed
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine("Error parsing in JSON");
            }
        }
    }
}
