using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models.StatePattern
{
    public class DaGiao : State
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();

        public override void ChangeState(string iddonhang)
        {
            string user = HttpContext.Current.Session["QuyenUser"] as string;

            if (user == "3")
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
        }
    }
}