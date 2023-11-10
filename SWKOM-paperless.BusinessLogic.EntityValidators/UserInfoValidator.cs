using FluentValidation;
using SWKOM_paperless.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.EntityValidators
{
    public class UserInfoValidator : AbstractValidator<UserInfo>
    {
        public UserInfoValidator() 
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
