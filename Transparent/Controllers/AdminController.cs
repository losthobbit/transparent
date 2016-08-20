using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Transparent.Data.Interfaces;
using Transparent.Data.Models;
using Transparent.ViewModels.Admin;

namespace Transparent.Controllers
{
    public class AdminController : Controller
    {
        private ITags tags;

        public AdminController(ITags tags)
        {
            this.tags = tags;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Tags(int id = 0)
        {
            if (id == 0)
                id = tags.Root.Id;

            var tagViewModel = CreateTagViewModel(id);
            
            return View(tagViewModel);
        }


        public ActionResult CreateTag()
        {
            var viewModel = new CreateTagViewModel
            {
                AllTags = tags.IndentedTags
            };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateTag(CreateTagViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var tag = new Tag
                {
                    Description = viewModel.Description,
                    Name = viewModel.Name
                };
                var id = tags.CreateTag(tag, viewModel.ParentIds);
                return RedirectToAction("Tags", new { id = id });
            }
            viewModel.AllTags = tags.IndentedTags;
            return View(viewModel);
        }

        private TagViewModel CreateTagViewModel(int tagId)
        {
            var tag = tags.Find(tagId);

            var tagViewModel = new TagViewModel
            {
                Json = tags.SerializeTag(tagId),
                Name = tag.Name,
                Description = tag.Description,
                CompetentPoints = tag.CompetentPoints,
                ExpertPoints = tag.ExpertPoints
            };

            return tagViewModel;
        }
    }
}
