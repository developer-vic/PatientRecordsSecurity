using PatientRecordsSecurity.Controls;

namespace PatientRecordsSecurity;

public partial class SignUpPage : ContentPage
{
    public Staff Staff { get; set; } = new Staff();
    private FirebaseClass firebaseClass;

    public SignUpPage()
	{
		InitializeComponent(); 
        firebaseClass = new FirebaseClass();
        BindingContext = this;
    }

    private void loginLinkButton_Clicked(object sender, EventArgs e)
    {
        VUtils.GetoPage(new SignInPage(), true); 
    }

    private async void signUpButton_Clicked(object sender, EventArgs e)
    { 
        try
        {
            Staff.StaffId = Staff.Username; 
            Staff.FirstName = Staff.Company; Staff.LastName = ".";
            Staff.Role = "Doctor"; Staff.Designation = "Admin";
            ShowLoading(true);
            string errMsg = await VUtils.StaffFieldsAreValid(Staff, true);
            if (string.IsNullOrEmpty(errMsg))
            {
                await firebaseClass.SaveUpdateStaffAsync(Staff);
                VUtils.ShowMessage("Registration Successful");
                VUtils.LoggedInUser = Staff;
                VUtils.GetoPage(new MainPage(), true);
            }
            else VUtils.ShowMessage(errMsg);
        }
        catch (Exception)
        {
            VUtils.ToastText("Registration Failed\nPlease check network and try again.");
        }
        ShowLoading(false);
    }

    private void ShowLoading(bool isRunning)
    {
        showLoading.IsVisible = isRunning;
        showLoading.IsRunning = isRunning;
    }
}