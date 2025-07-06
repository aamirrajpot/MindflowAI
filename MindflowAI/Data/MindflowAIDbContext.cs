using Microsoft.EntityFrameworkCore;
using MindflowAI.Entities.AppUser;
using MindflowAI.Entities.Books;
using MindflowAI.Entities.EmailOtp;
using MindflowAI.Entities.WellnessCheckin;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;

namespace MindflowAI.Data;

public class MindflowAIDbContext : AbpDbContext<MindflowAIDbContext>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<EmailOtp> EmailOtps { get; set; }
    public DbSet<WellnessCheckIn>  WellnessCheckIns{ get; set; }

    public const string DbTablePrefix = "App";
    public const string DbSchema = null;

    public MindflowAIDbContext(DbContextOptions<MindflowAIDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigurePermissionManagement();
        builder.ConfigureBlobStoring();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        
        builder.Entity<Book>(b =>
        {
            b.ToTable(DbTablePrefix + "Books",
                DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(x => x.Name).IsRequired().HasMaxLength(128);
        });

        /* Configure your own entities here */

        builder.Entity<AppUser>(b =>
        {
            b.ConfigureByConvention();
            b.Property(u => u.FirstName).HasMaxLength(64);
            b.Property(u => u.LastName).HasMaxLength(64);
            b.Property(u => u.DateOfBirth);
        });
    }
}

