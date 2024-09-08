using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input; 

namespace PatientRecordsSecurity.ContentViews;

public partial class PatientView : ContentView
{
	public PatientView()
	{
		InitializeComponent();
		BindingContext = new PatientViewVM(); 
    }

    private class PatientViewVM : BaseViewModel
    {
        private string? _searchQuery = null;
        private bool showLoading;
        private ObservableCollection<Patient> _patients = new ObservableCollection<Patient>();
        private ObservableCollection<Patient> _Patients = new ObservableCollection<Patient>();
        public bool ShowLoading { get => showLoading; set { SetProperty(ref showLoading, value); } } 
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                FilterPatients();
            }
        } 
        public ObservableCollection<Patient> Patients
        {
            get => _patients;
            set
            {
                _patients = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPatientAddMenu { get; set; }
        public bool ShowPatientDeleteMenu { get; set; }

        public ICommand ViewPatientCommand { get; }
        public ICommand EditPatientCommand { get; }
        public ICommand DeletePatientCommand { get; } 

        public PatientViewVM()
        { 
            ViewPatientCommand = new Command<Patient>(OnViewPatient);
            EditPatientCommand = new Command<Patient>(OnEditPatient);
            DeletePatientCommand = new Command<Patient>(OnDeleteStaff);
            InitializeData();
        }

        private async void InitializeData()
        {  
            ObservableCollection<Role> Roles = VUtils.GetRoles();
            Role role = Roles.Where(p => p.Name == VUtils.LoggedInUser?.Role).First();
            ObservableCollection<Permission> permissions = role.Permissions; 
            ShowPatientAddMenu = permissions.Where(p => p.Name == "Edit Patient Record").First().IsGranted;
            ShowPatientDeleteMenu = permissions.Where(p => p.Name == "Delete Patient Record").First().IsGranted;
            ShowLoading = true;
            Patients = await VUtils.GetPatients(); _Patients = Patients;
            ShowLoading = false;
        }

        private void FilterPatients()
        {
            if (string.IsNullOrEmpty(SearchQuery))
            {
                Patients = _Patients;
            }
            else
            {
                Patients = new ObservableCollection<Patient>(_Patients.Where(p => p.PatientID.Contains(SearchQuery) || p.FullName.Contains(SearchQuery)));
            }
        }

        private void OnViewPatient(Patient patient)
        {
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new PatientAddEdit(patient, false);
        }

        private void OnEditPatient(Patient patient)
        {
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new PatientAddEdit(patient);
        }
        private async void OnDeleteStaff(Patient patient)
        {
            ShowLoading = true;
            try
            {
                if (Application.Current?.MainPage != null)
                {
                    bool answer = await Application.Current.MainPage.DisplayAlert("Confirm Logout", $"Are you sure you want to delete '{patient.PatientID}' ?", "Yes", "No");
                    if (answer)
                    {
                        await new FirebaseClass().DeletePatientAsync(patient.PatientID); InitializeData();
                        await Application.Current.MainPage.DisplayAlert("Logged Out", $"'{patient.PatientID}' patient has been deleted.", "OK");
                    }
                }
            }
            catch (Exception)
            {
                VUtils.ToastText("Error occurred.\nCheck network and try again.");
            }
            ShowLoading = false;
        }
    }
}