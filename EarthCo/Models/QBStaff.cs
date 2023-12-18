using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBStaff
    {
        public class StaffResponse
        {
            public Employee Employee { get; set; }
            public DateTime Time { get; set; }
        }

        public class Employee
        {
            public string EmployeeNumber { get; set; }
            public string SSN { get; set; }
            public bool BillableTime { get; set; }
            public DateTime BirthDate { get; set; }
            public string Gender { get; set; }
            public DateTime HiredDate { get; set; }
            public DateTime ReleasedDate { get; set; }
            public string Domain { get; set; }
            public bool Sparse { get; set; }
            public string Id { get; set; }
            public string SyncToken { get; set; }
            public MetaData MetaData { get; set; }
            public string Title { get; set; }
            public string GivenName { get; set; }
            public string MiddleName { get; set; }
            public string FamilyName { get; set; }
            public string DisplayName { get; set; }
            public string PrintOnCheckName { get; set; }
            public bool Active { get; set; }
            public PrimaryAddr PrimaryAddr { get; set; }
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

        public class MetaData
        {
            public DateTime CreateTime { get; set; }
            public DateTime LastUpdatedTime { get; set; }
        }
    }
}