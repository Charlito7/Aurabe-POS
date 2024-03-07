﻿using Core.Application.Commons.ServiceResult;
using Core.Application.Model.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Interfaces.Services.User;

public interface IUpdateUserPassword
{
    Task<ServiceResult<bool>> UpdateUserPassowrdAsync(UpdateUserPasswordModel model);
}
