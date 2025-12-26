using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Kategoriler.DTOs
{
    public sealed record SoftDeleteKategoriResponse
    {
        public int Id { get; init; }
    }
}
