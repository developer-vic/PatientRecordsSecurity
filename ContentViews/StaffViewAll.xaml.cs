using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class StaffViewAll : ContentView
{
	public StaffViewAll()
	{
		InitializeComponent();
		BindingContext = new StaffViewAllVM(); 
    }

    private class StaffViewAllVM : BaseViewModel
    {  
        private ObservableCollection<Staff> _staffList = new ObservableCollection<Staff>();  
        private string searchText = "";
        private ObservableCollection<Staff> staffList = new ObservableCollection<Staff>();

        public ObservableCollection<Staff> StaffList { get => staffList; set { SetProperty(ref staffList, value); } }

        public string SearchText { get => searchText; set { SetProperty(ref searchText, value); FilterStaffList(); } }

        public ICommand AddNewStaffCommand => new Command(OnAddNewStaff);
        public ICommand ViewStaffCommand => new Command<Staff>(OnViewStaff);
        public ICommand EditStaffCommand => new Command<Staff>(OnEditStaff);

        public StaffViewAllVM()
        { 
            _staffList = new ObservableCollection<Staff>
            {
                new Staff { StaffId = "1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "Doctor", Designation = "Surgeon", Username = "SUR 001", Password = "123" },
                new Staff { StaffId = "2", FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Role = "Nurse", Designation = "Orthopedic Nurse", Username = "ORT 001", Password = "12345678" }
            };
            StaffList = new ObservableCollection<Staff>(_staffList);
        }

        private void FilterStaffList()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                StaffList = new ObservableCollection<Staff>(_staffList);
            }
            else
            {
                StaffList = new ObservableCollection<Staff>(_staffList.Where(s =>
                    s.FirstName.ToLower().Contains(SearchText.ToLower()) ||
                    s.LastName.ToLower().Contains(SearchText.ToLower()) ||
                    s.Role.ToLower().Contains(SearchText.ToLower()) ||
                    s.Designation.ToLower().Contains(SearchText.ToLower())));
            }
        }

        private void OnAddNewStaff()
        { 
            ((MainPageVM)App.Current.MainPage.BindingContext).CurrentView = new StaffAddEdit();
        }

        private void OnViewStaff(Staff staff)
        {
            ((MainPageVM)App.Current.MainPage.BindingContext).CurrentView = new StaffAddEdit(staff, false);
        }

        private void OnEditStaff(Staff staff)
        {
            ((MainPageVM)App.Current.MainPage.BindingContext).CurrentView = new StaffAddEdit(staff);
        } 

    }
}