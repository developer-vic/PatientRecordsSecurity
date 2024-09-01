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
            InitializeData();
        }

        private async void InitializeData()
        {
            StaffList = await VUtils.GetStaffs();
            _staffList = StaffList;
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
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffAddEdit();
        }

        private void OnViewStaff(Staff staff)
        {
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffAddEdit(staff, false);
        }

        private void OnEditStaff(Staff staff)
        {
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffAddEdit(staff);
        }

    }
}