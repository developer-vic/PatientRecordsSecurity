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
        private ObservableCollection<Patient> _patients = new ObservableCollection<Patient>();
        private ObservableCollection<Patient> _Patients = new ObservableCollection<Patient>();

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

        public ICommand ViewPatientCommand { get; }
        public ICommand EditPatientCommand { get; }

        public PatientViewVM()
        { 
            Patients = VUtils.GetPatients(); _Patients = Patients;
            ViewPatientCommand = new Command<Patient>(OnViewPatient);
            EditPatientCommand = new Command<Patient>(OnEditPatient);
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
         
    }
}