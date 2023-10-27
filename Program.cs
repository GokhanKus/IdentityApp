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

	options.User.RequireUniqueEmail = true; // ayn� email ile farkl� username al�p kay�t olunabiliyordu, art�k olamaz.
	//options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";

	options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
	options.Lockout.MaxFailedAccessAttempts = 5;					//eger user giris icin 5 kere hatal� girisimde bulunursa hesab� 2 dk boyunca kitlensin.
});
builder.Services.ConfigureApplicationCookie(options =>//authorize, cookie ayarlar�n� buradan degistirebiliriz. 
{
	options.LoginPath = "/Account/Login"; //buras� zaten default olarak boyle gelir yazmasak da olur, ancak istersek degistirebiliriz.
	options.AccessDeniedPath = "/Account/AccessDenied";//kullan�c� kay�t olmustur, ancak rol� yoktur ve user erisme yetkisi olmayan k�sma erismek isterse erisemez ve bu durumda kullan�c�y� yonlendirmek istedi�imiz yer buras�d�r.
	options.SlidingExpiration = true;
	options.ExpireTimeSpan = TimeSpan.FromDays(30); //default olarak 14 gundur bu, bir cookie olusturuldugu zaman 14 gun gecerli bir cookie oluyor user giris yapt�ktan 14 gun sonra user hi� logout olmazsa bu cookinin s�resi biter. bu s�reyi degisterbiliriz 30 gun yapal�m.
													//user bu zaman aral�g�nda tekrar logout login islemini yaparsa sure resetlenir
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); //UseRouting() alt�na bu middleware'i tan�mlayal�m
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
