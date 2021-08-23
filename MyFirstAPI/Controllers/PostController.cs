using blogWebAPI.Authentication;
using DataAccess.Models;
using Base.BaseModels;
using DataAccess.Models.Repositoires;
using DataAccess.Repositoires;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Base.Hubs;
using Microsoft.AspNetCore.Authorization;

namespace blogWebAPI.Controllers
{

    public class PostController : Controller
    {

        private readonly IHostingEnvironment hosting;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPostsRepository<Post> postRepository;
        private readonly IUserRepository<Author> UserRepository;
        private readonly Authinticate authinticate;
        readonly PostHub postHub;

        public PostController(Authinticate authinticate, IPostsRepository<Post> postRepository, IUserRepository<Author> UserRepository, IHttpContextAccessor _httpContextAccessor, IHostingEnvironment hosting, PostHub postHub)
        {
            this.hosting = hosting;
            this._httpContextAccessor = _httpContextAccessor;
            this.authinticate = authinticate;
            this.postRepository = postRepository;
            this.UserRepository = UserRepository;
            this.postHub = postHub;
        }

        // GET: PostController

        public  ActionResult Index(int pageNumber  )
        {
            if(pageNumber ==0)
            { pageNumber = 1; }
            string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];

            if (cookieValueFromContext != null)
            {
                var Email = authinticate.Decode(cookieValueFromContext);
                var posts = postRepository.List();
                ViewBag.MyCookie = Email;
                var items =  posts.Skip((pageNumber - 1) * 5).Take(5).ToList();

                if (items.Count != 0)
                {
                    ViewBag.pageNumber = pageNumber;
                }
                else 
                {
                    
                 
                    return RedirectToAction("Index", "Post",1);
                }
                var NON = posts.Skip((pageNumber) * 5).Take(5).ToList();
                if(NON.Count == 0)
                {
                    ViewBag.notNow = 1;
                }

                // return View(await PaginatedList<Post>.CreateAsync(posts.AsQueryable(), pageNumber, pageSize));
                return View(items);
            }

            else
                return RedirectToAction("Login", "User");

        }

        // GET: PostController/Details/5
        public ActionResult Details(int id)
        {
            string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];

            if (cookieValueFromContext != null)
            {
                var Email = authinticate.Decode(cookieValueFromContext);
                var Post = postRepository.Find(id);

                return View(Post);
            }
            else
                return RedirectToAction("Login", "User");
        }

        // GET: PostController/Create
        public ActionResult Create()
        {
            var post = new PostViewModel();


            return View(post);
        }

        // POST: PostController/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PostViewModel post)
        {
            string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];
            var Email = authinticate.Decode(cookieValueFromContext);
            if (cookieValueFromContext != null)
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        string fileName = string.Empty;
                        if (post.imgFile != null)
                        {
                           // post.imgFile.FileName.ToLower().EndsWith("jpeg", StringComparison.OrdinalIgnoreCase
                            
                            if (post.imgFile.ContentType == "image/jpeg") 
                            {
                                 fileName = UploadFile(post.imgFile);
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, "We accept files of image just");
                                return View();
                            }

                        }




                        var author = UserRepository.Find(Email);
                        await postHub.SendMessage(author.FullName, post.Title);
                        Post DBpost = new Post
                        {

                            Title = post.Title,
                            Body = post.Body,
                            author = author,
                            ImageUrl = fileName,
                            Subtitle = post.Subtitle,
                            CreatedDate = DateTime.Now

                        };
                        //var author = UserRepository.Find(Request.Cookies["key"]);
                        ViewBag.FullName = author.FullName;

                        //  ViewBag.MyCookie = Request.Cookies["key"];

                        postRepository.Add(DBpost);

                        return RedirectToAction(nameof(Index));
                    }
                    catch
                    {
                        return View();
                    }
                }


                ModelState.AddModelError("", "You have to fill all the required fields!");
                return View();
            }
            else
                return RedirectToAction("Login", "User");
        }

        // GET: PostController/Edit/5
        public ActionResult Edit(int id)
        {
            var post = postRepository.Find(id);
            PostViewModel postDb = new PostViewModel
            {

                Title = post.Title,
                Body = post.Body,
                ImageUrl = post.ImageUrl,
                Subtitle = post.Subtitle,
                CreatedDate = post.CreatedDate
            };

            return View(postDb);
        }

        // POST: PostController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PostViewModel post)
        {

            try
            {
                string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];
                var Email = authinticate.Decode(cookieValueFromContext);
                if (cookieValueFromContext != null)
                {
                    // TODO: Add update logic here
                    string fileName = string.Empty;
                    if (post.imgFile != null)
                    {
                        // post.imgFile.FileName.ToLower().EndsWith("jpeg", StringComparison.OrdinalIgnoreCase

                        if (post.imgFile.ContentType == "image/jpeg")
                        {
                            fileName = UploadFile(post.imgFile, post.ImageUrl);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, "We accept files of image just");
                            return View();
                        }

                    }
                    


                    Post postDb = new Post
                    {

                        Title = post.Title,
                        Body = post.Body,
                        Subtitle = post.Subtitle,
                        ImageUrl = fileName
                    };

                    postRepository.Update(post.Id, postDb);

                    return RedirectToAction(nameof(Index));
                }
                else
                    return RedirectToAction("Login", "User");
            }
            catch (Exception)
            {
                return View();
            }
        }

        // GET: PostController/Delete/5
        public ActionResult Delete(int id)
        {
            var post = postRepository.Find(id);

            return View(post);
        }

        // POST: Book/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ConfirmDelete(int id)
        {
            try
            {
                string cookieValueFromContext = _httpContextAccessor.HttpContext.Request.Cookies["key"];
                var Email = authinticate.Decode(cookieValueFromContext);
                if (cookieValueFromContext != null)
                {
                    // TODO: Add delete logic here
                    postRepository.Delete(id);

                    return RedirectToAction(nameof(Index));
                }
                else
                    return RedirectToAction("Login", "User");
            }
            catch
            {
                return View();
            }
        }
        string UploadFile(IFormFile file, string imageUrl)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "uploads");

                string newPath = Path.Combine(uploads, file.FileName);
                string oldPath = Path.Combine(uploads, imageUrl);

                if (oldPath != newPath)
                {
                    System.IO.File.Delete(oldPath);
                    file.CopyTo(new FileStream(newPath, FileMode.Create));
                }

                return file.FileName;
            }

            return imageUrl;
        }


        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, "uploads");
                string fullPath = Path.Combine(uploads, file.FileName);
                file.CopyTo(new FileStream(fullPath, FileMode.Create));

                return file.FileName;
            }

            return null;
        }
    }

}
