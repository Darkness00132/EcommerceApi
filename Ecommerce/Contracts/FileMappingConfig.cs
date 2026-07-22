using Application.Common;
using AutoMapper;

namespace Ecommerce.Api.Contracts
{
    public class FileMappingConfig : Profile
    {
        public FileMappingConfig()
        {
            CreateMap<IFormFile, FileDto>()
                .ConvertUsing(file => file == null
                    ? null!
                    : new FileDto(
                        file.OpenReadStream(),
                        file.ContentType,
                        file.FileName));

        }
    }
}
