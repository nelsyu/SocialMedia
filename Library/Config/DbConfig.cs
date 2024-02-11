using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.Config
{
    public static class DbConfig
    {
        public static DbContextOptionsBuilder ConfigureDbContext(DbContextOptionsBuilder options)
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string dbType = config.GetConnectionString("DatabaseType") ?? "";
            string connectionString;

            switch (dbType.ToLower())
            {
                case "sqlserver":
                    connectionString = config.GetConnectionString("SqlServer") ?? "";
                    return options.UseSqlServer(connectionString);
                case "postgresql":
                    connectionString = config.GetConnectionString("PostgreSql") ?? "";
                    return options.UseNpgsql(connectionString);
                default:
                    connectionString = config.GetConnectionString("SqlServer") ?? "";
                    return options.UseSqlServer(connectionString);
            }
        }
    }
}
