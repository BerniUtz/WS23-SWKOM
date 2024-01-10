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
            RuleFor(x => x.Id).NotEmpty().NotNull();
            RuleFor(x => x.Bucket).NotEmpty().NotNull();
            RuleFor(x => x.Filename).NotEmpty().NotNull();
            //TODO create more validator rules (e.g. regex Filename, check for correct fileextensions,...)
        }
    }
}
