using FluentValidation;
using Geocode.Application.Models;

namespace FluentValidationDemo.Validation;

public class GeocodeValidator : AbstractValidator<GeocodeCache>
{
	public GeocodeValidator()
	{
		RuleFor(x => x.HouseNumber)
			.GreaterThan(0)
			.WithMessage("House number must be greater than 0.");
		RuleFor(x => x.Street)
			.NotEmpty()
			.WithMessage("Please specify the street.");
		RuleFor(x => x.City)
			.NotEmpty()
			.WithMessage("Please specify the city.");
		RuleFor(x => x.StateCode)
			.NotEmpty()
			.WithMessage("Please specify the state code");
		RuleFor(x => x.Zipcode)
			.InclusiveBetween(10000, 99999)
			.WithMessage("Zipcode value must be between 10000 and 99999");
		RuleFor(x => x.Country)
			.NotEmpty()
			.WithMessage("Please specify the country.");
	}
}
