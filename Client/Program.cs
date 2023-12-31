using Client.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.Configure<IdentitySettings>(builder.Configuration.GetSection("IdentitySettings"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
	.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
	options =>
	{
		options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
		options.SignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
		options.Authority = builder.Configuration["InteractiveServiceSettings:AuthorityUrl"];
		options.ClientId = builder.Configuration["InteractiveServiceSettings:ClientId"];
		options.ClientSecret = builder.Configuration["InteractiveServiceSettings:ClientSecret"];
		options.ResponseType = "code";
		options.SaveTokens = true;
		options.GetClaimsFromUserInfoEndpoint = true;
	});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
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
