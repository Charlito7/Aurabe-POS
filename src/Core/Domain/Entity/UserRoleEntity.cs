using Core.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Entities;

public class UserRoleEntity : IdentityRole
{
    public UserRoleEntity() { }

    //Creating User Roles
    public UserRoleEntity(UserRoleEnums role) : base(role.ToString()!) { }
    public UserRoleEntity(string role) : base(role) { }
}
