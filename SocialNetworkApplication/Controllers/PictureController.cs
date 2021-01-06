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
    public class PictureController : Controller
    {
        Models.ClientsEntities database = new Models.ClientsEntities();

        // GET: Picture
        public ActionResult Index()
        {
            int id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            ViewBag.theLike = database.Likes;
            return View(theProfile);
        }

        public ActionResult LikePictureDetails(int id)
        {
            Models.Like thePictureLike = database.Likes.SingleOrDefault(c => c.picture_id == id);
            thePictureLike.read = 1;
            database.SaveChanges();
            return RedirectToAction("LikePictureIndex", new { id = thePictureLike.picture_id });
        }


        public ActionResult LikePictureIndex()
        {
            int id = int.Parse(Session["user_id"].ToString());
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            ViewBag.theLike = database.Likes;
            return View(theProfile);
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
                 read = 0
             };

             database.Likes.Add(likePicture);
             database.SaveChanges();

             return RedirectToAction("Index", new { id = thePicture.profile_id });
        }


        public ActionResult UnLikePicture(int id)
        {
            Models.Like likePicture = database.Likes.SingleOrDefault(p => p.picture_id == id);
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);
            Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.picture_id == id);
            database.Likes.Remove(likePicture);
            database.SaveChanges();

            return RedirectToAction("Index", new { id = thePicture.profile_id });
        }

        

        // GET: Picture/Create
        public ActionResult Create()
        {
            int id = int.Parse(Session["user_id"].ToString());
            ViewBag.theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);
            return View();
        }

        // POST: Picture/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection, HttpPostedFileBase newPicture)
        {   
            try
            {
                // TODO: Add insert logic here 
                int id = int.Parse(Session["user_id"].ToString());
                Models.Profile theProfile = database.Profiles.SingleOrDefault(p => p.user_id == id);

                string[] types = { "image/gif", "image/jpeg", "image/png" };

                if (newPicture != null && newPicture.ContentLength > 0 && types.Contains(newPicture.ContentType))
                {               
                    Guid g = Guid.NewGuid();
                    string filename = g.ToString() + Path.GetExtension(newPicture.FileName);
                    string path = Server.MapPath("~/Images/");
                    path = Path.Combine(path, filename);
                    newPicture.SaveAs(path);                  

                    Models.Picture newPic = new Models.Picture()
                    {
                        caption = collection["caption"],
                        location = collection["location"],
                        time = collection["time"],
                        path = filename,
                        profile_id = theProfile.profile_id
                    };

                    database.Pictures.Add(newPic);
                    database.SaveChanges();

                    return RedirectToAction("Index", new { id = id });
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult MakeMyProfile(int id)
        {
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);
            thePicture.Profile.picture_id = id;
            database.SaveChanges();
            return RedirectToAction("Index", new { id = thePicture.profile_id });
        }

        // GET: Picture/Delete/5
        public ActionResult Delete(int id)
        {
            Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);
            return View(thePicture);
        }

        // POST: Picture/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                Models.Picture thePicture = database.Pictures.SingleOrDefault(p => p.picture_id == id);

                thePicture.Profile.picture_id = null;  
                string path = Server.MapPath("~/Images/" + thePicture.path);
                System.IO.File.Delete(path);
                database.SaveChanges();
           
                database.Comments.RemoveRange(thePicture.Comments);
                database.Likes.RemoveRange(thePicture.Likes);
                database.SaveChanges();

                database.Pictures.Remove(thePicture);
                database.SaveChanges();

                return RedirectToAction("Index", new { id = thePicture.profile_id });
            }
            catch
            {
                return View();
            }
        }
    }
}
