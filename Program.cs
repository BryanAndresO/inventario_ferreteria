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

// DbContext para datos de la aplicación (ya existente)
builder.Services.AddDbContext<InventarioContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// DbContext para Identity (misma base de datos)
builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Identity (relajado para desarrollo)
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
 {
 options.Password.RequireNonAlphanumeric = false;
 options.Password.RequireUppercase = false;
 options.Password.RequireLowercase = false;
 options.Password.RequireDigit = false;
 options.Password.RequiredLength =3;
 options.SignIn.RequireConfirmedAccount = false;
 })
 .AddRoles<IdentityRole>()
 .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
 .AddDefaultTokenProviders()
 .AddDefaultUI();

// Añadir soporte para controllers con vistas (MVC) y Razor Pages (Identity usa Razor Pages)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Registrar servicios de la capa Services
builder.Services.AddScoped<IServicioArticulos, ServicioArticulos>();

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

// Añade autenticación antes de autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear rutas para controllers (MVC)
app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Mapear Razor Pages (Identity UI, etc.)
app.MapRazorPages();

// Exponer endpoint SOAP en /Service.svc usando la sobrecarga para IApplicationBuilder
((IApplicationBuilder)app).UseSoapEndpoint<IServicioArticulos>("/Service.svc", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);

app.Run();