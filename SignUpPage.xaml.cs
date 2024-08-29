using PatientRecordsSecurity.Controls;

namespace PatientRecordsSecurity;

public partial class SignUpPage : ContentPage
{
	public SignUpPage()
	{
		InitializeComponent();
	}

    private void loginLinkButton_Clicked(object sender, EventArgs e)
    {
        VUtils.GetoPage(new SignInPage(), true); 
    }

    private void signUpButton_Clicked(object sender, EventArgs e)
    {
        VUtils.GetoPage(new MainPage(), true); 
    }
}