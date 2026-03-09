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


        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CommunityMessageRead>()
            .HasIndex(x => new { x.MessageId, x.UserId })
            .IsUnique();

        modelBuilder.Entity<CommunityMessageRead>()
            .HasOne(x => x.Message)
            .WithMany(m => m.Reads)
            .HasForeignKey(x => x.MessageId);

        modelBuilder.Entity<WatchHistory>()
    .HasOne(x => x.AppUser)
    .WithMany()
    .HasForeignKey(x => x.AppUserId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<WatchHistory>()
            .HasOne(x => x.Movie)
            .WithMany()
            .HasForeignKey(x => x.MovieId)
            .OnDelete(DeleteBehavior.Cascade);
    }


    public DbSet<CommunityMessageRead> CommunityMessageReads => Set<CommunityMessageRead>();
    public DbSet<WatchHistory> WatchHistories { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<UserMovie> UserMovies { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<MovieGenre> MovieGenres { get; set; }
    public DbSet<MovieTag> MovieTags { get; set; }
    public DbSet<MoviePerson> MoviePeople { get; set; }
    public DbSet<ReviewMovie> ReviewMovies { get; set; }
    public DbSet<MovieStatistics> MovieStatistics { get; set; }
    public DbSet<Season> Seasons { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<Slider2> Sliders2 { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<UserMembership> UserMemberships { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<WatchList> WatchLists { get; set; }
    public DbSet<CommunityMessage> CommunityMessages => Set<CommunityMessage>();
    public DbSet<CommunityGroup> CommunityGroups => Set<CommunityGroup>();


}
