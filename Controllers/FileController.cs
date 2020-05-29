using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using FileRepo.Models;
using Microsoft.AspNetCore.Routing;

namespace FileRepo.Controllers
{
    public class FileController : Controller
    {
        private readonly IWebHostEnvironment _FEnvironment;

        public FileController(IWebHostEnvironment FEnvironment)
        {
            _FEnvironment = FEnvironment;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile fileObject)
        {
            var filepath = Path.Combine(_FEnvironment.WebRootPath, "Files", fileObject.FileName);
            var stream = new FileStream(filepath, FileMode.Create);
            await fileObject.CopyToAsync(stream);
            stream.Close();
            return RedirectToAction("Index");
        }

        // Загрузка файла с сервера.
        [HttpGet]
        public IActionResult Get(string filename)
        {
            var fileDirectory = Path.Combine(_FEnvironment.WebRootPath, "Files");
            var filepath = Path.Combine("~/Files", filename);
            if (CheckFileDir(fileDirectory, filename))
            {
                return File(filepath, "application/unknown", filename);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [HttpHead]
        [Route("{controller=File}/{action=Download}/{filename}")]
        public IActionResult Download()
        {
            var filename = RouteData.Values["filename"].ToString();
            return Get(filename);
        }

        public IActionResult Index()
        {
            FileClass fileEntity = new FileClass();
            var fileDisplay = Path.Combine(_FEnvironment.WebRootPath, "Files");
            DirectoryInfo directoryInfo = new DirectoryInfo(fileDisplay);
            FileInfo[] fileInfo = directoryInfo.GetFiles();
            fileEntity.FileImage = fileInfo;
            return View(fileEntity);
        }

        public IActionResult Remove(string filepath)
        {
            filepath = Path.Combine(_FEnvironment.WebRootPath, "Files", filepath);
            FileInfo fileInfo = new FileInfo(filepath);
            // Checking whether we selected a file to remove.
            if (fileInfo != null)
            {
                System.IO.File.Delete(filepath);
                fileInfo.Delete();
            }
            return RedirectToAction("Index");
        }

        [Route("{controller=File}/{action=Delete}/{filepath}")]
        [HttpDelete]
        public IActionResult Delete()
        {
            var filepath = RouteData.Values["filepath"].ToString();
            return Remove(filepath);
        }

        private bool CheckFileDir(string filepath, string filename)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(filepath);
            FileInfo[] fileInfo = directoryInfo.GetFiles();
            // Переборка всех файлов в директории.
            foreach (var file in fileInfo)
            {
                if (file.Name == filename)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
