using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// 
var keycloakConfig = builder.Configuration.GetSection("Keycloak");
var authority = keycloakConfig["Authority"];
var clientId = keycloakConfig["ClientId"];
var clientSecret = keycloakConfig["ClientSecret"];
var responseType = keycloakConfig["ResponseType"] ?? "code";

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Cookie kimlik doðrulama þemasýný varsayýlan olarak ayarlar
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // OpenID Connect kimlik doðrulama þemasýný varsayýlan olarak ayarlar
}) // Authentication þemalarýný yapýlandýrýr
.AddCookie() // Cookie kimlik doðrulama þemasýný ekler
.AddOpenIdConnect(options =>
{
    options.Authority = authority; // Keycloak sunucusunun URL'si (örneðin, "https://keycloak.example.com/auth/realms/myrealm")
    options.ClientId = clientId; // Keycloak istemci kimliði
    options.ClientSecret = clientSecret; // Keycloak istemci gizli anahtarý
    options.ResponseType = responseType; // "code" veya "id_token" gibi deðerler kullanabilirsiniz
    options.RequireHttpsMetadata = false; // Geliþtirme ortamýnda HTTPS gereksinimini devre dýþý býrakmak için
    options.SaveTokens = true; // Eriþim ve yenileme token'larýný oturumda saklamak için
    options.GetClaimsFromUserInfoEndpoint = true; // Kullanýcý bilgilerini almak için UserInfo endpoint'ini kullanmak için
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name", // Keycloak'ta genelde "preferred_username" veya "name"
        RoleClaimType = "roles" // Keycloak’ta genelde "roles" veya "realm_access.roles"
    };
});

builder.Services.AddAuthorization(); // Authorization servisini ekle


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
