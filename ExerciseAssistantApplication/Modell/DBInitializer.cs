using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace ExerciseAssistantApplication.Modell
{
    public class DBInitializer : DropCreateDatabaseAlways<MyDbContext> //CreateDatabaseIfNotExists<TMCatalogDB>
    //public class DBInitializer : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            context.Users.Add(new User { UserId = 0, Username = "iza", Password = "123", Email = "iza", IsAdmin = true, IsActive = true});
            context.Users.Add(new User { UserId = 1, Username = "vivien", Password = "vivien", Email = "vivi", IsAdmin = false, IsActive = true});
        }
    }
}
