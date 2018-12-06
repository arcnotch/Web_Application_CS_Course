using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AppProject.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int PostID { get; set; }
        public string Title { get; set; }
        public string WrittenBy { get; set; }
        public string Website { get; set; }
        public DateTime PublishDate { get; set; }
        public string Text { get; set; }
        public virtual Post Post { get; set; }
    }
}