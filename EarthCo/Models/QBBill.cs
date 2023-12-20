using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBBill
    {
        public class BillResponse
        {
            public Bill Bill { get; set; }
            public DateTime Time { get; set; }
        }

        public class Bill
        {
            public DateTime DueDate { get; set; }
            public decimal Balance { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public string DocNumber { get; set; }
            public MetaData MetaData { get; set; }
            public DateTime TxnDate { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public List<Line> Line { get; set; }
            public VendorRef VendorRef { get; set; }
            public APAccountRef APAccountRef { get; set; }
            public decimal TotalAmt { get; set; }
        }

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }
        public class ItemRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }
        public class CurrencyRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class Line
        {
            public string Id { get; set; }
            public int LineNum { get; set; }
            public string Description { get; set; }
            public decimal Amount { get; set; }
            public string DetailType { get; set; }
            public ItemBasedExpenseLineDetail ItemBasedExpenseLineDetail { get; set; }
        }

        public class ItemBasedExpenseLineDetail
        {
            public string BillableStatus { get; set; }
            public ItemRef ItemRef { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal Qty { get; set; }
            public TaxCodeRef TaxCodeRef { get; set; }
        }


        public class TaxCodeRef
        {
            public string value { get; set; }
        }

        public class VendorRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class APAccountRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }
    }
}