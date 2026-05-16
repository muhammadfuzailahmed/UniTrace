using Microsoft.AspNetCore.Mvc;

namespace Database_lab_project.Models
{
    public class Item
    {
        public int itemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime LostDate { get; set; }
        public string LocationFound { get; set; }

        public string is_found { get; set; }

        public int userId { get; set; }

        public int requestingUser { get; set; }

    }
}
