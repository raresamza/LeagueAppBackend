﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.DTOs;

public class LoginDTO
{
    public string Email {get;set;}
    public string Password {get;set;}
}
