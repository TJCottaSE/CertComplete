using PaymentSDKNET.ChecoutServiceReference;
using System.Windows.Shapes;
using System;

namespace CertComplete
{
    public class ReceiptPrinter
    {
        string ReceiptHeader;
        string ReceiptText;
        string ReceiptAgreement;
        string ReceiptSignature;
        string MerchantCopy;
        string CustomerCopy;
        string ReceiptFooter;
        

        /// <summary>
        /// Default Constructor that sets up some common receipt data.
        /// </summary>
        public ReceiptPrinter()
        {
            ReceiptHeader  = "           Shift4 CPS Retail            " + Environment.NewLine;
            ReceiptHeader += "       1491 Center Crossing Road        " + Environment.NewLine;
            ReceiptHeader += "          Las Vegas, NV 89144           " + Environment.NewLine;
            ReceiptHeader += "              702 597 2480              " + Environment.NewLine + Environment.NewLine;
            ReceiptText = "";
            ReceiptAgreement  = "   I AGREE TO PAY ABOVE TOTAL AMOUNT    " + Environment.NewLine;
            ReceiptAgreement += "ACCORDING TO THE CARD ISSUER AGREEMENT  " + Environment.NewLine;
            ReceiptAgreement += " (MERCHANT AGREEMENT IF CREDIT VOUCHER) " + Environment.NewLine + Environment.NewLine;
            ReceiptSignature += Environment.NewLine + Environment.NewLine + Environment.NewLine;
            ReceiptSignature += "________________________________________" + Environment.NewLine + Environment.NewLine;

            MerchantCopy += "             MERCHANT COPY              " + Environment.NewLine + Environment.NewLine;
            CustomerCopy += "             CUSTOMER COPY              " + Environment.NewLine + Environment.NewLine;
            ReceiptFooter += "No Refunds                              " + Environment.NewLine;
            ReceiptFooter += "Store Credit Only                       " + Environment.NewLine + Environment.NewLine;
            ReceiptFooter += "Thank You                               " + Environment.NewLine;
            ReceiptFooter += "Please Come Again                       ";

        }

        /// <summary>
        /// Sets the Receipt Header
        /// </summary>
        /// <param name="header">The header string.</param>
        public void setReceiptHeader(string header)
        {
            this.ReceiptHeader = header;
            var line1 = new Line();
        }

        /// <summary>
        /// Sets the Transaction data of the receipt.
        /// </summary>
        /// <param name="TransactionText">The transaction data.</param>
        public void setTransactionData(string TransactionText)
        {
            this.ReceiptText = TransactionText;
        }

        /// <summary>
        /// Sets the Base Receipt Text plus EMV data.
        /// </summary>
        /// <param name="BaseText">The base receipt text.</param>
        /// <param name="EMVText">The EMV data text.</param>
        public void setTransactionData(string BaseText, string EMVText)
        {
            this.ReceiptText = BaseText + Environment.NewLine + Environment.NewLine + EMVText;
        }

        /// <summary>
        /// Sets the Receipt Footer
        /// </summary>
        /// <param name="Footer">The footer text.</param>
        public void setReceiptFooter(string Footer)
        {
            this.ReceiptFooter = Footer;
        }

        /// <summary>
        /// Gets the Merchant receipt 
        /// </summary>
        /// <returns>A string representation of the Merchant Receipt</returns>
        public string getMerchantReceipt()
        {
            return ReceiptHeader + ReceiptText + Environment.NewLine + ReceiptAgreement + ReceiptSignature + MerchantCopy + ReceiptFooter;
        }

        /// <summary>
        /// Gets the Customer Receipt
        /// </summary>
        /// <returns>A string representation of the Customer receipt.</returns>
        public string getCustomerReceipt()
        {
            return ReceiptHeader + ReceiptText + Environment.NewLine + ReceiptAgreement + ReceiptSignature + CustomerCopy + ReceiptFooter;
        }

        /// <summary>
        /// Gets the Merchant receipt 
        /// </summary>
        /// <returns>A string representation of the Merchant Receipt</returns>
        public string getMerchantReceiptNoSig()
        {
            return ReceiptHeader + ReceiptText + Environment.NewLine + ReceiptAgreement + MerchantCopy + ReceiptFooter;
        }

        /// <summary>
        /// Gets the Customer Receipt
        /// </summary>
        /// <returns>A string representation of the Customer receipt.</returns>
        public string getCustomerReceiptNoSig()
        {
            return ReceiptHeader + ReceiptText + Environment.NewLine + ReceiptAgreement + CustomerCopy + ReceiptFooter;
        }

        /// <summary>
        /// Gets both the Merchant and Customer Receipts
        /// </summary>
        /// <returns>A string with both Merchant and Customer Receipts.</returns>
        public string getReceipts()
        {
            return getMerchantReceipt() + Environment.NewLine + Environment.NewLine + getCustomerReceipt();
        }

        /// <summary>
        /// Gets a no signature line receipt.
        /// </summary>
        /// <returns></returns>
        public string getReceiptsNoSig()
        {
            return getMerchantReceiptNoSig() + Environment.NewLine + Environment.NewLine + getCustomerReceiptNoSig();
        }

        /// <summary>
        /// Override the the ToString() method to return both Merchant and Customer Receipts.
        /// </summary>
        /// <returns>A string with both Merchant and Customer Receipts.</returns>
        public override string ToString()
        {
            return getReceipts();
        }

        /// <summary>
        /// No signature version of the receipts
        /// </summary>
        /// <returns>A receipt without a signature line.</returns>
        public string ToStringNoSig()
        {
            return getReceiptsNoSig();
        }
    }
}