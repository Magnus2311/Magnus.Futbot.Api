namespace Magnus.Futtbot.Connections.Models.Auth
{
    public class LoginDetails
    {
        public LoginDetails(string email, string password, string twoFactorCode)
        {
            Email = email;
            Password = password;
            TwoFactorCode = twoFactorCode;
        }

        public string Email { get; }

        public string Password { get; }

        public string TwoFactorCode { get; }
    }
}
