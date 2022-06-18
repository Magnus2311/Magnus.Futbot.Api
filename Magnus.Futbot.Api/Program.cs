using AutoMapper;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Hubs;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Api.Services.Selenium;
using Magnus.Futbot.Database.Repositories;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);


var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services
    .AddSwaggerGen();
builder.Services
    .AddTransient<ProfilesService>()
    .AddTransient<ProfilesRepository>()
    .AddScoped<LoginSeleniumService>()
    .AddSingleton<Initializer>();

builder.Services
    .AddHttpClient<SsoConnectionService>();

builder.Services.AddSingleton<IUserIdProvider, UserProvider>();

builder.Services.AddSignalR();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ProfilesHub>("/hubs/profiles");
    endpoints.MapControllers();
});

var initializer = app.Services.GetService<Initializer>()!;
initializer.InitSeleniumProfiles();

app.Run();
