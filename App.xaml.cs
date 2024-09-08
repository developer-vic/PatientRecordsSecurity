
namespace PatientRecordsSecurity
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            if(Application.Current != null)
                Application.Current.UserAppTheme = AppTheme.Light;
            MainPage = new LandingPage();  
        }
    }
}
