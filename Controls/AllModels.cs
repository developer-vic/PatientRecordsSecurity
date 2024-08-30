using System.Collections.ObjectModel; 

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
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Designation { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public string Company { get; set; } = "";
        public string PermissionsSummary { get; set; } = "";

        public string FullName => $"{FirstName} {LastName}";
    }
    public class Role
    {
        public string Name { get; set; } = "";
        public ObservableCollection<Permission> Permissions { get; set; } = new ObservableCollection<Permission>();
    }

    public class Permission : BaseViewModel
    {
        private bool _isGranted;

        public string Name { get; set; } = "";
        public bool IsGranted
        {
            get => _isGranted;
            set
            {
                SetProperty(ref _isGranted, value);
            }
        }
    }

    public class Patient
    {
        public string PatientID { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string Gender { get; set; } = "";
        public string ContactNumber { get; set; } = "";
        public string EmailAddress { get; set; } = "";
        public string MedicalHistory { get; set; } = "";
        public string Password { get; set; } = "";
        public string Company { get; set; } = "";

        public string FullName => $"{FirstName} {LastName}";
    }
}
