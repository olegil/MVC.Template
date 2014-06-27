﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Template.Objects
{
    public class ProfileEditView : BaseView
    {
        [Required]
        public String Username { get; set; }

        [Required]
        public String Password { get; set; }
        public String NewPassword { get; set; }

        [Required]
        [EmailAddress]
        public String Email { get; set; }
    }
}