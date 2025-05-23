﻿using System.ComponentModel.DataAnnotations;
using BenMakesGames.Webdiatr.Core;

namespace DemoApp.Features;

public sealed class GetWeather: IRequestHandler<GetWeatherRequest, GetWeatherResponse>
{
    public Task<GetWeatherResponse> Handle(GetWeatherRequest request, CancellationToken cancellationToken)
    {
        Validator.ValidateObject(request, new ValidationContext(request));

        var tempC = Random.Shared.NextSingle() * 40 + -10;

        return Task.FromResult(new GetWeatherResponse()
        {
            City = request.City,
            TemperatureC = tempC,
            TemperatureF = tempC * 9 / 5 + 32
        });
    }
}

public sealed class GetWeatherRequest
{
    [Required, MinLength(1), MaxLength(100)]
    public required string City { get; init; }
}

public sealed class GetWeatherResponse
{
    public required string City { get; init; }
    public required float TemperatureC { get; init; }
    public required float TemperatureF { get; init; }
}
