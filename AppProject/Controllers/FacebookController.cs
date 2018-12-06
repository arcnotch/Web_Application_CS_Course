using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AppProject.Controllers
{
    public class FacebookController : Controller
    {
        //Get the last post from the facebook page and present it on the header of the layout (The title near the logo)
        public ActionResult Index()
        {
            //Token from https://developers.facebook.com/tools/explorer
            var accessToken = "EAABrkcYm6ZB4BAHEZBIbv0Sld1Y7ZBPX7pgJPNaBLUSu917wqkA0cWO50JmXiEanRHUAzapIRwpoWYY0KcFLgan8GfzoeOiaKEkdPKhRv5zuZBJnICsyCGg4IZAPocU3A92ltZBwhZAkLdGvDBhbkxPNXVIqw2ssPc7GsNCocs9GgZDZD";
            var client = new FacebookClient(accessToken);
            dynamic result = client.Get("me", new { fields = "feed.limit(5)" });
            return Json(new
            {
                id = result.id,
                message = result.feed.data[0].message,
                date = result.feed.data[0].created_time,
            }, JsonRequestBehavior.AllowGet);
        }
    }
}