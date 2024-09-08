using PatientRecordsSecurity.Controls;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class ChangePassword : ContentView
{
    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand LogoutCommand { get; private set; }
    FirebaseClass firebaseClass;

    public ChangePassword()
    {
        InitializeComponent();
        SaveCommand = new Command(OnSave);
        CancelCommand = new Command(OnCancel);
        LogoutCommand = new Command(OnLogout);
        firebaseClass = new FirebaseClass();
        BindingContext = this;
    }

    private async void OnSave()
    {
        var user = VUtils.LoggedInUser; if (user == null) return;
        if (string.IsNullOrEmpty(OldPasswordEntry.Text) || string.IsNullOrEmpty(NewPasswordEntry.Text) || string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
            VUtils.ToastText("Please fill all information");
        else if (user.Password != OldPasswordEntry.Text) VUtils.ToastText("Old Password do not match");
        else if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text) VUtils.ToastText("New Password do not match");
        else
        {
            ShowLoading(true);
            try
            {
                if (user.IsPatient)
                {
                    var patient = await firebaseClass.GetPatientAsync(user.Username);
                    patient.Password = NewPasswordEntry.Text;
                    await firebaseClass.SaveUpdatePatientAsync(patient);
                }
                else
                {
                    user.Password = NewPasswordEntry.Text;
                    await firebaseClass.SaveUpdateStaffAsync(user);
                }
                VUtils.ShowMessage("Password has been updated successful");
                VUtils.LoggedInUser.Password = NewPasswordEntry.Text; OnCancel();
            }
            catch (Exception)
            {
                VUtils.ToastText("Error occurred\nCheck network and try again");
            }
            ShowLoading(false);
        }
    }

    private void ShowLoading(bool isRunning)
    {
        showLoading.IsVisible = isRunning;
        showLoading.IsRunning = isRunning;
    }

    private void OnCancel()
    {
        ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new DefaultView();
    }

    private void OnLogout()
    {
        VUtils.LogOut();
    }

}