using Database_lab_project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Database_lab_project.Controllers
{
    public class DashboardController : Controller
    {
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

                while (reader.Read())
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
    }
}
