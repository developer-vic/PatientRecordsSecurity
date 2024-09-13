using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class DefaultView : ContentView
{ 
    public Staff? User { get; set; }
    public ICommand? MyCommand { get; private set; }
    private Patient _patient = new Patient(); 

    public bool ShowStaffMenu { get; set; }
    public bool ShowPatientAddMenu { get; set; }
    public bool ShowPatientViewMenu { get; set; }
    public bool ShowPatientMyRecord { get; set; }

    public DefaultView()
	{
		InitializeComponent(); 
        InitializeData(); RunCommands(); 
        BindingContext = this;
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
                case "StaffAddEdit":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new StaffAddEdit();
                    break;
                case "StaffViewAll":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new StaffViewAll();
                    break;
                case "StaffEditPermission":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new StaffEditAddPermission("");
                    break;
                case "UserAccessView":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new UserAccessView();
                    break;
                case "UserAccessAdd":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new StaffEditAddPermission();
                    break;
                case "PatientAdd":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new PatientAddEdit();
                    break;
                case "PatientView":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new PatientView();
                    break;
                case "PatientRecord":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new PatientAddEdit(_patient, false);
                    break;
                case "ChangePassword":
                    ((MainPageVM) VUtils.GetMainPage().BindingContext).CurrentView = new ChangePassword();
                    break; 
            }
        });
    }
}