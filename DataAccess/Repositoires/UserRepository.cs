using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositoires
{

    public class UserRepository : IUserRepository<Author>
    {
        PostsDbContext db;
        public UserRepository(PostsDbContext _db)
        {
            this.db = _db;
        }
        public void Add(Author entity)
        {
            
            db.Authors.Add(entity);
            db.SaveChanges();
        }

        public Author Find(string userEmail)
        {
            var author = db.Authors.SingleOrDefault(b => b.UserEmail.ToLower() == userEmail.ToLower());
            if (author != null)
                return author;
            else
                return null;
        }
    }
}
