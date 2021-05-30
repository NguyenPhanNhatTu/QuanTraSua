using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models.StatePattern
{
    public class DaHuy : State
    {
        private QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1();

        public override void ChangeState(string iddonhang)
        {
            HoaDon hd = db.HoaDons.Find(iddonhang);
            hd.TrangThai = "Đã hủy";
            db.SaveChanges();
        }
    }
}