using Newtonsoft.Json;
using System;
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
                    Application.Current.MainPage = new NavigationPage(contentPage);
                else if (Application.Current.MainPage != null)
                    Application.Current.MainPage.Navigation.PushAsync(contentPage);
            }
        }
        internal static void GoBack()
        {
            if (Application.Current?.MainPage != null)
                Application.Current.MainPage.Navigation.PopAsync();
        }
        internal async static void LogOut()
        {
            if (Application.Current?.MainPage != null)
            {
                bool answer = await Application.Current.MainPage.DisplayAlert( "Confirm Logout", "Are you sure you want to log out?", "Yes", "No"); 
                if (answer)
                { 
                    Application.Current.MainPage = new NavigationPage(new LandingPage()); LoggedInUser = null; 
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
        internal static Staff? LoggedInUser; 
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
                    new Permission { Name = "Edit Patient Record", IsGranted = true },
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

        internal static ObservableCollection<Staff> GetStaffs()
        {
            return new ObservableCollection<Staff>
            {
                new Staff { StaffId = "1", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Role = "Doctor", Designation = "Surgeon", Username = "SUR 001", Password = "123" },
                new Staff { StaffId = "2", FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Role = "Nurse", Designation = "Orthopedic Nurse", Username = "ORT 001", Password = "12345678" }
            };
        }

        internal static ObservableCollection<Patient> GetPatients()
        {
           return new ObservableCollection<Patient>
{
    new Patient { PatientID = "PAT001", FirstName = "John", LastName = "Doe", DateOfBirth = new DateTime(1985, 5, 15), Gender = "Male", ContactNumber = "123-456-7890", EmailAddress = "john.doe@example.com", MedicalHistory = "No significant medical history.", Password = "123" },
    new Patient { PatientID = "PAT002", FirstName = "Jane", LastName = "Smith", DateOfBirth = new DateTime(1990, 8, 22), Gender = "Female", ContactNumber = "098-765-4321", EmailAddress = "jane.smith@example.com", MedicalHistory = "Asthma and seasonal allergies.", Password = "12345678" }
};
        }

        internal static string GetPatientID()
        {
            return $"PAT{new Random().Next(1000, 9999):D4}";
        } 

        internal static string StaffFieldsAreValid(Staff staff)
        {
            if (string.IsNullOrEmpty(staff.FirstName) || string.IsNullOrEmpty(staff.LastName) || string.IsNullOrEmpty(staff.Email)
                 || string.IsNullOrEmpty(staff.Role) || string.IsNullOrEmpty(staff.Designation) || string.IsNullOrEmpty(staff.Username)
                 || string.IsNullOrEmpty(staff.Password) || string.IsNullOrEmpty(staff.Company) || string.IsNullOrEmpty(staff.Phone))
                return "Please fill all information";
            else if (!IsValidEmailFormat(staff.Email)) return "Email is not valid";
            else return "";
        } 
    }
    class FileModel
    {
        public string file { get; set; } = "";
    }  

}
