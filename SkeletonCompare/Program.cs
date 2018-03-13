using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream;
using System.IO;

namespace SkeletonCompare
{
    public class Program
    {
        /// <summary>
        /// Path of the data
        /// </summary>
        private static String path = "..\\..\\..\\UnityData\\";
        /// <summary>
        /// Object
        /// </summary>
        private static Compare skeletonCompare;

        /// <summary>
        /// Main
        /// Compare the user and the reference skeletons
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("Enter the exercise name: ");
                //read exercise name
                String exerciseName = Console.ReadLine();
                String exerciseNameRef = "";
                int isInt;
                if (Int32.TryParse(exerciseName[exerciseName.Length - 1].ToString(), out isInt))
                {
                    foreach (char c in exerciseName)
                    {
                        if (!Int32.TryParse(c.ToString(), out isInt))
                        {
                            exerciseNameRef = exerciseNameRef + c.ToString();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                else
                {
                    exerciseNameRef = exerciseName;
                }


                Teaching teaching = new Teaching(exerciseNameRef);
                teaching.TeachingSkeleton();

                //if the exercise is exist
                if (File.Exists(path + "User\\" + exerciseName + ".txt")
                    && File.Exists(path + "Reference\\" + exerciseNameRef + "Ref.txt"))
                {
                    Console.WriteLine("Result: ");
                    //compare the user and the reference skeletons
                    skeletonCompare = new Compare(teaching, path, "User\\" + exerciseName + ".txt", "Reference\\" + exerciseNameRef + "Ref.txt");
                    var time = DateTime.Now;
                    skeletonCompare.DTW();//the dtw algorithm
                    Console.WriteLine(Environment.NewLine + (DateTime.Now - time).ToString());
                    Console.WriteLine(Environment.NewLine + "Press enter and try again or press esc and application close"+ Environment.NewLine);
                    //if press esc, the application close
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
                else
                {
                    Console.WriteLine("The exercise name is not correct !!!");
                    Console.WriteLine(Environment.NewLine + "Press enter and try again or press esc and application close"+ Environment.NewLine);
                    //if press esc, the application close
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
            }

            //skeletonCompare = new Compare(path, "User\\skeleton.txt", "Reference\\skeletonRef.txt");
        }
    }

}
