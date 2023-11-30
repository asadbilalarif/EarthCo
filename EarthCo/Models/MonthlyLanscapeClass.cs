using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Controllers
{
    public class MonthlyLanscapeClass
    {
        public int MonthlyLandsacpeId { get; set; }
        public int CustomerId { get; set; }
        public int ContactId { get; set; }
        public int ServiceLocationId { get; set; }
        public int RequestBy { get; set; }
        public string SupervisorVisitedthejobweekly { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CompletedLitterpickupofgroundareas { get; set; }
        public string Completedsweepingorblowingofwalkways { get; set; }
        public string HighpriorityareaswereVisitedweekly { get; set; }
        public string VDitcheswerecleanedandinspected { get; set; }
        public string WeepscreeninspectedandcleanedinrotationsectionId { get; set; }
        public string Fertilizationoftrufoccoured { get; set; }
        public string Trufwasmovedandedgedweekly { get; set; }
        public string Shrubstrimmedaccordingtorotationschedule { get; set; }
        public string FertilizationofShrubsoccoured { get; set; }
        public string WateringofflowerbedsCompletedandchecked { get; set; }
        public string Headswereadjustedformaximumcoverage { get; set; }
        public string Repairsweremadetomaintainaneffectivesystem { get; set; }
        public string Controllerswereinspectedandadjusted { get; set; }
        public string Mainlinewasrepaired { get; set; }
        public string Valvewasrepaired { get; set; }
        public string Thismonthexpectedrotationschedule { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public bool isActive { get; set; }
        public bool isDelete { get; set; }
        public string CompanyName { get; set; }
        public string RequestByName { get; set; }
        public string ServiceLocationName { get; set; }
    }
}