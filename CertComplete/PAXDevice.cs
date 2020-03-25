using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;

namespace CertComplete
{
    public class PAXDevice
    {
        POSLink.PosLink paxDevice = new POSLink.PosLink();
        POSLink.ProcessTransResult result = new POSLink.ProcessTransResult();
        POSLink.CommSetting com = new POSLink.CommSetting();
        POSLink.PaymentRequest request = new POSLink.PaymentRequest();
        POSLink.ManageRequest manage = new POSLink.ManageRequest();

        /// <summary>
        /// Default Constructor
        /// </summary>
        public PAXDevice()
        {
            // Intentionally left blank.
        }

        /// <summary>
        /// Initializes the device using stored config file.
        /// </summary>
        /// <param name="DeviceConfigFilePath">File path of the device configuration file</param>
        /// <returns>True if no errors configuring device, False otherwise.</returns>
        public bool initializeDevice(string DeviceConfigFilePath)
        {
            bool initailized = false;
            try
            {
                // Read in config file
                string settingsString = System.IO.File.ReadAllText(DeviceConfigFilePath);

                // Parse out as JSON components
                Newtonsoft.Json.Linq.JObject settings = Newtonsoft.Json.Linq.JObject.Parse(settingsString);
                Newtonsoft.Json.Linq.JToken destinationIP = settings.GetValue("DestinationIP");
                Newtonsoft.Json.Linq.JToken destinationPort = settings.GetValue("DestinationPort");
                Newtonsoft.Json.Linq.JToken serialPort = settings.GetValue("SerialPort");
                Newtonsoft.Json.Linq.JToken baudRate = settings.GetValue("BaudRate");
                Newtonsoft.Json.Linq.JToken commType = settings.GetValue("CommType");
                Newtonsoft.Json.Linq.JToken timeOut = settings.GetValue("TimeOut");

                // Set Device Parameters
                setComSettings(destinationIP.ToString(), destinationPort.ToString(), serialPort.ToString(), baudRate.ToString(), commType.ToString(), timeOut.ToString());
                initailized = true;
            }
            catch (Exception e)
            {
                initailized = false;
                Console.WriteLine(e.StackTrace);
            }
            return initailized;
        }

        /// <summary>
        /// Processes a transaction with the given data supplied by the JSON component.
        /// </summary>
        /// <param name="transactionData">A JSON object containing the parameters to be sent in the transaction</param>
        /// <returns>A string with the assembled response from Shift4 and the device.</returns>
        public async Task<string> processTransaction(Newtonsoft.Json.Linq.JToken transactionData)
        {
            string resultString = "";
            // 1.5 Parse out JSON
            string[] transactionDetails = parseTransactionDetails(transactionData);

            // 2. Set the Payment/Management Request Settings
            setRequestType(transactionDetails[0].ToUpper(), transactionDetails[1].ToUpper());

            if (transactionDetails[1].ToUpper() == "MANAGE")
            {
                // Do this flow for a management request
                // 3. Set the management Variables
                manage.VarName = transactionDetails[20];
                manage.VarValue = transactionDetails[21];

                // 4. Execute the request
                result = executeManagement(manage);

                // 5. Handle Response
                resultString = handleResponse(result);
            }
            else
            {
                // 3. Set the PayLink Settings
                setPayLinkSettings(transactionDetails[2],
                                    "3",
                                    transactionDetails[5],
                                    transactionDetails[6],
                                    transactionDetails[7],
                                    transactionDetails[4],
                                    transactionDetails[3],
                                    transactionDetails[8],
                                    transactionDetails[15],
                                    transactionDetails[16],
                                    transactionDetails[19]
                                  );

                // 3.5 Set ExtData <XML/>
                setExtData(transactionDetails[12], transactionDetails[13], transactionDetails[9], transactionDetails[10], transactionDetails[11], transactionDetails[14]);

                // 4. Execute the Payment
                result = executeTransaction(request);

                // 5. Handle Response
                resultString = handleResponse(result);

            }
            return resultString;
        }

        /// <summary>
        /// Gets the OrigRefNum used for sending subsequent data.
        /// </summary>
        /// <returns>The original reference number if available, 0 otherwise.</returns>
        public int getOriginalReferenceNumber()
        {
            try
            {
                return Int32.Parse(paxDevice.PaymentResponse.RefNum);
            }
            catch (Exception e)
            {
                return 0;
            }
        }

        // Consider Refactoring this or part of it out.
        /// <summary>
        /// Builds and formats a receipt based on the current response in memory.
        /// </summary>
        /// <returns>A formatted receipt</returns>
        public async Task<string> printEMVReceipt()
        {
            string receiptText = formatTransactionData();
            receiptText += Environment.NewLine + formatEMVData();

            ReceiptPrinter rp = new ReceiptPrinter();
            rp.setTransactionData(receiptText);
            return rp.ToString();
        }

