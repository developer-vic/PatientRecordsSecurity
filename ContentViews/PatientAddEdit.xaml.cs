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

    private class PatientAddEditVM : BaseViewModel
    {
        private bool showLoading;
        private bool _IS_NEW = true;
        private string patientID = "";

        public ObservableCollection<string> Genders { get; } = new ObservableCollection<string> { "Male", "Female", "Other" };
        public string? ConfirmPassword { get; set; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public bool FieldsAreEnable { get; set; }
        public Patient Patient { get; set; }
        public string? Title { get; }
        public bool ShowLoading { get => showLoading; set { SetProperty(ref showLoading, value); } }
        public string PatientID { get => patientID; set { SetProperty(ref patientID, value); } }

        public PatientAddEditVM(Patient? patient, bool fieldsAreEnable)
        {
            _IS_NEW = patient == null || string.IsNullOrEmpty(patient.PatientID);
            FieldsAreEnable = fieldsAreEnable; Patient = patient ?? new Patient();
            PatientID = Patient.PatientID;

            Title = !FieldsAreEnable ? "View Patient" : "Add New Patient";
            if (!string.IsNullOrEmpty(PatientID))
            {
                if (FieldsAreEnable)
                {
                    Title = "Edit Patient"; ConfirmPassword = Patient.Password;
                }
            }
            else InitNewPatientID();

            SaveCommand = new Command(OnSave);
            CancelCommand = new Command(OnCancel);
        }

        private async void InitNewPatientID()
        {
            ShowLoading = true;
            var allPatient = await VUtils.GetPatients();
            int totalDesn = allPatient.Count; totalDesn++;
            if (allPatient.Count > 0)
            {
                string lastID = allPatient.Last().PatientID;
                totalDesn = int.Parse(lastID.Replace("PAT", ""));
                totalDesn++;
            }
            PatientID = $"PAT{totalDesn.ToString("D3")}";
            Patient.PatientID = PatientID;
            ShowLoading = false;
        }

        private async void OnSave()
        {
            try
            {
                Patient.Company = VUtils.LoggedInUser?.Company ?? ""; ShowLoading = true;
                string errMsg = await VUtils.PatientFieldsAreValid(Patient, _IS_NEW);
                if (string.IsNullOrEmpty(errMsg))
                {
                    if (Patient.Password != ConfirmPassword)
                        VUtils.ToastText("Password do not match");
                    else
                    {
                        await new FirebaseClass().SaveUpdatePatientAsync(Patient);
                        if (_IS_NEW) VUtils.ShowMessage("Patient has been added successful");
                        else VUtils.ShowMessage("Patient has been updated successful");
                        ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new PatientView();
                    }
                }
                else VUtils.ShowMessage(errMsg);
            }
            catch (Exception)
            {
                VUtils.ToastText("Error occurred\nCheck network and try again");
            }
            ShowLoading = false;
        }

        private void OnCancel()
        {
            if (Title == "Add New Patient")
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new DefaultView();
            else
                ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new PatientView();
        }

    }

}