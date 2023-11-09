using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using SWKOM_paperless.BusinessLogic.Entities;

namespace SWKOM_paperless.BusinessLogic.EntityValidators
{
    internal class DocumentValidator : AbstractValidator<Document>
    {
        public DocumentValidator() 
        {
            RuleFor(x => x.Id).NotNull().NotEmpty();
            RuleFor(x => x.Title).NotNull().NotEmpty();
        }
    }
    
}