        /// <summary>
        /// Builds and formats a receipt based on the current response in memory.
        /// </summary>
        /// <returns>A formatted receipt</returns>
        public async Task<string> printEMVReceiptNoSig()
        {
            string receiptText = formatTransactionData();
            receiptText += Environment.NewLine + formatEMVData();

            ReceiptPrinter rp = new ReceiptPrinter();
            rp.setTransactionData(receiptText);
            return rp.ToStringNoSig();
        }

        /// <summary>
        /// Formats the transaction data from the response.
        /// </summary>
        /// <returns>A string representation of the transaction data formatted 40 chars wide.</returns>
        private string formatTransactionData()
        {
            string receiptText = "";
            int TransType = request.TransType;
            int TenderType = request.TenderType;

            receiptText += centerText(parseTransType(TransType), 40) + Environment.NewLine;
            receiptText += lineSpace("Date / Time", formatDateTime(paxDevice.PaymentResponse.Timestamp), 40) + Environment.NewLine;
            receiptText += lineSpace("Tender", parseTenderType(TenderType), 40) + Environment.NewLine;
            receiptText += lineSpace("Response", paxDevice.PaymentResponse.ResultTxt, 40) + Environment.NewLine;
            receiptText += lineSpace("Auth Code", paxDevice.PaymentResponse.AuthCode, 40) + Environment.NewLine;
            receiptText += lineSpace("Invoice", paxDevice.PaymentResponse.HostCode, 40) + Environment.NewLine;
            receiptText += lineSpace("Entry Method", parseEntryMethod(paxDevice.PaymentResponse.ExtData), 40) + Environment.NewLine;
            receiptText += lineSpace("Card Type", paxDevice.PaymentResponse.CardType, 40) + Environment.NewLine;
            receiptText += lineSpace("Card #", "XXXX XXXX XXXX " + paxDevice.PaymentResponse.BogusAccountNum, 40) + Environment.NewLine;
            bool needsTotalLine = false;
            string tax = parseEXTValue(paxDevice.PaymentResponse.ExtData, "TaxAmount");
            string tip = parseEXTValue(paxDevice.PaymentResponse.ExtData, "TipAmount");
            string cashBack = parseEXTValue(paxDevice.PaymentResponse.ExtData, "CashBackAmount");
            string surcharge = parseEXTValue(paxDevice.PaymentResponse.ExtData, "MerchantFee");
            string ta = "", ti = "", cash = "", sur = "";
            
            if(tax != null && tax != "" && tax != "0")
            {
                ta += lineSpace("Tax", formatAmount(tax), 40) + Environment.NewLine;
                needsTotalLine = true;
            }           
            if (tip != null && tip != "" && tip != "0")
            {
                ti += lineSpace("Tip", formatAmount(tip), 40) + Environment.NewLine;
                needsTotalLine = true;
            }            
            if (cashBack != null && cashBack != "" && cashBack != "0")
            {
                cash += lineSpace("Cash Back", formatAmount(cashBack), 40) + Environment.NewLine;
                needsTotalLine = true;
            }            
            if (surcharge != null && surcharge != "" && surcharge != "0")
            {
                sur += lineSpace("Surcharge", formatAmount(surcharge), 40) + Environment.NewLine;
                needsTotalLine = true;
            }
            if (needsTotalLine)
            {
                string total = calcSubTotal(paxDevice.PaymentResponse.ApprovedAmount, tax, tip, cashBack, surcharge);
                receiptText += lineSpace("Subtotal", formatAmount(total.ToString()), 40) + Environment.NewLine;
            }
            receiptText += ta + ti + cash + sur;
            string amtString = (paxDevice.PaymentResponse.ApprovedAmount == "") ? request.Amount : paxDevice.PaymentResponse.ApprovedAmount;
            receiptText += lineSpace("Total", "USD$ " + formatAmount(amtString), 40) + Environment.NewLine;
            return receiptText;
        }

