namespace PatientRecordsSecurity;

public partial class SignInPage : ContentPage
{
	public SignInPage()
	{
		InitializeComponent();
	}

    private void loginButton_Clicked(object sender, EventArgs e)
    {
        Application.Current.MainPage = new MainPage();
    } 
    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        Application.Current.MainPage = new SignUpPage();
    }
}