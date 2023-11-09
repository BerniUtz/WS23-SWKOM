using FluentValidation;
using SWKOM_paperless.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.EntityValidators
{
    public class NewCorrespondentValidator : AbstractValidator<NewCorrespondent>
    {
        public NewCorrespondentValidator() 
        {
            RuleFor(x => x.Name).NotNull().NotEmpty();
        }
    }
}
