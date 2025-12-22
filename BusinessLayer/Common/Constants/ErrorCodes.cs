using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Common.Constants
{
    public static class ErrorCodes
    {
        public static class Kategori
        {
            public const string SlugExists = "kategori_slug_exists";
            public const string ParentNotFound = "kategori_parent_not_found";
            public const string NotFound = "kategori_not_found";
        }

        public static class Ilan
        {
            public const string NotFound = "ilan_not_found";
            public const string NotActive = "ilan_not_active";
        }

        public static class Common
        {
            public const string ValidationError = "validation_error";
            public const string Unexpected = "unexpected_error";
        }
    }
}
