using Microsoft.AspNetCore.Mvc;

public class ImageController : Controller
{
    private readonly BlobStorageService _blobStorageService;

    public ImageController(BlobStorageService blobStorageService)
    {
        _blobStorageService = blobStorageService;
    }

    [HttpGet]
    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            ViewBag.Error = "Please select a file";
            return View();
        }

        // Validate file type
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            ViewBag.Error = "Invalid file type. Only images allowed.";
            return View();
        }

        try
        {
            var imageUrl = await _blobStorageService.UploadImageAsync(file);
            ViewBag.ImageUrl = imageUrl;
            ViewBag.Message = "Image uploaded successfully!";
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Upload failed: {ex.Message}";
        }

        return View();
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        try
        {
            var images = await _blobStorageService.ListImagesAsync();
            return View(images);
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Error loading images: {ex.Message}";
            return View(new List<string>());
        }
    }
    public async Task<IActionResult> Delete(string imageName)
    {
        if (string.IsNullOrEmpty(imageName))
        {
            ViewBag.Error = "Image name is required";
            return RedirectToAction("List");
        }
        try
        {
            await _blobStorageService.DeleteImageAsync(imageName);
            ViewBag.Message = "Image deleted successfully!";
        }
        catch (Exception ex)
        {
            ViewBag.Error = $"Delete failed: {ex.Message}";
        }
        return RedirectToAction("List");
    }
}


