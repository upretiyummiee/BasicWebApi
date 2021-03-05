using BasicWebApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BasicWebApi.Data
{
    public class AppDbContext : DbContext
    {
  
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Department> Departments { get; set; }
    }

    /*public class Factory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("workstation id=MyDatabase1232221.mssql.somee.com;packet size=4096;user id=Wizard9116_SQLLogin_1;pwd=8aqwhq8lb9;data source=MyDatabase1232221.mssql.somee.com;persist security info=False;initial catalog=MyDatabase1232221");

            return new AppDbContext(optionsBuilder.Options);
        }
    }*/

}
