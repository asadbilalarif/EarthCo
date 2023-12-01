using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetWeeklyReportFiles
    {
        public SPGetWeeklyReportData_Result Data { get; set; }
        public List<SPGetWeeklyReportFileData_Result> FileData { get; set; }
    }
}