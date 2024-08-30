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
                bool loginSuccess = await firebaseClass.TryLogin(Staff.Username, Staff.Password);
                if (loginSuccess)
                {
                    VUtils.ShowMessage("Login Successful");
                    VUtils.LoggedInUser = Staff;
                    VUtils.GetoPage(new MainPage(), true);
                }
                else VUtils.ShowMessage("Login Failed\nInvalid Username or Password");
            }
            else VUtils.ShowMessage("Pls enter username and password");
        }
        catch (Exception ex)
        {
            VUtils.ToastText("Login Failed\nPlease try again.");
        }
    }
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        VUtils.GetoPage(new SignUpPage(), true);
    }
}