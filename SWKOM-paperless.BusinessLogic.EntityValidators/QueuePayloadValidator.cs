using FluentValidation;
using SWKOM_paperless.BusinessLogic.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.BusinessLogic.EntityValidators
{
    public class QueuePayloadValidator : AbstractValidator<QueuePayload>
    {
        public QueuePayloadValidator() 
        {
            RuleFor(x => x.bucket).NotEmpty().NotNull();
            RuleFor(x => x.filename).NotEmpty().NotNull();
            //TODO create more validator rules (e.g. regex filename, check for correct fileextensions,...)
        }
    }
}