        /// <summary>
        /// Formats the EMV data from the response.
        /// </summary>
        /// <returns>A string representation of the EMV data formatted 40 chars wide.</returns>
        private string formatEMVData()
        {
            // Format EMV Data
            string xmlExtData = paxDevice.PaymentResponse.ExtData;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml("<xml>" + xmlExtData + "</xml>");

            // Tags below applicable for EMV Contact/Contactless transactions only
            XmlNode TC = xDoc.SelectSingleNode("xml/TC");
            XmlNode TVR = xDoc.SelectSingleNode("xml/TVR");
            XmlNode AID = xDoc.SelectSingleNode("xml/AID");
            XmlNode TSI = xDoc.SelectSingleNode("xml/TSI");
            XmlNode ATC = xDoc.SelectSingleNode("xml/ATC");
            XmlNode APPLAB = xDoc.SelectSingleNode("xml/APPLAB");
            XmlNode APPN = xDoc.SelectSingleNode("xml/APPN");
            XmlNode IAD = xDoc.SelectSingleNode("xml/IAD");
            XmlNode ARC = xDoc.SelectSingleNode("xml/ARC");
            XmlNode CID = xDoc.SelectSingleNode("xml/CID");
            XmlNode CVM = xDoc.SelectSingleNode("xml/CVM");
            // Tags below applicable for FAILED EMV
            XmlNode AC = xDoc.SelectSingleNode("xml/AC");
            XmlNode AIP = xDoc.SelectSingleNode("xml/AIP");
            XmlNode AVN = xDoc.SelectSingleNode("xml/AVN");
            XmlNode IAUTHD = xDoc.SelectSingleNode("xml/IAUTHD");
            XmlNode CDOL2 = xDoc.SelectSingleNode("xml/CDOL2");
            XmlNode HRED = xDoc.SelectSingleNode("xml/HRED");

            string receiptText = centerText("EMV DETAILS", 40) + Environment.NewLine;
            // MUST BE IN ORDER FOR TSYS
            if (AID != null) { receiptText += lineSpace("AID", AID.InnerText, 40) + Environment.NewLine; }
            if (TVR != null) { receiptText += lineSpace("TVR", TVR.InnerText, 40) + Environment.NewLine; }
            if (IAD != null) { receiptText += lineSpace("IAD", IAD.InnerText, 40) + Environment.NewLine; }
            if (TSI != null) { receiptText += lineSpace("TSI", TSI.InnerText, 40) + Environment.NewLine; }
            if (ARC != null) { receiptText += lineSpace("ARC", ARC.InnerText, 40) + Environment.NewLine; }

            //if (TC != null) { receiptText += lineSpace("TC", TC.InnerText, 40) + Environment.NewLine; } 
            //if (ATC != null) { receiptText += lineSpace("ATC", ATC.InnerText, 40) + Environment.NewLine; }


            //if (APPLAB != null) { receiptText += lineSpace("APPLAB", APPLAB.InnerText, 40) + Environment.NewLine; }
            if (APPLAB != null) { receiptText += lineSpace("APPLAB", "", 40) + Environment.NewLine; }


            if (APPN != null) { receiptText += lineSpace("APPN", APPN.InnerText, 40) + Environment.NewLine; }
            //if (CID != null) { receiptText += lineSpace("CID", TC.InnerText, 40) + Environment.NewLine; }
            if (CVM != null) { receiptText += lineSpace("CVM", parseCVM(CVM.InnerText), 40) + Environment.NewLine; }
            if (AC != null) { receiptText += lineSpace("AC", AC.InnerText, 40) + Environment.NewLine; }
            if (AIP != null) { receiptText += lineSpace("AIP", AIP.InnerText, 40) + Environment.NewLine; }
            if (AVN != null) { receiptText += lineSpace("AVN", AVN.InnerText, 40) + Environment.NewLine; }
            if (IAUTHD != null) { receiptText += lineSpace("IAUTHD", IAUTHD.InnerText, 40) + Environment.NewLine; }
            if (CDOL2 != null) { receiptText += lineSpace("CDOL2", CDOL2.InnerText, 40) + Environment.NewLine; }
            if (HRED != null) { receiptText += lineSpace("HRED", HRED.InnerText, 40) + Environment.NewLine; }

            // AMEX hack for receipts
            receiptText += lineSpace("TID", "001", 40) + Environment.NewLine;

            return receiptText;
        }
        
        // Consider refactoring this out
        /// <summary>
        /// Builds a receipt line item.
        /// </summary>
        /// <param name="Name">Left aligned Key in a key value pair to be printed.</param>
        /// <param name="Value">Right aligned Value in a key value pair to be printed</param>
        /// <param name="Width">Total line width of the receipt in chars.</param>
        /// <returns>A string representation of "Name     Value" of a given width.</returns>
        private string lineSpace(string Name, string Value, int Width)
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
        /// Centers text in a line.
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
        /// Parses the transaction type out of PAX API into human readable value.
        /// </summary>
        /// <param name="TransType">Integer value representing a transaction type</param>
        /// <returns>A human readable transaction type.</returns>
        private string parseTransType(int TransType)
        {
            string ret = "";
            switch (TransType)
            {
                case 0:
                    ret = "UNKNOWN";
                    break;
                case 1:
                    ret = "AUTH";
                    break;
                case 2:
                    ret = "SALE";
                    break;
                case 3:
                    ret = "RETURN";
                    break;
                case 4:
                    ret = "VOID";
                    break;
                case 5:
                    ret = "SALE";           // PAX POSTAUTH
                    break;
                case 6:
                    ret = "AUTH";           // PAG FORCEAUTH
                    break;
                case 7:
                    ret = "CAPTURE";
                    break;
                case 15:
                    ret = "VOID";
                    break;
                case 16:
                    ret = "VOID";
                    break;
                case 17:
                    ret = "VOID";
                    break;
                case 18:
                    ret = "VOID";
                    break;
                case 19:
                    ret = "VOID";
                    break;
                case 20:
                    ret = "VOID";
                    break;
                case 41:
                    ret = "INCREMENTAL AUTH";
                    break;
                default:
                    ret = TransType.ToString();
                    break;
            }

            return ret;
        }

