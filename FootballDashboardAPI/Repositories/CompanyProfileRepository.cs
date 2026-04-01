using FootballDashboardAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FootballDashboardAPI.Repositories
{
    public interface ICompanyProfileRepository
    {
        Task<CompanyProfile?> GetAsync();
        Task<CompanyProfile> UpsertAsync(CompanyProfileDto dto);
    }

    public class CompanyProfileRepository : ICompanyProfileRepository
    {
        private readonly Data.AppDbContext _context;
        public CompanyProfileRepository(Data.AppDbContext context)
        {
            _context = context;
        }

        public async Task<CompanyProfile?> GetAsync()
        {
            return await _context.CompanyProfiles.FirstOrDefaultAsync(cp => cp.Id == 1);
        }

        public async Task<CompanyProfile> UpsertAsync(CompanyProfileDto dto)
        {
            var entity = await _context.CompanyProfiles.FirstOrDefaultAsync(cp => cp.Id == 1);
            if (entity == null)
            {
                entity = new CompanyProfile { Id = 1 };
                _context.CompanyProfiles.Add(entity);
            }
            // Map fields
            entity.CompanyName = dto.CompanyName;
            entity.ShortName = dto.ShortName;
            entity.Tagline = dto.Tagline;
            entity.Description = dto.Description;
            entity.FoundedYear = dto.FoundedYear;
            entity.LogoUrl = dto.LogoUrl;
            entity.PrimaryColor = dto.PrimaryColor;
            entity.Email = dto.Email;
            entity.PhoneNumber = dto.PhoneNumber;
            entity.AlternatePhone = dto.AlternatePhone;
            entity.AddressLine1 = dto.AddressLine1;
            entity.AddressLine2 = dto.AddressLine2;
            entity.AreaLocality = dto.AreaLocality;
            entity.City = dto.City;
            entity.District = dto.District;
            entity.State = dto.State;
            entity.Country = dto.Country;
            entity.PostalCode = dto.PostalCode;
            entity.OrganizationType = dto.OrganizationType;
            entity.SportType = dto.SportType;
            entity.FacebookUrl = dto.FacebookUrl;
            entity.InstagramUrl = dto.InstagramUrl;
            entity.TwitterUrl = dto.TwitterUrl;
            entity.LinkedinUrl = dto.LinkedinUrl;
            entity.YoutubeUrl = dto.YoutubeUrl;

            await _context.SaveChangesAsync();
            return entity;
        }
    }
}