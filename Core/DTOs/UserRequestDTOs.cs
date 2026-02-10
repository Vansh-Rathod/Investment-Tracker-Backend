using System;

namespace Core.DTOs
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class UpdateUserRequest
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
    }
}
