using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;


namespace ExerciseAssistantApplication.Modell
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() //: base("name=ExerciseAssistantDB") // ha nevvel adod akkor kell a connstring az appconfigba
        { }
        public DbSet<User> Users { get; set; }

    }
}
