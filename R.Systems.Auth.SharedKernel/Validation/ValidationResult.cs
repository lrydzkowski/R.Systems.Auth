using R.Systems.Auth.SharedKernel.Interfaces;
using System.Collections.Generic;

namespace R.Systems.Auth.SharedKernel.Validation
{
    public class ValidationResult : IDependencyInjectionScoped
    {
        public bool Result
        {
            get
            {
                if (Errors.Count == 0)
                {
                    return true;
                }
                return false;
            }
        }

        public List<ErrorInfo> Errors { get; } = new();
    }
}
