using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KKTCSatiyorum.Extensions
{
    public static class ResultMvcExtensions
    {

        public static void AddToModelState(this Result result, ModelStateDictionary modelState, string? prefix=null)
        {
            if (result.IsSuccess) return;

            static string Key(string? p, string field)
            {
                return string.IsNullOrWhiteSpace(p) ? field : $"{p}.{field}";
            }

            if(result.Error?.Type == ErrorType.Validation)
            {
                foreach(var ve in result.ValidationErrors)
                {
                    modelState.AddModelError(Key(prefix, ve.PropertyName), ve.ErrorMessage);
                }
                return;
            }

            if (result.Error == null)
            {
                modelState.AddModelError(string.Empty, "Beklenmeyen bir hata oluştu.");
                return;
            }

            switch (result.Error.Code)
            {
                case ErrorCodes.Kategori.SlugExists:
                    modelState.AddModelError(Key(prefix, "Ad"), result.Error.Message);
                    break;

                case ErrorCodes.Kategori.ParentNotFound:
                case ErrorCodes.Kategori.ParentConflict:
                case ErrorCodes.Kategori.CycleDetected:
                    modelState.AddModelError(Key(prefix, "UstKategoriId"), result.Error.Message);
                    break;

                default:
                    modelState.AddModelError(string.Empty, result.Error.Message);
                    break;
            }

        }

    }
}
