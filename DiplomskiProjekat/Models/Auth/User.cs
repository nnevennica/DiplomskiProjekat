namespace DiplomskiProjekat.Models.Auth
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";

        public string City { get; set; } = ""; 
    }
}
