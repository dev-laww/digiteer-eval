namespace TaskManager.Dto;

public class ErrorDetail
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int Code { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public ErrorDetail? Error { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "Success", int code = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Code = code,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string message, int code, ErrorDetail? error = null, T? data = default)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Code = code,
            Message = message,
            Data = data,
            Error = error
        };
    }
}


