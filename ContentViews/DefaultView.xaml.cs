using PatientRecordsSecurity.Controls; 

namespace PatientRecordsSecurity.ContentViews;

public partial class DefaultView : ContentView
{ 
    public Staff User { get; set; }
    public DefaultView()
	{
		InitializeComponent();
        User = VUtils.LoggedInUser ?? new Staff();
		BindingContext = this;
    }
}