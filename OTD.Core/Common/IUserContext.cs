namespace OTD.Core.Common
{
    public interface IUserContext
    {
        Guid UserId { get; }
        string Email { get; }
        string FirstName { get; }
        string LastName { get; }
        string DisplayName { get; }
    }
}
