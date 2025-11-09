namespace TalentBridge.Common.DTOs.Responses;

public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
    public static ServiceResult<T> SuccessResult(T data, string message = null)
    {
        return new ServiceResult<T>
        {
            Success = true,
            Data = data,
            Message = message ?? "Operation completed successfully"
        };
    }
        
    public static ServiceResult<T> FailureResult(string error)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Errors = new List<string> { error }
        };
    }
        
    public static ServiceResult<T> FailureResult(List<string> errors)
    {
        return new ServiceResult<T>
        {
            Success = false,
            Errors = errors
        };
    }
}