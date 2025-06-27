using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MindflowAI.Data;

public class MindflowAIDbContextFactory : IDesignTimeDbContextFactory<MindflowAIDbContext>
{
    public MindflowAIDbContext CreateDbContext(string[] args)
    {
        MindflowAIEfCoreEntityExtensionMappings.Configure();
        var configuration = BuildConfiguration();

        var builder = new DbContextOptionsBuilder<MindflowAIDbContext>()
            .UseSqlServer(configuration.GetConnectionString("Default"));

        return new MindflowAIDbContext(builder.Options);
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}