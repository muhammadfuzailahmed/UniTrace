using Database_lab_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Database_lab_project.Controllers
{
    public class ItemController : Controller
    {
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
                return RedirectToAction("Dashboard", "Dashboard");
            }

        }
        public IActionResult SearchPage(string searchText)
        {

            List<Item> items = new List<Item>();
            int? currentUserId = HttpContext.Session.GetInt32("userID");
            ViewBag.userId = currentUserId;
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select * from items where title like @title AND is_found = @is_found";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@title", "%" + searchText + "%");
                cmd.Parameters.AddWithValue("@is_found", "not found");
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
    }
}
