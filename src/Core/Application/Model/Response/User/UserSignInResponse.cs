﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Model.Response;

public class UserSignInResponse
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public string? FullName { get; set; }
    public IList<string>? UserRoles { get; set; }
}
