using HealthTrack.Core.Models.Entities;
using HealthTrack.Core.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace HealthTrack.Core.Services
{
    public class AuthService(UserManager<User> userManager, SignInManager<User> signInManager)
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly SignInManager<User> _signInManager = signInManager;

        public async Task<IdentityResult> RegisterAsync(string firstName, string lastName, string email, string password)
        {
            var user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                UserName = email,
                Email = email,
                Role = UserRole.Patient,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
                await _signInManager.SignInAsync(user, isPersistent: false);

            return result;
        }

        public async Task<SignInResult> LoginAsync(string email, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: false);
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}