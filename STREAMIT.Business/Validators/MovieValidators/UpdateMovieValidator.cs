using FluentValidation;
using STREAMIT.Business.Dtos.MovieDtos;
using STREAMIT.Business.Helpers;

namespace STREAMIT.Business.Validators
{
    public class UpdateMovieDtoValidator : AbstractValidator<UpdateMovieDto>
    {
        public UpdateMovieDtoValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Movie ID must be greater than 0.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(150).WithMessage("Title can't exceed 150 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required.");

            When(x => x.Poster != null, () =>
            {
                RuleFor(x => x.Poster)
                    .Must(x => x?.CheckSize(2) ?? true)
                    .WithMessage("Poster maximum size is 2 MB.")
                    .Must(x => x?.CheckType("image") ?? true)
                    .WithMessage("Only image format is allowed.");
            });

          

            When(x => x.Movie != null, () =>
            {
                RuleFor(x => x.Movie)
                    .Must(x => x?.CheckSize(500) ?? true)
                    .WithMessage("Movie maximum size is 500 MB.")
                    .Must(x => x?.CheckType("video") ?? true)
                    .WithMessage("Only video format is allowed.");
            });


            RuleFor(x => x.Duration)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.");

            RuleFor(x => x.MembershipId)
                .GreaterThan(0).WithMessage("MembershipId must be greater than 0.");

            RuleFor(x => x.LanguageId)
                .GreaterThan(0).WithMessage("LanguageId must be greater than 0.");

           
            RuleForEach(x => x.GenreIds)
                .GreaterThan(0).WithMessage("GenreIds must be greater than 0.");

            RuleForEach(x => x.TagIds)
                .GreaterThan(0).WithMessage("TagIds must be greater than 0.");

            RuleForEach(x => x.PersonIds)
                .GreaterThan(0).WithMessage("PersonIds must be greater than 0.");
        }
    }
}
