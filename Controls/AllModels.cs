using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatientRecordsSecurity.Controls
{
    internal class AllModels
    {
    }
    public class Staff
    {
        public string StaffId { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Designation { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";

        public string FullName => $"{FirstName} {LastName}";
    }
}
