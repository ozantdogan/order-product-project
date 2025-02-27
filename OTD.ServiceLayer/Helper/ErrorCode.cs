namespace OTD.ServiceLayer.Helper;

public class ErrorCode
{
    public static ErrorCode Success = new ErrorCode("0", "Success");
    public static ErrorCode Failed = new ErrorCode("1", "Failed");
    public static ErrorCode NotFound = new ErrorCode("2", "Entity not found");
    public static ErrorCode EmailAlreadyInUse = new ErrorCode("3", "Email already in use.");
    public static ErrorCode EmailAlreadyConfirmed = new ErrorCode("4", "Email is already confirmed.");
    public static ErrorCode OtpExpired = new ErrorCode("4", "One-time password has expired. Request a new one.");

    public ErrorCode(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Code { get; set; }
    public string Message { get; set; }
}
