using BenMakesGames.Webdiatr;
using DemoApp.Features;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebdiatr();

var app = builder.Build();

app.MapGet("/", () => "Hi! 👋");

app.MapGetToQuery<GetWeatherRequest, GetWeatherResponse>("/weather");

app.Run();
