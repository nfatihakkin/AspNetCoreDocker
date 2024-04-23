using AspNetCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System.Diagnostics;

namespace AspNetCore.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IFileProvider _fileProvider;

		public HomeController(ILogger<HomeController> logger,IFileProvider provider)
		{
			_logger = logger;
			_fileProvider = provider;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
		public IActionResult ImageShow()
		{
			var images = _fileProvider.GetDirectoryContents("wwwroot/Images").ToList().Select(x => x.Name);

			return View(images);
		}
		[HttpPost]
		public IActionResult ImageShow(string name)
		{
			var file = _fileProvider.GetDirectoryContents("wwwroot/Images").ToList().First(x => x.Name==name);

			System.IO.File.Delete(file.PhysicalPath);
			return RedirectToAction("ImageShow");
		}

		public IActionResult ImageSave()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ImageSave(IFormFile imageFile)
		{
			if (imageFile == null) return BadRequest();
			if (imageFile.Length<=0) return BadRequest();

			var fileNime = Guid.NewGuid().ToString()+Path.GetExtension(imageFile.FileName);
			var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", fileNime);

			using (var stream = new FileStream(path,FileMode.Create))
			{
				await imageFile.CopyToAsync(stream);
			}

			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}