namespace OTD.ServiceLayer.Helper;

public class ErrorCode
{
    public static ErrorCode Success = new ErrorCode("0", "Success");
    public static ErrorCode Failed = new ErrorCode("1", "Failed");

    public ErrorCode(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; set; }
    public string Message { get; set; }
}
