using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerTests.Services
{
    interface IModelValidatorService
    {
        bool IsPostedModelValid();
        void AddError(string key, string errorMessage);
    }
}
