using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class ServerSideTableClass
    {
        public string sEcho { get; set; }
        public string sSearch { get; set; }
        public int? DisplayLength { get; set; }
        public int? DisplayStart { get; set; }
        public int iColumns { get; set; }
        public int iSortCol_0 { get; set; }
        public string sSortDir_0 { get; set; }
        public int iSortingCols { get; set; }
        public string sColumns { get; set; }
    }
}