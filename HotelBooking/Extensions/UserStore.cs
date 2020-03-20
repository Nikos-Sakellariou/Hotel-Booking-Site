using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using HotelBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Extensions
{
    public class UserStore : IUserStore<User>, IUserPasswordStore<User>
    {
        private readonly WdaContext _db;
        public UserStore(WdaContext db)
        {
            this._db = db;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._db?.Dispose();
            }
        }

        public async Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            _db.User.Add(user);
            await _db.SaveChangesAsync(cancellationToken);
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (int.TryParse(userId, out int id))
                return await _db.User.FindAsync(id);
            else
                return await Task.FromResult((User)null);
        }

        public async Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return await _db.User.SingleOrDefaultAsync(p => p.Username.Equals(normalizedUserName, StringComparison.OrdinalIgnoreCase), cancellationToken);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.PasswordHash);

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.UserId.ToString());

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken) => Task.FromResult(user.Username);

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken) => Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken) => Task.FromResult((object)null);

        public async Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            await _db.SaveChangesAsync();
        }

        public async Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            await _db.SaveChangesAsync();
        }

        public async Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            this._db.User.Update(user);
            await _db.SaveChangesAsync();
            return IdentityResult.Success;
        }
    }
}
