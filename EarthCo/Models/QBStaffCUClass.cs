using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBStaffCUClass
    {
        public class QBStaffClass
        {
            public string SSN { get; set; }
            public string SyncToken { get; set; }
            public string Id { get; set; }
            public PrimaryAddr PrimaryAddr { get; set; }
            public string GivenName { get; set; }
            public string FamilyName { get; set; }
            public string Title { get; set; }
            public PrimaryPhone PrimaryPhone { get; set; }
        }

        public class PrimaryAddr
        {
            public string Id { get; set; }
            public string Line1 { get; set; }
            public string City { get; set; }
            public string CountrySubDivisionCode { get; set; }
            public string PostalCode { get; set; }
        }

        public class PrimaryPhone
        {
            public string FreeFormNumber { get; set; }
        }
    }
}