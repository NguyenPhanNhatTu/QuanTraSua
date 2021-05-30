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
using EPPlusTest;
using OfficeOpenXml;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace Doan1.Controllers
{
    public class HoaDonsController : Controller
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();
        public IQueryable<HoaDon> hoadon;
        [HttpGet]

        // GET: /Link/
        public ActionResult Index(int? size, int? page, string searchString)
        {
            // 1. Tạo biến ViewBag gồm sortOrder, searchValue, sortProperty và page

            ViewBag.searchValue = searchString;
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





            // 4. Truy vấn lấy tất cả đường dẫn
            hoadon = from l in db.HoaDons
                     select l;
            if ((string)Session["QuyenUser"] == null)
            {
                return RedirectToAction("Login", "Login");
            }    
            if ((string)Session["QuyenUser"] == "1" || (string)Session["QuyenUser"] == "2")
            {
                if ((string)Session["PhanLoai"] == "2")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Chờ xác nhận"
                             select l;

                }
                else if ((string)Session["PhanLoai"] == "3")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Chờ lấy hàng"
                             select l;
                }
                else if ((string)Session["PhanLoai"] == "4")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Đang giao"
                             select l;
                }
                else if ((string)Session["PhanLoai"] == "5")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Đã giao"
                             select l;
                }
                else if ((string)Session["PhanLoai"] == "6")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Đã hủy"
                             select l;
                }
                else
                {
                    hoadon = from l in db.HoaDons
                             select l;
                }
            }
            else if((string)Session["QuyenUser"] == "3")
            {
                var temp = (string)Session["SDT"];
                if ((string)Session["PhanLoai"] == "2")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Chờ xác nhận" && l.SoDienThoai== temp
                             select l;

                }
                else if ((string)Session["PhanLoai"] == "3")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Chờ lấy hàng" && l.SoDienThoai == temp
                             select l;
                }
                else if ((string)Session["PhanLoai"] == "4")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Đang giao" && l.SoDienThoai == temp
                             select l;
                }
                else if ((string)Session["PhanLoai"] == "5") 
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Đã giao" && l.SoDienThoai == temp
                             select l;
                }
                else if ((string)Session["PhanLoai"] == "6")
                {
                    hoadon = from l in db.HoaDons
                             where l.TrangThai == "Đã hủy" && l.SoDienThoai == temp
                             select l;
                }
                else
                {
                    hoadon = from l in db.HoaDons
                             where l.SoDienThoai == temp
                             select l;
                }
            }
            // 5.1. Thêm phần tìm kiếm
            if (!String.IsNullOrEmpty(searchString))
            {
                hoadon = hoadon.Where(s => s.MaHD.Contains(searchString));
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
            int checkTotal = (int)(hoadon.ToList().Count / pageSize) + 1;
            // Nếu trang vượt qua tổng số trang thì thiết lập là 1 hoặc tổng số trang
            if (pageNumber > checkTotal) pageNumber = checkTotal;

            // 7. Trả về các hóa đơn được phân trang theo kích thước và số trang.




            return View(hoadon.OrderBy("MaHD" + " desc").ToPagedList(pageNumber, pageSize));

        }

        public ActionResult btnChoXacNhan(object sender, EventArgs e)
        {

            Session["PhanLoai"] = "2";
            return RedirectToAction("Index", "HoaDons");

        }
        public ActionResult btnTatCa(object sender, EventArgs e)
        {

            Session["PhanLoai"] = "1";
            return RedirectToAction("Index", "HoaDons");

        }
        public ActionResult btnChoLayHang(object sender, EventArgs e)
        {

            Session["PhanLoai"] = "3";
            return RedirectToAction("Index", "HoaDons");

        }
        public ActionResult btnDangGiao(object sender, EventArgs e)
        {

            Session["PhanLoai"] = "4";
            return RedirectToAction("Index", "HoaDons");

        }
        public ActionResult btnDaGiao(object sender, EventArgs e)
        {

            Session["PhanLoai"] = "5";
            return RedirectToAction("Index", "HoaDons");

        }
        public ActionResult btnDaHuy(object sender, EventArgs e)
        {

            Session["PhanLoai"] = "6";
            return RedirectToAction("Index", "HoaDons");

        }


        // GET: HoaDons/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon1 = db.HoaDons.Find(id);
            if (hoaDon1 == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon1);
        }

        // GET: HoaDons/Create
        public ActionResult Create()
        {
            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh");
            ViewBag.SoDienThoai = new SelectList(db.KhachHangs, "SoDienThoai", "TenKH");
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "TenNV");
            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MaHD,SoDienThoai,MaNV,MaChiNhanh,NgayBan,TongGia,TrangThai")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                db.HoaDons.Add(hoaDon);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh", hoaDon.MaChiNhanh);
            ViewBag.SoDienThoai = new SelectList(db.KhachHangs, "SoDienThoai", "TenKH", hoaDon.SoDienThoai);
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "TenNV", hoaDon.MaNV);
            return View(hoaDon);
        }

        // GET: HoaDons/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = db.HoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh", hoaDon.MaChiNhanh);
            ViewBag.SoDienThoai = new SelectList(db.KhachHangs, "SoDienThoai", "TenKH", hoaDon.SoDienThoai);
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "TenNV", hoaDon.MaNV);
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MaHD,SoDienThoai,MaNV,MaChiNhanh,NgayBan,TongGia,TrangThai")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoaDon).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaChiNhanh = new SelectList(db.ChiNhanhs, "MaChiNhanh", "TenChiNhanh", hoaDon.MaChiNhanh);
            ViewBag.SoDienThoai = new SelectList(db.KhachHangs, "SoDienThoai", "TenKH", hoaDon.SoDienThoai);
            ViewBag.MaNV = new SelectList(db.NhanViens, "MaNV", "TenNV", hoaDon.MaNV);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            HoaDon hoaDon = db.HoaDons.Find(id);
            if (hoaDon == null)
            {
                return HttpNotFound();
            }
            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            HoaDon hoaDon = db.HoaDons.Find(id);
            db.HoaDons.Remove(hoaDon);
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
        public void Xuat()
        {
            var a = from l in db.HoaDons
                    where l.TrangThai=="Đã giao"
                    orderby l.NgayBan descending
                    select l;
            var list = a.ToList();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage Ep = new ExcelPackage();
            ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("Report");
            Sheet.Cells["A1"].Value = "Mã Hóa Đơn";
            Sheet.Cells["B1"].Value = "SDT Khách";
            Sheet.Cells["C1"].Value = "Nhân Viên";
            Sheet.Cells["D1"].Value = "Chi Nhánh";
            Sheet.Cells["E1"].Value = "Ngày Bán";
            Sheet.Cells["F1"].Value = "Tổng Giá";
            int row = 2;
            double tongcong = 0;
            foreach (var item in list)
            {
                Sheet.Cells[string.Format("A{0}", row)].Value = item.MaHD;
                if(item.SoDienThoai==null)
                {
                    Sheet.Cells[string.Format("B{0}", row)].Value = "null";
                }
                else
                {
                    Sheet.Cells[string.Format("B{0}", row)].Value = item.SoDienThoai;

                }

                if(item.MaNV==null)
                {
                    Sheet.Cells[string.Format("C{0}", row)].Value = "null";
                }
                else
                {
                    Sheet.Cells[string.Format("C{0}", row)].Value = item.MaNV + "_" + item.NhanVien.TenNV;
                }
                Sheet.Cells[string.Format("D{0}", row)].Value = item.ChiNhanh.TenChiNhanh;
                Sheet.Cells[string.Format("E{0}", row)].Value = item.NgayBan.ToString();
                Sheet.Cells[string.Format("F{0}", row)].Value = item.TongGia;
                tongcong += item.TongGia;
                row++;
            }
            Sheet.Cells[string.Format("D{0}", row)].Value = "Số đơn đã bán: " + (row-2);
            Sheet.Cells[string.Format("F{0}", row)].Value = "Tổng doanh thu: "+ tongcong;


            Sheet.Cells["A:AZ"].AutoFitColumns();
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment: filename=" + "Report.xlsx");
            Response.BinaryWrite(Ep.GetAsByteArray());
            Response.End();
        }
        public ActionResult xacnhan(string iddonhang)
        {
            if((string)Session["QuyenUser"]=="1" || (string)Session["QuyenUser"] == "2")
            {
                HoaDon hd = db.HoaDons.Find(iddonhang);
                hd.TrangThai = "Chờ lấy hàng";
                db.SaveChanges();
            }
            return RedirectToAction("Index");


        }
        public ActionResult layhang(string iddonhang)
        {
            if ((string)Session["QuyenUser"] == "1" || (string)Session["QuyenUser"] == "2")
            {
                HoaDon hd = db.HoaDons.Find(iddonhang);
                hd.TrangThai = "Đang giao";
                db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }
        public ActionResult dagiao(string iddonhang)
        {
            if ((string)Session["QuyenUser"] == "3")
            {
                HoaDon hd = db.HoaDons.Find(iddonhang);
                hd.TrangThai = "Đã giao";
                List<ChiTietHoaDon> chitiet = (from l in db.ChiTietHoaDons
                                              where l.MaHD == hd.MaHD
                                              select l).ToList();
                foreach (var item in chitiet)
                {
                    Menu menu = (from a in db.Menus
                                where a.MaMon == item.MaMon
                                select a).SingleOrDefault();
                    int sl = int.Parse(item.SoLuong.ToString());
                    menu.SoLuongDaBan = menu.SoLuongDaBan + sl;
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        public ActionResult huy(string iddonhang)
        {
            
                HoaDon hd = db.HoaDons.Find(iddonhang);
                hd.TrangThai = "Đã hủy";
                db.SaveChanges();
            
            return RedirectToAction("Index");

        }
    }
}
