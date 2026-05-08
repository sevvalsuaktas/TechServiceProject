namespace TechService.Models.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } // "Admin" veya "Employee" değerlerini alacak
    }
}
