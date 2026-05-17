using Microsoft.AspNetCore.Mvc;

namespace Database_lab_project.Models
{
    public class ClaimRequest 
    {
        public int ItemId { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public string LocationFound { get; set; }
        public string is_found { get; set; }
        public string RequestingUser { get; set; }

    }
}
