using BulletinBoard;
using QRCoder;
using SocialNetworkApplication.ActionFilters;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace SocialNetworkApplication.Controllers
{
    public class HomeController : Controller
    {

        Models.ClientsEntities database = new Models.ClientsEntities();

  
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        // POST: Home
        [HttpPost]
        public ActionResult Index(FormCollection collection)
        {
            //find the user record
            string username = collection["username"];
            Models.User theUser = database.Users.SingleOrDefault(u => u.username.Equals(username));

            if (theUser != null && Crypto.VerifyHashedPassword(theUser.password_hash, collection["password_hash"]))
            {
                Session["user_id"] = theUser.user_id;
                int id = int.Parse(Session["user_id"].ToString());
                Models.Profile theProfile = database.Profiles.SingleOrDefault(u => u.user_id == id);

                //Verifying if the profile for this user already exists
                if (theProfile != null)
                {
                    //If yes, return to profile index
                    return RedirectToAction("Index", "Profile");
                }

                //Verifying if the validation code is not null
                if (theUser.secret != null)
                {
                    Totp totp = new Totp(theUser.secret);
                    string theCode = totp.AuthenticationCode;
                    //If yes not null, return the user to profile index
                    if (theCode.Equals(collection["validation"]))
                    {
                        Session["user_id"] = theUser.user_id;
                        return RedirectToAction("Index", "Profile");
                    }
                    //If yes null, return an error message
                    else
                    {
                        ViewBag.error = "Wrong Username/Password/2FA combination!";
                        return View();
                    }
                }
                //If not, return to profile creation
                else
                {
                    Session["user_id"] = theUser.user_id;
                    return RedirectToAction("Create", "Profile");
                }
            }
            else
            {
                ViewBag.error = "Wrong Username/Password combination!";
                return View();
            }

        }

        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // GET: Home/Register
        public ActionResult Register()
        {
            // Two-factor authentication
            // Secret that has 16 chars A-Z, not 1, but 2-7
            string secret = RandomBase32String(16);
            string otpauth = "otpauth://totp/coolapplication:someaccount?secret=" + secret + "&issuer=coolapplication";

            // To generate a QR code
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            ImageConverter converter = new ImageConverter();
            ViewBag.QRCode = (byte[])converter.ConvertTo(qrCodeImage, typeof(byte[]));
            Session["secret"] = secret;

            return View();
        }

        private static Random random = new Random();

        private static string RandomBase32String(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ234567";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // POST: Home/Register
        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            try
            {
                // Two-factor authentication
                string secret = Session["secret"].ToString();
                Totp totp = new Totp(secret);

                String theSecret = null;

                if(totp.AuthenticationCode == collection["validation"])
                {
                    // Use the 2FA as a login feature
                    // Add this secret to the user information
                    theSecret = secret;
                }

                // TODO: Add insert logic here
                string username = collection["username"];

                Models.User theUser = database.Users.SingleOrDefault(u => u.username.Equals(username));

                if (theUser != null)
                {
                    return RedirectToAction("Register", "Home");
                }

                Models.User newUser = new Models.User()
                {
                    username = collection["username"],
                    password_hash = Crypto.HashPassword(collection["password_hash"]),
                    secret = theSecret
                };

                database.Users.Add(newUser);
                database.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            catch
            {
                return View();
            }
        }



        // GET: Home/Edit
        public ActionResult Edit()
        {
            int id = int.Parse(Session["user_id"].ToString());
            Models.User theUser = database.Users.SingleOrDefault(p => p.user_id == id);
            return View(theUser);
        }

        // POST: Home/Edit
        [HttpPost]
        public ActionResult Edit(FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                int id = int.Parse(Session["user_id"].ToString());
                Models.User theUser = database.Users.SingleOrDefault(p => p.user_id == id);
                ViewBag.Profile = database.Profiles.SingleOrDefault(p => p.profile_id == id);

                theUser.password_hash = Crypto.HashPassword(collection["password_hash"]);      
                theUser.user_id = id;

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
