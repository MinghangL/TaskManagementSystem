using GroupProject1025.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GroupProject1025.Startup))]
namespace GroupProject1025
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            createRolesandUsers();
        }

        // In this method we will create default User roles and Admin user for login    
        private void createRolesandUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            // In Startup iam creating first Admin Role and creating a default Admin User     
            if (!roleManager.RoleExists("Admin"))
            {

                // first we create Admin rool role    
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website   
                var user = new ApplicationUser();
                user.UserName = "admin@test.com";
                user.Email = "admin@test.com";
                string userPWD = "Ab.123123";
                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    // add to UserRoles
                    var result1 = UserManager.AddToRole(user.Id, "Admin");
                }

            }

            // create additional users
            var nick = new ApplicationUser();
            nick.UserName = "Nick@test.com";
            nick.Email = "Nick@test.com";
            var nickPWD = "Ab.123123";
            var createNick = UserManager.Create(nick, nickPWD);

            var minghang = new ApplicationUser();
            minghang.UserName = "Minghang@test.com";
            minghang.Email = "Minghang@test.com";
            var minghangPWD = "Ab.123123";
            var createM = UserManager.Create(minghang, minghangPWD);

            var eric = new ApplicationUser();
            eric.UserName = "Eric@test.com";
            eric.Email = "Eric@test.com";
            var ericPWD = "Ab.123123";
            var createE = UserManager.Create(eric, ericPWD);

            var testUser = new ApplicationUser();
            testUser.UserName = "testUser@test.com";
            testUser.Email = "testUser@test.com";
            var testUserPWD = "Ab.123123";
            var createT = UserManager.Create(testUser, testUserPWD);

            testUser = new ApplicationUser();
            testUser.UserName = "Mike@test.com";
            testUser.Email = "Mike@test.com";
            testUserPWD = "Ab.123123";
            createT = UserManager.Create(testUser, testUserPWD);

            testUser = new ApplicationUser();
            testUser.UserName = "Tom@test.com";
            testUser.Email = "Tom@test.com";
            testUserPWD = "Ab.123123";
            createT = UserManager.Create(testUser, testUserPWD);

            testUser = new ApplicationUser();
            testUser.UserName = "Jack@test.com";
            testUser.Email = "Jack@test.com";
            testUserPWD = "Ab.123123";
            createT = UserManager.Create(testUser, testUserPWD);

            // creating Creating Manager role     
            if (!roleManager.RoleExists("PM"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "PM";
                roleManager.Create(role);
            }

            // creating Creating Employee role     
            if (!roleManager.RoleExists("SD"))
            {
                var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                role.Name = "SD";
                roleManager.Create(role);

            }



        }

    }
}
