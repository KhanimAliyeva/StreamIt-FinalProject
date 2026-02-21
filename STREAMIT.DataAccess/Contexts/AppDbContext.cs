using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using STREAMIT.Core.Entities;
using System.Reflection;

namespace STREAMIT.DataAccess.Contexts;


public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.Entity<Movie>().HasQueryFilter(p => !p.IsDeleted);
        modelBuilder.Entity<TVShow>().HasQueryFilter(p => !p.IsDeleted);

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<UserMovie> UserMovies { get; set; }
    public DbSet<UserTvShow> UserTvShows { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<TVShow> TVShows { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<MovieTag> MovieTags { get; set; }
    public DbSet<MoviePerson> MoviePeople { get; set; }
    public DbSet<TvShowGenre> TvShowGenres { get; set; }
    public DbSet<TvShowTag> TvShowTags { get; set; }
    public DbSet<TvShowPerson> TvShowPeople { get; set; }
    public DbSet<ReviewMovie> ReviewMovies { get; set; }
    public DbSet<ReviewTvShow> ReviewTvShows { get; set; }
    public DbSet<MovieStatistics> MovieStatistics { get; set; }
    public DbSet<TvShowStatistics> TvShowStatistics { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }
}
