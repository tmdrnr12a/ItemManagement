using System;
using System.IO;
using System.Threading.Tasks;
using ItemManagement.Models;
using ItemManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItemManagement.Controllers
{
    public class ItemController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnv;

        public ItemController(AppDbContext db, IWebHostEnvironment hostEnv)
        {
            _db = db;
            webHostEnv = hostEnv;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Add Item

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                Item item = new Item
                {
                    Name = model.Name,
                    Price = model.Price,
                    Icon = UploadedFile(model),
                    Description = model.Description
                };

                _db.Add(item);
                await _db.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View();
        }



        private string UploadedFile(ItemViewModel model)
        {
            string fileName = null;

            if (model.Icon != null)
            {
                string uploadFolder = Path.Combine(webHostEnv.WebRootPath, "images");
                fileName = $"{Guid.NewGuid()}_{model.Icon.FileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Icon.CopyTo(fileStream);
                }
            }
            return fileName;
        }

        #endregion Add Item


        //public IActionResult Upsert(int? id)
        //{
        //    Item = new Item();

        //    if (id == null)
        //    {
        //        ItemViewModel ivm = new ItemViewModel();
        //        //create
        //        return View(Item);
        //        //return View(ivm);
        //    }

        //    //update

        //    Item = _db.Items.FirstOrDefault(u => u.Id == id);

        //    if (Item == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(Item);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Upsert()
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (Item.Id == 0)
        //        {
        //            //create
        //            _db.Items.Add(Item);
        //        }
        //        else
        //        {
        //            _db.Items.Update(Item);
        //        }

        //        _db.SaveChanges();

        //        return RedirectToAction("Index");
        //    }
        //    return View(Item);
        //}


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Json(new { data = await _db.Items.ToListAsync() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var itemFromDB = await _db.Items.FirstOrDefaultAsync(u => u.Id == id);

            if (itemFromDB == null)
            {
                return Json(new { success = false, message = "삭제하는 동안 에러가 발생했습니다." });
            }

            _db.Items.Remove(itemFromDB);
            await _db.SaveChangesAsync();

            return Json(new { success = true, message = "삭제되었습니다." });
        }
    }
}