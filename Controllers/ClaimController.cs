using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Database_lab_project.Controllers
{
    public class ClaimController : Controller
    {
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
            return RedirectToAction("Dashboard", "Dashboard");
        }

        public IActionResult ApproveRequest(int itemId)
        {
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "update items set is_found = @is_found, requesting_user = null where item_id = @item_id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@is_found", "found");
                cmd.Parameters.AddWithValue("@item_id", itemId);
                cmd.ExecuteNonQuery();

                string query2 = "select id from items where item_id = @item_id";
                SqlCommand cmd2 = new SqlCommand(query2, conn);
                cmd2.Parameters.AddWithValue("@item_id", itemId);
                SqlDataReader reader = cmd2.ExecuteReader();
                int user_id = 0;
                while (reader.Read())
                {
                    user_id = Convert.ToInt32(reader["id"]);
                }
                reader.Close();

                string null_other_user = "update users set other_user = @other_user where id = @id";
                SqlCommand cmd3 = new SqlCommand(null_other_user, conn);
                cmd3.Parameters.AddWithValue("@other_user", DBNull.Value);
                cmd3.Parameters.AddWithValue("@id", user_id);
                cmd3.ExecuteNonQuery();

                TempData["Message"] = "Request Approved!";
                TempData["Type"] = "success";

                return RedirectToAction("Dashboard", "Dashboard");
            }
        }

        public IActionResult RejectRequest(int itemId)
        {
            string connectionString = "Server=DESKTOP-87911Q0\\SQLEXPRESS;Database=dbms_lab_project;Trusted_Connection=True;TrustServerCertificate=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "update items set requesting_user = null where item_id = @item_id";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@item_id", itemId);
                cmd.ExecuteNonQuery();

                string query2 = "select id from items where item_id = @item_id";
                SqlCommand cmd2 = new SqlCommand(query, conn);
                cmd2.Parameters.AddWithValue("@item_id", itemId);
                SqlDataReader reader = cmd2.ExecuteReader();
                int user_id = 0;
                while (reader.Read())
                {
                    user_id = Convert.ToInt32(reader["id"]);
                }
                reader.Close();

                string null_other_user = "update users set other_user = @other_user where id = @id";
                SqlCommand cmd3 = new SqlCommand(null_other_user, conn);
                cmd3.Parameters.AddWithValue("@other_user", DBNull.Value);
                cmd3.Parameters.AddWithValue("@id", user_id);
                cmd3.ExecuteNonQuery();

                TempData["Message"] = "Request Rejected!";
                TempData["Type"] = "error";

                return RedirectToAction("Dashboard", "Dashboard");
            }
        }
    }
}
