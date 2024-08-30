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
            string errMsg = VUtils.StaffFieldsAreValid(Staff);
            if (string.IsNullOrEmpty(errMsg))
            {  
                await firebaseClass.SaveUpdateStaffAsync(Staff);
                VUtils.ShowMessage("Registration Successful");
                VUtils.LoggedInUser = Staff;
                VUtils.GetoPage(new MainPage(), true);
            }
            else VUtils.ShowMessage(errMsg);
        }
        catch (Exception ex)
        {
            VUtils.ToastText("Registration Failed\nPlease try again.");
        }
    }
}