using System.Net;

namespace InventoryManagementSystemServices
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsSuccess => StatusCode >= HttpStatusCode.OK && StatusCode < HttpStatusCode.BadRequest;

        public ServiceResult(T data, HttpStatusCode statusCode, string errorMessage = null)
        {
            Data = data;
            StatusCode = statusCode;
            ErrorMessage = errorMessage;
        }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>(data, HttpStatusCode.OK);
        }

        public static ServiceResult<T> NotFound(string errorMessage)
        {
            return new ServiceResult<T>(default, HttpStatusCode.NotFound, errorMessage);
        }

        public static ServiceResult<T> BadRequest(string errorMessage)
        {
            return new ServiceResult<T>(default, HttpStatusCode.BadRequest, errorMessage);
        }

        public static ServiceResult<T> InternalServerError(string errorMessage)
        {
            return new ServiceResult<T>(default, HttpStatusCode.InternalServerError, errorMessage);
        }
    }
}
