using Ganss.Xss;

namespace BugFixer.Application.Security
{
    public static class XssSecurity
    {
        public static string SanitizeText(this string text)
        {
            var sanitize = new HtmlSanitizer()
            {
                AllowedTags = { "p" },
                AllowDataAttributes = true
            };

            //// Add allowed tags
            //sanitize.AllowedTags.Add("p");

            //// Add allowed attributes
            //sanitize.AllowedAttributes.Add("class"); // Example: Add specific attributes as needed

            return sanitize.Sanitize(text);
        }
    }
}
