namespace NHLytics.Models
{
    public class FbModel
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires_at { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AppScopedUserId { get; set; }
        public PagesResponse Pages { get; set; }
    }
}