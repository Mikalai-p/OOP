namespace AutoServiceShop.Models
{
    public class UserProfile
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Language { get; set; } = "ru";
        public string Theme { get; set; } = "LightTheme";
    }
}