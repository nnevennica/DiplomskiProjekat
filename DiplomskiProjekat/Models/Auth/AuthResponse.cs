namespace DiplomskiProjekat.Models.Auth
{
    public class AuthResponse
    {
        public object User { get; set; } = default!;
        public string AccessToken { get; set; } = "";
    }
}
