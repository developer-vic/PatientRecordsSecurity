using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PatientRecordsSecurity.Controls
{
    public class VUtils
    {
        internal static void ToastText(string text)
        {
            ShowMessage(text);
            // await Toast.Make(text, ToastDuration.Short).Show();
        }

        public static string GetTransactionRef()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff");
        }

        private static Regex _regex = new Regex(
 @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
 RegexOptions.CultureInvariant | RegexOptions.Singleline);

        internal static bool IsValidEmailFormat(string emailInput)
        {
            return _regex.IsMatch(emailInput);
        }

        internal async static void CopyTextToClipBoard(string book_decription)
        {
            try
            {
                await Clipboard.SetTextAsync(book_decription);
                ToastText("copied");
            }
            catch (Exception ex)
            {
                ToastText(ex.Message);
            }
        }

        internal static Page GetMainPage()
        {
            if (App.Current?.MainPage != null)
                return App.Current.MainPage;
            return new Page();
        }
        internal static void GetoPage(Page contentPage, bool isMainPage = false)
        {
            if (Application.Current != null)
            {
                if (isMainPage)
                    Application.Current.MainPage = contentPage;
                else if (Application.Current.MainPage != null)
                    Application.Current.MainPage.Navigation.PushAsync(contentPage);
            }
        }
        internal async static void LogOut()
        {
            if (Application.Current?.MainPage != null)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert("Confirm Logout", "Are you sure you want to log out?", "Yes", "No");
                if (answer)
                {
                    Application.Current.MainPage = new LandingPage(); LoggedInUser = new Staff();
                    await Application.Current.MainPage.DisplayAlert("Logged Out", "You have been logged out.", "OK");
                }
            }
        }
        internal static void ShowMessage(string message)
        {
            if (Application.Current?.MainPage != null)
                Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
        }

        //DATABASE 
        internal static Staff LoggedInUser = new Staff();
        internal static ObservableCollection<Role> GetRoles()
        {
            return new ObservableCollection<Role>
        {
            new Role { Name = "Doctor", Permissions = new ObservableCollection<Permission>
                {
                    new Permission { Name = "View Patient Record", IsGranted = true },
                    new Permission { Name = "Edit Patient Record", IsGranted = true },
                    new Permission { Name = "Delete Patient Record", IsGranted = true },
                    new Permission { Name = "Manage Staff", IsGranted = true }
                }},
            new Role { Name = "Nurse", Permissions = new ObservableCollection<Permission>
                {
                    new Permission { Name = "View Patient Record", IsGranted = true },
                    new Permission { Name = "Edit Patient Record", IsGranted = true }, //TODO 
                    new Permission { Name = "Delete Patient Record", IsGranted = false },
                    new Permission { Name = "Manage Staff", IsGranted = false }
                }},
            new Role { Name = "Others", Permissions = new ObservableCollection<Permission>
                {
                    new Permission { Name = "View Patient Record", IsGranted = true },
                    new Permission { Name = "Edit Patient Record", IsGranted = false },
                    new Permission { Name = "Delete Patient Record", IsGranted = false },
                    new Permission { Name = "Manage Staff", IsGranted = false }
                }}
        };
        }

        internal static async Task<ObservableCollection<Staff>> GetStaffs()
        {
            ObservableCollection<Staff> StaffList = new ObservableCollection<Staff>();
            try
            {
                var tempStaffList = await new FirebaseClass().GetAllStaffsAsync();
                if (tempStaffList != null)
                {
                    tempStaffList = tempStaffList.Where(P => P.Company == LoggedInUser.Company).ToList();
                    tempStaffList.Remove(tempStaffList.Where(p => p.Designation == "Admin").First());
                    if (tempStaffList.Where(p => p.StaffId == LoggedInUser?.StaffId).FirstOrDefault() != null)
                        tempStaffList.Remove(tempStaffList.Where(p => p.StaffId == LoggedInUser?.StaffId).First());
                }
                StaffList = new ObservableCollection<Staff>(tempStaffList ?? new List<Staff>());
            }
            catch (Exception) { }
            return StaffList;
        }

        internal async static Task<ObservableCollection<Patient>> GetPatients()
        { 
            ObservableCollection<Patient> PatientList = new ObservableCollection<Patient>();
            try
            {
                var tempPatientList = await new FirebaseClass().GetAllPatientsAsync();
                PatientList = new ObservableCollection<Patient>(tempPatientList ?? new List<Patient>());
            }
            catch (Exception) { }
            return PatientList;
        }

        internal static async Task<string> StaffFieldsAreValid(Staff staff, bool _IS_NEW)
        {
            if (string.IsNullOrEmpty(staff.FirstName) || string.IsNullOrEmpty(staff.LastName) || string.IsNullOrEmpty(staff.Email)
                 || string.IsNullOrEmpty(staff.Role) || string.IsNullOrEmpty(staff.Designation) || string.IsNullOrEmpty(staff.Username)
                 || string.IsNullOrEmpty(staff.Password) || string.IsNullOrEmpty(staff.Company) || string.IsNullOrEmpty(staff.Phone))
                return "Please fill all information";
            else if (!IsValidEmailFormat(staff.Email)) return "Email is not valid";
            else if (_IS_NEW)
            {
                Staff? user = await new FirebaseClass().GetStaffAsync(staff.StaffId);
                if (user == null) return "";
                else return "Username is already existing";
            }
            else return "";
        }

        internal static async Task<string> PatientFieldsAreValid(Patient patient, bool _IS_NEW)
        {
            if (string.IsNullOrEmpty(patient.FirstName) || string.IsNullOrEmpty(patient.LastName) || string.IsNullOrEmpty(patient.EmailAddress)
            || string.IsNullOrEmpty(patient.ContactNumber) || string.IsNullOrEmpty(patient.Gender) || string.IsNullOrEmpty(patient.MedicalHistory)
                || string.IsNullOrEmpty(patient.Password) || string.IsNullOrEmpty(patient.Company) || string.IsNullOrEmpty(patient.PatientID))
                return "Please fill all information";
            else if (!IsValidEmailFormat(patient.EmailAddress)) return "Email is not valid";
            else if (_IS_NEW)
            {
                Patient? user = await new FirebaseClass().GetPatientAsync(patient.PatientID);
                if (user == null) return "";
                else return "Patient ID is already existing";
            }
            else return "";
        }
    }

}
