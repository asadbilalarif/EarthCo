using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class EmailClass
    {
        public string Email { get; set; }
        public string CCEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}