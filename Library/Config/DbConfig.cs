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
        public static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            string connectionString = config.GetConnectionString("SocialMedia") ?? "";

            return connectionString;
        }
    }
}
