using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using Amebook.Crypto;
using Amebook.Models;
using Amebook.ViewModels;
using Microsoft.AspNet.Identity;

namespace Amebook.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        public ActionResult Index(string key)
        {
            if (key != null)
            {
                string forcopy = key;
                return View((object)forcopy);
            }
            //if (Session["privateKey"] == null)
            //{
            //    return View();
            //}
            return View();
        }

        [Authorize]
        public ActionResult GetPosts(string currentId)
        {
            var currentUser = db.Accounts.Single(x => x.AccountId == currentId);
            var privateKey = (string)Session["privateKey"];
            var model = new List<PostViewModel>();
            foreach (var post in currentUser.Posts.OrderByDescending(x => x.Date))
            {
                var modelPost = new PostViewModel();
                var content = TextEncryption.DecryptionPost(post, currentUser);
                modelPost.Author = post.Author;
                modelPost.Content = content;
                modelPost.Date = post.Date;
                modelPost.Id = post.OrginId;
                modelPost.Rated = post.Rated;
                modelPost.Plus = post.Plus;
                modelPost.Minus = post.Minus;
                model.Add(modelPost);
            }

            return PartialView("_Posts", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult AddPrivateKey(string privateKey)
        {
            Session["privateKey"] = privateKey;
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult AddPost(string content)
        {
            var currentId = User.Identity.GetUserId();
            var post = new Post
            {
                PostId = Guid.NewGuid().ToString(),
                Date = DateTime.Now,
                Author = User.Identity.GetUserName(),
                Rated = false,
                Plus = 0,
                Minus = 0
            };
            post.OrginId = post.PostId;
            var account = db.Accounts.Single(x => x.AccountId == currentId);
            var publicKey = account.PublicKey;
            post = TextEncryption.EncryptionPost(post, content, account);
            account.Posts.Add(post);
            db.Accounts.AddOrUpdate(account);
            db.SaveChanges();

            foreach (var friend in account.Friends)
            {
                var friendTmp = db.Accounts.Single(x => x.AccountId == friend.FriendId);
                var friendPublicKey = friendTmp.PublicKey;
                var postTmp = new Post
                {
                    PostId = Guid.NewGuid().ToString(),
                    Author = post.Author,
                    Date = post.Date,
                    OrginId = post.OrginId,
                    Minus = post.Minus,
                    Plus = post.Plus,
                    Rated = post.Rated
                };
                postTmp = TextEncryption.EncryptionPost(postTmp, content, friendTmp);
                friendTmp.Posts.Add(postTmp);
                db.Accounts.AddOrUpdate(friendTmp);
                db.SaveChanges();
            }
            return View("Index");
        }

        [Authorize]
        public ActionResult Rating(string orginId, bool option)
        {
            var currentId = User.Identity.GetUserId();
            var user = db.Accounts.Single(x => x.AccountId == currentId);
            var userPost = user.Posts.Single(x => x.OrginId == orginId);
            if (userPost.Rated) return Json(new Rate {Minus = userPost.Minus, Plus = userPost.Plus}, JsonRequestBehavior.AllowGet);
            userPost.Rated = true;
            db.Posts.AddOrUpdate(userPost);
            db.SaveChanges();
            var posts = db.Posts.Where(x => x.OrginId == orginId).ToList();
            foreach (var post in posts)
            {
                if (option)
                    post.Plus++;
                else
                    post.Minus++;
                db.Posts.AddOrUpdate(post);
                db.SaveChanges();
            }
            Rate rate = new Rate
            {
                Minus = userPost.Minus++,
                Plus = userPost.Plus++
            };
            return Json(rate, JsonRequestBehavior.AllowGet);
        }
    }
}