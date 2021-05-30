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
    public class TrangChuController : Controller
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();
        // GET: TrangChu
        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult LienHe()
        {
            return View(db.LienHes.ToList());
        }
    }
}