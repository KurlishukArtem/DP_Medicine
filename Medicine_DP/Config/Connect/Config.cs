using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Medicine_DP.Config.Connect
{
    public static class Config
    {
        public static readonly string connection;
        public static readonly MySqlServerVersion version = new MySqlServerVersion(new Version(8, 0, 11));

        static Config()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            connection = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
