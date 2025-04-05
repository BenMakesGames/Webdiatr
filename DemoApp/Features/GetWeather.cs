﻿using BenMakesGames.Webdiatr.Core;

namespace DemoApp.Features;

public sealed class GetWeather: IRequestHandler<GetWeatherRequest, string>
{
    public Task<string> Handle(GetWeatherRequest request, CancellationToken cancellationToken)
    {
        var tempC = Random.Shared.NextSingle() * 40 + -10;

        return Task.FromResult(tempC.ToString("F1"));
        /*return Task.FromResult(new GetWeatherResponse()
        {
            City = request.City,
            TemperatureC = tempC,
            TemperatureF = tempC * 9 / 5 + 32
        });*/
    }
}

public sealed class GetWeatherRequest
{
    public required string City { get; init; }
}

public sealed class GetWeatherResponse
{
    public required string City { get; init; }
    public required float TemperatureC { get; init; }
    public required float TemperatureF { get; init; }
}
