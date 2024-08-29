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
        private string? selectedDesignation;
        private string? username;
         
        public ObservableCollection<string> Roles { get; } = new ObservableCollection<string>
        {
            "Doctor", "Nurse"
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
                Username = $"{designationCode} 001";
            }
        }
        public string? ConfirmPassword { get; set; }

        public ICommand? MyCommand { get; protected set; }
        public ICommand? PickerCommand { get; protected set; }
        public Staff Staff { get; }
        public bool FieldsAreEnable { get; }
        public string Title { get; set; }

        public StaffAddEditVM(Staff? staff, bool fieldsAreEnable)
        {
            Staff = staff ?? new Staff(); FieldsAreEnable = fieldsAreEnable;
            Username = Staff.Username; SelectedDesignation = Staff.Designation;

            Title = !FieldsAreEnable ? "View Staff" : "Add New Staff";
            if (!string.IsNullOrEmpty(Staff.StaffId) && FieldsAreEnable)
                Title = "Edit Staff";

            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "Save":

                        break;
                    case "Cancel":
                        if(Title == "Add New Staff")
                            App.Current.MainPage.BindingContext = new MainPageVM();
                        else
                            ((MainPageVM)App.Current.MainPage.BindingContext).CurrentView = new StaffViewAll();
                        break;
                }
            });
            PickerCommand = new Command<Picker>((Picker picker) =>
            {
                picker.Focus();
            });
        } 
    }
}