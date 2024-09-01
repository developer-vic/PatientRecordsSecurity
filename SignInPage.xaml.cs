using PatientRecordsSecurity.Controls;

namespace PatientRecordsSecurity;

public partial class SignInPage : ContentPage
{
    public Staff Staff { get; set; } = new Staff();
    private FirebaseClass firebaseClass;

    public SignInPage()
    {
        InitializeComponent();
        firebaseClass = new FirebaseClass();
        BindingContext = this;
    }

    private async void loginButton_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!string.IsNullOrEmpty(Staff.Username) && !string.IsNullOrEmpty(Staff.Password))
            {
                ShowLoading(true);
                bool loginSuccess = await firebaseClass.TryLogin(Staff.Username, Staff.Password);
                if (loginSuccess)
                { 
                    VUtils.ShowMessage("Login Successful");
                    VUtils.GetoPage(new MainPage(), true);
                }
                else VUtils.ShowMessage("Login Failed\nInvalid Username or Password");
            }
            else VUtils.ShowMessage("Pls enter username and password");
        }
        catch (Exception)
        {
            VUtils.ToastText("Login Failed\nPlease check network and try again.");
        }
        ShowLoading(false);
    }

    private void ShowLoading(bool isRunning)
    {
        showLoading.IsVisible = isRunning;
        showLoading.IsRunning = isRunning;
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        VUtils.GetoPage(new SignUpPage(), true);
    }
}