using PatientRecordsSecurity.Controls;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class ChangePassword : ContentView
{
    public ICommand SaveCommand { get; private set; }
    public ICommand CancelCommand { get; private set; }
    public ICommand LogoutCommand { get; private set; }


    public ChangePassword()
	{
		InitializeComponent();
        SaveCommand = new Command(OnSave);
        CancelCommand = new Command(OnCancel);
        LogoutCommand = new Command(OnLogout);
        BindingContext = this;
	}

    private void OnSave()
    {
        VUtils.GetMainPage().BindingContext = new MainPageVM();
    }

    private void OnCancel()
    {
        VUtils.GetMainPage().BindingContext = new MainPageVM();
    }

    private void OnLogout()
    {
        VUtils.LogOut();
    }

}