        /// <summary>
        /// Parses the tender type out of the PAX API into a human readable value.
        /// </summary>
        /// <param name="TenderType">Integer value representing a tender type</param>
        /// <returns>A human readable tender type.</returns>
        private string parseTenderType(int TenderType)
        {
            string res = "";
            switch (TenderType)
            {
                case 0:
                    res = "ALL";
                    break;
                case 1:
                    res = "CREDIT";
                    break;
                case 2:
                    res = "DEBIT";
                    break;
                default:
                    res = "Unknown";
                    break;
            }
            return res;
        }

        /// <summary>
        /// Formats the timestamp from the device for receipt printing. If no timestamp exists
        /// then todays date and time will be used.
        /// </summary>
        /// <param name="DateTime">DateTime string in PAX format</param>
        /// <returns>A formated DateTime string for receipt printing</returns>
        private string formatDateTime(string DateTime)
        {
            if (DateTime != null && DateTime != "")
            {
                char[] stamp = DateTime.ToCharArray();
                string ret = stamp[4].ToString() + stamp[5].ToString() + "/" + stamp[6].ToString() + stamp[7].ToString() + "/";
                ret += stamp[0].ToString() + stamp[1].ToString() + stamp[2].ToString() + stamp[3].ToString() + " ";
                ret += stamp[8].ToString() + stamp[9].ToString() + ":" + stamp[10].ToString() + stamp[11].ToString() + ":" + stamp[12].ToString() + stamp[13].ToString();
                return ret;
            }
            else
            {
                DateTime localDate = System.DateTime.Now;
                var culture = new CultureInfo("en-US");
                string ret = localDate.ToString(culture);
                string ret2 = localDate.ToString(culture);
                return ret;
            }
        }

        /// <summary>
        /// Parses out the Entry Method returned from the PAX API for a given transaction request.
        /// </summary>
        /// <param name="ExtData">XML style data block returned from transaction result.</param>
        /// <returns>A string representation of the Entry Method for the transaction.</returns>
        private string parseEntryMethod(string ExtData)
        {
            string ret = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml("<xml>" + ExtData + "</xml>");
            try
            {
                XmlNode entryMode = xDoc.SelectSingleNode("xml/PLEntryMode");
                switch (Int32.Parse(entryMode.InnerText))
                {
                    case 0:
                        ret = "Manual";
                        break;
                    case 1:
                        ret = "Swiped";
                        break;
                    case 2:
                        ret = "Contactless";
                        break;
                    case 3:
                        ret = "Scanner";
                        break;
                    case 4:
                        ret = "Chip";
                        break;
                    case 5:
                        ret = "Fallback Swipe";
                        break;
                    default:
                        ret = "Unknown";
                        break;
                }
            } catch (Exception e)
            {
                ret = "Card On File";
            }

            return ret;
        }

