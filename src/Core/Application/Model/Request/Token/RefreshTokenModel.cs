﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Model.Request;

public class RefreshTokenModel
{
    public string? Token { get; set; }
    public string? RefreshToken { get; set; } 
}
