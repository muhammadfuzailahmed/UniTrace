using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Database_lab_project.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {

            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "select id, email, password from users";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string dbEmail = reader["email"].ToString();
                    string dbpassword = reader["password"].ToString();
                    int userId = Convert.ToInt32(reader["id"]);

                    if (username == dbEmail && password == dbpassword)
                    {
                        HttpContext.Session.SetInt32("userID", userId);
                        TempData["Message"] = "Login Successful";
                        TempData["Type"] = "success";
                        return RedirectToAction("Dashboard", "Dashboard");
                    }

                }
                reader.Close();

            }
            TempData["Message"] = "Login Failed";
            TempData["Type"] = "error";
            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string name, string email, string password)
        {
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string oldUser = "select email from users where email = @email";
                SqlCommand oldUserCmd = new SqlCommand(oldUser, conn);
                oldUserCmd.Parameters.AddWithValue("@email", email);

                SqlDataReader reader = oldUserCmd.ExecuteReader();
                bool found = false;
                while (reader.Read())
                {
                    if (email == reader["email"].ToString())
                    {
                        found = true;
                        break;
                    }
                }

                if (found == true)
                {
                    TempData["Message"] = "User Already Exists";
                    TempData["Type"] = "error";

                    return RedirectToAction("Register", "Auth");
                }


                string query = "insert into users (name, email, password) values (@Name, @Email, @Password)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                cmd.ExecuteNonQuery();
                TempData["Message"] = "Account Created Successfully";
                TempData["Type"] = "success";

                return RedirectToAction("Login", "Auth");
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Auth");
        }

    }
}
