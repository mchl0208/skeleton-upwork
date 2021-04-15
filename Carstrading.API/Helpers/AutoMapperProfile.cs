using AutoMapper;
using Carstrading.API.Dtos;
using Carstrading.Core.Entities;

namespace Carstrading.API.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}
