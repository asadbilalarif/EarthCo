using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetCustomerData
    {
        public SPGetCustomerData_Result Data { get; set; }
        public List<SPGetCustomerContactData_Result> ContactData { get; set; }
        public List<SPGetCustomerServiceLocationData_Result> ServiceLocationData { get; set; }
    }
}