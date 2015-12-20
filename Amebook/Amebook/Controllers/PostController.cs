using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amebook.Crypto;
using Amebook.Models;
using Amebook.ViewModels;
using Microsoft.AspNet.Identity;

namespace Amebook.Controllers
{
    public class PostController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Post
        [Authorize]
        public ActionResult Index(string id)
        {
            var currentId = User.Identity.GetUserId();
            var currentUser = db.Accounts.Single(x => x.AccountId==currentId);
            var post = currentUser.Posts.Single(x => x.OrginId == id);
            var model = new PostViewModel();
            var content = TextEncryption.DecryptionPost(post, currentUser);
            model.Author = post.Author;
            model.Content = content;
            model.Date = post.Date;
            model.Id = post.OrginId;
            model.Rated = post.Rated;
            model.Plus = post.Plus;
            model.Minus = post.Minus;

            return View(model);
        }

        [Authorize]
        public ActionResult GetComments(string orginId)
        {
            var model = new List<CommentViewModel>();
            var currentId = User.Identity.GetUserId();
            var currentUser = db.Accounts.Single(x=>x.AccountId== currentId);

            var post = currentUser.Posts.Single(x => x.OrginId == orginId);
            foreach (var comment in post.Comments.OrderBy(x=>x.Date))
            {
                var modelComment = new CommentViewModel
                {
                    Date = comment.Date,
                    Author = comment.Author,
                    Id = comment.OrginId,
                };
                modelComment.Content = TextEncryption.DecryptionComment(comment, currentUser);
                model.Add(modelComment);
            }
            return PartialView("_Comments", model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult AddComment(string id, string content)
        {

            var currentId = User.Identity.GetUserId();
            var account = db.Accounts.Single(x => x.AccountId == currentId);
            var comment = new Comment
            {
                CommentId = Guid.NewGuid().ToString(),
                Author = User.Identity.GetUserName(),
                Date = DateTime.Now
            };
            comment.OrginId = comment.CommentId;

            var posts = db.Posts.Where(x => x.OrginId == id).ToList();
            foreach (var post in posts)
            {
                var commentTmp = new Comment
                {
                    CommentId = Guid.NewGuid().ToString(),
                    OrginId = comment.OrginId,
                    Author = comment.Author,
                    Date = comment.Date,
                };
                commentTmp = TextEncryption.EncryptionComment(commentTmp, content, post.Account);
                post.Comments.Add(commentTmp);
                db.Posts.AddOrUpdate(post);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Post", new {id = id});
        }
    }
}