using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace EVChargingStation.CARC.Domain.TruongNN;

public class FA25_SWD392_SE183428_G6_EvChargingStationFactory : IDesignTimeDbContextFactory<FA25_SWD392_SE183428_G6_EvChargingStation>
{
    public FA25_SWD392_SE183428_G6_EvChargingStation CreateDbContext(string[] args)
    {
        // For migrations and other design-time operations
        var optionsBuilder = new DbContextOptionsBuilder<FA25_SWD392_SE183428_G6_EvChargingStation>();

        // Set up configuration to read connection string
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        // Get the connection string from configuration
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        // For PostgreSQL: Npgsql.EntityFrameworkCore.PostgreSQL
        optionsBuilder.UseNpgsql(connectionString,
            npgsqlOptionsAction =>
                npgsqlOptionsAction.MigrationsAssembly(typeof(FA25_SWD392_SE183428_G6_EvChargingStation).Assembly.FullName));

        return new FA25_SWD392_SE183428_G6_EvChargingStation(optionsBuilder.Options);
    }
}