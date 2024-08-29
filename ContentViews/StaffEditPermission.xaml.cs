using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class StaffEditPermission : ContentView
{
    public StaffEditPermission()
    {
        InitializeComponent();
        BindingContext = new StaffEditPermissionVM();
    }

    private class StaffEditPermissionVM : BaseViewModel
    {
        private string _staffId;
        private Staff _selectedStaff;
        private string _statusMessage;
        private bool _isStaffFound;

        public string StaffId
        {
            get => _staffId;
            set
            {
                SetProperty(ref _staffId, value);
                SearchStaff();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                SetProperty(ref _statusMessage, value); 
            }
        }

        public bool IsStaffFound
        {
            get => _isStaffFound;
            set
            {
                SetProperty(ref _isStaffFound, value); 
            }
        }

        public Staff SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                SetProperty(ref _selectedStaff, value); 
            }
        }

        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string> { "Doctor", "Nurse" };
        public ObservableCollection<Permission> Permissions { get; } = new ObservableCollection<Permission>
        {
            new Permission { Name = "View Patient Record", IsGranted = false },
            new Permission { Name = "Edit Patient Record", IsGranted = false },
            new Permission { Name = "Delete Patient Record", IsGranted = false },
            new Permission { Name = "Manage Staff", IsGranted = false }
        };

        public ICommand SaveCommand => new Command(OnSave);
        public ICommand CancelCommand => new Command(OnCancel);

        public StaffEditPermissionVM()
        {
            // Initialize with any required data
        }

        private void SearchStaff()
        {
            // Simulate staff search. In a real application, you would query a database or service.
            var staffList = new ObservableCollection<Staff>
            {
                new Staff { StaffId = "1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "Doctor", Designation = "Surgeon", Username = "SUR 001" },
                new Staff { StaffId = "2", FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Role = "Nurse", Designation = "Orthopedic Nurse", Username = "ORT 001" }
            };

            SelectedStaff = staffList.FirstOrDefault(s => s.StaffId == StaffId);

            if (SelectedStaff != null)
            {
                StatusMessage = "Found";
                IsStaffFound = true;
                // Load existing permissions if necessary
            }
            else
            {
                StatusMessage = "Not Found";
                IsStaffFound = false;
            }
        }

        private void OnSave()
        {
            // Save the permissions and staff role changes
        }

        private void OnCancel()
        {
            App.Current.MainPage.BindingContext = new MainPageVM();
        }
    }

    public class Permission : BaseViewModel
    {
        private bool _isGranted;

        public string Name { get; set; }
        public bool IsGranted
        {
            get => _isGranted;
            set
            {
                SetProperty(ref _isGranted, value); 
            }
        } 
    }
}