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
            if (Session["privateKey"] == null)
            {
                return View();
            }

            var currentId = User.Identity.GetUserId();
            var currentUser = db.Accounts.Single(x => x.AccountId == currentId);
            var privateKey = (string)Session["privateKey"];
            var model = new List<PostViewModel>();
            foreach (var post in currentUser.Posts)
            {
                var modelPost = new PostViewModel();
                var content = TextEncryption.DecryptionPost(post, privateKey);
                modelPost.Author = post.Author;
                modelPost.Content = content;
                modelPost.Date = post.Date;
                model.Add(modelPost);
            }
            return View(model);
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
                Author = User.Identity.GetUserName()
            };
            var account = db.Accounts.Single(x => x.AccountId == currentId);
            var publicKey = account.PublicKey;
            post = TextEncryption.EncryptionPost(post, content, publicKey);
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
                };
                postTmp = TextEncryption.EncryptionPost(postTmp, content, friendPublicKey);
                friendTmp.Posts.Add(postTmp);
                db.Accounts.AddOrUpdate(friendTmp);
                db.SaveChanges();
            }
            return null;
        }
    }
}