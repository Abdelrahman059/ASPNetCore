using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models.Repositoires
{
    public class PostRepository : IPostsRepository<Post>
    {
        PostsDbContext db;
        public PostRepository(PostsDbContext _db)
        {
            this.db = _db;
        }
        public void Add(Post entity)
        {
            db.Posts.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
               var post = Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
        }

        public Post Find(int id)
        {
            var post = db.Posts.Include(a => a.author).SingleOrDefault(b => b.Id == id);
            return post;
        }

        public IList<Post> List()
        {

          return db.Posts.Include(a => a.author).ToList();
        }

        public void Update(int id, Post entity)
        {
               db.Posts.Update(entity);
                db.SaveChanges();
        }
    }
}
