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
        private static void ShowMessage(string message)
        {
            if (Application.Current?.MainPage != null)
                Application.Current.MainPage.DisplayAlert("Alert", message, "OK");
        }

        internal static async Task<string> GetApiAudio(string meetingContent)
        {
            try
            {
                var client = new HttpClient(); var request = new HttpRequestMessage(HttpMethod.Post, "https://play.ht/api/transcribe");
                var content = new StringContent("{\"userId\":\"public-access\",\"platform\":\"landing_demo\",\"ssml\":\"<speak><p>" + meetingContent + "</p></speak>\",\"voice\":\"en-NG-EzinneNeural\",\"narrationStyle\":\"Neural\",\"method\":\"file\"}", null, "application/json");
                request.Content = content; var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode(); string resMraw = await response.Content.ReadAsStringAsync();
                FileModel? fileModel = JsonConvert.DeserializeObject<FileModel>(resMraw);
                return fileModel?.file ?? "";
            }
            catch (Exception)
            {
            }
            return "";
        }

        //DATABASE 
        internal static Staff? LoggedInUser;
        private static HttpClient GetVHttpClient()
        {
            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.ServerCertificateCustomValidationCallback = (sender, certificate, chain, sslPolicyErrors) =>
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                {
                    return true;
                }
                var certificateValidator = new SelfSignedCertificateValidator();
                bool isValid = false;
                if (certificate != null)
                    isValid = certificateValidator.ValidateCertificate(certificate.GetRawCertData(), "https://programmergwin.com");
                return isValid;
            };
            var mclient = new HttpClient(httpClientHandler);
            return mclient;
        }
        private async static Task<string> PostRequest(string actionname, string key, string value)
        {
            try
            {
                if (Connectivity.NetworkAccess != NetworkAccess.Internet) return "";
                using (var mclient = GetVHttpClient())
                {
                    string url = "https://programmergwin.com" + actionname;
                    if (!string.IsNullOrEmpty(key)) url += "?key=" + key;
                    if (!string.IsNullOrEmpty(value)) url += "&value=" + value;

                    mclient.Timeout = TimeSpan.FromMinutes(1);
                    using (var request = new HttpRequestMessage(new HttpMethod("POST"), url))
                    {
                        var response = await mclient.SendAsync(request);
                        if (response.IsSuccessStatusCode)
                            return await response.Content.ReadAsStringAsync();
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                            return "";
                    }
                }
            }
            catch (Exception) { }
            return "";
        }
        internal static async Task<List<string>> GetAllOrganizations()
        {
            List<string> AllOrgs = new List<string>();
            try
            {
                List<Staff>? UserList = new List<Staff>();
                string savedUserList = await PostRequest("/noteTakerGet", "savedUserList", "");
                if (!string.IsNullOrEmpty(savedUserList))
                    UserList = JsonConvert.DeserializeObject<List<Staff>>(savedUserList);
                if (UserList != null && UserList.Count != 0)
                    AllOrgs = UserList.DistinctBy(p => p.Company).Select(s => s.Company).ToList();
            }
            catch (Exception)
            {
            }
            return AllOrgs;
        }
        internal static async Task<bool> RegisterUser(Staff newUser)
        {
            try
            {
                List<Staff>? UserList = new List<Staff>();
                string savedUserList = await PostRequest("/noteTakerGet", "savedUserList", "");
                if (!string.IsNullOrEmpty(savedUserList))
                    UserList = JsonConvert.DeserializeObject<List<Staff>>(savedUserList);
                if (UserList == null) UserList = new List<Staff>();
                if (UserList.Where(p => p.Email == newUser.Email).Count() > 0)
                {
                    ShowMessage("Email Already Exist"); return false;
                }
                UserList.Add(newUser); string newMrawList = JsonConvert.SerializeObject(UserList);
                await PostRequest("/noteTakerSet", "savedUserList", newMrawList);
                LoggedInUser = newUser; ShowMessage("Registration Successful");
                return true;
            }
            catch (Exception)
            {
                ShowMessage("Registration Failed"); return false;
            }
        }
        internal static async Task<bool> LoginUser(string email, string password)
        {
            try
            {
                List<Staff>? UserList = new List<Staff>(); LoggedInUser = null;
                string savedUserList = await PostRequest("/noteTakerGet", "savedUserList", "");
                if (!string.IsNullOrEmpty(savedUserList))
                    UserList = JsonConvert.DeserializeObject<List<Staff>>(savedUserList);
                if (UserList != null && UserList.Count != 0)
                    LoggedInUser = UserList.Where(p => p.Email == email && p.Password == password).FirstOrDefault();
                //if (LoggedInUser != null)
                //{
                //    ShowMessage("Login Successful"); return true;
                //} 
                return LoggedInUser != null;
            }
            catch (Exception)
            {
            }
            ShowMessage("Incorrect Email or Password"); return false;
        }

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

        //internal static async Task<bool> AddUpdateBook(BookModel newBook, bool showMsg = true)
        //{
        //    try
        //    {
        //        if (AllBookList == null) return false;
        //        var existBook = AllBookList.Where(p => p.bookId == newBook.bookId).FirstOrDefault();
        //        if (existBook != null)
        //        {
        //            AllBookList.Remove(existBook);
        //            existBook.content = newBook.content; existBook.title = newBook.title;
        //            existBook.readUsers = newBook.readUsers;
        //            existBook.dateTime = DateTime.Now.ToString("MMM dd, yyyy - hh:mm tt");
        //        }
        //        else existBook = newBook; AllBookList.Add(existBook);

        //        string mraw = JsonConvert.SerializeObject(AllBookList);
        //        await PostRequest("/noteTakerSet", "savedBookList", mraw);
        //        if (showMsg) ShowMessage("Meeting Updated Successfully");
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        ShowMessage("Error Updating Meeting"); return false;
        //    }
        //}
        //static List<BookModel>? AllBookList = new List<BookModel>();
        //internal static async Task<List<BookModel>> GetBookList()
        //{
        //    try
        //    {
        //        string savedBookList = await PostRequest("/noteTakerGet", "savedBookList", "");
        //        if (!string.IsNullOrEmpty(savedBookList))
        //            AllBookList = JsonConvert.DeserializeObject<List<BookModel>>(savedBookList);
        //        if (AllBookList == null) AllBookList = new List<BookModel>();
        //        return AllBookList.Where(p => p.organization == LoggedInUser?.Organization).ToList();
        //    }
        //    catch (Exception)
        //    {
        //        return new List<BookModel>();
        //    }
        //}
        //internal static async Task<bool> DeleteBook(BookModel delBook)
        //{
        //    try
        //    {
        //        if (AllBookList == null) return false;
        //        var existBook = AllBookList.Where(p => p.bookId == delBook.bookId).FirstOrDefault();
        //        if (existBook != null) AllBookList.Remove(existBook);
        //        else { ShowMessage("Meeting Not Found"); return false; }

        //        string mraw = JsonConvert.SerializeObject(AllBookList);
        //        await PostRequest("/noteTakerSet", "savedBookList", mraw);
        //        ShowMessage("Meeting Deleted Successfully"); return true;
        //    }
        //    catch (Exception)
        //    {
        //        ShowMessage("Error Deleting Meeting"); return false;
        //    }
        //}

    }
    class FileModel
    {
        public string file { get; set; } = "";
    }
    public class SelfSignedCertificateValidator : ICertificateValidator
    {
        public bool ValidateCertificate(byte[] certificateData, string host)
        {
            // Add logic here to validate the certificate.
            // For development, we'll accept any certificate, but in production,
            // you should implement proper validation logic.

            // For example, you can check the certificate's thumbprint or issuer.
            // Here, we're accepting any certificate.
            return true;
        }
    }

    public interface ICertificateValidator
    {
        bool ValidateCertificate(byte[] certificateData, string host);
    }

}
