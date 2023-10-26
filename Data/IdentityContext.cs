using IdentityApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
    //normalde context.cs'imiz :DbContextten türetilirdi ama şuan DbContextten daha üst seviyede olan ve extra authentication özelliklerini
    //bünyesinde barındıran tabloları olan IdentityDbContextten türettik DbContext gibi aslında sadece extra bilgiler var user bilgileri
    public class IdentityContext : IdentityDbContext<AppUser, AppRole,string>
	{
		public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
		{
			//migration ile bu contexti oluştururken içinde dbSet tablolarımın (entitylerimiz) olmamasına rağmen migrationa bakacak olursak
			//icerisinde 7 adet tablo default olarak eklendiğini (users,roles vs.) görüyoruz bunu DbContext yerine IdentityDbContext yazdığımız icin oldu
			//add-migration InitialCreate -context IdentityContext yazdık sonra;
			//update-database -context IdentityContext yazdık (burada önemli olan bir adet context.cs'imiz varsa -context belirtmemize gerek yok ancak;
			//birden fazla context.cs varsa belirtmeliyiz.)
		}
	}
}
