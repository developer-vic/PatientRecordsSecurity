using PatientRecordsSecurity.ContentViews;
using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PatientRecordsSecurity
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageVM();
        }
    }

    public class MainPageVM : BaseViewModel
    {
        private Patient _patient = new Patient();
        private string manageStaffDrop = "dropdown.png";
        private bool manageStaffMenuVisible;
        private string manageAccessDrop = "dropdown.png";
        private bool manageAccessMenuVisible;
        private string managePatientDrop = "dropdown.png";
        private bool managePatientMenuVisible;
        private string settingsDrop = "dropdown.png";
        private bool settingsMenuVisible;
        private string passwordDrop = "dropdown.png";
        private bool passwordMenuVisible;
        private ContentView currentView = new DefaultView();

        public string ManageStaffDrop { get => manageStaffDrop; set { SetProperty(ref manageStaffDrop, value); } }
        public bool ManageStaffMenuVisible { get => manageStaffMenuVisible; set { SetProperty(ref manageStaffMenuVisible, value); } }
        public string ManageAccessDrop { get => manageAccessDrop; set { SetProperty(ref manageAccessDrop, value); } }
        public bool ManageAccessMenuVisible { get => manageAccessMenuVisible; set { SetProperty(ref manageAccessMenuVisible, value); } }
        public string ManagePatientDrop { get => managePatientDrop; set { SetProperty(ref managePatientDrop, value); } }
        public bool ManagePatientMenuVisible { get => managePatientMenuVisible; set { SetProperty(ref managePatientMenuVisible, value); } }
        public string SettingsDrop { get => settingsDrop; set { SetProperty(ref settingsDrop, value); } }
        public bool SettingsMenuVisible { get => settingsMenuVisible; set { SetProperty(ref settingsMenuVisible, value); } }
        public string PasswordDrop { get => passwordDrop; set { SetProperty(ref passwordDrop, value); } }
        public bool PasswordMenuVisible { get => passwordMenuVisible; set { SetProperty(ref passwordMenuVisible, value); } }

        public ContentView CurrentView { get => currentView; set { SetProperty(ref currentView, value); } }

        public ICommand? MyCommand { get; private set; }
        public Staff? User { get; set; }
        public bool ShowStaffMenu { get; set; }
        public bool ShowPatientAddMenu { get; set; }
        public bool ShowPatientViewMenu { get; set; }
        public bool ShowPatientMyRecord { get; set; }


        public MainPageVM()
        {
            RunCommands();
            InitializeData();
        }

        private async void InitializeData()
        {
            User = VUtils.LoggedInUser ?? new Staff();
            ObservableCollection<Role> Roles = VUtils.GetRoles();
            Role role = Roles.Where(p => p.Name == User.Role).First();
            ObservableCollection<Permission> permissions = role.Permissions;
            ShowStaffMenu = permissions.Where(p => p.Name == "Manage Staff").First().IsGranted;
            ShowPatientAddMenu = permissions.Where(p => p.Name == "Edit Patient Record").First().IsGranted;
            if (User.IsPatient)
            {
                ShowPatientMyRecord = true; _patient = await new FirebaseClass().GetPatientAsync(User.Username);
            }
            else ShowPatientViewMenu = permissions.Where(p => p.Name == "View Patient Record").First().IsGranted;
        }

        private void RunCommands()
        {
            MyCommand = new Command<string>((string par) =>
            {
                switch (par)
                {
                    case "ManageStaffDrop":
                        ManageStaffMenuVisible = !ManageStaffMenuVisible;
                        ManageStaffDrop = ManageStaffMenuVisible ? "dropup.png" : "dropdown.png";
                        break;
                    case "ManageAccessDrop":
                        ManageAccessMenuVisible = !ManageAccessMenuVisible;
                        ManageAccessDrop = ManageAccessMenuVisible ? "dropup.png" : "dropdown.png";
                        break;
                    case "ManagePatientDrop":
                        ManagePatientMenuVisible = !ManagePatientMenuVisible;
                        ManagePatientDrop = ManagePatientMenuVisible ? "dropup.png" : "dropdown.png";
                        break;
                    case "SettingsDrop":
                        SettingsMenuVisible = !SettingsMenuVisible;
                        SettingsDrop = SettingsMenuVisible ? "dropup.png" : "dropdown.png";
                        break;
                    case "PasswordDrop":
                        PasswordMenuVisible = !PasswordMenuVisible;
                        PasswordDrop = PasswordMenuVisible ? "dropup.png" : "dropdown.png";
                        break;
                    //clicks 
                    case "StaffAddEdit":
                        CurrentView = new StaffAddEdit();
                        break;
                    case "StaffViewAll":
                        CurrentView = new StaffViewAll();
                        break;
                    case "StaffEditPermission":
                        CurrentView = new StaffEditAddPermission("");
                        break;
                    case "UserAccessView":
                        CurrentView = new UserAccessView();
                        break;
                    case "UserAccessAdd":
                        CurrentView = new StaffEditAddPermission();
                        break;
                    case "PatientAdd":
                        CurrentView = new PatientAddEdit();
                        break;
                    case "PatientView":
                        CurrentView = new PatientView();
                        break;
                    case "PatientRecord":
                        CurrentView = new PatientAddEdit(_patient, false);
                        break;
                    case "ChangePassword":
                        CurrentView = new ChangePassword();
                        break;
                    case "DefaultView":
                        CurrentView = new DefaultView();
                        break;
                    case "Logout":
                        VUtils.LogOut();
                        break;
                }
            });
        }
    }

}
