using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OutCom.Components;
using OutCom.Components.Account;
using OutCom.Data;
using OutCom.Services;
using OutCom.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? 
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

// Registrar servicios personalizados
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IRoleManagementService, RoleManagementService>();
builder.Services.AddScoped<IDataSeederService, DataSeederService>();
builder.Services.AddScoped<IFileManagerService, FileManagerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

// Middleware personalizado para autorización administrativa
app.UseAdminAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Endpoint SEGURO para que el usuario autenticado descargue SUS PROPIOS archivos
app.MapGet("/download/{fileName}", async (HttpContext context, string fileName) =>
{
    if (!context.User.Identity.IsAuthenticated)
    {
        return Results.Unauthorized();
    }

    var userId = context.User.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value;
    var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();

    var filePath = Path.Combine(env.WebRootPath, "UserFiles", userId, fileName);

    if (!File.Exists(filePath))
    {
        return Results.NotFound("Archivo no encontrado.");
    }

    return Results.File(filePath, "application/octet-stream", fileName);
}).RequireAuthorization(); // Requiere que el usuario est� logueado

// Endpoint P�BLICO para acceder a archivos mediante un enlace compartido
app.MapGet("/share/{linkId}", async (Guid linkId, HttpContext context) =>
{
    var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
    var sharedLink = await dbContext.SharedLinks.FindAsync(linkId);

    if (sharedLink == null)
    {
        return Results.NotFound("Enlace no v�lido o expirado.");
    }

    // Aqu� puedes a�adir l�gica de expiraci�n si la implementaste
    // if (sharedLink.ExpirationDate.HasValue && sharedLink.ExpirationDate < DateTime.UtcNow) ...

    var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
    var filePath = Path.Combine(env.WebRootPath, "UserFiles", sharedLink.OwnerUserId, sharedLink.FileName);

    if (!File.Exists(filePath))
    {
        return Results.NotFound("El archivo asociado a este enlace ya no existe.");
    }

    // Usamos `inline: true` para que los navegadores intenten mostrarlo (PDFs, im�genes) en lugar de solo descargarlo
    return Results.File(filePath, "application/octet-stream", fileDownloadName: sharedLink.FileName);
});

// Inicializar datos por defecto
using (var scope = app.Services.CreateScope())
{
    var dataSeeder = scope.ServiceProvider.GetRequiredService<IDataSeederService>();
    await dataSeeder.SeedDefaultDataAsync();
}

app.Run();
