using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppProject.Models
{
    public class Fan
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime Birthday { get; set; }
        public int Venteran { get; set; }
    }

    public class FanDbContext : DbContext
    {
        public DbSet<Fan> Fans { get; set; }
    }
}