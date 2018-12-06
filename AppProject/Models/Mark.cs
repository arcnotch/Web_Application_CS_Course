using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppProject.Models
{
    public class Mark
    {
        public int MarkID { get; set; }
        public float Lat { get; set; } //X
        public float Lng { get; set; } //Y
        public string Title { get; set; } //Title
    }

    public class MarkDbContext : DbContext
    {
        public DbSet<Mark> Marks { get; set; }
    }
}