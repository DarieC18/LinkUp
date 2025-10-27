namespace LinkUp.Application.DTOs
{
    public class EditProfileDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? ProfilePhotoPath { get; set; }
    }
}
