using System.Web.Mvc;

namespace RestfulRouting.Spec.TestObjects
{
    public class ProductsController : Controller
    {
        public ActionResult Index()
        {
            return Content("");
        }

        public ActionResult New()
        {
            return Content("");
        }

        public ActionResult Create()
        {
            return Content("");
        }

        public ActionResult Edit(string code)
        {
            return Content("");
        }

        public ActionResult Update(string code)
        {
            return Content("");
        }

        public ActionResult Delete(string code)
        {
            return Content("");
        }

        public ActionResult Destroy(string code)
        {
            return Content("");
        }

        public ActionResult Show(string code)
        {
            return Content("");
        }

        public ActionResult Post(int year, string code)
        {
            return null;
        }

        public ActionResult Latest()
        {
            return null;
        }

        public ActionResult Hello(string code)
        {
            return null;
        }
    }
}
