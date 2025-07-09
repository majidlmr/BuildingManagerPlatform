using FluentValidation;
using System;
using BuildingManager.API.Domain.Entities; // For AssignmentStatus

namespace BuildingManager.API.Application.Features.UserManagement.Commands.AssignRole
{
    public class AssignRoleToUserCommandValidator : AbstractValidator<AssignRoleToUserCommand>
    {
        public AssignRoleToUserCommandValidator()
        {
            RuleFor(x => x.UserPublicId)
                .NotEmpty().WithMessage("شناسه عمومی کاربر اجباری است.");

            RuleFor(x => x.RoleNormalizedName)
                .NotEmpty().WithMessage("نام نرمال شده نقش اجباری است.")
                .MaximumLength(100);

            // TargetEntityPublicId is optional, so no NotEmpty() here.
            // If provided, it should be a valid Guid.
            RuleFor(x => x.TargetEntityPublicId)
                .NotEqual(Guid.Empty).When(x => x.TargetEntityPublicId.HasValue)
                .WithMessage("شناسه عمومی هدف نامعتبر است.");

            RuleFor(x => x.InitialAssignmentStatus)
                .IsInEnum().WithMessage("وضعیت تخصیص اولیه نامعتبر است.");
        }
    }
}
