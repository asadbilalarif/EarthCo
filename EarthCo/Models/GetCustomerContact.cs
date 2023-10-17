using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetCustomerContact
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public int ContactId { get; set; }
        public string ContactName { get; set; }
        public string ContactCompany { get; set; }
        public string ContactEmail { get; set; }
    }
}