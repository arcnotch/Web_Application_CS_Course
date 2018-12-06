using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppProject.Models
{
    public class Post
    {
        public int PostID { get; set; }
        public string Title { get; set; }
        public string WrittenBy { get; set; }
        public string Website { get; set; }
        public DateTime PublishedDate { get; set; }
        public string Text { get; set; }
        public bool IsImage { get; set; }
        public string Image { get; set; }
        public bool IsVideo { get; set; }
        public string Video { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}