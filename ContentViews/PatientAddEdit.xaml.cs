using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel; 
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class PatientAddEdit : ContentView
{
	public PatientAddEdit(Patient? patient = null, bool fieldsAreEnable = true)
	{
		InitializeComponent();
		BindingContext = new PatientAddEditVM(patient, fieldsAreEnable); 
    }

    private class PatientAddEditVM
    {
        private string _EditPatientID = string.Empty; 
        public ObservableCollection<string> Genders { get; } = new ObservableCollection<string> { "Male", "Female", "Other" }; 
        public string? ConfirmPassword { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public bool FieldsAreEnable { get; set; }
        public Patient Patient {  get; set; }   
        public string? Title { get; }


        public PatientAddEditVM(Patient? patient, bool fieldsAreEnable)
        {
            FieldsAreEnable = fieldsAreEnable;
            Patient = patient ?? new Patient();
            _EditPatientID = Patient.PatientID;

            Title = !FieldsAreEnable ? "View Patient" : "Add New Patient";
            if (!string.IsNullOrEmpty(_EditPatientID))
            {
                if(FieldsAreEnable) Title = "Edit Patient";
            }
            else Patient.PatientID = VUtils.GetPatientID();

            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
        }


        private void OnSave()
        {
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new PatientView();
        }

        private void OnCancel()
        {
            if (Title == "Add New Patient")
                VUtils.GetMainPage().BindingContext = new MainPageVM();
            else
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new PatientView(); 
        }
          
    }
}