using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amebook.Models;
using Microsoft.AspNet.Identity;

namespace Amebook.Controllers
{
    public class FriendsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Friends

        [Authorize]
        public ActionResult Index()
        {
            
            if (Request.IsAjaxRequest())
            {
                var userId = User.Identity.GetUserId();
                var count = db.Invitations.Count(x => x.AccountId == userId && x.Used == false && x.ExpirationDate > DateTime.Now);
                if (count < 5)
                {
                    Invitation invitation = new Invitation();
                    invitation.AccountId = User.Identity.GetUserId();
                    db.Invitations.Add(invitation);
                    db.SaveChanges();
                    return Json(invitation.InvitationKey, JsonRequestBehavior.AllowGet);
                }
                return Json("TooMuchKeys", JsonRequestBehavior.AllowGet);
            }

            string currentUserId = User.Identity.GetUserId();
            var invitations =
                db.Invitations.Where(
                    m => m.Used == false && m.ExpirationDate > DateTime.Now && m.AccountId == currentUserId)
                    .ToList();

            return View(invitations);
        }

        [Authorize]
        public JsonResult AddFriend(string key)
        {

            var currentUserId = User.Identity.GetUserId();

            if (key == "")
                return Json("Podaj klucz", JsonRequestBehavior.AllowGet);
            if (!db.Invitations.Any(m => m.InvitationKey == key))
                return Json("Nie ma takiego klucza", JsonRequestBehavior.AllowGet);
            var invitation = db.Invitations.Single(m => m.InvitationKey == key);

            if (invitation.ExpirationDate < DateTime.Now)
                return Json("Klucz jest nieaktualny", JsonRequestBehavior.AllowGet);
            if(invitation.Used)
                return Json("Klucz został już wykorzystany",JsonRequestBehavior.AllowGet);
            if (invitation.AccountId == currentUserId)
                return Json("Nie można dodać samego siebie", JsonRequestBehavior.AllowGet);

            //aktualnie zalogowany user
            var user = db.Accounts.Single(m => m.AccountId == currentUserId);

            //sprawdzenie czy juz posiada tego goscia w znajomych
            if(user.Friends.Any(m => m.FriendId == invitation.AccountId))
                return Json("Posiadasz już w znajomych tego użytkownika", JsonRequestBehavior.AllowGet);
            
            var friend = new Friend
            {
                AccountId = currentUserId,
                FriendId = invitation.AccountId,
                ConnectionDate = DateTime.Now
            };

            user.Friends.Add(friend);

            //zmiana id na tego ktory wygenerowal klucz i dodanie do jego znajomych aktualnie zalogowanego
            var user2 = db.Accounts.Single(m => m.AccountId == invitation.AccountId);
            var friend2 = new Friend
            {
                AccountId = user2.AccountId,
                FriendId = currentUserId,
                ConnectionDate = friend.ConnectionDate
            };
            user2.Friends.Add(friend2);

            db.Accounts.AddOrUpdate(user);
            db.Accounts.AddOrUpdate(user2);
            invitation.Used = true;
            db.Invitations.AddOrUpdate(invitation);
            db.SaveChanges();
            
            return Json("Znajomy dodany poprawnie", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public JsonResult RemoveKey(string key)
        {
            var currentUser = User.Identity.GetUserId();
            var keyToRemove = db.Invitations.Single(m => m.InvitationKey == key);
            if (keyToRemove.AccountId != currentUser) return Json(new {success = false}, JsonRequestBehavior.AllowGet);
            db.Invitations.Remove(keyToRemove);
            db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}