using PatientRecordsSecurity.Controls;
using System.Collections.ObjectModel; 
using System.Windows.Input;

namespace PatientRecordsSecurity.ContentViews;

public partial class UserAccessView : ContentView
{
    public UserAccessView()
    {
        InitializeComponent();
        BindingContext = new UserAccessViewVM();
    }

    private class UserAccessViewVM : BaseViewModel
    { 
        private ObservableCollection<Staff> _Staffs = new ObservableCollection<Staff>();
        private ObservableCollection<Staff> staffs = new ObservableCollection<Staff>();
        private string searchQuery = "";

        public string SearchQuery { get => searchQuery; set { SetProperty(ref searchQuery, value); FilterStaffs(); } } 

        public ObservableCollection<Staff> Staffs { get => staffs; set { SetProperty(ref staffs, value); } }

        public ICommand ViewStaffCommand => new Command<Staff>(OnViewStaff);
        public ICommand EditStaffCommand => new Command<Staff>(OnEditStaff);

        public UserAccessViewVM()
        { 
            Staffs = VUtils.GetStaffs();
            _Staffs = Staffs;
        }

        private void FilterStaffs()
        { 
            if (string.IsNullOrEmpty(SearchQuery))
            {
                Staffs = new ObservableCollection<Staff>(_Staffs);
            }
            else
            {
                Staffs = new ObservableCollection<Staff>(_Staffs.Where(u => u.FullName.Contains(SearchQuery) || u.StaffId.Contains(SearchQuery)));
            }
        }

        private void OnViewStaff(Staff Staff)
        { 
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffEditAddPermission(Staff.StaffId, false);
        }

        private void OnEditStaff(Staff Staff)
        { 
            ((MainPageVM)VUtils.GetMainPage().BindingContext).CurrentView = new StaffEditAddPermission(Staff.StaffId);
        } 
    } 
}