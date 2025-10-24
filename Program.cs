using inventario_ferreteria.Models;
using inventario_ferreteria.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// DbContext para datos de la aplicaci�n (ya existente)
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

// A�adir soporte para controllers con vistas (MVC) y Razor Pages (Identity usa Razor Pages)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
 app.UseExceptionHandler("/Error");
 app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// A�ade autenticaci�n antes de autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Mapear rutas para controllers (MVC)
app.MapControllerRoute(
 name: "default",
 pattern: "{controller=Home}/{action=Index}/{id?}");

// Mapear Razor Pages (Identity UI, etc.)
app.MapRazorPages();
app.Run();