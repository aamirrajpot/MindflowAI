using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using MindflowAI.Entities.AppUser;
using MindflowAI.Entities.EmailOtp;
using MindflowAI.Services.Dtos.AppUser;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;
using Volo.Abp.Users;

namespace MindflowAI.Services.User
{
    public class UserAccountAppService : ApplicationService,IUserAccountAppService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<EmailOtp, Guid> _otpRepo;
        private readonly IEmailSender _emailSender;
        private readonly ISettingProvider _settingProvider;
        private readonly ISettingEncryptionService _encryptionService;
        private readonly IUserRepository<AppUser> _userRepository;

        public UserAccountAppService(
            IdentityUserManager userManager,
            IRepository<EmailOtp, Guid> otpRepo,
            IEmailSender emailSender,
            ISettingProvider settingProvider,
            ISettingEncryptionService encryptionService,
            IUserRepository<AppUser> userRepository)
        {
            _userManager = userManager;
            _otpRepo = otpRepo;
            _emailSender = emailSender;
            _settingProvider = settingProvider;
            _encryptionService = encryptionService;
            _userRepository = userRepository;
        }
        [AllowAnonymous]
        public async Task<Guid> RegisterAsync(RegisterWithOtpDto input)
        {
            var user = new AppUser(
                GuidGenerator.Create(),
                input.Email,
                input.Email)
            {
                FirstName = input.FirstName,
                LastName = input.LastName,
            };
            user.SetProperty("IsActive", false);
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();
            user.SetIsActive(false);

            var otpCode = GenerateOtp();
            await _otpRepo.InsertAsync(new EmailOtp(
                Guid.NewGuid(), user.Id, otpCode, DateTime.UtcNow.AddMinutes(5)
            ), autoSave: true);
            await _emailSender.SendAsync(input.Email
                , "Your Mindflow AI OTP Code"
                , $@"
                    Hi {user.Name},<br/><br/>
                    Welcome to <b>Mindflow AI</b>! To complete your registration, please use the following One-Time Password (OTP):<br/><br/>
                    <h2 style='color:#2e86de;'>🔐 {otpCode}</h2><br/>
                    This code is valid for the next <b>5 minutes</b>.<br/><br/>
                    If you did not request this, you can safely ignore this email.<br/><br/>
                    Thanks,<br/>
                    <b>Mindflow AI Team</b>
                    ",true
                );

            //Role removed
            return user.Id;
        }
        [AllowAnonymous]
        public async Task VerifyOtpAsync(OtpVerificationDto input)
        {
            var otp = await _otpRepo.FirstOrDefaultAsync(x =>
                x.UserId == input.UserId &&
                x.Code == input.Code &&
                !x.IsUsed &&
                x.ExpirationTime > DateTime.UtcNow
            );

            if (otp == null)
                throw new UserFriendlyException("Invalid or expired OTP");

            otp.IsUsed = true;
            await _otpRepo.UpdateAsync(otp);

            var user = await _userManager.FindByIdAsync(input.UserId.ToString());
            user.SetIsActive(true);
            await _userManager.UpdateAsync(user);
        }
        [Authorize] // Requires authentication
        public async Task<bool> ChangePasswordAsync(ChangePasswordDto input)
        {
            // Get the current user
            var user = await _userManager.GetByIdAsync(CurrentUser.Id!.Value);

            // Verify current password
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, input.CurrentPassword);
            if (!isPasswordValid)
            {
                throw new UserFriendlyException("Current password is incorrect.");
            }

            // Change the password
            var result = await _userManager.ChangePasswordAsync(user,input.CurrentPassword, input.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new UserFriendlyException($"Failed to change password: {errors}");
            }

            return true;
        }
        [Authorize]
        public async Task<MyProfileDto> GetMyProfileAsync()
        {
            var user = await _userRepository.GetAsync(CurrentUser.GetId());

            return new MyProfileDto
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.GetProperty<string>("FirstName"),
                LastName = user.GetProperty<string>("LastName"),
                DateOfBirth = user.GetProperty<DateTime?>("DateOfBirth")
            };
        }
        public async Task UpdateMyProfileAsync(UpdateMyProfileDto input)
        {
            var user = await _userRepository.GetAsync(CurrentUser.GetId());

            user.SetProperty("FirstName", input.FirstName?.Trim());
            user.SetProperty("LastName", input.LastName?.Trim());
            user.SetProperty("DateOfBirth", input.DateOfBirth);

            await _userManager.UpdateAsync(user);
        }
        private string GenerateOtp() =>
            new Random().Next(1000, 9999).ToString();
    }



}
