using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class StaffEditAddPermission : ContentView
{
    public StaffEditAddPermission(string? staff = null, bool fieldsAreEnable = true)
    {
        InitializeComponent();
        BindingContext = new StaffEditAddPermissionVM(staff, fieldsAreEnable);
    }

    private class StaffEditAddPermissionVM : BaseViewModel
    {
        private string? _staffId;
        private Staff? _selectedStaff;
        private string _statusMessage = "";
        private bool? _isStaffFound;
        private Role? _selectedRole;
        private bool showLoading;
        private ObservableCollection<Staff> staffList = new ObservableCollection<Staff>();

        public string? StaffId
        {
            get => _staffId;
            set
            {
                SetProperty(ref _staffId, value);
                if (value != null) SearchStaff();
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

        public bool? IsStaffFound
        {
            get => _isStaffFound;
            set
            {
                SetProperty(ref _isStaffFound, value);
            }
        }

        public Staff? SelectedStaff
        {
            get => _selectedStaff;
            set
            {
                SetProperty(ref _selectedStaff, value);
            }
        }

        public ObservableCollection<Role> Roles { get; } = VUtils.GetRoles();


        public Role? SelectedRole
        {
            get => _selectedRole;
            set
            {
                SetProperty(ref _selectedRole, value);
                Permissions.Clear();
                if (_selectedRole != null)
                    foreach (var permission in _selectedRole.Permissions)
                    {
                        Permissions.Add(permission);
                    }
            }
        }
        public ObservableCollection<Permission> Permissions { get; } = new ObservableCollection<Permission>();


        public bool ShowLoading { get => showLoading; set { SetProperty(ref showLoading, value); } } 
        public bool FieldsAreEnable { get; }

        public ICommand SaveCommand => new Command(OnSave);
        public ICommand CancelCommand => new Command(OnCancel);
        public string Title { get; }

        public StaffEditAddPermissionVM(string? __staffId, bool fieldsAreEnable)
        {
            InitializeData(__staffId); FieldsAreEnable = fieldsAreEnable;
            Title = !FieldsAreEnable ? "View User Access" : "Add User Access";
            if ((__staffId == "" || __staffId != null) && FieldsAreEnable) Title = "Edit Permissions";
        }

        private async void InitializeData(string? __staffId)
        {
            ShowLoading= true;
            staffList = await VUtils.GetStaffs();
            if (!string.IsNullOrEmpty(__staffId)) StaffId = __staffId; 
            ShowLoading = false;
        }

        private void SearchStaff()
        { 
            SelectedStaff = staffList.FirstOrDefault(s => s.StaffId == StaffId); 
            if (SelectedStaff != null)
            {
                StatusMessage = "Found"; IsStaffFound = true;
                SelectedRole = Roles.FirstOrDefault(r => r.Name == SelectedStaff.Role);
            }
            else
            {
                StatusMessage = "Not Found"; IsStaffFound = false; SelectedRole = null;
            }
        }

        private async void OnSave()
        {
            if (SelectedStaff != null && SelectedRole != null)
            {
                SelectedStaff.Role = SelectedRole.Name;
                try
                {
                    ShowLoading = true;
                    await new FirebaseClass().SaveUpdateStaffAsync(SelectedStaff);
                    VUtils.ShowMessage("Role Updated Successfully");
                    ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new UserAccessView();
                }
                catch (Exception)
                {
                    VUtils.ToastText("Error occurred\nCheck network and try again");
                }
                ShowLoading = false;
            }
            else VUtils.ToastText("Please select role");
        }

        private void OnCancel()
        {
            if (Title == "Add User Access" || string.IsNullOrEmpty(StaffId))
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new DefaultView();
            else
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new UserAccessView();
        }
    }

}