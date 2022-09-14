using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DatabaseGenerator.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext db;

        public IndexModel(AppDbContext _db)
        {
            db = _db;
        }

        public void OnGet()
        {
            var user = new MasCom.Lib.User()
            {
                LastName = "Eric",
                Name = "Hotegni",
                UserName = "Baloch",
                PasswordHash = "7efz7efez"
            };


            db.Users.Add(user);

            db.SaveChanges();

        }
    }
}
