using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class BillTest
    {
        public class AccountBasedExpenseLineDetail
        {
            public AccountRef AccountRef { get; set; }
            public string BillableStatus { get; set; }
            public TaxCodeRef TaxCodeRef { get; set; }
        }

        public class Bill
        {
            public string DueDate { get; set; }
            public double Balance { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
            public string TxnDate { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public string PrivateNote { get; set; }
            public List<LinkedTxn> LinkedTxn { get; set; }
            public List<Line> Line { get; set; }
            public VendorRef VendorRef { get; set; }
            public APAccountRef APAccountRef { get; set; }
            public double TotalAmt { get; set; }
        }

        public class AccountRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }

        public class CurrencyRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class LinkedTxn
        {
            public string TxnId { get; set; }
            public string TxnType { get; set; }
        }

        public class Line
        {
            public string Id { get; set; }
            public int LineNum { get; set; }
            public double Amount { get; set; }
            public string DetailType { get; set; }
            public AccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
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