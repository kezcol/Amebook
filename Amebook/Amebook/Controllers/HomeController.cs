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
using Microsoft.AspNet.Identity;

namespace Amebook.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [Authorize]
        public ActionResult Index(string key)
        {
            string forcopy = key;

            return View((object)forcopy);
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