using FluentValidation;

namespace BuildingManager.API.Application.Features.RoleManagement.Commands.AssignPermission
{
    public class AssignPermissionToRoleCommandValidator : AbstractValidator<AssignPermissionToRoleCommand>
    {
        public AssignPermissionToRoleCommandValidator()
        {
            RuleFor(x => x.RoleNormalizedName)
                .NotEmpty().WithMessage("نام نرمال شده نقش اجباری است.")
                .MaximumLength(100);

            RuleFor(x => x.PermissionName)
                .NotEmpty().WithMessage("نام مجوز اجباری است.")
                .MaximumLength(150); // Matching Permission entity's Name max length
        }
    }
}
