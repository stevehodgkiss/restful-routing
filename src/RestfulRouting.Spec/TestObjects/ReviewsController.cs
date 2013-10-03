using System.Web.Mvc;

namespace RestfulRouting.Spec.TestObjects
{
    public class ReviewsController : Controller
    {
        public ActionResult Index(string productCode)
        {
            return Content("");
        }

        public ActionResult New(string productCode)
        {
            return Content("");
        }

        public ActionResult Create(string productCode)
        {
            return Content("");
        }

        public ActionResult Edit(string productCode, int id)
        {
            return Content("");
        }

        public ActionResult Update(string productCode, int id)
        {
            return Content("");
        }

        public ActionResult Delete(string productCode, int id)
        {
            return Content("");
        }

        public ActionResult Destroy(string productCode, int id)
        {
            return Content("");
        }

        public ActionResult Show(string productCode, int id)
        {
            return Content("");
        }
    }
}
