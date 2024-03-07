using AutoMapper;
using Core.Application.Model.Request;
using Core.Application.Model.Response;
using Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mapper.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile() 
    {
        CreateMap<CreateUserModel, UserEntity>();
        CreateMap<UserEntity, GetProfileDataResponse>();    
    }

   
}
