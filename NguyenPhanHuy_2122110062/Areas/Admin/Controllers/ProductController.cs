using NguyenPhanHuy_2122110062.Helpers;
using NguyenPhanHuy_2122110062.Models;
using System.IO;
using System;
using System.Linq;
using System.Web.Mvc;

namespace NguyenPhanHuy_2122110062.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        EcommerceEntities entities = new EcommerceEntities();

        // GET: Admin/Product
        public ActionResult Index()
        {
            var listProduct = entities.Products.Where(x => x.IsDeleted == false).ToList();
            return View(listProduct);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Category category)
        {
            try
            {
                if (string.IsNullOrEmpty(category.Title))
                {
                    ModelState.AddModelError("Title", "Please enter a category name.");
                    return View();
                }

                category.Slug = GenerateSlug.GenerateSlugs(category.Title);
                category.CreatedAt = DateTime.UtcNow;
                category.IsDeleted = false;

                if (category.Upload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(category.Upload.FileName);
                    string extension = Path.GetExtension(category.Upload.FileName);
                    fileName = fileName + "_" + long.Parse(DateTime.UtcNow.ToString("yyyyMMddhhmmss")) + extension;
                    category.ImageUrl = fileName;
                    string path = Path.Combine(Server.MapPath("~/Content/images/Category"), fileName);
                    category.Upload.SaveAs(path);
                }

                entities.Categories.Add(category);
                entities.SaveChanges();

                TempData["success"] = "Added new success!";

                return RedirectToAction("Index", "Category");
            }
            catch (Exception)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction("Index", "Category", new { area = "Admin" });
            }
        }
    }
}