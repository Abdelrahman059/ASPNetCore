using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
    public class PostsDbContext : DbContext
    {
        public PostsDbContext(DbContextOptions<PostsDbContext> options) : base(options)
        {

        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Author>().HasData(
new Author { Id = 1, FullName = "Abdelrahman", UserEmail = "Abdelrahman@gmail.com", Password = "0592141324" },
new Author { Id = 2, FullName = "mosab", UserEmail = "a@gmail.com", Password = "123456" });

            modelBuilder.Entity<Post>().HasData(new[] {
           new Post
           {
               Id=1 ,
               Title = "Hi Hi Iam Seeding",
               Subtitle = "Hi Hi Iam Seeding",
               Body = "Hi Hi Iam Seeding",
               AuthorId = 1,
               CreatedDate = DateTime.Now
           },
           new Post
           {
               Id=2 ,
               Title = "Hi Hi Iam Seeding",
               Subtitle = "Hi Hi Iam Seeding",
               Body = "Hi Hi Iam Seeding",
               AuthorId = 2,
               CreatedDate = DateTime.Now
           } 
            });
        }
    }
}






