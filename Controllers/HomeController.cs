using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace Database_lab_project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            List<string> names = new List<string>();

            string conStr = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection con = new SqlConnection(conStr))
            {
                con.Open();

                string query = "SELECT Name FROM Students";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    names.Add(reader["Name"].ToString());
                }
            }

            return View(names);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {

            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";

            using (SqlConnection conn = new SqlConnection(connectionString)) {
                conn.Open();

                string query = "select id, email, password from users";

                SqlCommand cmd = new SqlCommand(query, conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    string dbEmail = reader["email"].ToString();
                    string dbpassword = reader["password"].ToString();
                    int userId = Convert.ToInt32(reader["id"]);

                    if(username == dbEmail && password == dbpassword)
                    {
                        HttpContext.Session.SetInt32("userID", userId);
                        TempData["Message"] = "Login Successful";
                        TempData["Type"] = "success";
                        return RedirectToAction("Dashboard");
                    }

                }

            }
            TempData["Message"] = "Login Failed";
            TempData["Type"] = "error";
            return RedirectToAction("Login");
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string name, string email, string password)
        {
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "insert into users (name, email, password) values (@Name, @Email, @Password)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                cmd.ExecuteNonQuery();
                TempData["Message"] = "Account Created Successfully";
                TempData["Type"] = "success";

                return RedirectToAction("Login");
            }
        }

        public IActionResult Dashboard()
        {
            int? currentUserId = HttpContext.Session.GetInt32("userID");
            ViewBag.userId = currentUserId;
            if (currentUserId == null)
            {
                return RedirectToAction("Login");
            }
                return View();
        }

        [HttpPost]
        public IActionResult ReportFound(string itemTitle, string category, DateTime lostDate, string description, string lostLocation)
        {
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                int? currentUser = HttpContext.Session.GetInt32("userID");
                string query = "insert into items (title, description, category, lost_date, location_found, is_found, id) values (@title, @description, @category, @date, @lostLocation, @is_found, @id)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", itemTitle);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@category", category);
                cmd.Parameters.AddWithValue("@date", lostDate);
                cmd.Parameters.AddWithValue("@lostLocation", lostLocation);
                cmd.Parameters.AddWithValue("@is_found", "not found");
                cmd.Parameters.AddWithValue("@id", currentUser);
                cmd.ExecuteNonQuery();
                TempData["Message"] = "Item Reported Successfully!";
                TempData["Type"] = "success";
                return RedirectToAction("Dashboard");
            }

        }

    }
}