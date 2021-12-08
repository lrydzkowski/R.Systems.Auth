using System.Collections.Generic;

namespace R.Systems.Auth.SharedKernel.Validation
{
    public class ErrorInfo
    {
        public ErrorInfo(
            string errorKey,
            string? elementKey = null,
            string? errorMsg = null,
            Dictionary<string, string>? data = null)
        {
            ErrorKey = errorKey;
            ElementKey = elementKey;
            ErrorMsg = errorMsg;
            Data = data;
        }

        public string ErrorKey { get; }
        public string? ElementKey { get; }
        public string? ErrorMsg { get; }
        public Dictionary<string, string>? Data { get; }
    }
}
