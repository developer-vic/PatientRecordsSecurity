using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class StaffAddEdit : ContentView
{
	public StaffAddEdit(Controls.Staff? staff = null, bool fieldsAreEnable = true)
	{
		InitializeComponent();
		BindingContext = new StaffAddEditVM(staff, fieldsAreEnable); 
    }

    private class StaffAddEditVM : BaseViewModel
    {
        private bool _IS_NEW = true;
        private string? selectedDesignation;
        private string? username;
        private bool showLoading;
        private List<Staff> _staffList = new List<Staff>();


        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string>
        {
            "Doctor", "Nurse", "Others"
        };
        public ObservableCollection<string> Designations { get; } = new ObservableCollection<string>
        {
            "Gynecologist", "Pediatrician", "Psychiatrist", "Surgeon", "Cardiac Nurse",
            "Orthopedic Nurse", "Labor & Delivery Nurse"
        };
        public string? SelectedDesignation { get => selectedDesignation; set { SetProperty(ref selectedDesignation, value); GenerateUsername(); } }

        public string? Username { get => username; set { SetProperty(ref username, value); } }
        private void GenerateUsername()
        {
            if (!string.IsNullOrEmpty(SelectedDesignation))
            {
                string designationCode = SelectedDesignation.Substring(0, 3).ToUpper();
                int totalDesn = _staffList.Where(p=>p.Designation==SelectedDesignation).Count();
                totalDesn++; 
                if (_staffList.Count > 0)
                {
                    string lastID = _staffList.Last().StaffId;
                    totalDesn = int.Parse(lastID.Replace(designationCode, ""));
                    totalDesn++;
                } 
                Username = $"{designationCode}{totalDesn.ToString("D3")}";
            }
        }
        public string? ConfirmPassword { get; set; }

        public ICommand? MyCommand { get; protected set; }
        public ICommand? PickerCommand { get; protected set; }
        public Staff Staff { get; set; }
        public bool FieldsAreEnable { get; }
        public string Title { get; set; }
        public bool ShowLoading { get => showLoading; set { SetProperty(ref showLoading, value); } }

        public StaffAddEditVM(Staff? staff, bool fieldsAreEnable)
        {
            _IS_NEW = staff == null || string.IsNullOrEmpty(staff.StaffId);
            Staff = staff ?? new Staff(); FieldsAreEnable = fieldsAreEnable;
            Username = Staff.Username; SelectedDesignation = Staff.Designation;
            Title = !FieldsAreEnable ? "View Staff" : "Add New Staff";
            if (!string.IsNullOrEmpty(Staff.StaffId) && FieldsAreEnable)
            {
                Title = "Edit Staff"; ConfirmPassword = Staff.Password;
            }
            RunCommands(); InitializeData();
        }

        private async void InitializeData()
        {
            _staffList = await new FirebaseClass().GetAllStaffsAsync();
        }

        private void RunCommands()
        {
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "Save":
                        TrySaveStaff();
                        break;
                    case "Cancel":
                        OnCancelClick();
                        break;
                }
            });
            PickerCommand = new Command<Picker>((Picker picker) =>
            {
                picker.Focus();
            });
        }

        private void OnCancelClick()
        {
            if (Title == "Add New Staff")
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new DefaultView();
            else
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffViewAll();
        }

        private async void TrySaveStaff()
        {
            try
            {
                Staff.Username = Username ?? ""; Staff.Phone = Staff.Email;
                Staff.Company = VUtils.LoggedInUser?.Company ?? "";  
                Staff.Designation = SelectedDesignation ?? "";
                ShowLoading = true;
                string errMsg = await VUtils.StaffFieldsAreValid(Staff, _IS_NEW);
                if (string.IsNullOrEmpty(errMsg))
                {
                    if (Staff.Password != ConfirmPassword)
                        VUtils.ToastText("Password do not match");
                    else
                    {
                        await new FirebaseClass().SaveUpdateStaffAsync(Staff);
                        if(_IS_NEW) VUtils.ShowMessage("Staff has been added successful");
                        else VUtils.ShowMessage("Staff has been updated successful");
                        ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffViewAll();
                    }
                }
                else VUtils.ShowMessage(errMsg); 
            }
            catch (Exception)
            {
                VUtils.ToastText("Error occurred\nCheck network and try again");
            }
            ShowLoading = false;
        }
    }
}