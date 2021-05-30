using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Doan1.Models;

namespace Doan1.Controllers
{
    public class LienHesController : Controller
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();


        // GET: LienHes/Edit/5
        public ActionResult Edit()
        {
            LienHe lienHe = db.LienHes.Find("1");
            if (lienHe == null)
            {
                return HttpNotFound();
            }
            return View(lienHe);
        }

        // POST: LienHes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeController]
        public ActionResult Edit([Bind(Include = "MaLienHe,Email,SoDienThoai,DiaChi,FaceBook")] LienHe lienHe)
        {
            if (ModelState.IsValid)
            {
                lienHe.MaLienHe = "1";
                db.Entry(lienHe).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("LienHe", "TrangChu");
            }
            return View(lienHe);
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
