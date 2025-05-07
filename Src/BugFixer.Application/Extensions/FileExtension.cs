using Microsoft.AspNetCore.Http;

namespace BugFixer.Application.Extensions
{
    public static class FileExtension
    {
        public static bool UploadFile(this IFormFile file, string fileName, string path, List<string?> validFormats = null)
        {

            if (validFormats != null && validFormats.Any())
            {
                var fileFormat = Path.GetExtension(file.FileName);

                if (validFormats.All(validFormats => validFormats != fileFormat))
                {
                    return false;
                }
            }

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var finalPath = Path.Combine(path, fileName);

            using (var stream = new FileStream(finalPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return true;

        }
    }
}
