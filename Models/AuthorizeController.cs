using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Doan1.Models
{
    public class AuthorizeController : ActionFilterAttribute
    {
        // phương thức thực thi khi action được gọi
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {


            string user = HttpContext.Current.Session["QuyenUser"] as string;
            //nếu session=null thì trả về trang đăng nhập

            if (user != "1")
            {
                filterContext.Result = new RedirectResult("~/Login/Login");
            }

            
        }
    }
}