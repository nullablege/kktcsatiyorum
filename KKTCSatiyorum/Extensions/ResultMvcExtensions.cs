using BusinessLayer.Common.Constants;
using BusinessLayer.Common.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace KKTCSatiyorum.Extensions
{
    public static class ResultMvcExtensions
    {

        public static void AddToModelState(this Result result, ModelStateDictionary modelState)
        {
            if (result.IsSuccess) return;

            if(result.Error?.Type == ErrorType.Validation)
            {
                foreach(var ve in result.ValidationErrors)
                {
                    modelState.AddModelError(ve.Field, ve.Message);
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
                    modelState.AddModelError("SeoSlug", result.Error.Message);
                    break;
                case ErrorCodes.Kategori.ParentNotFound:
                    modelState.AddModelError("UsetKategoriId", result.Error.Message);
                    break;
                default:
                    modelState.AddModelError(string.Empty, result.Error.Message);
                    break;
            }

        }

    }
}
