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


    public class KhachHangsController : Controller
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();
        [HttpGet]
        [AuthorizeController]
        // GET: KhachHangs
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

            // 3. Lấy tất cả tên thuộc tính của lớp khachhangs
            var properties = typeof(KhachHang).GetProperties();
            List<Tuple<string, bool, int>> list = new List<Tuple<string, bool, int>>();
            foreach (var item in properties)
            {
                int order = 999;
                var isVirtual = item.GetAccessors()[0].IsVirtual;

                if (item.Name == "TenKH") order = 2;
                if (item.Name == "SoDienThoai") order = 1;
                if (item.Name == "Email") continue;
                if (item.Name == "MatKhau") continue;
                if (item.Name == "DiaChi") continue;
                if (item.Name == "HoaDons") continue;
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
            var khachhang = from l in db.KhachHangs
                            select l;

            // 5. Tạo thuộc tính sắp xếp mặc định là "MaMon"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "SoDienThoai";

            // 5. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc") khachhang = khachhang.OrderBy(sortProperty + " desc");
            else if (sortOrder == "asc") khachhang = khachhang.OrderBy(sortProperty);
            else khachhang = khachhang.OrderBy("SoDienThoai");

            // 5.1. Thêm phần tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                khachhang = khachhang.Where(s => s.TenKH.Contains(searchString));
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
            int checkTotal = (int)(khachhang.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 7. Trả về các khachhang được phân trang theo kích thước và số trang.
            return View(khachhang.ToPagedList(pageNumber, pageSize));
        }

        // GET: KhachHangs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Create
        [AuthorizeController]
        public ActionResult Create()
        {
            return View();
        }

        // POST: KhachHangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeController]
        public ActionResult Create([Bind(Include = "SoDienThoai,TenKH,DiaChi,Email,MatKhau")] KhachHang khachHang)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    khachHang.DaMua = 0;
                    db.KhachHangs.Add(khachHang);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(khachHang);
            }
            catch
            {
                Session["Check"] = "Số Điện Thoại Khách Hàng Đã Tồn Tại ";
                return RedirectToAction("Create");
            }
        }

        // GET: KhachHangs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SoDienThoai,TenKH,DiaChi,Email,MatKhau")] KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
            {
                return HttpNotFound();
            }
            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            KhachHang khachHang = db.KhachHangs.Find(id);
            db.KhachHangs.Remove(khachHang);
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
        public ActionResult ThongTinKhachHang()
        {
            if ((string)Session["QuyenUser"] == "3")
            {
                string sdt = (string)Session["SDT"];
                KhachHang khachHang = db.KhachHangs.Find(sdt);
                if (khachHang == null)
                {
                    return HttpNotFound();
                }
                return View(khachHang);

            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
        [AuthorizeController]
        public ActionResult ThongKe()
        {
            var khachhang = from l in db.KhachHangs
                            orderby l.DaMua descending
                            select l;
            List<KhachHang> lst = new List<KhachHang>();
            lst = khachhang.Take(5).ToList();
            var soLuongKH = db.KhachHangs.Select(n => n.SoDienThoai).Count();

            ViewBag.soLuongKH = soLuongKH;
            return View(lst);
        }
    }
}
