using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetWeeklyReportRCFiles
    {
        public SPGetWeeklyReportRCData_Result Data { get; set; }
        public List<SPGetWeeklyReportRCFileData_Result> FileData { get; set; }
    }
}