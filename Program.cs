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

// DbContext para datos de la aplicación
builder.Services.AddDbContext<InventarioContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DbContext para Identity (misma base de datos)
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Identity (relajada para desarrollo)
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

// Registrar servicio del SOAP
builder.Services.AddScoped<IServicioArticulos, ArticuloRepository>();

// Registrar SoapCore
builder.Services.AddSoapCore();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

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


// Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Rutas Identity
app.MapRazorPages();

app.Run();
