using System;
using System.IO;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Gonzaga.SalesDataProcessor.Api.Features.Sales.ProcessSalesReport
{
    /// <summary>
    /// Validates that a <see cref="ProcessSalesReportCommand"/> carries a non-empty CSV file.
    /// </summary>
    public class ProcessSalesReportCommandValidator : AbstractValidator<ProcessSalesReportCommand>
    {
        public ProcessSalesReportCommandValidator()
        {
            RuleFor(x => x.File)
                .NotNull()
                .WithMessage("A file is required.");

            RuleFor(x => x.File)
                .Must(f => f != null && f.Length > 0)
                .WithMessage("The file is empty.")
                .When(x => x.File != null);

            RuleFor(x => x.File)
                .Must(f => f != null && string.Equals(Path.GetExtension(f.FileName), ".csv", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only CSV files are allowed.")
                .When(x => x.File != null && !string.IsNullOrWhiteSpace(x.File.FileName));
        }
    }
}
