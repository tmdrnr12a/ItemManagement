using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ItemManagement.Models;
using ItemManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ItemManagement.Controllers
{
    public class ItemController : Controller
    {
        #region Variables

        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment webHostEnv;

        [BindProperty]
        public Item Item { get; set; }

        [BindProperty]
        public ItemViewModel ItemViewModel { get; set; }

        #endregion Variables

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
            Item = new Item
            {
                Name = model.Name,
                Price = model.Price,
                Icon = UploadFile(model),
                Description = model.Description
            };

            _db.Add(Item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private string UploadFile(ItemViewModel model)
        {
            string fileName = null;

            if (model.Icon != null)
            {
                string uploadFolder = Path.Combine(webHostEnv.WebRootPath, "images");
                fileName = $"{Guid.NewGuid()}_{model.Icon.FileName}";
                string filePath = Path.Combine(uploadFolder, fileName);
                using FileStream fileStream = new FileStream(filePath, FileMode.Create);
                model.Icon.CopyTo(fileStream);
            }
            return fileName;
        }

        #endregion Add Item

        #region Edit Item

        public IActionResult Edit(int? id)
        {
            Item = _db.Items.FirstOrDefault(u => u.Id == id);

            if (Item == null)
            {
                return NotFound();
            }

            ItemViewModel = new ItemViewModel()
            {
                Id = Item.Id,
                Name = Item.Name,
                Price = Item.Price,
                Description = Item.Description,
                Icon = ReturnFormFile(Item.Icon)
            };

            return View(ItemViewModel);
        }

        public IFormFile ReturnFormFile(string fileName)
        {
            string filePath = $"wwwroot/images/{fileName}";

            FileStream fs = null;
            FileStreamResult fsr;

            var ms = new MemoryStream();

            try
            {
                fs = new FileStream(filePath, FileMode.Open);
                fsr = new FileStreamResult(fs, "image/jpeg");

                fsr.FileStream.CopyTo(ms);
                return new FormFile(ms, 0, fs.Length, fileName, fsr.FileDownloadName);
            }
            catch (Exception)
            {
                if (ms != null)
                    ms.Dispose();

                if (fs != null)
                    fs.Dispose();

                return null;
            }
            finally
            {
                if (ms != null)
                    ms.Dispose();

                if (fs != null)
                    fs.Dispose();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ItemViewModel model)
        {
            Item = new Item
            {
                Id = model.Id,
                Name = model.Name,
                Price = model.Price,
                Icon = UploadFile(model),
                Description = model.Description
            };

            _db.Items.Update(Item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        #endregion Edit Item

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