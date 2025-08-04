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
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Cookie kimlik do�rulama �emas�n� varsay�lan olarak ayarlar
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme; // OpenID Connect kimlik do�rulama �emas�n� varsay�lan olarak ayarlar
}) // Authentication �emalar�n� yap�land�r�r
.AddCookie() // Cookie kimlik do�rulama �emas�n� ekler
.AddOpenIdConnect(options =>
{
    options.Authority = authority; // Keycloak sunucusunun URL'si (�rne�in, "https://keycloak.example.com/auth/realms/myrealm")
    options.ClientId = clientId; // Keycloak istemci kimli�i
    options.ClientSecret = clientSecret; // Keycloak istemci gizli anahtar�
    options.ResponseType = responseType; // "code" veya "id_token" gibi de�erler kullanabilirsiniz
    options.RequireHttpsMetadata = false; // Geli�tirme ortam�nda HTTPS gereksinimini devre d��� b�rakmak i�in
    options.SaveTokens = true; // Eri�im ve yenileme token'lar�n� oturumda saklamak i�in
    options.GetClaimsFromUserInfoEndpoint = true; // Kullan�c� bilgilerini almak i�in UserInfo endpoint'ini kullanmak i�in
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name", // Keycloak'ta genelde "preferred_username" veya "name"
        RoleClaimType = "roles" // Keycloak�ta genelde "roles" veya "realm_access.roles"
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
