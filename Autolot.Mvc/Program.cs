global using AutoLot.Dal.Initialization;
global using DAL.EfMainStructures;
global using Microsoft.EntityFrameworkCore;
using DAL.Repos;
using DAL.Repos.Interfaces;
using Services.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
// Add services to the container.
 var connectionString = builder.Configuration.GetConnectionString("Default");
 builder.Services.AddDbContextPool<ApplicationContext>(options => 
     options.UseNpgsql(connectionString,sqlOp =>
         sqlOp.EnableRetryOnFailure()));
 builder.Services.AddScoped<ICarRepo,CarRepo>(); 
 /*builder.Services.AddScoped<ICustomerRepo,CustomerRepo>();
 builder.Services.AddScoped<ICreditRiskRepo,CreditRiskRepo>();
 builder.Services.AddScoped<IMakeRepo,MakeRepo>();
 builder.Services.AddScoped<IOrderRepo,OrderRepo>();*/
 
 
 /*builder.Services.AddScoped(typeof(IAppLogging<>),typeof(AppLogging<>));
builder.Host.ConfigureSerilog();*/



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    if (app.Configuration.GetValue<bool>("RebuildDataBase"))
    {
        var context = new DatabaseContextFactory().CreateDbContext(new string[1]);
        SampleDataInitializer.InitializeData(context);
    }

    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();