using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBAccount
    {
        public class AccountResponse
        {
            public Account Account { get; set; }
            public DateTime Time { get; set; }
        }

        public class Account
        {
            public string Name { get; set; }
            public bool SubAccount { get; set; }
            public string FullyQualifiedName { get; set; }
            public bool Active { get; set; }
            public string Classification { get; set; }
            public string AccountType { get; set; }
            public string AccountSubType { get; set; }
            public decimal CurrentBalance { get; set; }
            public decimal CurrentBalanceWithSubAccounts { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
        }

        public class CurrencyRef
        {
            public string value { get; set; }
            public string name { get; set; }
        }

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }
    }
}