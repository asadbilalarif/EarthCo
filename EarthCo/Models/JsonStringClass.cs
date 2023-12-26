using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EarthCo.Models
{
    public class JsonStringClass
    {
        [AllowHtml] public string Data { get; set; }
    }
}