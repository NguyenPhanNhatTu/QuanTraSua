using Doan1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using System.Web.Services.Description;
using System.Web.UI;
using MailKit.Net.Smtp;
using MimeKit;
using MailKit;
namespace Doan1.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        public void btntestClick(string test)
        {
            test = "khong dc";
        }
        [HttpPost]
        public ActionResult Login(string Username, string Pass, string PhanQuyen)
        {
            using (var _context = new QuanLyCuaHangTraSuaEntities1())
            {
                Session.Clear();
                if (PhanQuyen == "3")
                {

                    var user = from u in _context.KhachHangs
                               where u.SoDienThoai == Username && u.MatKhau == Pass
                               select u;
                    if (user.Count() == 1)
                    {
                        Session["QuyenUser"] = "3"; //Gán Session cho khách hàng
                        Session["Ten"] = user.First().TenKH;
                        Session["SDT"] = user.First().SoDienThoai;
                        return RedirectToAction("TrangChu", "TrangChu");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else if (PhanQuyen == "2")
                {
                    var user = from u in _context.NhanViens
                               where u.MaNV == Username && u.MatKhau == Pass
                               select u;
                    if (user.Count() == 1)
                    {
                        Session["QuyenUser"] = "2"; //Gán Session cho nhân viên
                        Session["Ten"] = user.First().TenNV;
                        Session["MaNV"] = user.First().MaNV;
                        return RedirectToAction("TrangChu", "TrangChu");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
                else
                {
                    var user = from u in _context.QuanLies
                               where u.TaiKhoan == Username && u.MatKhau == Pass
                               select u;
                    if (user.Count() == 1)
                    {
                        Session["QuyenUser"] = "1"; //Gán Session cho quản lý
                        Session["Ten"] = user.First().TaiKhoan;

                        return RedirectToAction("Index", "KhachHangs");
                    }
                    else
                    {
                        return RedirectToAction("Login", "Login");
                    }
                }
            }
        }
        //Quên mật khẩu
        public ActionResult ForgotPass(string SDT, string Email)
        {
            using (var _context = new QuanLyCuaHangTraSuaEntities1())
            {
                KhachHang us = _context.KhachHangs.SingleOrDefault(n => n.SoDienThoai == SDT && n.Email == Email);
                if (us != null) return Content("true");
            }
            return Content("false");
        }
        public ActionResult VerifyEmail(string Email, string SDT)
        {




            Random matkhaumoi = new Random();
            int mk;
            if (ModelState.IsValid)
            {
                using (var _context = new QuanLyCuaHangTraSuaEntities1())
                {
                    var user = (from u in _context.KhachHangs
                                where u.SoDienThoai == SDT
                                select u).Single();
                    mk = matkhaumoi.Next(1000, 9999);
                    user.MatKhau = mk.ToString();
                    _context.SaveChanges();
                }
                var message = new MimeMessage();
                //From Address
                message.From.Add(new MailboxAddress("Ngoc Nghia", "trinhphovodoi1@gmail.com"));
                //To Address
                message.To.Add(new MailboxAddress("Dot net", Email));
                //Subject
                message.Subject = "Hello";
                //Body
                message.Body = new TextPart("plain")
                {
                    Text ="Tài Khoản:"+SDT+"\nMật khẩu mới của quý khách là: " + mk.ToString()

                };
                //Configure send email
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("trinhphovodoi1@gmail.com", "15082000AsZx");
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            return Content("true");
        }
        public ActionResult ChangePass(string Username, string  NewPass, string OldPass)
        {
            if (Username == null )
                return View();
            using (var _context = new QuanLyCuaHangTraSuaEntities1())
            {
                try
                {
                    if ((string)Session["QuyenUser"] == "3")
                    {
                        
                        var user = (from u in _context.KhachHangs
                                    where u.SoDienThoai == Username && u.MatKhau == OldPass
                                    select u).Single();

                        user.MatKhau = NewPass;
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Login");
                    }
                    else if ((string)Session["QuyenUser"] == "2")
                    {
                        var user = (from u in _context.NhanViens
                                    where u.MaNV == Username && u.MatKhau == OldPass
                                    select u).Single();

                        user.MatKhau = NewPass;
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Login");
                    }
                    else if ((string)Session["QuyenUser"] == "1")
                    {
                        var user = (from u in _context.QuanLies
                                    where u.TaiKhoan == Username && u.MatKhau == OldPass
                                    select u).Single();

                        user.MatKhau = NewPass;
                        _context.SaveChanges();
                        return RedirectToAction("Login", "Login");
                    }
                    else
                    {
                        return View();
                    }
                }
                catch
                {
                    return View();
                }
            }
                
        }
    }
}