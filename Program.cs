using IdentityApp.Data;
using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//var connectionString = builder.Configuration.GetConnectionString("SqLite_Connection");
//builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlite(connectionString));


builder.Services.AddDbContext<IdentityContext>(
	options => options.UseSqlite(builder.Configuration["ConnectionStrings:SqLite_Connection"]));
builder.Services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<IdentityContext>();


builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireLowercase = false;
	options.Password.RequireUppercase = false;
	options.Password.RequiredLength = 6;

	options.User.RequireUniqueEmail = true; // ayný email ile farklý username alýp kayýt olunabiliyordu, artýk olamaz.
	//options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
	options.Lockout.MaxFailedAccessAttempts = 5;					//eger user giris icin 5 kere hatalý girisimde bulunursa hesabý 2 dk boyunca kitlensin.
});
builder.Services.ConfigureApplicationCookie(options =>//authorize, cookie ayarlarýný buradan degistirebiliriz. 
{
	options.LoginPath = "/Account/Login"; //burasý zaten default olarak boyle gelir yazmasak da olur, ancak istersek degistirebiliriz.
	options.AccessDeniedPath = "/Account/AccessDenied";//kullanýcý kayýt olmustur, ancak rolü yoktur ve user erisme yetkisi olmayan kýsma erismek isterse erisemez ve bu durumda kullanýcýyý yonlendirmek istediðimiz yer burasýdýr.
	options.SlidingExpiration = true;
	options.ExpireTimeSpan = TimeSpan.FromDays(30); //default olarak 14 gundur bu, bir cookie olusturuldugu zaman 14 gun gecerli bir cookie oluyor user giris yaptýktan 14 gun sonra user hiç logout olmazsa bu cookinin süresi biter. bu süreyi degisterbiliriz 30 gun yapalým.
													//user bu zaman aralýgýnda tekrar logout login islemini yaparsa sure resetlenir
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //UseRouting() altýna bu middleware'i tanýmlayalým
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
else //app.Environment.IsDevelopment()
{
	app.UseDeveloperExceptionPage();
	IdentityDataSeeding.IdentityTestUser(app);
}

app.Run();
