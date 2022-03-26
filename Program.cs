using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using cSharpDemo.Data;
using dbChange.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddSignalR();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



ServiceProvider serviceProvider = builder.Services.BuildServiceProvider();

IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();

var extensionType = typeof(HubEndpointRouteBuilderExtensions);
var mapHubMethod = extensionType.GetMethod("MapHub", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static, new[] { typeof(IEndpointRouteBuilder), typeof(string) });
var myHubTypeName = config.GetSection("Hubs").GetValue(typeof(string), "MyHub").ToString();
// Assume it is something like this : 
// "MyHubNameSpace.MyHub, MyAssembly, Version=1.2.3.0, Culture=neutral, PublicKeyToken=null"
var myHubType = Type.GetType(myHubTypeName!); // read from config
var path = "foo"; // read from config
var genericMapHub = mapHubMethod!.MakeGenericMethod(myHubType!);


app.UseEndpoints(endpoints =>
{
    genericMapHub.Invoke(null, new object[] { endpoints, path });
});
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
