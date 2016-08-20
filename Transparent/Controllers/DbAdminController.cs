using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Interfaces;
using Transparent.ViewModels.DbAdmin;

namespace Transparent.Controllers
{
    [Authorize(Roles = "admin")]
    public class DbAdminController : Controller
    {
        private readonly IUsersContext db;
        private readonly IDatabaseDirectService databaseDirectService;

        public DbAdminController(IDatabaseDirectService databaseDirectService, IUsersContext db)
        {
            this.databaseDirectService = databaseDirectService;
            this.db = db;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CsvUpload()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CsvUpload(CsvUploadViewModel viewModel)
        {
            try
            {
                databaseDirectService.InsertData(db, viewModel.TableName, viewModel.Data);
                viewModel.Status = "Success";
            }
            catch(Exception e)
            {
                viewModel.Status = e.Message;
            }
            return View(viewModel);
        }
    }
}
