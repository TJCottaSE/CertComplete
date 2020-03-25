using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CertComplete
{
    public partial class CertComplete : Form
    {
        public static readonly string VERSION_NUMBER = "0.2.3 (beta)";
        public static readonly string S300_DEVICE_CONFIG = ".\\lib\\settings\\PAX_S300_CONFIG.json";
        public static readonly string INNOWI_DEVICE_CONFIG = ".\\lib\\settings\\INNOWI_CHECOUTM_CONFIG.json";
        public static readonly string APPLICATION_PREFERENCES = ".\\lib\\settings\\APP_SETTINGS.json";
        public LogWriter log = LogWriter.getInstance;
        public List<Transaction> sessionTransactions = new List<Transaction>();
        SettingsHandler settingsHandler = SettingsHandler.Instance;
        CancellationTokenSource source;
        CancellationToken token;

        Newtonsoft.Json.Linq.JObject parameters = null;
        Newtonsoft.Json.Linq.JToken tests = null;
        Newtonsoft.Json.Linq.JToken[] testArray = null;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public CertComplete()
        {
            log.Write("Starting CertComplete");
            InitializeComponent();
            setColors();
        }

        /// <summary>
        /// Activates the device configuraiton window for the PAX S300 Device
        /// </summary>
        private void s300ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Write("s300ToolStripMenuItemClicked");
            // Draw new PAX config window
            PAX_Settings_Form form = new PAX_Settings_Form(S300_DEVICE_CONFIG);
            form.Show();
        }

        /// <summary>
        /// Activates the device configuration window for the Innowi ChecOut M Device
        /// </summary>
        private void checOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Write("checOutToolStripMenuItemClicked");
            Innowi_Settings_Form form = new Innowi_Settings_Form(INNOWI_DEVICE_CONFIG);
            form.Show();
        }

        /// <summary>
        /// Runs the test using the predifined parameters from the left text box with the
        /// appropriate device from the radio button selectors.
        /// </summary>
        private async void run_Click(object sender, EventArgs e)
        {
            /*
             * 1. Validate JSON request
             * 2. Initialize Payment Device
             * 3. 
             */
            log.Write("runClicked");
            resultData.Text = "";
            Transaction transaction = new Transaction();
            source = new CancellationTokenSource();
            if (PAX_S300.Checked)
            {
                // Run the test on the PAX S300 Device
                Activity.Text = "Validating JSON";
                // Validate the JSON               

                Activity.Text = "Initializing PAX S300";
                log.Write("Initializing PAX S300");
                // Initialize device with current settings
                PAXDevice s300 = new PAXDevice();
                if (!s300.initializeDevice(S300_DEVICE_CONFIG))
                {
                    log.Write("Device Initialization Failed");
                    Activity.Text = "Device initializaiton failed";
                }
                else
                {
                    bool errorCondition = false;
                    // Check Device Object Compatability
                    Activity.Text = "Checking Device Object Compatability";

                    // TO-DO: Check that all necessary fields are being sent to the respective device
                    //Newtonsoft.Json.Linq.JObject parameters = null;
                    //Newtonsoft.Json.Linq.JToken tests = null;
                    //Newtonsoft.Json.Linq.JToken[] testArray = null;
                    try
                    {
                        parameters = Newtonsoft.Json.Linq.JObject.Parse(requestData.Text);
                    }
                    catch (Exception parseException)
                    {
                        log.Write("Error: Invalid JSON: " + requestData.Text);
                        Activity.Text = "Error: Invalid JSON";
                        errorCondition = true;
                    }
                    if (!errorCondition)
                    {
                        try
                        {
                            tests = parameters.GetValue("TestSequenceData");
                            testArray = tests.ToArray();
                        }
                        catch (Exception noTestSequence)
                        {
                            log.Write("Error: No Test Sequenct Specified");
                            Activity.Text = "Error: No Test Sequence Specified";
                            errorCondition = true;
                        }
                    }

                    int count = 0;
                    int referenceNumber = 0;
                    while (!errorCondition && count < testArray.Length)
                    {
                        if (count > 0)
                        {
                            // Add the reference number to the request
                            Newtonsoft.Json.Linq.JObject to = (Newtonsoft.Json.Linq.JObject) testArray[count];
                            to.Add(new Newtonsoft.Json.Linq.JProperty("OrigRefNum", referenceNumber));
                        }
                        // Send Request to device
                        Activity.Text = "Sending Request to PAX S300 Device... Complete transaction on device";
                        string res = await Task.Run(() => s300.processTransaction(testArray[count]));
                        resultData.Text += res + Environment.NewLine + Environment.NewLine;

                        createListBoxItem(requestData.Text, count, res);

                        referenceNumber = s300.getOriginalReferenceNumber();

                        if (PrintReceipt.Checked)
                        {
                            string receipt = "";
                            if (NoSig.Checked)
                            {
                                receipt = await Task.Run(() => s300.printEMVReceiptNoSig());
                            }
                            else {
                                receipt = await Task.Run(() => s300.printEMVReceipt());
                                
                            }
                            saveReceipts(receipt);
                        }

                        count++;
                        Activity.Text = "Completed... Awaiting next command";
                        log.Write("Completed... Awaiting next command");
                    }
                }
            }
            else if (Innowi_ChecOut.Checked)
            {
                bool isAvailable = false;
                // Run the test on the PAX S300 Device
                Activity.Text = "Validating JSON";
                // Validate the JSON    
                
                log.Write("Initializing Innowi ChecOutM Device");
                Activity.Text = "Initializing Innowi ChecOutM device";
                // Initialize device with current settings
                InnowiDevice checkOutM = new InnowiDevice();
                if (!checkOutM.initializeDevice(INNOWI_DEVICE_CONFIG))
                {
                    log.Write("Device Initialization Failed");
                    Activity.Text = "Device initializaiton failed";
                }
                else
                {
                    try
                    {
                        bool errorCondition = false;
                        // Check Device Object Compatability
                        Activity.Text = "Checking Device Object Compatability";

                        // TO-DO: Check that all necessary fields are being sent to the respective device
                        
                        try
                        {
                            parameters = Newtonsoft.Json.Linq.JObject.Parse(requestData.Text);
                        }
                        catch (Exception parseException)
                        {
                            log.Write("Error: Invalid JSON: " + requestData.Text);
                            Activity.Text = "Error: Invalid JSON";
                            errorCondition = true;
                        }
                        if (!errorCondition)
                        {
                            try
                            {
                                tests = parameters.GetValue("TestSequenceData");
                                testArray = tests.ToArray();
                            }
                            catch (Exception noTestSequence)
                            {
                                log.Write("Error: No Test Sequence Specified");
                                Activity.Text = "Error: No Test Sequence Specified";
                                errorCondition = true;
                            }
                        }
                        int count = 0;
                        int referenceNumber = 0;
                        //Console.WriteLine("TestArray.Length: " + testArray.Length);
                        token = source.Token;
                        while (!errorCondition && count < testArray.Length && !token.IsCancellationRequested)
                        {
                            // Send Request to device
                            log.Write("Sending Request to Innowi ChecOut M Device...");
                            Activity.Text = "Sending Request to Innowi ChecOut M device...";
                            //Console.WriteLine("TestArray[count]: " + testArray[count]);
                            
                            try
                            {
                                string res = await Task.Run(() => checkOutM.processTransaction(testArray[count]), token);
                                resultData.Text += res + Environment.NewLine + Environment.NewLine;
                                Activity.Text = "Received Response from Innowi ChecOut M device";
                                log.Write("Received Response from Innowi Device: " + res);

                                createListBoxItem(requestData.Text, count, res);

                                if (PrintReceipt.Checked)
                                {
                                    string receipt = "";
                                    if (NoSig.Checked)
                                    {
                                        receipt = await Task.Run(() => checkOutM.printEMVReceiptNoSig());
                                        saveReceipts(receipt);
                                    }
                                    else
                                    {
                                        receipt = await Task.Run(() => checkOutM.printEMVReceipt());
                                        Newtonsoft.Json.Linq.JObject ex = Newtonsoft.Json.Linq.JObject.Parse(res);
                                        Newtonsoft.Json.Linq.JArray sig = (Newtonsoft.Json.Linq.JArray) ex.SelectToken("Signature", false);
                                        saveReceipts(receipt, sig);
                                    }  
                                }
                                count++;
                                Activity.Text = "Completed... Awaiting next command";
                                log.Write("Completed... Awaiting next command");
                            }
                            catch (NullReferenceException nu)
                            {
                                errorCondition = true;
                                Activity.Text = "Error: Connection lost to client... Awaiting next command";
                                log.Write("Error: Connection lost to client... Awaiting next command" + nu.StackTrace);
                            } 
                            catch (OperationCanceledException cxl)
                            {
                                errorCondition = true;
                                Activity.Text = "Operation Canceled... Awaiting next command";
                                log.Write("Operation Canceled... Awaiting next command");
                            }
                        }
                    }
                    catch (ArgumentNullException e1)
                    {
                        Console.WriteLine("ArgumentNullException: {0}", e1);
                        log.Write("ArgumentNullException: " + e1.ToString());
                    }
                    catch (SocketException e2)
                    {
                        Console.WriteLine("SocketException: {0}", e2);
                        log.Write("SocketException: " + e2.ToString());
                    }
                }
            }
            /*
             * Used for future device additions
             * Add additional device hooks here
             */
             else
            {
                Activity.Text = "Please select a device.";
            }
        }

        /// <summary>
        /// Creates a transaction item to be added to the Transaction History
        /// </summary>
        /// <param name="requestJSON">The request body in JSON</param>
        /// <param name="sequenceNumber">Integer number representing which step in the sequence it is.</param>
        /// <param name="responseDetails">The response details returned from Shift4 to the PAX API.</param>
        private void createListBoxItem(string requestJSON, int sequenceNumber, string responseDetails)
        {
            log.Write("Creating Transaction History Item");
            Transaction t = new Transaction(requestJSON, sequenceNumber, responseDetails);
            tranHistory.Items.Add(t);
            log.Write("History Item " + t.ToString() + " Added");
        }

        /// <summary>
        /// Exits the Program.
        /// </summary>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Copies the text from the Request Details Pane to the clipboard
        /// </summary>
        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (requestData.SelectedText != null && requestData.SelectedText != "")
            {
                Clipboard.SetText(requestData.SelectedText);
            }
            else
            {
                Clipboard.SetText(requestData.Text);
            }
        }

        /// <summary>
        /// NOT IMPLEMENTED AT THIS TIME.
        /// </summary>
        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (requestData.SelectedText != null && requestData.SelectedText != "")
            {
                // TO-DO
            }
        }

        /// <summary>
        /// Displays the About Window
        /// </summary>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string message = "CertComplete" + Environment.NewLine;
            message += "Version: " + VERSION_NUMBER + Environment.NewLine;
            message += "Copyright: Shift4 Corporation 2019" + Environment.NewLine;
            message += "Author: Tony Cotta";
            MessageBox.Show(message);
        }

        /// <summary>
        /// Save Test Handler File->Save->Test
        /// </summary>
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\lib\\tests\\";
            saveDialog.Filter = "Test Files (*.json)|*.json|All files (*.*)|*.*";

            if (saveDialog.ShowDialog() == DialogResult.OK)
            {
                string name = saveDialog.FileName;
                File.WriteAllText(name, requestData.Text);
            }
        }

        /// <summary>
        /// Import Test Handler File->Import->Test
        /// </summary>
        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.InitialDirectory = Directory.GetCurrentDirectory() + "\\lib\\tests\\";
            openDialog.Filter = "Test Files (*.json)|*.json|All files (*.*)|*.*";

            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                string filepath = openDialog.FileName;
                try
                {
                    colorCodeText(ref requestData, System.IO.File.ReadAllText(filepath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("There was an error reading the file");
                }
            }
        }

        /// <summary>
        /// New Test Handler File->New->Test
        /// </summary>
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            requestData.Text = "";
            Test_Creation_Form form = new Test_Creation_Form();
            form.Show();
        }

        /// <summary>
        /// Overrides the draw list item defualt to do color context highlighting
        /// </summary>
        private void tranHistory_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            Graphics g = e.Graphics;

            string a = sender.ToString();
            ListBox temp = (ListBox)sender;
            if (temp.Items[e.Index].ToString().Contains("Response: A"))
            {// Draw the background color
                g.FillRectangle(new SolidBrush(Color.White), e.Bounds);
            }
            else if (temp.Items[e.Index].ToString().Contains("Response: D"))
            {// Draw the background color
                g.FillRectangle(new SolidBrush(Color.Red), e.Bounds);
            }
            else
            {
                g.FillRectangle(new SolidBrush(Color.Yellow), e.Bounds);
            }
            // Draw the text of the list item
            g.DrawString(temp.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), new PointF(e.Bounds.X, e.Bounds.Y));

            e.DrawFocusRectangle();
        }

        /// <summary>
        /// Exports the current receipt data as a PDF.
        /// </summary>
        /// <param name="receiptData">A string representation of the receipts.</param>
        public void saveReceipts(string receiptData)
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.FileName = "Receipt";
            dlg.Filter = "PDF Files|*.pdf";
            dlg.FilterIndex = 0;
            string fileName = string.Empty;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = dlg.FileName;
                Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
                doc.Open();
                iTextSharp.text.Font monoSpaceFont = FontFactory.GetFont("Courier");
                doc.Add(new Paragraph(receiptData, monoSpaceFont));
                doc.Close();
            }

        }

        public void saveReceipts(string receiptData, Newtonsoft.Json.Linq.JArray signatureCords)
        {
            if (signatureCords == null)
            {
                // If no signature coordinates to draw signature call base save receipts instead.
                saveReceipts(receiptData);
            }
            else
            {
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.FileName = "Receipt";
                dlg.Filter = "PDF Files|*.pdf";
                dlg.FilterIndex = 0;
                string fileName = string.Empty;

                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    fileName = dlg.FileName;
                    Document doc = new Document(iTextSharp.text.PageSize.LETTER, 10, 10, 42, 35);
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
                    doc.Open();
                    iTextSharp.text.Font monoSpaceFont = FontFactory.GetFont("Courier");

                    // Split the receipt at the signature line.
                    int index = receiptData.IndexOf("MERCHANT COPY");
                    index = index + 202;
                    char[] r = receiptData.ToCharArray();
                    string bottomHalf = receiptData.Substring(index);
                    string topHalf = receiptData.Substring(0, index - 1);
                    

                    // Add the top half to the PDF
                    doc.Add(new Paragraph(topHalf, monoSpaceFont));

                    // Draw the signature
                    PdfContentByte cb = writer.DirectContent;
                    for (int i = 0; i < signatureCords.Count; i++)
                    {
                        cb.MoveTo(float.Parse((string)signatureCords[i][0]), float.Parse((string)signatureCords[i][1]) + 150);
                        cb.LineTo(float.Parse((string)signatureCords[i][2]), float.Parse((string)signatureCords[i][3]) + 150);
                        cb.Stroke();
                    }

                    // Add the bottom half
                    doc.Add(new Paragraph(bottomHalf, monoSpaceFont));
                    doc.Close();
                }
            }
        }

        private void colorCodeText(ref RichTextBox box, string rawString)
        {
            Color[] colors = settingsHandler.getColors();
            Color myBackgroundColor = colors[0];
            Color myTextColor = colors[1];
            Color VariableColor = colors[2];
            Color StringColor = colors[3];
            Color NumericColor = colors[4];
            Color SymbolColor = colors[5];

            string[] theseLines;
            string thisLine;
            bool comma = false;

            int i = 0;
            theseLines = rawString.Split("\r\n".ToArray(), StringSplitOptions.RemoveEmptyEntries);

            foreach (string x in theseLines)
            {
                thisLine = x;

                if (thisLine.Contains("\": \"")) // paramter / string
                {
                    if (thisLine.Substring(thisLine.Length - 1) == ",")
                    {
                        thisLine = thisLine.Substring(0, thisLine.Length - 1);
                        comma = true;
                    }
                    else
                    {
                        comma = false;
                    }
                    for (i = 0; i < thisLine.Length; i++)
                    {
                        if (thisLine[i] == ':')
                        {
                            break;
                        }
                    }
                    box.AppendText(thisLine.Substring(0, i), VariableColor);
                    box.AppendText(thisLine.Substring(i, 1), SymbolColor);
                    box.AppendText(thisLine.Substring(i + 1), StringColor);
                    if (comma)
                    {
                        box.AppendText(",", SymbolColor);
                    }

                }
                else if (thisLine.Contains("\": 0") || thisLine.Contains("\": 5") ||
                    thisLine.Contains("\": 1") || thisLine.Contains("\": 6") ||
                    thisLine.Contains("\": 2") || thisLine.Contains("\": 7") ||
                    thisLine.Contains("\": 3") || thisLine.Contains("\": 8") ||
                    thisLine.Contains("\": 4") || thisLine.Contains("\": 9")) // parm / int
                {
                    if (thisLine.Substring(thisLine.Length - 1) == ",")
                    {
                        thisLine = thisLine.Substring(0, thisLine.Length - 1);
                        comma = true;
                    }
                    else
                    {
                        comma = false;
                    }
                    for (i = 0; i < thisLine.Length; i++)
                    {
                        if (thisLine[i] == ':')
                        {
                            break;
                        }
                    }
                    box.AppendText(thisLine.Substring(0, i), VariableColor);
                    box.AppendText(thisLine.Substring(i, 1), SymbolColor);
                    box.AppendText(thisLine.Substring(i + 1), NumericColor);
                    if (comma)
                    {
                        box.AppendText(",", SymbolColor);
                    }
                }
                else if (thisLine.Contains("\": [") || thisLine.Contains("\": {")) // parm / array or brace
                {
                    for (i = 0; i < thisLine.Length; i++)
                    {
                        if (thisLine[i] == ':')
                        {
                            break;
                        }
                    }
                    box.AppendText(thisLine.Substring(0, i), VariableColor);
                    box.AppendText(thisLine.Substring(i), SymbolColor);

                }
                else
                {
                    if (thisLine.Substring(thisLine.Length - 1) == "\"")
                    {
                        box.AppendText(thisLine, StringColor);
                    }
                    else if (thisLine.Length > 1 && thisLine.Substring(thisLine.Length - 2) == "\",")
                    {
                        box.AppendText(thisLine, StringColor);
                    }//put code here to color code ints that are on a solo line - i.e. array items
                    else
                    {
                        box.AppendText(thisLine, SymbolColor);
                    }

                }

                box.AppendText(Environment.NewLine);
            }
        }

        /// <summary>
        /// Opens the development docs in the default html handler.
        /// </summary>
        private void docsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + "/Docs/index.html");
        }

        /// <summary>
        /// Opens the application preferences window.
        /// </summary>
        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preferences pf = new Preferences();
            pf.ShowDialog();
            setColors();
        }

        /// <summary>
        /// Starts running specific tests of the CertComplete Application.
        /// </summary>
        private async void debugCertCompleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String str = resultData.Text;
            InnowiDevice checkOutM = new InnowiDevice();
            if (str == null || str == "")
            {
                throw new Exception("Nothing to test");
            }
            else
            {
                //createListBoxItem(requestData.Text, count, res);

                if (PrintReceipt.Checked)
                {
                    string receipt = "";
                    if (NoSig.Checked)
                    {
                        receipt = await Task.Run(() => checkOutM.printEMVReceiptNoSig());
                        saveReceipts(receipt);
                    }
                    else
                    {
                        receipt = await Task.Run(() => checkOutM.printEMVReceipt());
                        Newtonsoft.Json.Linq.JObject ex = Newtonsoft.Json.Linq.JObject.Parse(str);
                        Newtonsoft.Json.Linq.JArray sig = (Newtonsoft.Json.Linq.JArray)ex.SelectToken("Signature", false);
                        saveReceipts(receipt, sig);
                    }
                }
            }
        }

        /// <summary>
        /// Sets the Color Look and Feel of the Screen
        /// </summary>
        /// <param name="colors">An Array of colors.</param>
        private void setColors()
        {
            Color[] colors = settingsHandler.getColors();

            Color myBackgroundColor = colors[0];
            Color myTextColor = colors[1];
            Color VariableColor = colors[2];
            Color StringColor = colors[3];
            Color NumericColor = colors[4];
            Color SymbolColor = colors[5];

            // Background Colors
            this.BackColor = myBackgroundColor;
            menuStrip1.BackColor = myBackgroundColor;
            textBox1.BackColor = myBackgroundColor;
            textBox2.BackColor = myBackgroundColor;
            textBox3.BackColor = myBackgroundColor;
            textBox4.BackColor = myBackgroundColor;
            panel1.BackColor = myBackgroundColor;
            requestData.BackColor = myBackgroundColor;
            resultData.BackColor = myBackgroundColor;
            tranHistory.BackColor = myBackgroundColor;

            fileToolStripMenuItem.BackColor = myBackgroundColor;
            newToolStripMenuItem.BackColor = myBackgroundColor;
            testToolStripMenuItem.BackColor = myBackgroundColor;
            importToolStripMenuItem.BackColor = myBackgroundColor;
            testToolStripMenuItem1.BackColor = myBackgroundColor;
            saveToolStripMenuItem.BackColor = myBackgroundColor;
            exitToolStripMenuItem.BackColor = myBackgroundColor;

            editToolStripMenuItem.BackColor = myBackgroundColor;
            cutToolStripMenuItem.BackColor = myBackgroundColor;
            copyToolStripMenuItem.BackColor = myBackgroundColor;
            pasteToolStripMenuItem.BackColor = myBackgroundColor;
            deviceConfigurationToolStripMenuItem.BackColor = myBackgroundColor;
            pAXToolStripMenuItem.BackColor = myBackgroundColor;
            innowiToolStripMenuItem.BackColor = myBackgroundColor;
            s300ToolStripMenuItem.BackColor = myBackgroundColor;
            checOutToolStripMenuItem.BackColor = myBackgroundColor;
            preferencesToolStripMenuItem.BackColor = myBackgroundColor;

            extraToolStripMenuItem.BackColor = myBackgroundColor;
            debugCertCompleteToolStripMenuItem.BackColor = myBackgroundColor;

            helpToolStripMenuItem.BackColor = myBackgroundColor;
            aboutToolStripMenuItem.BackColor = myBackgroundColor;
            docsToolStripMenuItem.BackColor = myBackgroundColor;
            updateToolStripMenuItem.BackColor = myBackgroundColor;

            // Standard Text Colors
            menuStrip1.ForeColor = myTextColor;
            groupBox1.ForeColor = myTextColor;
            textBox1.ForeColor = myTextColor;
            textBox2.ForeColor = myTextColor;
            textBox3.ForeColor = myTextColor;
            textBox4.ForeColor = myTextColor;
            PAX_S300.ForeColor = myTextColor;
            label1.ForeColor = myTextColor;
            Activity.ForeColor = myTextColor;

            fileToolStripMenuItem.ForeColor = myTextColor;
            newToolStripMenuItem.ForeColor = myTextColor;
            testToolStripMenuItem.ForeColor = myTextColor;
            importToolStripMenuItem.ForeColor = myTextColor;
            testToolStripMenuItem1.ForeColor = myTextColor;
            saveToolStripMenuItem.ForeColor = myTextColor;
            exitToolStripMenuItem.ForeColor = myTextColor;

            editToolStripMenuItem.ForeColor = myTextColor;
            cutToolStripMenuItem.ForeColor = myTextColor;
            copyToolStripMenuItem.ForeColor = myTextColor;
            pasteToolStripMenuItem.ForeColor = myTextColor;
            deviceConfigurationToolStripMenuItem.ForeColor = myTextColor;
            pAXToolStripMenuItem.ForeColor = myTextColor;
            s300ToolStripMenuItem.ForeColor = myTextColor;
            innowiToolStripMenuItem.ForeColor = myTextColor;
            checOutToolStripMenuItem.ForeColor = myTextColor;
            preferencesToolStripMenuItem.ForeColor = myTextColor;

            extraToolStripMenuItem.ForeColor = myTextColor;
            debugCertCompleteToolStripMenuItem.ForeColor = myTextColor;

            helpToolStripMenuItem.ForeColor = myTextColor;
            aboutToolStripMenuItem.ForeColor = myTextColor;
            docsToolStripMenuItem.ForeColor = myTextColor;
            updateToolStripMenuItem.ForeColor = myTextColor;

            //requestData.ForeColor = myTextColor;
            resultData.ForeColor = myTextColor;
        }

        /// <summary>
        /// Stops the current thread from talking with a device
        /// </summary>
        private void Cancel_Click(object sender, EventArgs e)
        {
            if (source != null)
            {
                source.Cancel();
            }
        }

    }
}
