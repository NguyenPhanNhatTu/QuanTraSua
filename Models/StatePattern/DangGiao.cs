using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models.StatePattern
{
    public class DangGiao : State
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();

        public override void ChangeState(string iddonhang)
        {
            string user = HttpContext.Current.Session["QuyenUser"] as string;

            if (user == "1" || user == "2")
            {
                HoaDon hd = db.HoaDons.Find(iddonhang);
                hd.TrangThai = "Đang giao";
                db.SaveChanges();
            }
        }
    }
}