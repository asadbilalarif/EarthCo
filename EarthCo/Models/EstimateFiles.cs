using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace EarthCo.Models
{
    public class EstimateFiles
    {
        public tblEstimate EstimateData { get; set; }
        public List<HttpPostedFile> Files { get; set; }
    }
}