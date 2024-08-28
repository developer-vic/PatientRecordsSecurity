namespace PatientRecordsSecurity;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private void loginLinkButton_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new SignInPage();
    }

    private void signUpButton_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new MainPage();
    }
}