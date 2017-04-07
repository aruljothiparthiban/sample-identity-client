using SampleClient.Models;
using System.Web.Mvc;
using System.Collections.Generic;

namespace SampleClient.Controllers
{
    [Authorize]
    public class WelcomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}