﻿using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplication1.Models;
using WebApplication1.DAL;
using System;
using PagedList;

namespace WebApplication1.Controllers
{
    public class StudentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        // GET: /Student/
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;

            //to toggle between the sort order column links Last Name and Enrollment Date
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var students = from s in db.Students select s ;
           

            if (!String.IsNullOrEmpty(searchString))
            {
                // When you call the Contains  method on an IEnumerable  collection, you get the .NET Framework implementation;
                // when you call it on an IQueryable  object, you get the database provider implementation.
                // contains is converted to like on the database which is case insensitive for the entity framework 
                // provider implementation
                students = students.Where( s => s.FirstMidName.Contains(searchString) || s.LastName.Contains(searchString) );
            }
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.LastName);
                    break;
                case "Date":
                    students = students.OrderBy(s => s.EnrollmentDate);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.EnrollmentDate);
                    break;
                default:
                    students = students.OrderBy(s => s.LastName);
                    break;
            }

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(students.ToPagedList(pageNumber,pageSize));
        }

        // GET: /Student/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: /Student/Create
        public ActionResult Create()
        {
            return View();
        }

        /*
         *  I Added this Note: 
         *  The Bind  attribute is one way to protect against over-posting in create scenarios.
         *  so because the StudentID is generated by the database it should be removed in the bind include list
         *  so that the model binder doesn't include it and hence protected from overpost attack using a tool
         *  such as fiddler or javascript to post StudentID.
         *  
         *  Previously: ([Bind(Include="StudentID,LastName,FirstMidName,EnrollmentDate")] Student student)
         *  Exclude =" " - can also be used but include is safer to protect when new attributes are added. 
         */
        // POST: /Student/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstMidName,EnrollmentDate")] Student student)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Students.Add(student);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                /*
                 * If an exception that derives from DataException is caught while the changes are being saved, a generic error
                 * message is displayed. DataException exceptions are sometimes caused by something external to the
                 * application rather than a programming error, so the user is advised to try again. Although not implemented
                 * in this sample, a production quality application would log the exception. For more information, see the Log
                 * for insight section in Monitoring and Telemetry (Building Real-World Cloud Apps with Azure).
                 * click on the downloaded documentation with the link.
                 */

                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");

            }

            return View(student);
        }

        // GET: /Student/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }
        /*
         * removed StudentID property from the bind list.
         * and renamed ActionResult Edit(int? id) to ActionResult EditPost(int? id) to make the post method's 
         * signature different from the get method's signature. then added the ActionName("Edit) part to the
         * HttpPost attribute.
         */
        // POST: /Student/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var studentToUpdate = db.Students.Find(id);
            if (TryUpdateModel(studentToUpdate, "",
               new string[] { "LastName", "FirstMidName", "EnrollmentDate" }))
            {
                try
                {
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DataException /* dex */)
                {
                    //Log the error (uncomment dex variable name and add a line here to write a log.
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            return View(studentToUpdate);
        }

        // GET: /Student/Delete/5
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                Student student = db.Students.Find(id);
                db.Students.Remove(student);
                /* for high performance application the following 2 lines can be used instead of the above 2
                 * or If improving performance in a high-volume application is a priority then
                 * 
                 * Student studentToDelete = new Student() { ID = id };
                 * db.Entry(studentToDelete).State = EntityState.Deleted;
                 *
                 */

                db.SaveChanges();
            }
            catch (DataException)
            {
                RedirectToAction("Delete", new { id = id, saveChangesError = true });
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
