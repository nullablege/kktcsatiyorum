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
            public const string ParentConflict = "kategori_parent_conflict";
            public const string HasChildren = "kategori_has_children";
            public const string CycleDetected = "kategori_cycle_conflict_detected";
        }

        public static class Ilan
        {
            public const string NotFound = "ilan_not_found";
            public const string NotActive = "ilan_not_active";
            public const string DuplicateSlug = "ilan_duplicate_slug";
            public const string DuplicateAttribute = "ilan_duplicate_attribute";
            public const string InvalidState = "ilan_invalid_state";
        }

        public static class KategoriAlani
        {
            public const string NotFound = "kategori_alani_not_found";
            public const string DuplicateKey = "kategori_alani_duplicate_key";
            public const string InvalidOptionForDataType = "kategori_alani_invalid_option";
            public const string OptionNotFound = "kategori_alani_option_not_found";
        }

        public static class Common
        {
            public const string ValidationError = "validation_error";
            public const string Unexpected = "unexpected_error";
            public const string CommitFail = "commit_failed";
            public const string Forbidden = "forbidden";
        }
    }
}
