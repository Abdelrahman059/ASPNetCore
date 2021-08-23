using Base;
using blogWebAPI.Authentication;
using DataAccess.Models;
using DataAccess.Models.Repositoires;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Web.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using DataAccess.Repositoires;
using Base.BaseModels;
using Base.Hubs;
using System.Net;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace blogWebAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class PostsAPIController : ControllerBase
    {
        private readonly IPostsRepository<Post> postRepository;
        private readonly IUserRepository<Author> userRepository;
        private readonly Authinticate authinticate;
        readonly PostHub postHub;
        public PostsAPIController(Authinticate authinticate, IPostsRepository<Post> postRepository, IUserRepository<Author> userRepository, PostHub postHub)
        {
            this.authinticate = authinticate;
            this.postRepository = postRepository;
            this.userRepository = userRepository;
            this.postHub = postHub;
        }



        [Authorize]
        // GET: api/<PostsAPI>
        [HttpGet("List")]
        public IEnumerable<Post> Get()
        {

            var posts = postRepository.List();
            return posts;
        }
        [Authorize]
        // GET api/<PostsAPI>/5

        [HttpGet("Search={id}")]
        public IActionResult Get(int id)
        {
            var post = postRepository.Find(id);
            if (post == null)
                return NotFound("the id didn't found");
            else
            {
                var Rpost = new JsonResult(post);
                return Rpost;
            }
        }
        [Authorize]
        // POST api/<PostsAPI>
        [HttpPost("Create")]
        public async Task<IActionResult> Post([FromBody] Post post)
        {

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var Useremail = authinticate.Decode(accessToken);
            if (ModelState.IsValid)
            {
                WebClient webClient = new WebClient();
                string fullpath = $@"D:\asp.net\MyFirstAPI\wwwroot\uploads\{DateTime.Now.Ticks}.jpg";
                if (post.ImageUrl != null)
                {
                    webClient.DownloadFile(post.ImageUrl, fullpath);
                    fullpath = fullpath.Replace(@"D:\asp.net\MyFirstAPI\wwwroot\uploads\", "");
                }
                //string fileName = UploadFile(post.imgFile) ?? string.Empty;
                var author = userRepository.Find(Useremail);
                await postHub.SendMessage(author.FullName, post.Title);
                Post DBpost = new Post
                {

                    Title = post.Title,
                    Body = post.Body,
                    author = author,
                    ImageUrl = fullpath,
                    Subtitle = post.Subtitle,
                    CreatedDate = DateTime.Now

                };
                postRepository.Add(DBpost);
                return Ok("Post Created");
            }
            else
                return BadRequest();

        }

        [AllowAnonymous]
        [HttpPost("authinticate")]
        public IActionResult Post([FromBody] LoginViewModel user)
        {
            var token = authinticate.Authintication(user.UserEmail, user.Password);

            if (token == null)
                return Unauthorized();

            return Ok(token);


        }
        [Authorize]
        // PUT api/<PostsAPI>/5
        [HttpPut("Edit")] //Edit?id=1
        public async Task<IActionResult> Edit(Post post)
        {

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var Useremail = authinticate.Decode(accessToken);
            if (postRepository.Find(post.Id) != null)
            {
                if (Useremail == postRepository.Find(post.Id).author.UserEmail)
                {
                    if (ModelState.IsValid)
                    {

                        WebClient webClient = new WebClient();
                        string fullpath = $@"D:\asp.net\MyFirstAPI\wwwroot\uploads\{DateTime.Now.Ticks}.jpg";
                        if (post.ImageUrl != null)
                        {
                            webClient.DownloadFile(post.ImageUrl, fullpath);
                            fullpath = fullpath.Replace(@"D:\asp.net\MyFirstAPI\wwwroot\uploads\", "");
                        }
                        var author = userRepository.Find(Useremail);
                        await postHub.SendMessage(author.FullName, post.Title);
                        Post DBpost = new Post
                        {
                            Title = post.Title,
                            Body = post.Body,
                            ImageUrl = fullpath,
                            Subtitle = post.Subtitle,
                        };
                        postRepository.Update(post.Id, DBpost);
                        return Ok("Post Edited");
                    }
                    else
                        return BadRequest();
                }
                else
                {
                    return BadRequest("you arent the author of this post");
                }

            }
            else
                return NotFound("This Id not Used");
        }


        [Authorize]
        // DELETE api/<PostsAPI>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {

            var post = postRepository.Find(id);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var Useremail = authinticate.Decode(accessToken);
            if (postRepository.Find(id) != null)
            {
                if (Useremail == postRepository.Find(id).author.UserEmail)
                {

                    postRepository.Delete(id);
                    return Ok("Done");

                }
                else
                {
                    return BadRequest("you arent the author of this post");
                }

            }
            else
                return NotFound("This Id not Used");
        }
    }

}

