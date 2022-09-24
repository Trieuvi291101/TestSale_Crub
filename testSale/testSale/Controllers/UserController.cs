using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using testSale.Models;

namespace testSale.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        DataClasses1DataContext data = new DataClasses1DataContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UserView(string searchString)
        {
            if (Session["idUser"] != null)
            {
                var links = from l in data.users select l;

                if (!String.IsNullOrEmpty(searchString))
                {
                    links = links.Where(s => s.phone.Contains(searchString));
                }

                return View(links);
             }
            else
            {
                return RedirectToAction("Login");
            }
        }
        public ActionResult Create()
        {
            //ViewData["ROLE"] = new SelectList(data.users, "user_role");
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection form, user u)
        {
            int p = data.users.Select(s => s.id).Max();

            u.id = p;
            u.id++;

            u.password = GetMD5(u.password);
            data.users.InsertOnSubmit(u);
            data.SubmitChanges();
            return RedirectToAction("UserView");
        }
        public ActionResult Edit(int id)
        {
            var p = data.users.Where(s => s.id == id).FirstOrDefault();
            return View(p);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection form)
        {
            var p = data.users.Where(s => s.id == id).FirstOrDefault();

            p.first_name = form["first_name"];
            p.last_name = form["last_name"];
            p.email = form["email"];
            p.phone = form["phone"];
            p.username = form["username"];
            p.password = form["password"];
            p.user_role = form["user_role"];
            p.password = GetMD5(p.password);
            data.SubmitChanges();
            //UpdateModel(p);

			return RedirectToAction("UserView");
        }
		public ActionResult Delete(int id)
        {
            var p = data.users.FirstOrDefault(s => s.id == id);
            return View(p);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormatException form)
        {
            var v = data.users.FirstOrDefault(s=>s.id == id);
            data.users.DeleteOnSubmit(v);
            data.SubmitChanges();
            return RedirectToAction("UserView");
        }

        public ActionResult Login()
        {
            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (ModelState.IsValid)
            {


                var pw = GetMD5(password);
                var d = data.users.Where(s => s.username.Equals(username) && s.password.Equals(password)).ToList();
                if (d.Count() > 0)
                {
                    //add session
                    Session["FullName"] = d.FirstOrDefault().first_name + " " + d.FirstOrDefault().last_name;
                    Session["Email"] = d.FirstOrDefault().email;
                    Session["idUser"] = d.FirstOrDefault().id;
                    return RedirectToAction("UserView");
                }
                else
                {
                    ViewBag.error = "Login failed";
                    return RedirectToAction("Login");
                }
            }
            return View();
        }


        //Logout
        public ActionResult Logout()
        {
            Session.Clear();//remove session
            return RedirectToAction("Login");
        }



        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }


    }
}