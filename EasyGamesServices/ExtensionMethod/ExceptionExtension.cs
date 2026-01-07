using System;

namespace EasyGames.Services.ExtensionMethod
{
    public static class ExceptionExtension
    {
        public static string ToErrorMessage(this Exception exception, string separator = null)
        {
            if (exception == null)
                return string.Empty;

            separator ??= "! ";

            var message = exception.Message;
            var inner = exception.InnerException;
            var lastMessage = exception.Message;

            while (inner != null)
            {
                if (lastMessage != inner.Message)
                {
                    message = string.Join(separator, message, inner.Message);
                }

                lastMessage = inner.Message;
                inner = inner.InnerException;
            }

            return message;
        }
    }
}
