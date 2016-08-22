using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FitBitMVC.Models;
using FitBitMVC.ViewModels;
using Microsoft.Owin.Security.Google;

namespace FitBitMVC.Controllers
{
    public class GroupsController : Controller
    {
        private FitbitContext db = new FitbitContext();

        // GET: Groups
        public ActionResult Index()
        {
            return View(db.Groups.ToList());
        }

        // GET: Groups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group userGroup = db.Groups.Find(id);
            if (userGroup == null)
            {
                return HttpNotFound();
            }
            return View(userGroup);
        }

        // GET: Groups/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Enabled")] Group Group)
        {
            if (ModelState.IsValid)
            {
                db.Groups.Add(Group);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Group);
        }

        // GET: Groups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Group Group = db.Groups.Include(g => g.Users).Single(g => g.ID == id);
            Group Group = null;
            UserGroupViewModel userGroup = null;
            try
            {
                Group = db.Groups.Single(g => g.ID == id);
                //if (Group != null) GetUsersInGroup(Group);
                userGroup = new UserGroupViewModel() {FbGroup = Group};
                //userGroup.FbGroups.Add(Group);
            }
            catch (Exception e)
            {
                
            }
            
            if (Group == null)
            {
                return HttpNotFound();
            }
            return View(userGroup);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddMember(UserGroupViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.FitbitUsers.Add(model.FBUser);
                    //select the groups from the database that we are going to add this user to
                    //var groups = from g1 in db.Groups
                    //    join g2 in userGroupViewModel.FbGroups
                    //        on g1.ID equals g2.ID
                    //    select g1;
                    var group = db.Groups.Single(g => g.ID == model.FbGroup.ID);
                    
                    group.Users.Add(model.FBUser);
                    
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors);
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine($"Exception while adding group member: {e.Message}");
            }

            return View(model.FBUser);
        }

        private void GetUsersInGroup(Group group)
        {
            //var viewModel = new List<AssignedUsers>();

            //foreach (var user in group.Users)
            //{
            //    viewModel.Add(new AssignedUsers
            //    {
            //        FitbitID = user.FitbitID,
            //        FirstName = user.FirstName,
            //        LastName = user.LastName
            //    });
            //}

            var viewModel = group.Users;

            ViewBag.GroupUsers = viewModel;

        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Enabled")] Group userGroup)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userGroup).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(userGroup);
        }

        // GET: Groups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group Group = db.Groups.Find(id);
            if (Group == null)
            {
                return HttpNotFound();
            }
            return View(Group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group Group = db.Groups.Find(id);
            db.Groups.Remove(Group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class AssignedUsers
    {
        public string FitbitID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    
}
