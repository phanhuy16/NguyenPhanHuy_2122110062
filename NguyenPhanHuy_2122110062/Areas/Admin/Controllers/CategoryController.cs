
using NguyenPhanHuy_2122110062.Models;
using System;
using System.IO;
using System.Web.Mvc;
using NguyenPhanHuy_2122110062.Helpers;
using System.Linq;
using System.Data.Entity;

namespace NguyenPhanHuy_2122110062.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        EcommerceEntities entities = new EcommerceEntities();

        public ActionResult Index()
        {
            var listCategory = entities.Categories.Where(x => x.IsDeleted == false).ToList();

            return View(listCategory);
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

        [HttpGet]
        public ActionResult Details(int id)
        {
            var category = entities.Categories.Where(x => x.CategoryId == id).FirstOrDefault();

            return View(category);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var category = entities.Categories.Where(x => x.CategoryId == id).FirstOrDefault();

            return View(category);
        }


        [HttpPost]
        public ActionResult Edit(Category category)
        {
            try
            {
                if (category == null)
                {
                    TempData["error"] = "Category not found!";
                    return RedirectToAction("Index", new { area = "Admin" });
                }

                var exisCategory = entities.Categories.Find(category.CategoryId);

                exisCategory.Title = category.Title;
                exisCategory.Status = category.Status;
                exisCategory.IsDeleted = category.IsDeleted;
                exisCategory.Slug = GenerateSlug.GenerateSlugs(category.Title);
                exisCategory.UpdatedAt = DateTime.UtcNow;

                if (category.Upload != null)
                {
                    string oldPath = Path.Combine(Server.MapPath("~/Content/images/Category"), exisCategory.ImageUrl);

                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }

                    string fileName = Path.GetFileNameWithoutExtension(category.Upload.FileName);
                    string extension = Path.GetExtension(category.Upload.FileName);
                    fileName = fileName + "_" + long.Parse(DateTime.UtcNow.ToString("yyyyMMddhhmmss")) + extension;

                    exisCategory.ImageUrl = fileName;

                    string path = Path.Combine(Server.MapPath("~/Content/images/Category"), fileName);
                    category.Upload.SaveAs(path);
                }

                entities.Entry(exisCategory).State = EntityState.Modified;

                entities.SaveChanges();

                TempData["success"] = "Edited successfully!";
                return RedirectToAction("Index", "Category", new { area = "Admin" });
            }
            catch (Exception)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction("Index", "Category", new { area = "Admin" });
            }
        }

        public ActionResult SoftDelete(int id)
        {
            var category = entities.Categories.Find(id);

            if (category != null)
            {
                category.IsDeleted = true;
                entities.SaveChanges();

                TempData["success"] = "Delete successfully!";
            }
            else
            {
                TempData["error"] = "No category found!";
            }
            return RedirectToAction("Index", "Category", new { area = "Admin" });
        }

        [HttpGet]
        public ActionResult Trash()
        {
            var category = entities.Categories.Where(x => x.IsDeleted == true).ToList();
            return View(category);
        }

        public ActionResult Restore(int id)
        {
            var category = entities.Categories.FirstOrDefault(x => x.CategoryId == id);
            if (category != null)
            {
                category.IsDeleted = false;
                entities.SaveChanges();

                TempData["success"] = "Restore successfully!";
            }
            else
            {
                TempData["error"] = "Restore failed because Catalog not found or catalog not deleted!";
            }
            return RedirectToAction("Trash", "Category", new { area = "Admin" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var category = entities.Categories.Find(id);
            if (category != null)
            {
                entities.Categories.Remove(category);
                entities.SaveChanges();

                TempData["success"] = "Permanent deletion successful!";
            }

            return RedirectToAction("Trash", "Category", new { area = "Admin" });
        }

        [HttpGet]
        public ActionResult Status(int id, bool status)
        {
            try
            {
                var cate = entities.Categories.Find(id);

                if (cate == null)
                {
                    TempData["error"] = "Category not found!";
                    return RedirectToAction("Index", "Category", new { area = "Admin" });

                }

                cate.Status = status;
                cate.UpdatedAt = DateTime.UtcNow;

                entities.Entry(cate).State = EntityState.Modified;
                entities.SaveChanges();

                TempData["success"] = "Status updateed successfuly!";
                return RedirectToAction("Index", "Category", new { area = "Admin" });

            }
            catch (Exception)
            {
                TempData["error"] = "Category not found!";
                return RedirectToAction("Index", "Category", new { area = "Admin" });
            }
        }
    }
}