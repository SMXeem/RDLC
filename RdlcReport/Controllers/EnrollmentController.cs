using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RdlcReport.Models;
using Microsoft.Reporting.WebForms;
using System.IO;

namespace RdlcReport.Controllers
{
    public class EnrollmentController : Controller
    {
        private SchoolContext db = new SchoolContext();

        //RDLC Report for enrollment
        public ActionResult EnrollmentReport()
        {
            LocalReport localReport = new LocalReport();

            string path = Path.Combine(Server.MapPath("~/Reports"), "EnrollmentReport.rdlc");
            localReport.ReportPath = path;

            //list of data
            List<Enrollment> enrollments = new List<Enrollment>();
            enrollments = db.Enrollments.ToList();
            List<EnrollmentVm> enrollmentVm = new List<EnrollmentVm>();
            EnrollmentVm enrollment;
            foreach (var item in enrollments)
            {
                enrollment = new EnrollmentVm
                {
                    CourseID = item.Course.Title,
                    StudentID = item.Student.FirstMidName+" "+item.Student.LastName,
                    EnrollmentID = item.EnrollmentID,
                    Grade = item.Grade.ToString()
                };
                enrollmentVm.Add(enrollment);
            }
            //Dataset name. (rdlc dataset)
            ReportDataSource reportDataSource = new ReportDataSource("EnrollmentDataSet", enrollmentVm);

            string a = "Done";
            localReport.DataSources.Add(reportDataSource);

            List<Student> aStudents = db.Students.ToList();
            reportDataSource = new ReportDataSource("StdDataset", aStudents);
            localReport.DataSources.Add(reportDataSource);
            ReportParameter[] parameters = new ReportParameter[2];
            parameters[0] = new ReportParameter("SchoolName", a);
            parameters[1] = new ReportParameter("SectionName", "Section Name");

            localReport.SetParameters(parameters);
            //render to web/ anything else
            //Render() takes 7 parameters
            string mediaType, encoding, fileName;
            string[] stringArray;
            Warning[] warnings;
            byte[] dataByte = localReport.Render("PDF", null, out mediaType, out encoding, out fileName, out stringArray, out warnings);

            return File(dataByte, mediaType);
        }

        // GET: Enrollment
        public async Task<ActionResult> Index()
        {
            var enrollments = db.Enrollments.Include(e => e.Course).Include(e => e.Student);
            return View(await enrollments.ToListAsync());
        }

        // GET: Enrollment/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // GET: Enrollment/Create
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title");
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName");
            return View();
        }

        // POST: Enrollment/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Enrollments.Add(enrollment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollment/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName", enrollment.StudentID);
            return View(enrollment);
        }

        // POST: Enrollment/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "EnrollmentID,CourseID,StudentID,Grade")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enrollment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "Title", enrollment.CourseID);
            ViewBag.StudentID = new SelectList(db.Students, "ID", "LastName", enrollment.StudentID);
            return View(enrollment);
        }

        // GET: Enrollment/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return HttpNotFound();
            }
            return View(enrollment);
        }

        // POST: Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Enrollment enrollment = await db.Enrollments.FindAsync(id);
            db.Enrollments.Remove(enrollment);
            await db.SaveChangesAsync();
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
