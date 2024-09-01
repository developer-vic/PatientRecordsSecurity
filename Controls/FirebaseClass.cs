using Firebase.Database;
using Firebase.Database.Query;

namespace PatientRecordsSecurity.Controls
{
    public class FirebaseClass
    {
        private readonly FirebaseClient _firebaseClient;
        private const string MATRIC_NO = "CS-HND-F22-3336";

        public FirebaseClass()
        {
            _firebaseClient = new FirebaseClient("https://fedpoffacs-default-rtdb.firebaseio.com/");
        }

        public async Task SaveUpdatePatientAsync(Patient patient)
        {
            await _firebaseClient
                .Child(MATRIC_NO)
                .Child("patients")
                .Child(patient.PatientID)
                .PutAsync(patient);
        }
        public async Task<List<Patient>> GetAllPatientsAsync()
        {
            try
            {
                var patients = await _firebaseClient
                    .Child(MATRIC_NO)
                    .Child("patients")
                    .OnceAsync<Patient>();

                return patients.Select(item => item.Object).ToList();
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<Patient> GetPatientAsync(string patientID)
        {
            var patient = await _firebaseClient
                .Child(MATRIC_NO)
                .Child("patients")
                .Child(patientID)
                .OnceSingleAsync<Patient>();

            return patient;
        }
        public async Task DeletePatientAsync(string patientID)
        {
            await _firebaseClient
                .Child(MATRIC_NO)
                .Child("patients")
                .Child(patientID)
                .DeleteAsync();
        }

        //STAFF FUNCTIONS
        public async Task SaveUpdateStaffAsync(Staff staff)
        {
            await _firebaseClient
                .Child(MATRIC_NO)
                .Child("staffs")
                .Child(staff.Username)
                .PutAsync(staff);
        }
        public async Task<List<Staff>> GetAllStaffsAsync()
        {
            try
            {
                var staffs = await _firebaseClient
                    .Child(MATRIC_NO)
                    .Child("staffs")
                    .OnceAsync<Staff>();

                return staffs.Select(item => item.Object).ToList();
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<Staff?> GetStaffAsync(string staffID)
        {
            try
            {
                var patient = await _firebaseClient
                    .Child(MATRIC_NO)
                    .Child("staffs")
                    .Child(staffID)
                    .OnceSingleAsync<Staff>();

                return patient;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task DeleteStaffAsync(string staffID)
        {
            await _firebaseClient
                .Child(MATRIC_NO)
                .Child("staffs")
                .Child(staffID)
                .DeleteAsync();
        }

        public async Task<bool> TryLogin(string username, string password)
        {
            try
            {
                Staff? staffAcc = await GetStaffAsync(username);
                if (staffAcc != null && staffAcc.Password == password)
                {
                    VUtils.LoggedInUser = staffAcc; return true;
                }
                else
                {
                    Patient patientAcc = await GetPatientAsync(username);
                    if (patientAcc != null && patientAcc.Password == password)
                    {
                        staffAcc = new Staff()
                        {
                            Designation = "Patient",
                            Company = patientAcc.Company,
                            Email = patientAcc.EmailAddress,
                            Password = password,
                            FirstName = patientAcc.FirstName,
                            LastName = patientAcc.LastName,
                            Username = username,
                            Phone = patientAcc.ContactNumber,
                            Role = "Others",
                            StaffId = patientAcc.PatientID,
                            PermissionsSummary = patientAcc.MedicalHistory
                        };
                        VUtils.LoggedInUser = staffAcc; return true;
                    }
                }
            }
            catch (Exception) { }
            return false;
        }

    }
}