using System.ComponentModel.DataAnnotations;
using service;

namespace Recipe_Web_App.ValidationModels;

public class ValidationEmailExist : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? email, ValidationContext validationContext)
    {
        if (email == null || string.IsNullOrEmpty(email.ToString())) // For edit user
        {
            return ValidationResult.Success;
        }

        var service = validationContext.GetService(typeof(UserService)) as UserService;
        if (service != null && email is string emailStr)
        {
            if (!service.DoesEmailExist(emailStr))
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult("This email already exists");
    }
}