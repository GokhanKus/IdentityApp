using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp.Data
{
    //normalde context.cs'imiz :DbContextten türetilirdi ama şuan DbContextten daha üst seviyede olan ve extra authentication özelliklerini
    //bünyesinde barındıran tabloları olan IdentityDbContextten türettikc
	public class IdentityContext: IdentityDbContext<IdentityUser>
	{
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
                
        }
    }
}