        /// <summary>
        /// Formats the amount to be a standard schema namely X.XX
        /// </summary>
        /// <param name="amount">The amount to be formatted.</param>
        /// <returns>A string representation of the Amount.</returns>
        private string formatAmount(string amount)
        {
            try
            {
                double dd = Double.Parse(amount);
                dd = dd / 100.00;
                return string.Format("{0:N2}", dd);
            }catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Parses out a given value from EXT Data in the transaction response.
        /// </summary>
        /// <param name="EXTData">The whole EXTData block returned from a transaction.</param>
        /// <param name="value">The value to parse out.</param>
        /// <returns></returns>
        private string parseEXTValue(string EXTData, string value)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml("<xml>" + EXTData + "</xml>");
            try
            {
                XmlNode node = xDoc.SelectSingleNode("xml/" + value);
                return node.InnerText;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Calculates the Subtotal if on exists for a given transaction.
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
        /// Parses out the CVM value into a human readable form.
        /// </summary>
        /// <param name="CVM">The CVM value</param>
        /// <returns>A human readable form for CVM.</returns>
        private string parseCVM(string CVM)
        {
            string ret = "NONE";
            try
            {
                int val = Int32.Parse(CVM);
                switch (val)
                {
                    case 0:
                        ret = "Fail CVM processing";
                        break;
                    case 1:
                        ret = "PIN VERIFIED";
                        break;
                    case 2:
                        ret = "PIN VERIFIED";
                        break;
                    case 3:
                        ret = "PIN VERIFIED SIGN";
                        break;
                    case 4:
                        ret = "PIN VERFIED";
                        break;
                    case 5:
                        ret = "PIN VERIFIED SIGN";
                        break;
                    case 6:
                        ret = "SIGN";
                        break;
                    case 7:
                        ret = "NONE";
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Parsing Integer" + e.StackTrace);
            }
            return ret;
        }

        /// <summary>
        /// 1. Set the Connection Settings for talking to the device
        /// </summary>
        /// <param name="DestinationIP">The IP Address of the PAX Device</param>
        /// <param name="DestinationPort">The Port the device is listening on</param>
        /// <param name="SerialPort">The serial port of the device</param>
        /// <param name="BaudRate">The Baud Rate</param>
        /// <param name="CommunicationType">The Communication type</param>
        /// <param name="TimeOut">The amount of time to wait before a timeout occurs</param>
        private void setComSettings(string DestinationIP, string DestinationPort, string SerialPort, string BaudRate, string CommunicationType, string TimeOut)
        {
            // Set Defaults
            com.DestIP = "10.0.33.102";
            com.DestPort = "10009";
            com.SerialPort = "COM1";
            com.BaudRate = "9600";
            com.CommType = "TCP";
            com.TimeOut = "-1";                                 // Timeout ms

            // Try to set custom settings
            if (DestinationIP != null)
                com.DestIP = DestinationIP;
            if (DestinationPort != null)
                com.DestPort = DestinationPort;
            if (SerialPort != null)
                com.SerialPort = SerialPort;
            if (BaudRate != null)
                com.BaudRate = BaudRate;
            if (CommunicationType != null)
                com.CommType = CommunicationType;
            if (TimeOut != null)
                com.TimeOut = TimeOut;

            // Apply the settings to the POSLink
            paxDevice.CommSetting = com;
        }

        /// <summary>
        /// 2. Set the Payment Request Settings
        /// </summary>
        /// <param name="TenderType">The Tender Type</param>
        /// <param name="TransactionType">The Transactino Type</param>
        private void setRequestType(string TenderType, string TransactionType)
        {
            // Set Default Values
            request.TenderType = request.ParseTenderType("CREDIT");
            request.TransType = request.ParseTransType("SALE");

            // Try to set custom settings
            if (TenderType != null && TenderType != "")
                request.TenderType = request.ParseTenderType(TenderType);
            if (TransactionType != null && TransactionType != "")
                request.TransType = request.ParseTransType(TransactionType);

            // Set the management request value instead. We will discard the request object later (I think)
            if (TransactionType == "MANAGE") {
                manage.TransType = manage.ParseTransType("SETVAR");
                manage.EDCType = manage.ParseEDCType(TenderType);
                    
            }
        }

        /// <summary>
        /// 3. Set the PayLink Settings
        /// </summary>
        /// <param name="Amount">The Total Amount of the Transaction</param>
        /// <param name="ECRRefNum">The Reference Number form the Interface</param>
        /// <param name="Clerk">The Clerk Number</param>
        /// <param name="Invoice">The Invoice Number</param>
        /// <param name="Street">The Billing Address</param>
        /// <param name="Tax">The Tax Amount</param>
        /// <param name="Tip">The Tip Amount</param>
        /// <param name="Zip">The Billing Zip Code</param>
        /// <param name="Surcharge">The Surcharge Amount</param>
        /// <param name="CashBack">The Cashback Amount</param>
        /// <param name="OrigRefNum">The Original transaction number from the device.</param>
        private void setPayLinkSettings(string Amount, string ECRRefNum, string Clerk, string Invoice, string Street, string Tax, string Tip, string Zip, string Surcharge, string CashBack, string OrigRefNum)
        {
            /* REQUIRED */

            request.Amount = decimalRemover(Amount);
            request.ECRRefNum = ECRRefNum;                                                                      // ECRRefNum is used to tell the device to perform a separate transaction
                                                                                                                // by passing a different int value. Handle in caller logic is a better choice.
            // OPTIONAL
            request.ClerkID = Clerk;                                                                            // Clerk                    -> Shift4 Clerk
            //request.CustomerName = "Sterling Archer";                                                         // CustomerName             -> Set in ExtData -> Why?
            //request.DestinationZipCode = "78000";                                                             // DestinationZipCode       -> Set in ExtData -> Why?
            //request.InvNum = Invoice;                                                                         // Invoice                  -> Unknown - Not Shift4 Invoice Number
            request.ECRTransID = Invoice;                                                                       // Invoice                  -> Shift4 Invoice Number
            //request.ProductDescription = "Toys";                                                              // Product DescriptorN      -> Unknown - Set in ExtData -> Why?
            request.Street = Street;                                                                            // Billing Address          -> Shift4 StreetAddress
            request.TaxAmt = decimalRemover(Tax);                                                               // TaxAmount                -> Shift4 TaxAmount - Included in Amount
            request.TipAmt = decimalRemover(Tip);                                                               // TipAmount                -> Shift4 SecondaryAmount - Not Included in Amount, Not Allowed in Auth? Why?
            request.Zip = Zip;                                                                                  // Billing Zip Code         -> Shift4 ZipCode
            request.OrigRefNum = OrigRefNum;                                                                    // Used to tell the device what previous transaction this is related too.
            //request.AuthCode = "123456";                                                                      // Check for where allowed.
            request.SurchargeAmt = decimalRemover(Surcharge);                                                   // SurchargeAmt             -> Shift4 Surcharge
            request.CashBackAmt = decimalRemover(CashBack);                                                     // CashBackAmt              -> Shift4 Cashback
        }

        /// <summary>
        /// 3.5 Set ExtData <XML/>
        /// </summary>
        /// <param name="CustomerCode">The Customer Reference</param>
        /// <param name="ProductDescription">The Product Description</param>
        /// <param name="DestinationZipCode">The Destination Zip Code</param>
        /// <param name="FirstName">Customer First Name</param>
        /// <param name="LastName">Customer Last Name</param>
        /// <param name="Token">The Shift4 Token</param>
        private void setExtData(string CustomerCode, string ProductDescription, string DestinationZipCode, string FirstName, string LastName, string Token)
        {
            if (FirstName != null && FirstName != "")                                                           // FirstName
                request.ExtData += "<FirstName>" + FirstName + "</FirstName>";                                  //          \            
            if (LastName != null && LastName != "")                                                             //           +  -->         -> Shift4 CustomerName
                request.ExtData += "<LastName>" + LastName + "</LastName>";                                     //          /
                                                                                                                // Last name
            //request.ExtData += "<CashBack>210</CashBack>";
            if (CustomerCode != null && CustomerCode != "")                                                     
                request.ExtData += "<CustomerCode>" + CustomerCode + "</CustomerCode>";                         // CustomerCode             -> Shift4 CustomerReference
            if (Token != null && Token != "")
                request.ExtData += "<Token>" + Token + "</Token>";                                              // Token                    -> Shift4 UniqueID
            if (DestinationZipCode != null && DestinationZipCode != "")
                request.ExtData += "<DestinationZipCode>" + DestinationZipCode + "</DestinationZipCode>";       // DestinationZipCode       -> Shift4 DestinationZipCode
            if (ProductDescription != null && ProductDescription != "")
                request.ExtData += "<ProductDescription>" + ProductDescription + "</ProductDescription>";       // ProductDescription       -> Shift4 ProductDescriptor1
            // ^^ There is a known limitation on Product Descriptors right now, limited to ProductDescriptor1 in the Shift4 API as of 1/22/19

            //request.ExtData += "<MM_ID>" + MerchantID + "</MM_ID>";    Used to select which merchant AccessToken to send, configured in PAX Store web portal.
            //request.ExtData += "<MM_Name> + MerchantName + </MM_Name>"; Used to select the Merchant AccessToken by name to send, config ^^   ^^    ^^   ^^ .

            /* Still Unknown handlings
             * APIOptions -> Will probably need this at some point.
             * MetaToken Support -> if so, how?
             * Notes -> How are notes passed? Any Mismatched restrictions?
             * OverrideBusDate -> Possible? Known Use cases?
             * TokenSerialNumber -> Can this be set with MM_ID?
             * TaxIndicator -> No Direct Correlation - Maybe OK
             * HSA/FSA Support -> if so, how?
             * All of Signature Features
             * Need to Verify CardPresent Situations
             * 
             * All of Hotel - Not certified
             * All of Auto
             */
        }

        /// <summary>
        /// 4. Execute the Transaction
        /// </summary>
        /// <param name="request">A populated Request Object</param>
        /// <returns>A transaction result object.</returns>
        private POSLink.ProcessTransResult executeTransaction(POSLink.PaymentRequest request)
        {
            paxDevice.PaymentRequest = request;
            return paxDevice.ProcessTrans();
        }

        /// <summary>
        /// 4. Execute the management transaction
        /// </summary>
        /// <param name="request">A populated management request</param>
        /// <returns>A transaction result object.</returns>
        private POSLink.ProcessTransResult executeManagement(POSLink.ManageRequest request)
        {
            paxDevice.ManageRequest = request;
            return paxDevice.ProcessTrans();
        }
        /// <summary>
        /// 5. Handle Response
        /// </summary>
        /// <param name="result">The Transaction result</param>
        /// <returns>A string representation of the response object.</returns>
        private string handleResponse(POSLink.ProcessTransResult result)
        {
            string output = "";
            if (result.Code == POSLink.ProcessTransResultCode.OK)
            {
                POSLink.PaymentResponse res = paxDevice.PaymentResponse;
                POSLink.ManageResponse mRes = paxDevice.ManageResponse;

                if (res != null && res.ResultCode != null)
                {
                    output += "Approved Amount: " + res.ApprovedAmount + Environment.NewLine;
                    output += "Auth Code: " + res.AuthCode + Environment.NewLine;
                    output += "AVS Response: " + res.AvsResponse + Environment.NewLine;
                    output += "Bogus Account Number: " + res.BogusAccountNum + Environment.NewLine;
                    output += "Card Type: " + res.CardType + Environment.NewLine;
                    output += "CVV Response: " + res.CvResponse + Environment.NewLine;
                    output += "EXT Data: " + res.ExtData + Environment.NewLine;
                    output += "Extra Balance: " + res.ExtraBalance + Environment.NewLine;
                    output += "Invoice: " + res.HostCode + Environment.NewLine;             // HostCode = Shift4 Invoice Number
                    output += "Host Response: " + res.HostResponse + Environment.NewLine;
                    output += "Message: " + res.Message + Environment.NewLine;
                    output += "Raw Response: " + res.RawResponse + Environment.NewLine;
                    output += "Reference Number: " + res.RefNum + Environment.NewLine;
                    output += "Remaining Balance: " + res.RemainingBalance + Environment.NewLine;
                    output += "Requested Amount: " + res.RequestedAmount + Environment.NewLine;
                    output += "Result Code: " + res.ResultCode + Environment.NewLine;
                    output += "Result Text: " + res.ResultTxt + Environment.NewLine;
                    output += "Sig File Name: " + res.SigFileName + Environment.NewLine;
                    output += "Time Stamp: " + res.Timestamp + Environment.NewLine;
                }
                if (mRes != null)
                {
                    // Do something here to handle the management response.
                    //output += "Approved Amount: 0.00"  + Environment.NewLine;
                    //output += "Auth Code: " + mRes.ResultTxt + Environment.NewLine;
                    //output += "AVS Response: " + res.AvsResponse + Environment.NewLine;
                    //output += "Bogus Account Number: " + res.BogusAccountNum + Environment.NewLine;
                    //output += "Card Type: " + res.CardType + Environment.NewLine;
                    //output += "CVV Response: " + res.CvResponse + Environment.NewLine;
                    //output += "EXT Data: " + res.ExtData + Environment.NewLine;
                    //output += "Extra Balance: " + res.ExtraBalance + Environment.NewLine;
                    //output += "Invoice: " + res.HostCode + Environment.NewLine;             // HostCode = Shift4 Invoice Number
                    if (mRes.ResultTxt == "OK")
                      output += "Host Response: A" + Environment.NewLine;
                    //output += "Message: " + res.Message + Environment.NewLine;
                    //output += "Raw Response: " + res.RawResponse + Environment.NewLine;
                    //output += "Reference Number: " + res.RefNum + Environment.NewLine;
                    //output += "Remaining Balance: " + res.RemainingBalance + Environment.NewLine;
                    //output += "Requested Amount: " + res.RequestedAmount + Environment.NewLine;
                    output += "Result Code: " + mRes.ResultCode + Environment.NewLine;
                    output += "Result Text: " + mRes.ResultTxt + Environment.NewLine;
                    //output += "Sig File Name: " + res.SigFileName + Environment.NewLine;
                    //output += "Time Stamp: " + res.Timestamp + Environment.NewLine;
                }
            }
            else if (result.Code == POSLink.ProcessTransResultCode.ERROR)
            {
                Console.WriteLine("Ruh Roh! Something went wrong, the PAX device returned an error.");
                output += "Ruh Roh! Something went wrong, the PAX device returned an error.";
            }
            else if (result.Code == POSLink.ProcessTransResultCode.TimeOut)
            {
                Console.WriteLine("Timeout: I dunno Scoob, it looks like they ran out of ScoobySnacks before we could get here.");
                output += "Timeout: I dunno Scoob, it looks like they ran out of ScoobySnacks before we could get here.";
            }

            return output;
        }

        /// <summary>
        /// Removes decimal points from a string.
        /// </summary>
        /// <param name="Amount">The amount string to have decimal points removed from.</param>
        /// <returns>A string with no decimal points.</returns>
        private string decimalRemover(string Amount)
        {
            string newAmount = "";
            if (Amount != null && Amount != "")
            {
                if (Amount.Contains("."))
                {
                    double d = Double.Parse(Amount);
                    double dd = d * 100;
                    int Amt = (int)Math.Round(dd);
                    newAmount = Amt.ToString();
                }
                else
                {
                    newAmount = (Int32.Parse(Amount) * 100).ToString();
                }
            }
            return newAmount;
        }

        /// <summary>
        /// Builds a data array with transaction data from the JSON request.
        /// </summary>
        /// <param name="transactionData">JSON representation of a request.</param>
        /// <returns>A string array with values pulled from the JSON.</returns>
        private string[] parseTransactionDetails(Newtonsoft.Json.Linq.JToken transactionData)
        {
            string[] values = new string[22];
            Newtonsoft.Json.Linq.JToken tenderType = transactionData.SelectToken("CardType", false);
            Newtonsoft.Json.Linq.JToken transactionType = transactionData.SelectToken("TransactionType", false);
            Newtonsoft.Json.Linq.JToken primaryAmount = transactionData.SelectToken("PrimaryAmount", false);
            Newtonsoft.Json.Linq.JToken secondaryAmount = transactionData.SelectToken("SecondaryAmount", false);
            Newtonsoft.Json.Linq.JToken taxAmount = transactionData.SelectToken("TaxAmount", false);
            Newtonsoft.Json.Linq.JToken clerk = transactionData.SelectToken("Clerk", false);
            Newtonsoft.Json.Linq.JToken invoice = transactionData.SelectToken("Invoice", false);
            Newtonsoft.Json.Linq.JToken streetAddress = transactionData.SelectToken("StreetAddress", false);
            Newtonsoft.Json.Linq.JToken billingZipCode = transactionData.SelectToken("BillingZipCode", false);
            Newtonsoft.Json.Linq.JToken destinationZipCode = transactionData.SelectToken("DestinationZipCode", false);
            Newtonsoft.Json.Linq.JToken customerFirstName = transactionData.SelectToken("CustomerFirstName", false);
            Newtonsoft.Json.Linq.JToken customerLastName = transactionData.SelectToken("CustomerLastName", false);
            Newtonsoft.Json.Linq.JToken customerReference = transactionData.SelectToken("CustomerReference", false);
            Newtonsoft.Json.Linq.JToken productDescriptor = transactionData.SelectToken("ProductDescriptor", false);
            Newtonsoft.Json.Linq.JToken token = transactionData.SelectToken("Token", false);
            Newtonsoft.Json.Linq.JToken surcharge = transactionData.SelectToken("Surcharge", false);
            Newtonsoft.Json.Linq.JToken cashBack = transactionData.SelectToken("CashBack", false);
            Newtonsoft.Json.Linq.JToken origRefNum = transactionData.SelectToken("OrigRefNum", false);
            // Per PAX not currently supported as of 1/22/19 - left here in anticipation of future support.
            Newtonsoft.Json.Linq.JToken cardPresent = transactionData.SelectToken("CardPresent", false);
            Newtonsoft.Json.Linq.JToken cardEntryMode = transactionData.SelectToken("CardEntryMode", false);
            // Add support for sending management requests
            Newtonsoft.Json.Linq.JToken varName = transactionData.SelectToken("VarName", false);
            Newtonsoft.Json.Linq.JToken varValue = transactionData.SelectToken("VarValue", false);

            if (tenderType != null)
                values[0] = tenderType.ToString();
            if (transactionType != null)
                values[1] = transactionType.ToString();
            if (primaryAmount != null)
                values[2] = primaryAmount.ToString();
            if (secondaryAmount != null)
                values[3] = secondaryAmount.ToString();
            if (taxAmount != null)
                values[4] = taxAmount.ToString();
            if (clerk != null)
                values[5] = clerk.ToString();
            if (invoice != null)
                values[6] = invoice.ToString();
            if (streetAddress != null)
                values[7] = streetAddress.ToString();
            if (billingZipCode != null)
                values[8] = billingZipCode.ToString();
            if (destinationZipCode != null)
                values[9] = destinationZipCode.ToString();
            if (customerFirstName != null)
                values[10] = customerFirstName.ToString();
            if (customerLastName != null)
                values[11] = customerLastName.ToString();
            if (customerReference != null)
                values[12] = customerReference.ToString();
            if (productDescriptor != null)
                values[13] = productDescriptor.ToString();
            if (token != null)
                values[14] = token.ToString();
            if (surcharge != null)
                values[15] = surcharge.ToString();
            if (cashBack != null)
                values[16] = cashBack.ToString();
            if (cardPresent != null)
                values[17] = cardPresent.ToString();
            if (cardEntryMode != null)
                values[18] = cardEntryMode.ToString();
            if (origRefNum != null)
                values[19] = origRefNum.ToString();
            if (varName != null)
                values[20] = varName.ToString();
            if (varValue != null)
                values[21] = varValue.ToString();
            return values;
        }
    }
}
