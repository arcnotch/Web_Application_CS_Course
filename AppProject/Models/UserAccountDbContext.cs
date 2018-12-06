using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AppProject.Models
{
    public class UserAccountDbContext : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
    }
}