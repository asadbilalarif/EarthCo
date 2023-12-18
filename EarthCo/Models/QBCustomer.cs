using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBCustomer
    {
        public class CustomerResponse
        {
            public Customer Customer { get; set; }
            public DateTime Time { get; set; }
        }

        public class Customer
        {
            public bool Taxable { get; set; }
            public BillAddr BillAddr { get; set; }
            public string Notes { get; set; }
            public bool Job { get; set; }
            public bool BillWithParent { get; set; }
            public decimal Balance { get; set; }
            public decimal BalanceWithJobs { get; set; }
            public CurrencyRef CurrencyRef { get; set; }
            public string PreferredDeliveryMethod { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
            public string FullyQualifiedName { get; set; }
            public string CompanyName { get; set; }
            public string DisplayName { get; set; }
            public string PrintOnCheckName { get; set; }
            public bool Active { get; set; }
            public PrimaryPhone PrimaryPhone { get; set; }
            public PrimaryEmailAddr PrimaryEmailAddr { get; set; }
            public DefaultTaxCodeRef DefaultTaxCodeRef { get; set; }
        }

        public class BillAddr
        {
            public string Id { get; set; }
            public string Line1 { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string CountrySubDivisionCode { get; set; }
            public string PostalCode { get; set; }
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

        public class PrimaryPhone
        {
            public string FreeFormNumber { get; set; }
        }

        public class PrimaryEmailAddr
        {
            public string Address { get; set; }
        }

        public class DefaultTaxCodeRef
        {
            public string value { get; set; }
        }
    }
}