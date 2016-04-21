using BugTracker2.Models;
using System.Linq;
using System.Web.Mvc;


namespace BugTracker2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Admin
        public ActionResult Index()
        {
            var users = db.Users.ToList();
            return View();
        }

        public ActionResult EditUser(string id = "59d21208-39c4-404c-bcca-cb563b2428b3")
        {
            var user = db.Users.Find(id);
            UserRolesHelper helper = new UserRolesHelper(db);
            var model = new AdminUserViewModel();
            model.Name = user.DisplayName;
            model.Id = user.Id;
            model.selected = helper.ListUserRoles(id).ToArray();
            model.roles = new MultiSelectList(db.Roles, "Name", "Name", model.selected);
            return View(model);
        }

        public ActionResult ListUsers()
        {
            var user = db.Users;
            return View(user);
        }
    }
}