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

namespace MindflowAI.Services.User
{
    public class UserAccountAppService : ApplicationService
    {
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<EmailOtp, Guid> _otpRepo;
        private readonly IEmailSender _emailSender;

        public UserAccountAppService(
            IdentityUserManager userManager,
            IRepository<EmailOtp, Guid> otpRepo,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _otpRepo = otpRepo;
            _emailSender = emailSender;
        }

        public async Task<Guid> RegisterAsync(RegisterWithOtpDto input)
        {
            var user = new AppUser(
                GuidGenerator.Create(),
                input.Email,
                input.Email)
            {
                FirstName = input.FirstName,
                LastName = input.LastName
            };
            user.SetProperty("IsActive", true);
            (await _userManager.CreateAsync(user, input.Password)).CheckErrors();

            var otpCode = GenerateOtp();
            await _otpRepo.InsertAsync(new EmailOtp(
                Guid.NewGuid(), user.Id, otpCode, DateTime.UtcNow.AddMinutes(10)
            ), autoSave: true);

            await _emailSender.SendAsync(input.Email, "Your OTP Code", $"Your OTP is: {otpCode}");

            return user.Id;
        }
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
            user.SetProperty("IsActive", true);
            await _userManager.UpdateAsync(user);
        }

        private string GenerateOtp() =>
            new Random().Next(100000, 999999).ToString();
    }



}
