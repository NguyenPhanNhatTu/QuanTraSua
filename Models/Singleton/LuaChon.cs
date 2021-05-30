using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Doan1.Models.Singleton
{
    public class LuaChon
    {
        private LuaChon() { }
        private static LuaChon instance = null;
        public static LuaChon Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LuaChon();
                }
                return instance;
            }
        }
        public string TatCa()
        {
            return "1";
        }
        public string  ChoXacNhan()
        {
            return "2";
        }
        public string ChoLayHang()
        {
            return "3";
        }
        public string DangGiao()
        {
            return "4";
        }
        public string DaGiao()
        {
            return "5";
        }
        public string DaHuy()
        {
            return "6";
        }
    }
}