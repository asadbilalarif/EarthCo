using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class CustomerContacts
    {
        public tblCustomer CustomerData { get; set; }
        public List<tblContact> ContactData { get; set; }
    }
}