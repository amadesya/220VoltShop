using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using courseProd.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
// Use local API during development
builder.Services.AddScoped<ApiClient>(_ => new ApiClient("http://localhost:5000/"));
// Cart service (uses JS localStorage via IJSRuntime)
builder.Services.AddScoped<CartService>();
// Toasts
builder.Services.AddSingleton<ToastService>();

var app = builder.Build();

// serve static files from wwwroot at both "/" and "/wwwroot" so requests to /wwwroot/... work
app.UseStaticFiles(); // existing root mapping

app.UseStaticFiles(new StaticFileOptions
{
	RequestPath = "/images",
	FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
		Path.Combine(Directory.GetCurrentDirectory(), "Images"))
});

app.UseStaticFiles(new StaticFileOptions
{
	// map requests starting with /wwwroot to the physical wwwroot folder
	RequestPath = "/wwwroot",
	FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
		Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"))
});

app.UseRouting();

// ensure Blazor hub and fallback are mapped
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
