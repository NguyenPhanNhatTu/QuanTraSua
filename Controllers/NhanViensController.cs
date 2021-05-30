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
    
    public class NhanViensController : Controller
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();

        // GET: NhanViens
        [HttpGet]
        // GET: /Link/
        [AuthorizeController]
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

            // 3. Lấy tất cả tên thuộc tính của lớp NhanViens
            var properties = typeof(NhanVien).GetProperties();
            List<Tuple<string, bool, int>> list = new List<Tuple<string, bool, int>>();
            foreach (var item in properties)
            {
                int order = 999;
                var isVirtual = item.GetAccessors()[0].IsVirtual;

                if (item.Name == "TenNV") order = 2;
                if (item.Name == "MaNV") order = 1;
                if (item.Name == "DiaChi") continue;
                if (item.Name == "Email") continue;
                if (item.Name == "MaChiNhanh") continue;
                if (item.Name == "MatKhau") continue;
                if (item.Name == "HoaDons") continue;
                if (item.Name == "SDT") continue; // Không hiển thị cột này                
                if (item.Name == "ChiNhanh") continue ;
                
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
            var nhanvien = from l in db.NhanViens
                       select l;

            // 5. Tạo thuộc tính sắp xếp mặc định là "MaMon"
            if (String.IsNullOrEmpty(sortProperty)) sortProperty = "MaNV";

            // 5. Sắp xếp tăng/giảm bằng phương thức OrderBy sử dụng trong thư viện Dynamic LINQ
            if (sortOrder == "desc") nhanvien = nhanvien.OrderBy(sortProperty + " desc");
            else if (sortOrder == "asc") nhanvien = nhanvien.OrderBy(sortProperty);
            else nhanvien = nhanvien.OrderBy("MaNV");

            // 5.1. Thêm phần tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                nhanvien = nhanvien.Where(s => s.TenNV.Contains(searchString));
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
            int checkTotal = (int)(nhanvien.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 7. Trả về các nhanvien được phân trang theo kích thước và số trang.
            return View(nhanvien.ToPagedList(pageNumber, pageSize));
        }

        

        // GET: NhanViens/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // GET: NhanViens/Create
        [AuthorizeController]
        public ActionResult Create()
        {
            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh");
            return View();
        }

        // POST: NhanViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizeController]
        public ActionResult Create([Bind(Include = "MaNV,TenNV,DiaChi,Email,SDT,MaChiNhanh,MatKhau")] NhanVien nhanVien)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.NhanViens.Add(nhanVien);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh", nhanVien.MaChiNhanh);
                return View(nhanVien);
            }
            catch
            {
                Session["Check"] = "Mã Nhân Viên Đã Tồn Tại";
                return RedirectToAction("Create");
            }
        }

        // GET: NhanViens/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh", nhanVien.MaChiNhanh);
            return View(nhanVien);
        }

        // POST: NhanViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaNV,TenNV,DiaChi,Email,SDT,MaChiNhanh,MatKhau")] NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh", nhanVien.MaChiNhanh);
            return View(nhanVien);
        }

        // GET: NhanViens/Delete/5
        [AuthorizeController]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NhanVien nhanVien = db.NhanViens.Find(id);
            if (nhanVien == null)
            {
                return HttpNotFound();
            }
            return View(nhanVien);
        }

        // POST: NhanViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizeController]
        public ActionResult DeleteConfirmed(string id)
        {
            NhanVien nhanVien = db.NhanViens.Find(id);
            db.NhanViens.Remove(nhanVien);
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

        public ActionResult ThongTinNhanVien()
        {
            if ((string)Session["QuyenUser"] == "2")
            {
                string manv = (string)Session["MaNV"];
                NhanVien nhanvien = db.NhanViens.Find(manv);
                if (nhanvien == null)
                {
                    return HttpNotFound();
                }
                return View(nhanvien);
             }
            else
            {
                return RedirectToAction("Login", "Login");
            }    
        }
    }
}
