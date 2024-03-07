using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Enums;

public enum UserRoleEnums
{
    Admin,
    User,
    ITAdmin,
    Manager,
    Planner,
    Seller

}
/* 
 ITAdmin: all read/write privileges
 Manager: all read 
 Admin: create stock/ some read write privilege
 Seller: Sales privileges
 User
 Planner

 */
