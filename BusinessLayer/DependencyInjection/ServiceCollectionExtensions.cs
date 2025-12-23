using BusinessLayer.Features.Kategoriler.DTOs;
using BusinessLayer.Features.Kategoriler.Services;
using BusinessLayer.Features.Kategoriler.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
        {
            services.AddScoped<IKategoriService, KategoriService>();
            services.AddScoped<IKategoriSlugService, KategoriSlugService>();
            services.AddScoped<IValidator<CreateKategoriRequest>, CreateKategoriValidator>();

            return services;
        }
    }
}
