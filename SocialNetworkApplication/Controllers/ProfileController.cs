using SocialNetworkApplication.ActionFilters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SocialNetworkApplication.Controllers
{
    [LoginFilter]
    public class ProfileController : Controller
    {
        Models.ClientsEntities database = new Models.ClientsEntities();

        // GET: Profile
        public ActionResult Index()
        {
            ViewBag.theFollowers = database.FriendLinks;
            return View(database.Profiles);
        }

        public ActionResult ViewMessages(int id)
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            int sender = database.Profiles.SingleOrDefault(p => p.user_id == user_id).profile_id;
            ViewBag.receiver = id;
            return View(database.Messages.Where(m => m.receiver == id && m.sender == sender
                                            || (m.receiver == sender && m.sender == id)));
        }

        public ActionResult CreateMessage()
        {
            int id = int.Parse(Session["user_id"].ToString());
            ViewBag.theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            return View();
        }

        [HttpPost]
        public ActionResult CreateMessage(int id, FormCollection collection)
        {
            try
            {
                int user_id = int.Parse(Session["user_id"].ToString());
                int sender = database.Profiles.SingleOrDefault(p => p.user_id == user_id).profile_id;      

                Models.Message newMessage = new Models.Message()
                {
                    sender = sender,
                    receiver = id,
                    message1 = collection["message1"],
                    timestamp = DateTime.Now,
                    read = 0
                };
                database.Messages.Add(newMessage);
                database.SaveChanges();

                return RedirectToAction("ViewMessages", new { id = id });
            }
            catch
            {
                return View();
            }
        }
  


    public ActionResult FriendRequestIndex()
        {
            int id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            ViewBag.theFollowers = database.FriendLinks;
            return View(theProfile);
        }

        public ActionResult ApproveRequest(int id)
        {           
            Models.FriendLink theFriendRequest = database.FriendLinks.SingleOrDefault(c => c.requester == id);
            theFriendRequest.read = 1;
            theFriendRequest.status = "Approved";
            theFriendRequest.approved = 1;
            database.SaveChanges();
            return RedirectToAction("FriendRequestIndex", new { id = theFriendRequest.requester });
        }
        
        public ActionResult ReadMessage(int id)
        {          
            Models.Message theMessage = database.Messages.SingleOrDefault(m => m.receiver == id);
            theMessage.read = 1;
            database.SaveChanges();
            return RedirectToAction("ViewMessages", new { id = id});

        }
        
        public ActionResult DeleteRequest(int id)
        {
            Models.FriendLink theFriendRequest = database.FriendLinks.SingleOrDefault(c => c.requester == id);
            database.FriendLinks.Remove(theFriendRequest);
            database.SaveChanges();
            return RedirectToAction("FriendRequestIndex", new { id = theFriendRequest.requester });
        }
        

        public ActionResult FollowUser(int id)
        {
            int user_id = int.Parse(Session["user_id"].ToString());        
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == user_id);
            Models.FriendLink friend = new Models.FriendLink()
            {
                requester = theProfile.profile_id,
                requested = id,
                timestamp = DateTime.Now,
                status = "To Be Approved",
                read = 0,
                approved = 0
            };

            database.FriendLinks.Add(friend);
            database.SaveChanges();

            return RedirectToAction("Index", new { id = theProfile.profile_id });
        }


        public ActionResult UnFollowUser(int id)
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == user_id);
            Models.FriendLink theFriend = database.FriendLinks.SingleOrDefault(p => p.requested == id);
            database.FriendLinks.Remove(theFriend);

            database.SaveChanges();

            return RedirectToAction("Index", new { id = theProfile.profile_id });
        }
        

        public ActionResult LikePicture(int id)
        {
            // TODO: Add insert logic here      
            int user_id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == user_id);
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);

            Models.Like likePicture = new Models.Like()
            {
                picture_id = thePicture.picture_id,
                profile_id = theProfile.profile_id,
                timestamp = DateTime.Now,
                read = 1
            };

            database.Likes.Add(likePicture);
            database.SaveChanges();

            return RedirectToAction("Details", new { id = thePicture.profile_id });
        }


        public ActionResult UnLikePicture(int id)
        {
            Models.Like likePicture = database.Likes.SingleOrDefault(p => p.picture_id == id);
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);
            database.Likes.Remove(likePicture);
            database.SaveChanges();

            return RedirectToAction("Details", new { id = thePicture.profile_id });
        }


        public ActionResult Search(string name)
        {                 
            IEnumerable<Models.Profile> result = database.Profiles.Where(p => (p.first_name + " " + p.last_name).Contains(name));
            return View("Index", result);
        }

        // GET: Profile/Details/5
        public ActionResult Details(int id)
        {
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.profile_id == id);
            ViewBag.theLike = database.Likes;
            return View(theProfile);
        }

        // GET: Profile/Create
        public ActionResult Create()
        {         
            return View();
        }

        // POST: Profile/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {           

            try
            {
                // TODO: Add insert logic here
                int id = int.Parse(Session["user_id"].ToString());         

                    Models.Profile newProfile = new Models.Profile()
                    {
                        first_name = collection["first_name"],
                        last_name = collection["last_name"],
                        notes = collection["notes"],
                        gender = collection["gender"],
                        user_id = id
                    };

                    database.Profiles.Add(newProfile);
                    database.SaveChanges();
        

                return RedirectToAction("Index", "Profile");
            }
            catch
            {
                return View();
            }
        }

        // GET: Profile/Edit/5
        public ActionResult Edit()
        {
            int id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            return View(theProfile);
        }

        // POST: Profile/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {         
            try
            {
                // TODO: Add update logic here
                int id = int.Parse(Session["user_id"].ToString());

                Models.Profile newProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);

                newProfile.first_name = collection["first_name"];
                newProfile.last_name = collection["last_name"];
                newProfile.notes = collection["notes"];
                newProfile.gender = collection["gender"];
                newProfile.user_id = id;
               
                database.SaveChanges();

                return RedirectToAction("Index", "Profile");
            }
            catch
            {
                return View();
            }
        }
    }
}
