using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Common.Text
{
    public class SlugHelper
    {
        public static string Slugify(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var s = input.Trim().ToLowerInvariant()
                         .Replace("ş", "s").Replace("ı", "i")
                         .Replace("ğ", "g").Replace("ç", "c")
                         .Replace("ö", "o").Replace("ü", "u");
            var sb = new System.Text.StringBuilder(s.Length);
            var lastDash = false;

            foreach(var ch in s) {
                if (char.IsLetterOrDigit(ch))
                {
                    sb.Append(ch);
                    lastDash = false;
                }
                else if (!lastDash)
                {
                    sb.Append('-');
                    lastDash = true;
                }
            }

            var slug = sb.ToString().Trim('-');
            if (slug.Length > 150)
                slug = slug[..150].Trim('-');
            
            return slug;
        }

    }
}
