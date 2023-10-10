using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class InviteEmployee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string InvitaionEmail { get; set; }
        public Nullable<System.DateTime> EmploymentDate { get; set; }
    }
}