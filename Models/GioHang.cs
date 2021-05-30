using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models
{
    public class GioHang
    {
        public string MaMon { get; set; }
        public string TenMon { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public double Gia { get; set; }
        public double ThanhTien { get; set; }
        public string Duong { get; set; }
        public string Da { get; set; }
        public string Anh { get; set; }
        public GioHang(string iMaMon,string size,string duong,string da,int sl,string anh)
        {
            using (QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1())
            {
                this.MaMon = iMaMon;
                Menu mn = db.Menus.Single(n => n.MaMon == iMaMon);
                this.TenMon = mn.TenMon;
                this.Gia = mn.GiaTien;
                this.SoLuong = sl;
                this.ThanhTien = Gia * SoLuong;
                this.Duong = duong;
                this.Da = da;
                this.Size = size;
                this.Anh = anh;
            }
        }

       
        public GioHang()
        {
            using (QuanLyCuaHangTraSuaEntities1 db = new QuanLyCuaHangTraSuaEntities1())
            {
                this.MaMon = "";
                this.TenMon ="";
                this.Gia = 0;
                this.SoLuong = 0;
                this.ThanhTien = Gia * SoLuong;
                this.Duong = "";
                this.Da = "";
                this.Size = "";
                this.Anh = "";
            }
        }

    }
}