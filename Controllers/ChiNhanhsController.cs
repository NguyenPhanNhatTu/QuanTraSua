using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Doan1.Models;
using PagedList;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Ajax.Utilities;

namespace Doan1.Controllers
{
    [AuthorizeController]
    public class ChiNhanhsController : Controller
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();
        [HttpGet]

        // GET: ChiNhanhs
        public ActionResult Index(int? size, int? page, string sortProperty, string sortOrder, string searchString)
        {
            // 1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page
            if (sortOrder == "asc") ViewBag.sortOrder = "desc";
            if (sortOrder == "desc") ViewBag.sortOrder = "";
            if (sortOrder == "") ViewBag.sortOrder = "asc";
            ViewBag.searchValue = searchString;
            ViewBag.sortProperty = sortProperty;
            ViewBag.page = page;

            // 2. Tạo danh sách chọn số trang
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "5", Value = "5" });
            items.Add(new SelectListItem { Text = "10", Value = "10" });
            items.Add(new SelectListItem { Text = "20", Value = "20" });
            

            // 2.1. Thiết lập số trang đang chọn vào danh sách List<SelectListItem> items
            foreach (var item in items)
            {
                if (item.Value == size.ToString()) item.Selected = true;
            }
            ViewBag.size = items;
            ViewBag.currentSize = size;

            // 3. Lấy tất cả tên thuộc tính của lớp chinhanhs
            var properties = typeof(ChiNhanh).GetProperties();
            List<Tuple<string, bool, int>> list = new List<Tuple<string, bool, int>>();
            foreach (var item in properties)
            {
                int order = 999;
                var isVirtual = item.GetAccessors()[0].IsVirtual;

                if (item.Name == "TenChiNhanh") order = 2;
                if (item.Name == "MaChiNhanh") order = 1;
                if (item.Name == "DiaChi") continue;
                if (item.Name == "NhanViens") continue;
                if (item.Name == "HoaDons") continue;
                if (item.Name == "Menus") continue;

                Tuple<string, bool, int> t = new Tuple<string, bool, int>(item.Name, isVirtual, order);
                list.Add(t);
            }
            list = list.OrderBy(x => x.Item3).ToList();

            // 3.1. Tạo Heading sắp xếp cho các cột
            foreach (var item in list)
            {
                if (!item.Item2)
                {
                    if (sortOrder == "desc" && sortProperty == item.Item1)
                    {
                        ViewBag.Headings += "<th><a href='?page=" + page + "&size=" + ViewBag.currentSize + "&sortProperty=" + item.Item1 + "&sortOrder=" +
                            ViewBag.sortOrder + "&searchString=" + searchString + "'>" + item.Item1 + "<i class='fa fa-fw fa-sort-desc'></i></th></a></th>";
                    }
                    else if (sortOrder == "asc" && sortProperty == item.Item1)
                    {
                        ViewBag.Headings += "<th><a href='?page=" + page + "&size=" + ViewBag.currentSize + "&sortProperty=" + item.Item1 + "&sortOrder=" +
                            ViewBag.sortOrder + "&searchString=" + searchString + "'>" + item.Item1 + "<i class='fa fa-fw fa-sort-asc'></a></th>";
                    }
                    else
                    {
                        ViewBag.Headings += "<th><a href='?page=" + page + "&size=" + ViewBag.currentSize + "&sortProperty=" + item.Item1 + "&sortOrder=" +
                           ViewBag.sortOrder + "&searchString=" + searchString + "'>" + item.Item1 + "<i class='fa fa-fw fa-sort'></a></th>";
                    }

                }
                else ViewBag.Headings += "<th>" + item.Item1 + "</th>";
            }

            // 4. Truy vấn lấy tất cả đường dẫn
            var chinhanh = from l in db.ChiNhanhs
                           select l;

            // 5. Tạo thuộc tính sắp xếp mặc định là "MaMon"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaChiNhanh";

            // 5. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc") chinhanh = chinhanh.OrderBy(sortProperty + " desc");
            else if (sortOrder == "asc") chinhanh = chinhanh.OrderBy(sortProperty);
            else chinhanh = chinhanh.OrderBy("MaChiNhanh");

            // 5.1. Thêm phần tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                chinhanh = chinhanh.Where(s => s.TenChiNhanh.Contains(searchString));
            }

            // 5.2. Nếu page = null thì đặt lại là 1.
            page = page ?? 1; //if (page == null) page = 1;

            // 5.3. Tạo kích thước trang (pageSize), mặc định là 5.
            int pageSize = (size ?? 5);

            ViewBag.pageSize = pageSize;

            // 6. Toán tử ?? trong C# mô tả nếu page khác null thì lấy giá trị page, còn
            // nếu page = null thì lấy giá trị 1 cho biến pageNumber. --- dammio.com
            int pageNumber = (page ?? 1);

            // 6.2 Lấy tổng số record chia cho kích thước để biết bao nhiêu trang
            int checkTotal = (int)(chinhanh.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 7. Trả về các chinhanh được phân trang theo kích thước và số trang.
            return View(chinhanh.ToPagedList(pageNumber, pageSize));
        }

        // GET: ChiNhanhs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiNhanh chiNhanh = db.ChiNhanhs.Find(id);
            if (chiNhanh == null)
            {
                return HttpNotFound();
            }
            return View(chiNhanh);
        }
        [AuthorizeController]
        // GET: ChiNhanhs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ChiNhanhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeController]
        public ActionResult Create([Bind(Include = "MaChiNhanh,TenChiNhanh,DiaChi")] ChiNhanh chiNhanh)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.ChiNhanhs.Add(chiNhanh);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(chiNhanh);
            }
            catch
            {
                Session["Check"] = "Mã Chi Nhánh Đã Tồn Tại";
                return RedirectToAction("Create");
            }
        }

        // GET: ChiNhanhs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiNhanh chiNhanh = db.ChiNhanhs.Find(id);
            if (chiNhanh == null)
            {
                return HttpNotFound();
            }
            return View(chiNhanh);
        }

        // POST: ChiNhanhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaChiNhanh,TenChiNhanh,DiaChi")] ChiNhanh chiNhanh)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiNhanh).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(chiNhanh);
        }

        // GET: ChiNhanhs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiNhanh chiNhanh = db.ChiNhanhs.Find(id);
            if (chiNhanh == null)
            {
                return HttpNotFound();
            }
            return View(chiNhanh);
        }

        // POST: ChiNhanhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            ChiNhanh chiNhanh = db.ChiNhanhs.Find(id);
            db.ChiNhanhs.Remove(chiNhanh);
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
}
