using StocksApi.Core.Constants;

namespace StocksApi.Core.Models
{
    public class Result<T>
    {
        public T Data { get; set; }
        public bool Success { get { return Error == null; } }
        public Error Error { get; set; }

        public static Result<T> OKResult(T data)
        {
            return new Result<T>
            {
                Data = data
            };
        }

        public static Result<T> ResultFromError(Error error)
        {
            return new Result<T>
            {
                Error = error
            };
        }

        public static Result<T> NotFoundResult(string errorMessage)
        {
            return new Result<T>
            {
                Error = new Error
                {
                    ErrorCode = ErrorCodes.NotFound,
                    Message = errorMessage
                }
            };
        }

        public static Result<T> BadRequestResult(string errorMessage)
        {
            return new Result<T>
            {
                Error = new Error
                {
                    ErrorCode = ErrorCodes.BadRequest,
                    Message = errorMessage
                }
            };
        }

        public static Result<T> UnexpectedErrorResult(string errorMessage)
        {
            return new Result<T>
            {
                Error = new Error
                {
                    ErrorCode = ErrorCodes.UnexpectedError,
                    Message = errorMessage
                }
            };
        }

        public bool IsNotFoundError() => Error?.ErrorCode == ErrorCodes.NotFound;
        public bool IsBadRequestError() => Error?.ErrorCode == ErrorCodes.BadRequest;
        public bool IsConflictError() => Error?.ErrorCode == ErrorCodes.Conflict;
        public bool IsUnauthorisedError() => Error?.ErrorCode == ErrorCodes.Unauthorised;
    }
}
