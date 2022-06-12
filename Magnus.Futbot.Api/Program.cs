using AutoMapper;
using Magnus.Futbot.Api.Helpers;
using Magnus.Futbot.Api.Services;
using Magnus.Futbot.Api.Services.Connections;
using Magnus.Futbot.Api.Services.Selenium;
using Magnus.Futbot.Common;
using Magnus.Futbot.Database.Repositories;

var builder = WebApplication.CreateBuilder(args);


var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MapperProfile());
});

IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddScoped<AppSettings>()
    .AddTransient<ProfilesService>()
    .AddTransient<ProfilesRepository>()
    .AddScoped<LoginSeleniumService>();

builder.Services
    .AddHttpClient<SsoConnectionService>();

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseCors("corsapp");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
