using AutoLot.Dal.Initialization;
using DAL.EfMainStructures;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using DAL.Repos;
using DAL.Repos.Interfaces;
using Serilog;
using Services.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContextPool<ApplicationContext>(options => 
    options.UseNpgsql(connectionString,sqlOp =>
        sqlOp.EnableRetryOnFailure()));
builder.Services.AddScoped<ICarRepo,CarRepo>();
builder.Services.AddScoped<ICustomerRepo,CustomerRepo>();
builder.Services.AddScoped<ICreditRiskRepo,CreditRiskRepo>();
builder.Services.AddScoped<IMakeRepo,MakeRepo>();
builder.Services.AddScoped<IOrderRepo,OrderRepo>();
builder.Host.ConfigureSerilog();
builder.Services.AddScoped(typeof(IAppLogging<>),typeof(AppLogging<>));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    if (app.Configuration.GetValue<bool>("RebuildDataBase"))
    {
        var context = new DatabaseContextFactory().CreateDbContext(new string[1]);
        SampleDataInitializer.InitializeData(context);
    }

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
