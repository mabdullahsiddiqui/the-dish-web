namespace TheDish.Common.Application.Common;

public class Response<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static Response<T> SuccessResult(T data, string? message = null)
    {
        return new Response<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static Response<T> FailureResult(string message, List<string>? errors = null)
    {
        return new Response<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}





