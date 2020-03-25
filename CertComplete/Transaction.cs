using System;
using System.Linq;

namespace CertComplete
{
    public class Transaction
    {
        public string request;
        public string response;
        public string outputString;
        Newtonsoft.Json.Linq.JObject completeRequest;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Transaction()
        {
            request = null;
            response = null;
        }

        /// <summary>
        /// Alternate Constructor
        /// </summary>
        /// <param name="request">A string representation of the individual request.</param>
        public Transaction(string request)
        {
            this.request = request;
            this.response = null;
        }

        /// <summary>
        /// Alternate Constructor
        /// </summary>
        /// <param name="request">A string representation of the individual request.</param>
        /// <param name="response">A string representation of the response to the request.</param>
        public Transaction (string request, string response)
        {
            this.request = request;
            this.response = response;
        }

        /// <summary>
        /// Alternate Constructor
        /// </summary>
        /// <param name="request">The full transaction sequence.</param>
        /// <param name="subRequestNumber">The integer representation of the ordered transaction components.</param>
        /// <param name="response">The response from the transaction request.</param>
        public Transaction (string request, int subRequestNumber, string response)
        {
            this.request = request;
            this.response = response;
            try
            {
                completeRequest = Newtonsoft.Json.Linq.JObject.Parse(request);
                Newtonsoft.Json.Linq.JToken subRequest = completeRequest.GetValue("TestSequenceData");
                Newtonsoft.Json.Linq.JToken[] array = subRequest.ToArray();
                Newtonsoft.Json.Linq.JToken subRequestData = array[subRequestNumber];
                outputString = requestToString(subRequestData);
            }
            catch (Exception e)
            {

            }
        }

        /// <summary>
        /// Builds a string representation of a transaction for the Transaction History Box.
        /// </summary>
        /// <param name="req">The request to stringify.</param>
        /// <returns>A string representation of the transaction.</returns>
        public string requestToString(Newtonsoft.Json.Linq.JToken req)
        {
            string res;
            Newtonsoft.Json.Linq.JToken deviceType = completeRequest.SelectToken("DeviceType", true);
            if (deviceType.ToString() == "PAX S300")
            {
                res = PAX_Request_toString(req);
            }
            else
            {
                res = Innowi_Request_toString(req);
            }
            return res;
        }

        /// <summary>
        /// Overrides the default ToString() operation with custom output.
        /// <see cref="requestToString(Newtonsoft.Json.Linq.JToken)"/>
        /// </summary>
        /// <returns>String representation of the transaction.</returns>
        public override string ToString()
        {
            return outputString;
        }

        /// <summary>
        /// Translates a PAX request into a transaction history item.
        /// </summary>
        /// <param name="req">The transaction requst.</param>
        /// <returns>A string representation of a PAX transaction.</returns>
        private string PAX_Request_toString(Newtonsoft.Json.Linq.JToken req)
        {
            string str = "";
            Newtonsoft.Json.Linq.JToken testNumber = completeRequest.SelectToken("TestNumber", false);
            Newtonsoft.Json.Linq.JToken testName = completeRequest.SelectToken("TestName", false);
            Newtonsoft.Json.Linq.JToken transType = req.SelectToken("TransactionType", false);

            str += transType.ToString();
            str += ": ";
            str += testName.ToString();
            str += ": ";
            str += testNumber.ToString();
            str += " Response: ";
            int index = response.IndexOf("Host Response: ");
            str += response.Substring(index + 15, 1);

            return str;
        }

        private string Innowi_Request_toString(Newtonsoft.Json.Linq.JToken req)
        {
            string str = "";
            Newtonsoft.Json.Linq.JToken testNumber = completeRequest.SelectToken("TestNumber", false);
            Newtonsoft.Json.Linq.JToken testName = completeRequest.SelectToken("TestName", false);
            Newtonsoft.Json.Linq.JToken transType = req.SelectToken("TransactionType", false);

            str += transType.ToString();
            str += ": ";
            str += testName.ToString();
            str += ": ";
            str += testNumber.ToString();
            str += " Response: ";
            str += mapInnowiResponseToString();

            return str;
        }

        public string mapInnowiResponseToString()
        {
            string str = "";
            Newtonsoft.Json.Linq.JObject res = Newtonsoft.Json.Linq.JObject.Parse(response);
            Newtonsoft.Json.Linq.JToken o = res.SelectToken("Status", true);

            switch (o.ToString())
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
                    str += "Partial Authorization";
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
                    str += "Unknown Error";
                    break;
            }
            return str;
        }
    }
}