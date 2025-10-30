using inventario_ferreteria.Models;
using inventario_ferreteria.Data;
using inventario_ferreteria.Services.Implementacion;
using inventario_ferreteria.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SoapCore;
using System.ServiceModel;

var builder = WebApplication.CreateBuilder(args);

// Escuchar en todas las IP en el puerto 7208 (HTTP)
builder.WebHost.UseUrls("http://0.0.0.0:7208");

// DbContext para datos de la aplicación
builder.Services.AddDbContext<InventarioContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DbContext para Identity
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 3;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationIdentityDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// MVC y Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Registrar servicio SOAP
builder.Services.AddLogging();
builder.Services.AddScoped<IServicioArticulos, ArticuloRepository>();
builder.Services.AddSoapCore();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Registrar endpoint SOAP
app.UseEndpoints(endpoints =>
{
    endpoints.UseSoapEndpoint<IServicioArticulos>(
        "/Service.svc",
        new SoapEncoderOptions(),
        SoapSerializer.XmlSerializer
    );

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}"
    );

    endpoints.MapRazorPages();
});

app.Run();
