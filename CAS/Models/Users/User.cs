// Inside CAS.Models.Users.User.cs

using System.ComponentModel.DataAnnotations;

namespace CAS.Models.Users
{
    public abstract class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty; 

        [Required]
        public string Email { get; set; } = string.Empty;    

        [Required]
        public string Password { get; set; } = string.Empty; 

        public string Role { get; set; } = string.Empty;    
    }
}