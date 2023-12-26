using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarthCo.Models
{
    public class EmailClass
    {
        public string Email { get; set; }
        public string CCEmail { get; set; }
        public string Subject { get; set; }
        [AllowHtml]
        public string Body { get; set; }
    }
}