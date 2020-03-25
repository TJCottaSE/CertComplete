using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CertComplete
{
    public class InnowiDevice
    {
        public LogWriter log = LogWriter.getInstance;
        bool initialized = false;
        bool sigRequired = false;
        string IPAddress = null;
        Int32 Port = 0;
        TcpClient client;
        string responseJSON = string.Empty;
        string requestJSON = string.Empty;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public InnowiDevice()
        {
            // Intentionally left blank for now
        }

        /// <summary>
        /// Initializes the device using stored config file.
        /// </summary>
        /// <param name="DeviceConfigFilePath">File path of the device configuration file</param>
        /// <returns>True if no errors configuring device, False otherwise.</returns>
        public bool initializeDevice(string DeviceConfigFilePath)
        {           
            try
            {
                // Read in config file
                string settingsString = System.IO.File.ReadAllText(DeviceConfigFilePath);
                // Trim off carrige returns and line feeds
                int len = settingsString.Length;
                while (settingsString[len - 1] != '}')
                {
                    settingsString = settingsString.Substring(0, len - 1);
                    len = settingsString.Length;
                }

                // Parse out as JSON
                Newtonsoft.Json.Linq.JObject settings = Newtonsoft.Json.Linq.JObject.Parse(settingsString);
                Newtonsoft.Json.Linq.JToken destinationIP = settings.GetValue("DestinationIP");
                Newtonsoft.Json.Linq.JToken destinationPort = settings.GetValue("DestinationPort");

                // Set the device parameters
                // TO-DO: setComSettings
                IPAddress = destinationIP.ToString();
                Port = Int32.Parse(destinationPort.ToString());

                initialized = true;
            }
            catch (Exception e)
            {
                initialized = false;
                log.Write("Error: Device Initialization " + e.StackTrace);
            }
            log.Write("Device Initialized: " + initialized);
            return initialized;
        }

        /// <summary>
        /// Reads and writes data to and from the network.
        /// </summary>
        /// <param name="transactionData">JSON representation of the transaction.</param>
        /// <returns></returns>
        public async Task<string> processTransaction(Newtonsoft.Json.Linq.JToken transactionData)
        {
            // Save the request for receipt printing
            // NOTE: possible data loss in multi-thread....
            requestJSON = transactionData.ToString();
            // Start the connection to the device server
            startListener();
            // Get a network stream to read and write to the device server
            NetworkStream stream = client.GetStream();
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(transactionData.ToString());
            // Write the json data to the stream
            log.Write("Writing request: " + requestJSON);
            stream.Write(data, 0, data.Length);
            Byte[] res = new byte[16384];
            // Block waiting for response on stream
            Int32 bytes = stream.Read(res, 0, res.Length);
            responseJSON = System.Text.Encoding.ASCII.GetString(res, 0, bytes);
            // Clean up and return
            stream.Close();
            //client.Close();
            log.Write("Response received: " + responseJSON);
            return responseJSON;
        }

        /// <summary>
        /// Starts the TCP listener
        /// </summary>
        public void startListener()
        {
            if (!initialized)
            {
                throw new Exception("Device not initialized");
            }
            try
            {
                // Create a TCP Client
                //Int32 port = 13000;
                client = new TcpClient(/*server*//*"10.17.2.218"*//*"192.168.17.53"*/IPAddress, Port);
                //Activity.Text = "Establishing Connection to the device.";

                // Translate the passed message into ASCII and store it as a Byte Array
                // OR MAYBE get a byte array of the test in the test sequence.
                //Byte[] data = System.Text.Encoding.ASCII.GetBytes(testArray[count].ToString());
                Byte[] data = null;
                // Get a client stream for reading and writing
                //NetworkStream stream = client.GetStream();

                // Send the message to the connected TCP Server
                //stream.Write(data, 0, data.Length);
                Console.WriteLine("Sent: I'm a request");

                // Receive the TCPServer.response
                // Buffer to store the response bytes.
                data = new byte[256];

                // String to store the ASCII representation
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                //Int32 bytes = stream.Read(data, 0, data.Length);
                //responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: " + responseData);

                //stream.Close(); // Maybe don't do this.....
                //client.Close();
            }
            catch (SocketException e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        /// <summary>
        /// Creates an EMV compliant receipt
        /// </summary>
        /// <returns>String representation of a receipt.</returns>
        public async Task<string> printEMVReceipt()
        {
            log.Write("Printing EMV Receipt");
            string receiptText = formatTransactionData();
            receiptText += Environment.NewLine + formatEMVData();

            ReceiptPrinter rp = new ReceiptPrinter();
            rp.setTransactionData(receiptText);
            if (sigRequired)
            {
                return rp.ToString();
            }
            else
            {
                return rp.ToStringNoSig();
            }
        }

        /// <summary>
        /// Formats Receipt data without signature line
        /// </summary>
        /// <returns>A receipt string without a signature line</returns>
        public async Task<string> printEMVReceiptNoSig()
        {
            log.Write("Printing EMV Receipt (no sig)");
            string receiptText = formatTransactionData();
            receiptText += Environment.NewLine + formatEMVData();

            ReceiptPrinter rp = new ReceiptPrinter();
            rp.setTransactionData(receiptText);
            return rp.ToStringNoSig();
        }

        /// <summary>
        /// Formats the EMV Data from the device
        /// </summary>
        /// <returns></returns>
        public string formatEMVData()
        {
            log.Write("Formatting EMV Data");
            string ret = string.Empty;
            JObject data = JObject.Parse(responseJSON);
            string dt = data.SelectToken("Receipt", false).ToString();
            if (dt != "")
            {
                log.Write("Receipt Array is not null: " + dt);
                JArray array = JArray.Parse(data.SelectToken("Receipt", true).ToString());
                //JArray sigArray = JArray.Parse(data.SelectToken("Signature", true).ToString());
                JToken sigArrayToken = data.SelectToken("Signature", false);
                if (sigArrayToken != null)
                {
                    JArray sigArray = JArray.Parse(data.SelectToken("Signature", true).ToString());
                    if (sigArray.Count > 1)
                    {
                        log.Write("Signature Required (signature data block detected)");
                        sigRequired = true;
                    }
                }

                foreach (JObject obj in array.Children<JObject>())
                {
                    string key = "";
                    string printName = "";
                    string printValue = "";
                    foreach (JProperty singleProp in obj.Properties())
                    {
                        string name = singleProp.Name;
                        string value = singleProp.Value.ToString();
                        if (name == "key")
                        {
                            key = value;
                        }
                        else if (name == "printName")
                        {
                            printName = value;
                        }
                        else if (name == "printValue")
                        {
                            printValue = value;
                        }
                        else
                        {
                            // Something went wrong parsing data.
                            log.Write("Something went wrong parsing data");
                            throw new Exception();
                        }
                    }
                    // After parsing properties of a receipt item, align them on the receipt
                    if (key == "SignatureRequired" && printValue == "Y")
                    {
                        log.Write("Signature Required (Required by tag)");
                        sigRequired = true;
                    }
                    else
                    {
                        ret += lrAlign(printName, printValue, 40);
                        ret += Environment.NewLine;
                    }
                }
            }
            else
            {
                ret = "";
                log.Write("No Receipt Data Returned from device");
            }

            return ret;
        }

        /// <summary>
        /// Formats the transaction data into part of a receipt.
        /// </summary>
        /// <returns>A string representation of receipt text</returns>
        public string formatTransactionData()
        {
            log.Write("Formatting Transaction Data");
            string receiptText = "";
            // Parse out receipt text values
            JObject request = JObject.Parse(requestJSON);
            JObject response = JObject.Parse(responseJSON);
            JToken transType = request.SelectToken("CardType", false);
            JToken responseCode = response.SelectToken("Status", false);
            JToken authCode = response.SelectToken("AuthCode", false);
            JToken invoice = response.SelectToken("Invoice", false);
            JToken entryMethod = response.SelectToken("CardEntryMode", false);
            JToken cardType = response.SelectToken("CardType", false);
            JToken cardNumber = response.SelectToken("Last4", false);
            JToken tax = request.SelectToken("TaxAmount", false);
            JToken tip = request.SelectToken("SecondaryAmount", false);
            JToken cashBack = request.SelectToken("CashbackAmount", false);
            JToken surcharge = request.SelectToken("Surcharge", false);
            JToken amount = response.SelectToken("ApprovedAmount", false);

            receiptText += centerText(request.SelectToken("TransactionType",true).ToString(), 40) + Environment.NewLine;
            receiptText += lrAlign("Date / Time", getDateTime(), 40) + Environment.NewLine;
            if (cardType != null)     { receiptText += lrAlign("Tender", transType.ToString(), 40) + Environment.NewLine; }
            if (responseCode != null) { receiptText += lrAlign("Response", mapInnowiResponseToString(responseCode.ToString()), 40) + Environment.NewLine; }
            if (authCode != null)     { receiptText += lrAlign("Auth Code", authCode.ToString(), 40) + Environment.NewLine; }
            if (invoice != null)      { receiptText += lrAlign("Invoice", invoice.ToString(), 40) + Environment.NewLine; }
            //if (entryMethod != null)  { receiptText += lrAlign("Entry Method", entryMethod.ToString(), 40) + Environment.NewLine; }
            if (cardType != null)     { receiptText += lrAlign("Card Type", cardType.ToString(), 40) + Environment.NewLine; }
            //if (cardNumber != null)   { receiptText += lrAlign("Card #", cardNumber.ToString(), 40) + Environment.NewLine; }
            bool needsTotalLine = false;
            string ta = "", ti = "", cash = "", sur = "";
            if (tax != null && tax.ToString() != "" && tax.ToString() != "0")
            {
                ta += lrAlign("Tax", formatAmount(tax.ToString()), 40) + Environment.NewLine;
                needsTotalLine = true;
            }
            if (tip != null && tip.ToString() != "" && tip.ToString() != "0")
            {
                ti += lrAlign("Tip", formatAmount(tip.ToString()), 40) + Environment.NewLine;
                needsTotalLine = true;
            }
            if (cashBack != null && cashBack.ToString() != "" && cashBack.ToString() != "0")
            {
                cash += lrAlign("Cash Back", formatAmount(cashBack.ToString()), 40) + Environment.NewLine;
                needsTotalLine = true;
            }
            if (surcharge != null && surcharge.ToString() != "" && surcharge.ToString() != "0")
            {
                sur += lrAlign("Surcharge", formatAmount(surcharge.ToString()), 40) + Environment.NewLine;
                needsTotalLine = true;
            }
            if (needsTotalLine)
            {
                string sAmt = (amount != null) ? amount.ToString() : null;
                string sTax = (tax != null) ? tax.ToString() : null;
                string sTip = (tip != null) ? tip.ToString() : null;
                string sCashBack = (cashBack != null) ? cashBack.ToString() : null;
                string sSurcharge = (surcharge != null) ? surcharge.ToString() : null;
                string total = calcSubTotal(sAmt, sTax, sTip, sCashBack, sSurcharge);
                receiptText += lrAlign("Subtotal", formatAmount(total.ToString()), 40) + Environment.NewLine;
            }
            receiptText += ta + ti + cash + sur;

            string amtString;
            if (amount.ToString() == "" || amount.ToString() == "0")
            {
                amtString = request.SelectToken("PrimaryAmount", true).ToString();
            }
            else
            {
                amtString = amount.ToString();
            }
            receiptText += lrAlign("Total", "USD$ " + formatAmount(amtString), 40) + Environment.NewLine;

            return receiptText;
        }

        /// <summary>
        /// Aligns key value pairs.   CONSIDER REFACTORING THIS OUT TO A DIFFERENT CLASS OR SUPERCLASS
        /// </summary>
        /// <param name="Name">Left side value</param>
        /// <param name="Value">Right side value</param>
        /// <param name="Width">Total line width</param>
        /// <returns>formatted lign of text.</returns>
        private string lrAlign(string Name, string Value, int Width)
        {
            string ret = "";
            int spaces = Width - Value.Length - Name.Length;
            ret += Name;
            for (int i = 0; i < spaces; i++)
            {
                ret += " ";
            }
            ret += Value;
            return ret;
        }

        /// <summary>
        /// Centers text in a line.   CONSIDER REFACTORING THIS OUT TO A DIFFERENT CLASS OR SUPERCLASS
        /// </summary>
        /// <param name="Text">Text to be centered.</param>
        /// <param name="Width">Total line width of the receipt in chars.</param>
        /// <returns>A string represention of "   text   " centered for a given width.</returns>
        private string centerText(string Text, int Width)
        {
            string ret = "";
            int charSize = Text.Length;
            int left = (Width - charSize) / 2;
            for (int i = 0; i < left; i++) { ret += " "; }
            ret += Text;
            int newSize = ret.Length;
            for (int k = newSize; k < Width; k++) { ret += " "; }
            return ret;
        }

        /// <summary>
        /// Gets the current date time en-US  CONSIDER REFACTORING THIS OUT TO A DIFFERENT CLASS OR SUPERCLASS
        /// </summary>
        /// <returns></returns>
        private string getDateTime()
        {
            DateTime localDate = System.DateTime.Now;
            var culture = new CultureInfo("en-US");
            string ret = localDate.ToString(culture);
            string ret2 = localDate.ToString(culture);
            return ret;
        }

        /// <summary>
        /// Formats the amount to be a standard schema namely X.XX   CONSIDER REFACTORING THIS OUT TO A DIFFERENT CLASS OR SUPERCLASS
        /// </summary>
        /// <param name="amount">The amount to be formatted.</param>
        /// <returns>A string representation of the Amount.</returns>
        private string formatAmount(string amount)
        {
            try
            {
                double dd = Double.Parse(amount);
                //dd = dd / 100.00;
                return string.Format("{0:N2}", dd);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the Subtotal if on exists for a given transaction.  CONSIDER REFACTORING THIS OUT TO A DIFFERENT CLASS OR SUPERCLASS
        /// </summary>
        /// <param name="Amount">The Total Amount</param>
        /// <param name="Tax">The Tax Amount</param>
        /// <param name="Tip">The Tip Amount</param>
        /// <param name="CashBack">The Cash Back Amount</param>
        /// <param name="Surcharge">The Surcharge Amount</param>
        /// <returns>The Total minus any tax, tip, cashback, or surcharges.</returns>
        private string calcSubTotal(string Amount, string Tax, string Tip, string CashBack, string Surcharge)
        {
            double total = 0.0, amt = 0.0, tax = 0.0, tip = 0.0, cash = 0.0, sur = 0.0;
            if (Amount != null) { amt = Double.Parse(Amount); }
            if (Tax != null) { tax = Double.Parse(Tax); }
            if (Tip != null) { tip = Double.Parse(Tip); }
            if (CashBack != null) { cash = Double.Parse(CashBack); }
            if (Surcharge != null) { sur = Double.Parse(Surcharge); }
            total = amt - tax - tip - cash - sur;
            return total.ToString();
        }

        /// <summary>
        /// Maps the Innowi Response Code to a human readable result
        /// </summary>
        /// <param name="value">The Response Code to be mapped to string</param>
        /// <returns>The response Code representation as a string</returns>
        private string mapInnowiResponseToString(string value)
        {
            string str = "";
            switch (value)
            {
                case "0":
                    str += "A";
                    break;
                case "-1":
                    str += "D";
                    break;
                case "2":
                    str += "Error";
                    break;
                case "3":
                    str += "Terminal not Available";
                    break;
                case "4":
                    str += "Terminal Busy";
                    break;
                case "5":
                    str += "R";
                    break;
                case "6":
                    str += "Innowi Internal Error";
                    break;
                case "7":
                    str += "Inalid Amount";
                    break;
                case "8":
                    str += "Transaction Timeout";
                    break;
                case "9":
                    str += "Transaction Cancelled";
                    break;
                case "10":
                    str += "Processor not selected";
                    break;
                case "11":
                    str += "Partial Auth"; // Guessing based on Shift4 Response Code of "P"
                    break;
                case "12":
                    str += "Invalid Parameter Value";
                    break;
                case "13":
                    str += "Need manual confirm";
                    break;
                case "14":
                    str += "Service Error";
                    break;
                case "15":
                    str += "Offline not supported";
                    break;
                default:
                    str += "Unknown Status Error from Innowi";
                    break;
            }
            return str;
        }
    }
}
