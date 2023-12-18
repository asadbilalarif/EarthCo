using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBCustomerCUClass
    {
        public class QBCustomerClass
        {
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public BillAddr BillAddr { get; set; }
            public string Notes { get; set; }
            public string DisplayName { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
            public string CompanyName { get; set; }
            public PrimaryPhone PrimaryPhone { get; set; }
            public PrimaryEmailAddr PrimaryEmailAddr { get; set; }
        }

        public class BillAddr
        {
            public string Line1 { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string CountrySubDivisionCode { get; set; }
            public string PostalCode { get; set; }
        }

        public class PrimaryPhone
        {
            public string FreeFormNumber { get; set; }
        }

        public class PrimaryEmailAddr
        {
            public string Address { get; set; }
        }
    }
}