using Microsoft.EntityFrameworkCore;
using MovieMagnet.Entities;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace MovieMagnet.Data;

public class MovieMagnetDbContext : AbpDbContext<MovieMagnetDbContext>
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Keyword> Keywords { get; set; }
    public DbSet<ProductionCountry> ProductionCountries { get; set; }
    public DbSet<ProductionCompany> ProductionCompanies { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<MovieCountry> MovieCountries { get; set; }
    public DbSet<MovieCompany> MovieCompanies { get; set; }
    public DbSet<MovieKeyword> MovieKeywords { get; set; }
    public DbSet<User> Users { get; set; }

    public DbSet<UserWatchList> UserWatchLists { get; set; }
    
    public DbSet<MovieWatching> MovieWatchings { get; set; }
    public MovieMagnetDbContext(DbContextOptions<MovieMagnetDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own entities here */
         builder.Entity<MovieGenre>(b =>
        {
            b.ToTable("MovieGenres");
            b.HasKey(x => new { x.MovieId, x.GenreId });
            b.HasOne(x => x.Movie).WithMany(x => x.MovieGenres).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.Genre).WithMany(x => x.MovieGenres).HasForeignKey(x => x.GenreId);
        });
        
        builder.Entity<MovieCountry>(b =>
        {
            b.ToTable("MovieCountries");
            b.HasKey(x => new { x.MovieId, x.CountryId });
            b.HasOne(x => x.Movie).WithMany(x => x.MovieCountries).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.Country).WithMany(x => x.MovieCountries).HasForeignKey(x => x.CountryId);
        });
        
        builder.Entity<MovieCompany>(b =>
        {
            b.ToTable("MovieCompanies");
            b.HasKey(x => new { x.MovieId, x.CompanyId });
            b.HasOne(x => x.Movie).WithMany(x => x.MovieCompanies).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.Company).WithMany(x => x.MovieCompanies).HasForeignKey(x => x.CompanyId);
        });
        
        builder.Entity<MovieKeyword>(b =>
        {
            b.ToTable("MovieKeywords");
            b.HasKey(x => new { x.MovieId, x.KeywordId });
            b.HasOne(x => x.Movie).WithMany(x => x.MovieKeywords).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.Keyword).WithMany(x => x.MovieKeywords).HasForeignKey(x => x.KeywordId);
        });
        
        builder.Entity<Movie>(b =>
        {
            b.ToTable("Movies");
            b.Property(x => x.Title).IsRequired().HasMaxLength(256);
            b.Property(x => x.Language).HasMaxLength(32);
            b.Property(x => x.Overview).HasMaxLength(512);
            b.HasIndex(x => x.ImdbId).IsUnique();
            b.HasMany(x => x.MovieGenres).WithOne(x => x.Movie).HasForeignKey(x => x.MovieId);
            b.HasMany(x => x.MovieCountries).WithOne(x => x.Movie).HasForeignKey(x => x.MovieId);
            b.HasMany(x => x.MovieCompanies).WithOne(x => x.Movie).HasForeignKey(x => x.MovieId);
        });
        
        builder.Entity<User>(b =>
        {
            b.ToTable("Users");
            b.Property(x => x.Username).IsRequired().HasMaxLength(32);
            b.Property(x => x.Password).IsRequired().HasMaxLength(265);
            b.Property(x => x.Email).IsRequired().HasMaxLength(32);
            b.Property(x => x.Fullname).IsRequired().HasMaxLength(32);
            b.Property(x => x.Avatar).HasMaxLength(256);
        });
        
        builder.Entity<Rating>(b =>
        {
            b.ToTable("Ratings");
            b.Property(x => x.Score).IsRequired();
            b.HasKey(x => new { x.MovieId, x.UserId });
            b.HasOne(x => x.Movie).WithMany(x => x.Ratings).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.User).WithMany(x => x.Ratings).HasForeignKey(x => x.UserId);
        });

        builder.Entity<UserWatchList>(b =>
        {
            b.ToTable("UserWatchList");
            b.Property(x => x.MovieId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.HasKey(x => new { x.MovieId, x.UserId });
            b.HasOne(x => x.Movie).WithMany(x => x.UserWatchList).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.User).WithMany(x => x.UserWatchList).HasForeignKey(x => x.UserId);
        });
        
        
        builder.Entity<MovieWatching>(b =>
        {
            b.ToTable("MovieWatchings");
            b.Property(x => x.lastViewMoment).IsRequired();
            b.HasOne(x => x.Movie).WithMany(x => x.MovieWatchings).HasForeignKey(x => x.MovieId);
            b.HasOne(x => x.User).WithMany(x => x.MovieWatchings).HasForeignKey(x => x.UserId);
        });
    }
}
