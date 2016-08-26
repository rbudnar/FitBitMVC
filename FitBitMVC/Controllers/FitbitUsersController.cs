using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using FitBitMVC.Models;
using FitBitMVC.ViewModels;
using OfficeOpenXml;
using File = FitBitMVC.Models.File;

namespace FitBitMVC.Controllers
{
    public class FitbitUsersController : Controller
    {
        private FitbitContext db = new FitbitContext();

        // GET: FitbitUsers
        public ActionResult Index()
        {
            return View(db.FitbitUsers.ToList());
        }

        // GET: FitbitUsers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FitbitUser fitbitUser = db.FitbitUsers.Find(id);
            if (fitbitUser == null)
            {
                return HttpNotFound();
            }
            return View(fitbitUser);
        }

        // GET: FitbitUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: FitbitUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UniqueID,Timestamp,FirstName,LastName,Email")]FitbitUser fitbitUser)
        {
            if (ModelState.IsValid)
            {
                db.FitbitUsers.Add(fitbitUser);
                try
                {
                    db.SaveChanges();

                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            Trace.TraceInformation("Property: {0} Error: {1}",
                                validationError.PropertyName,
                                validationError.ErrorMessage);
                        }
                    }
                }
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            return View(fitbitUser);
        }

        // GET: FitbitUsers/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FitbitUser fitbitUser = db.FitbitUsers.Find(id);

            var allIDS = new HashSet<int>(db.Groups.Select(g => g.ID));
            var excludedIDs = new HashSet<int>(fitbitUser.Groups.Select(g => g.ID));
            var groupsInList = fitbitUser.Groups.ToList();
            var groupsNotInList = db.Groups.Where(g => !excludedIDs.Contains(g.ID)).Select(g => g).ToList();

            var groupIDsNotInList = db.Groups.Where(g => !excludedIDs.Contains(g.ID)).Select(g => g.ID).ToArray();

            var GroupIDsInList = fitbitUser.Groups.Select(g => g.ID).ToArray();

            UserGroupViewModel model = new UserGroupViewModel() {FBUser = fitbitUser, GroupsIn = GroupIDsInList, GroupsNotIn = groupIDsNotInList, GroupsInList = groupsInList, GroupsNotInList = groupsNotInList}; 

            if (fitbitUser == null)
                return HttpNotFound();
            
            return View(model);
        }

        // POST: FitbitUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserGroupViewModel model, int[] groupsIn, int[] groupsNotIn) 
        {
            if (ModelState.IsValid)
            {
                db.Entry(model.FBUser).State = EntityState.Modified;

                var selectedGroups = new HashSet<int>();
                if(model.GroupsIn != null)
                    selectedGroups = new HashSet<int>(model.GroupsIn);

                var fitbitUser = db.FitbitUsers.Find(model.FBUser.UniqueID);
                var userGroups = new HashSet<int>();
                if(fitbitUser.Groups != null)
                    userGroups = new HashSet<int>(fitbitUser.Groups.Select(g => g.ID));
                else
                {
                    fitbitUser.Groups = new List<Group>();
                }

                foreach (var group in db.Groups)
                {
                    if (selectedGroups.Contains(group.ID))
                    {
                        if (!userGroups.Contains(group.ID))
                        {
                            fitbitUser.Groups.Add(group);
                        }
                    }
                    else
                    {
                        if (userGroups.Contains(group.ID))
                        {
                            fitbitUser.Groups.Remove(group); //model.FBUser
                        }
                    }
                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            return View(model);
        }

        // GET: FitbitUsers/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            FitbitUser fitbitUser = db.FitbitUsers.Find(id);
            if (fitbitUser == null)
            {
                return HttpNotFound();
            }
            return View(fitbitUser);
        }

        // POST: FitbitUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            FitbitUser fitbitUser = db.FitbitUsers.Find(id);
            db.FitbitUsers.Remove(fitbitUser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult UploadUsers()
        {
            GroupsViewModel viewModel = new GroupsViewModel(db);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadUsers(GroupsViewModel groups, HttpPostedFileBase upload)
        {
            //GroupsViewModel viewModel = new GroupsViewModel(db);
            if (ModelState.IsValid)
            {
                if (upload != null && upload.ContentLength > 0)
                {
                    //store file in database so we don't have to pass it back to the view during confirmation 
                    var uploadedFile = new File
                    {
                        FileName = upload.FileName,
                        ContentType = upload.ContentType
                    };

                    using (var reader = new System.IO.BinaryReader(upload.InputStream))
                    {
                        uploadedFile.Content = reader.ReadBytes(upload.ContentLength);
                        
                    }
                    db.Files.Add(uploadedFile);
                    db.SaveChanges();

                    int id = uploadedFile.FileId;

                    try
                    {
                        //create a dropdown list for the user to map the spreadsheet columns to the database columns
                        var columnNames = new List<SelectListItem>();

                        using (var excel = new ExcelPackage(new MemoryStream(uploadedFile.Content)))
                        {
                            var sheet = excel.Workbook.Worksheets.First();
                            int i = 0;
                            foreach (var firstRowCell in sheet.Cells[1, 1, 1, sheet.Dimension.End.Column])
                            {
                                //cells in this spreadsheet library are 1 indexed, not 0-indexed
                                columnNames.Add(new SelectListItem { Value = (i+1).ToString(), Text = firstRowCell.Text });
                                i++;
                            }

                        }

                        var groupName = groups.Groups.Single( g => g.Value == groups.SelectedGroup.ToString()).Text;

                        SSConfigViewModel ssc = new SSConfigViewModel { ColumnNames = columnNames, FileID = id, GroupID = groups.SelectedGroup, GroupName = groupName };
                            
                        TempData["SSCViewModel"] = ssc;
                        return RedirectToAction("SpreadSheetConfirm");

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message);
                    }
                    
                }

                return RedirectToAction("Index");
            }

            return View(groups);
        }

        public ActionResult SpreadSheetConfirm()
        {
            var ssc = TempData["SSCViewModel"] as SSConfigViewModel;

            return View(ssc);

        }

        [HttpPost]
        public ActionResult SpreadSheetConfirm(SSConfigViewModel ssc)
        {
            if (ModelState.IsValid)
            {

                var file = db.Files.Find(ssc.FileID);
                DataTable table = new DataTable();
                Group group = db.Groups.Find(ssc.GroupID);
                //now that I have the correct columns, I only need to grab those columns and store to DB.
                using (var excel = new ExcelPackage(new MemoryStream(file.Content)))
                {
                    var sheet = excel.Workbook.Worksheets.First();
                    
                    //Example - Sheet.Cells["A1:B3"].Value - [row, column]

                    for (int rowNum = 2; rowNum <= sheet.Dimension.End.Row; rowNum++)
                    {
                        //var wsRow = sheet.Cells[rowNum, 1, rowNum, sheet.Dimension.End.Column];
                        try
                        {
                            FitbitUser newUser = new FitbitUser
                            {
                                Email =
                                    ssc.AppendEmailDomain
                                        ? sheet.Cells[rowNum, ssc.EmailAddress].Value + ssc.EmailDomain
                                        : sheet.Cells[rowNum, ssc.EmailAddress].Value.ToString(),
                                FirstName = sheet.Cells[rowNum, ssc.FirstNameColumn].Value.ToString(),
                                LastName = sheet.Cells[rowNum, ssc.LastNameColumn].Value.ToString(),
                                Groups = new List<Group>()
                            };

                            newUser.Groups.Add(group);

                            db.FitbitUsers.Add(newUser);
                        }
                        catch (NullReferenceException nex)
                        {
                            Trace.WriteLine(DateTime.Now + ": A null reference exception was caught (a required cell was empty):" +
                                            nex.Message);
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine(DateTime.Now + ": An exception occurred:" + e.Message);
                        }

                    }
                    db.Files.Remove(file); //remove the file from the database, we don't need it anymore.
                    db.SaveChanges();
                    
                }


                return RedirectToAction("Index");
            }
            //if failed:
            return RedirectToAction("UploadUsers");
        }

        public ActionResult DeleteFromGroup()
        {
            var allGroups = new List<GroupForDeletionViewModel>();

            foreach (Group dbgroup in db.Groups.Include(g => g.Users))
            {
                var groupUsers = new List<UserInGroupViewModel>();

                foreach (FitbitUser fitbitUser in dbgroup.Users)
                {
                    groupUsers.Add(new UserInGroupViewModel {Email = fitbitUser.Email, UniqueID = fitbitUser.UniqueID, FullName = fitbitUser.FullName, IsSelected = false});    
                }

                var group = new GroupForDeletionViewModel
                {
                    GroupName = dbgroup.Name,
                    UniqueID = dbgroup.ID,
                    Users = groupUsers
                }; 

                allGroups.Add(group);
            }

            //get all users that are in groups
            //List<FitbitUser> users = db.Groups.SelectMany(gr => gr.Users).ToList();

            List<int> userIDs = db.Groups.SelectMany(gr => gr.Users).Select(u => u.UniqueID).ToList();


            //Now get the users that are not in any group
            var query =
                //from user in db.FitbitUsers
                //where !(from gru in users select gru.UniqueID).Contains(user.UniqueID)
                //select user;
                from user in db.FitbitUsers
                where !userIDs.Contains(user.UniqueID)
                select user;

            List<FitbitUser> list = query.ToList();
            var usersNotInAnyGroup = new List<UserInGroupViewModel>();

            //create a new list of these objects for the view model
            usersNotInAnyGroup.AddRange(
                    query.ToList()
                        .Select(
                            u =>
                                new UserInGroupViewModel
                                {
                                    Email = u.Email,
                                    FullName = u.FullName,
                                    IsSelected = false,
                                    UniqueID = u.UniqueID
                                }));

            //Create a group of users that aren't in any specific group for the view model
            var UsersNotInGroup = new GroupForDeletionViewModel
            {
                GroupName = "Users not in any group",
                UniqueID = -1,
                Users = usersNotInAnyGroup
            };
            
            allGroups.Add(UsersNotInGroup);

            AllUsersGroupsViewModel allViewModel = new AllUsersGroupsViewModel { ListOfAllGroups = allGroups };
            
            return View(allViewModel);
        }

        [HttpPost]
        public ActionResult DeleteFromGroup(AllUsersGroupsViewModel viewModel, bool RemoveFromDB = false)
        {
            if (ModelState.IsValid)
            {
                foreach (var group in viewModel.ListOfAllGroups)
                {

                    if (group.UniqueID >= 0)
                    {
                        var dbgroup = db.Groups.Find(group.UniqueID);

                        if (dbgroup.Users?.Count > 0)
                        {
                            List<int> usersInListToRemove = group.Users.Where(user => user.IsSelected).Select(user => user.UniqueID).ToList();

                            foreach (var uniqueID in usersInListToRemove)
                            {
                                var userToRemove = db.FitbitUsers.Find(uniqueID);
                        
                                dbgroup.Users.Remove(userToRemove);

                                if (RemoveFromDB)
                                    db.FitbitUsers.Remove(userToRemove);

                            }
                        }    
                    }
                    else if (group.UniqueID == -1 && RemoveFromDB) //this is here to remove users that are not in any group from the database
                    {

                        if (group.Users?.Count > 0)
                        {
                            List<int> usersInListToRemove = group.Users.Where(user => user.IsSelected).Select(user => user.UniqueID).ToList();

                            foreach (var uniqueID in usersInListToRemove)
                            {
                                var userToRemove = db.FitbitUsers.Find(uniqueID);

                                db.FitbitUsers.Remove(userToRemove);

                            }
                        }
                    }
                }

                db.SaveChanges();
            }

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
}
