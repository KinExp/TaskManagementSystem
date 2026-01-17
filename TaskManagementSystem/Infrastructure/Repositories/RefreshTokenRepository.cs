using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces.Repositories;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext;

        public RefreshTokenRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task AddAsync(RefreshToken token)
        {
            _dbContext.RefreshTokens.Add(token);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _dbContext.RefreshTokens.Update(token);
            await _dbContext.SaveChangesAsync();
        }
    }
}
