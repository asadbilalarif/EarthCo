//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EarthCo.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblMonthlyLandsacpe
    {
        public int MonthlyLandsacpeId { get; set; }
        public Nullable<int> CustomerId { get; set; }
        public Nullable<int> ContactId { get; set; }
        public Nullable<bool> SupervisorVisitedthejobweekly { get; set; }
        public Nullable<bool> CompletedLitterpickupofgroundareas { get; set; }
        public Nullable<bool> Completedsweepingorblowingofwalkways { get; set; }
        public Nullable<bool> HighpriorityareaswereVisitedweekly { get; set; }
        public Nullable<bool> VDitcheswerecleanedandinspected { get; set; }
        public Nullable<int> WeepscreeninspectedandcleanedinrotationsectionId { get; set; }
        public string Fertilizationoftrufoccoured { get; set; }
        public Nullable<bool> Trufwasmovedandedgedweekly { get; set; }
        public Nullable<bool> Shrubstrimmedaccordingtorotationschedule { get; set; }
        public string FertilizationofShrubsoccoured { get; set; }
        public Nullable<bool> WateringofflowerbedsCompletedandchecked { get; set; }
        public Nullable<bool> Headswereadjustedformaximumcoverage { get; set; }
        public Nullable<bool> Repairsweremadetomaintainaneffectivesystem { get; set; }
        public Nullable<bool> Controllerswereinspectedandadjusted { get; set; }
        public Nullable<bool> Mainlinewasrepaired { get; set; }
        public Nullable<bool> Valvewasrepaired { get; set; }
        public string Thismonthexpectedrotationschedule { get; set; }
        public string Notes { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> EditBy { get; set; }
        public Nullable<System.DateTime> EditDate { get; set; }
        public Nullable<bool> isActive { get; set; }
    
        public virtual tblCustomer tblCustomer { get; set; }
    }
}
