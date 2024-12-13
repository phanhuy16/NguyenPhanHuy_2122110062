

using System.Text.RegularExpressions;
using System.Text;

namespace NguyenPhanHuy_2122110062.Helpers
{
    public static class GenerateSlug
    {
        public static string GenerateSlugs(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return string.Empty;

            string slug = title.Normalize(NormalizationForm.FormD);
            slug = Regex.Replace(slug, @"[^a-zA-Z0-9\s-]", "");
            slug = Regex.Replace(slug, @"\s+", "-");
            return slug.ToLower();
        }
    }
}