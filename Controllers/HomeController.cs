using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using Database_lab_project.Models;

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

            List<Item> items = new List<Item>();
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string query = "select * from items where is_found = @not_found";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@not_found", "not found");

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    items.Add(new Item
                    {
                        itemId = Convert.ToInt32(reader["item_id"]),
                        Title = reader["title"].ToString(),
                        Description = reader["description"].ToString(),
                        Category = reader["category"].ToString(),
                        LostDate = Convert.ToDateTime(reader["lost_date"]),
                        LocationFound = reader["location_found"].ToString(),
                        is_found = reader["is_found"].ToString(),
                        userId = Convert.ToInt32(reader["id"]),
                        requestingUser = reader["requesting_user"] == DBNull.Value
                        ? 0
                        : Convert.ToInt32(reader["requesting_user"])
                    }
                    );
                }

            }

            List<ClaimRequest> claimRequests = new List<ClaimRequest>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select i.item_id, i.title, i.category, i.location_found, i.is_found, u.name AS requesting_user_name from items i inner join users u on i.requesting_user = u.id where i.id = @currentUserId AND i.requesting_user is not null";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@currentUserId", currentUserId);

                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    claimRequests.Add(new ClaimRequest
                    {
                        ItemId = Convert.ToInt32(reader["item_id"]),
                        Title = reader["title"].ToString(),
                        Category = reader["category"].ToString(),
                        LocationFound = reader["location_found"].ToString(),
                        is_found = reader["is_found"].ToString(),
                        RequestingUser = reader["requesting_user_name"].ToString()
                    });
                }

                ViewBag.Claims = claimRequests;
                return View(items);
            }
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
        public IActionResult SearchPage(string searchText)
        {

            List<Item> items = new List<Item>();
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select * from items where title like @title";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", "%" + searchText + "%");
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item
                    {
                        itemId = Convert.ToInt32(reader["item_id"]),
                        Title = reader["title"].ToString(),
                        Description = reader["description"].ToString(),
                        Category = reader["category"].ToString(),
                        LostDate = Convert.ToDateTime(reader["lost_date"]),
                        LocationFound = reader["location_found"].ToString(),
                        is_found = reader["is_found"].ToString(),
                        userId = Convert.ToInt32(reader["id"]),
                        requestingUser = reader["requesting_user"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["requesting_user"])
                    });
                }
            }
            return View(items);

        }

        [HttpPost]
        public IActionResult ClaimForm(string name, string hiddenDetails, string lostLocation, int itemId)
        {
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                int? requestingUserId = HttpContext.Session.GetInt32("userID");
                string findItemQuery = "select id, title, is_found, requesting_user from items where item_id = @item_id";
                SqlCommand cmd1 = new SqlCommand(findItemQuery, conn);
                cmd1.Parameters.AddWithValue("@item_id", itemId);
                SqlDataReader reader = cmd1.ExecuteReader();

                int posting_user = 0;
                string title = "";
                string is_found = "";
                int requesting_user = 0;
                while (reader.Read())
                {
                    posting_user = Convert.ToInt32(reader["id"]);
                    title = reader["title"].ToString();
                    is_found = reader["is_found"].ToString();
                    requesting_user = reader["requesting_user"] == DBNull.Value
                            ? 0
                            : Convert.ToInt32(reader["requesting_user"]);
                }
                reader.Close();

                /*update Reuesting UserId in users table*/
                string updatePostingUser = "update users set other_user = @other_user where id = @posted_by_userId";
                SqlCommand cmd2 = new SqlCommand(updatePostingUser, conn);
                cmd2.Parameters.AddWithValue("@posted_by_userId", posting_user);
                cmd2.Parameters.AddWithValue("@other_user", requestingUserId);
                cmd2.ExecuteNonQuery();

                /*update Reuesting UserId in items table*/
                string updateReuestingUserId = "update items set requesting_user = @requesting_user where item_id = @item_id";
                SqlCommand cmd3 = new SqlCommand(updateReuestingUserId, conn);
                cmd3.Parameters.AddWithValue("@requesting_user", requestingUserId);
                cmd3.Parameters.AddWithValue("@item_id", itemId);
                cmd3.ExecuteNonQuery();

                /*add record in claim table*/
                string adddRecordInClaimTable = "insert into claimItem (userId, itemId, requestingUserId, hiddenDetails, lostLocation) values (@userId, @itemId, @requestingUserId, @hiddenDetails, @lostLocation)";
                SqlCommand cmd4 = new SqlCommand(adddRecordInClaimTable, conn);
                cmd4.Parameters.AddWithValue("@userId", posting_user);
                cmd4.Parameters.AddWithValue("@itemId", itemId);
                cmd4.Parameters.AddWithValue("@requestingUserID", requestingUserId);
                cmd4.Parameters.AddWithValue("@hiddenDetails", hiddenDetails);
                cmd4.Parameters.AddWithValue("@lostLocation", lostLocation);
                cmd4.ExecuteNonQuery();

                TempData["Message"] = "Claim Request Submitted!";
                TempData["Type"] = "success";
            }
            return RedirectToAction("SearchPage");
        }
        

    }
}