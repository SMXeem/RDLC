using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RdlcReport.Models
{
    public class EnrollmentVm
    {
        public int EnrollmentID { get; set; }
        public string CourseID { get; set; }
        public string StudentID { get; set; }
        public string Grade { get; set; }
    }
}