using EasyGames.Class.Enum;
using EasyGames.Services.ExtensionMethod;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Text;


namespace EasyGames.Class.DATA
{
    public class ApiResponse
    {
        public ApiResponse(EnumEntityType entityCode, EnumEntityEvents eventCode)
        {
            EntityCode = entityCode;
            EventCode = eventCode;
            EventMessageId = eventCode.ToString();
        }

        public EnumEntityType EntityCode { get; }
        public EnumEntityEvents EventCode { get; }
        public string EventMessageId { get; protected set; }
    }

    // -------------------- BAD REQUEST --------------------

    public class ApiBadRequestResponse : ApiResponse
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object ErrorDetail { get; }

        // ModelState constructor
        public ApiBadRequestResponse(EnumEntityType entityCode, EnumEntityEvents eventCode, ModelStateDictionary modelState)
            : base(entityCode, eventCode)
        {
            var sb = new StringBuilder();

            foreach (var entry in modelState)
            {
                sb.Append(entry.Key).Append(" : ");

                foreach (var error in entry.Value.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(error.ErrorMessage))
                    {
                        sb.Append(error.ErrorMessage);
                    }
                    else if (error.Exception != null)
                    {
                        sb.Append(error.Exception.ToErrorMessage());
                    }

                    sb.Append("! ");
                }
            }

            ErrorDetail = sb.ToString();
        }

        // String error
        public ApiBadRequestResponse(EnumEntityType entityCode, EnumEntityEvents eventCode, string errorDetail)
            : base(entityCode, eventCode)
        {
            ErrorDetail = errorDetail;
        }

        // Object error
        public ApiBadRequestResponse(EnumEntityType entityCode, EnumEntityEvents eventCode, object errorDetail)
            : base(entityCode, eventCode)
        {
            ErrorDetail = errorDetail;
        }

        // Exception error
        public ApiBadRequestResponse(EnumEntityType entityCode, EnumEntityEvents eventCode, Exception error)
            : base(entityCode, eventCode)
        {
            if (error is CSApplicationException customException)
            {
                EventMessageId = customException.ErrorId;
            }

            ErrorDetail = error.ToErrorMessage();
        }
    }

    // -------------------- OK RESPONSE --------------------

    public class ApiOkResponse : ApiResponse
    {
        public object Data { get; }

        public ApiOkResponse(EnumEntityType entityCode, EnumEntityEvents eventCode, object result)
            : base(entityCode, eventCode)
        {
            Data = result;
        }
    }

    // -------------------- CREATED RESPONSE --------------------

    public class ApiCreatedResponse : ApiOkResponse
    {
        public string Location { get; set; }

        public ApiCreatedResponse(EnumEntityType entityCode, EnumEntityEvents eventCode, object result, string uri)
            : base(entityCode, eventCode, result)
        {
            Location = uri;
        }
    }
}
