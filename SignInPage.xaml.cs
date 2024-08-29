using PatientRecordsSecurity.Controls;

namespace PatientRecordsSecurity;

public partial class SignInPage : ContentPage
{
	public SignInPage()
	{
		InitializeComponent();
	}

    private void loginButton_Clicked(object sender, EventArgs e)
    {
        VUtils.GetoPage(new MainPage(), true); 
    } 
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        VUtils.GetoPage(new SignUpPage(), true); 
    }
}