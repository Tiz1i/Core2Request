#pragma warning disable CS8618
/* 
Disabled Warning: "Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable."
We can disable this safely because we know the framework will assign non-null values when it constructs this class for us.
*/
using Microsoft.EntityFrameworkCore;

// kujdes namespacennnnn!!!!!!!/////
namespace Core2Request.Models;
// the MyContext class representing a session with our MySQL database, allowing us to query for or save data
public class MyContext : DbContext
{
    public MyContext(DbContextOptions options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentLike> CommentLikes { get; set; }

    //Ketu eshte metode, e cila tregon lidhjen mes entiteteve qe kemi
    //Te dyja krijojne modelin dmth krijojne si pune tabele pra list reciver dhe list sender
    //Me foreign key bejme lidhjen me njera tjetren pra sic eshte ne tabelen e databazes
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        {
            modelBuilder.Entity<Request>()
                .HasOne(r => r.Reciver)
                .WithMany(u => u.Sender)
                .HasForeignKey(r => r.ReciverId);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.Sender)
                .WithMany(u => u.SenderRequests)
                .HasForeignKey(r => r.SenderId);

        }
    }

}
