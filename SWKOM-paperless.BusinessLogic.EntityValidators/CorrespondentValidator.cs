using FluentValidation;
using SWKOM_paperless.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.EntityValidators
{
    public class CorrespondentValidator : AbstractValidator<Correspondent>
    {
        public CorrespondentValidator() 
        {
            RuleFor(x => x.Id).NotEmpty().NotNull();
            RuleFor(x => x.Name).NotEmpty().NotNull();
        }
    }
}
