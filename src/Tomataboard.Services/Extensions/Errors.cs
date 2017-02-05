using System;

namespace Tomataboard.Services.Extensions
{
    public static class Errors
    {
        public static string GetError(this Exception ex)
        {
            var error = ex.Message;
            if (ex.InnerException != null)
            {
                error = error + "\n" + ex.InnerException.Message;
            }
            return error;
        }
    }
}