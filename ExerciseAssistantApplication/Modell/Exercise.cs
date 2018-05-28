using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExerciseAssistantApplication.Modell
{
    public class Exercise
    {
        [Key]
        public int UserId { get; set; }
        public string Username { get; set; }
        public string ExerciseName { get; set; }
        public string Date { get; set; }
        public string ResultJoint { get; set; }
        public string ResultAngle { get; set; }
    }
}
