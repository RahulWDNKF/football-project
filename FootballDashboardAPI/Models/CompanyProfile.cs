namespace FootballDashboardAPI.Models
{
    public class CompanyProfile
    {
        public int Id { get; set; } = 1;
        // Basic Info
        public string CompanyName { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Tagline { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? FoundedYear { get; set; }
        public string LogoUrl { get; set; } = string.Empty;
        public string PrimaryColor { get; set; } = string.Empty;

        // Contact Info
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string AlternatePhone { get; set; } = string.Empty;

        // Address
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AreaLocality { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;

        // Organization Details
        public string OrganizationType { get; set; } = string.Empty;
        public string SportType { get; set; } = string.Empty;

        // Social Media
        public string FacebookUrl { get; set; } = string.Empty;
        public string InstagramUrl { get; set; } = string.Empty;
        public string TwitterUrl { get; set; } = string.Empty;
        public string LinkedinUrl { get; set; } = string.Empty;
        public string YoutubeUrl { get; set; } = string.Empty;
    }
}
