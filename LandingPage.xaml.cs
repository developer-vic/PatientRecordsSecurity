using PatientRecordsSecurity.Controls;

namespace PatientRecordsSecurity;

public partial class LandingPage : ContentPage
{
	public LandingPage()
	{
		InitializeComponent();
	}

    private void Button_Clicked(object sender, EventArgs e)
    {
        VUtils.GetoPage(new SignUpPage(), true); 
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        VUtils.GetoPage(new SignInPage(), true); 
    }
}