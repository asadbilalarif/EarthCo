using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBVendor
    {
        public class VendorResponse
        {
            public Vendor Vendor { get; set; }
            public DateTime Time { get; set; }
        }

        public class Vendor
        {
            public decimal Balance { get; set; }
            public decimal BillRate { get; set; }
            public bool Vendor1099 { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
            public string Title { get; set; }
            public string GivenName { get; set; }
            public string MiddleName { get; set; }
            public string FamilyName { get; set; }
            public string CompanyName { get; set; }
            public string Suffix { get; set; }
            public string DisplayName { get; set; }
            public string PrintOnCheckName { get; set; }
            public bool Active { get; set; }
            public PrimaryPhone PrimaryPhone { get; set; }
            public PrimaryEmailAddr PrimaryEmailAddr { get; set; }
        }

        public class PrimaryPhone
        {
            public string FreeFormNumber { get; set; }
        }

        public class PrimaryEmailAddr
        {
            public string Address { get; set; }
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