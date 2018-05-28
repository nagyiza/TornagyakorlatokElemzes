using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stream;
using System.IO;

namespace SkeletonCompare
{
    /// <summary>
    /// This class teach the algorithm, and compare the moves
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Path of the data from unity
        /// </summary>
        private static String unityPath = "..\\..\\..\\UnityData\\";
        /// <summary>
        /// Object
        /// </summary>
        private static Compare skeletonCompare;

        /// <summary>
        /// Compare the user and the reference skeletons
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            bool isNewReference = false;
            while (true)
            {
                Console.WriteLine("Enter the exercise name: ");
                //read exercise name
                string exerciseName = Console.ReadLine();
                Console.WriteLine("Choose: 1 - unity compare, 2 - new reference data ");
                string choose = Console.ReadLine();
                isNewReference = int.Parse(choose) == 2 ? true : false;

                //exercise name without number
                string exerciseNameRef = "";
                Teaching teaching;
                int isInt;
                //check the last character
                if (Int32.TryParse(exerciseName[exerciseName.Length - 1].ToString(), out isInt))
                {
                    //delete the number in the exercise name
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

                teaching = new Teaching(exerciseName, exerciseNameRef, "..\\..\\..\\ReferenceData\\", isNewReference);


                //if the exercise is exist, and compare with unity data
                if (!isNewReference)
                {
                    if (File.Exists(unityPath + "User\\" + exerciseName + ".txt")
                        && File.Exists(unityPath + "Reference\\" + exerciseNameRef + "AverageRef.txt"))
                    {
                        Console.WriteLine("Result: ");
                        //compare the user and the reference skeletons
                        skeletonCompare = new Compare(teaching, unityPath, "User\\" + exerciseName + ".txt", "Reference\\" + exerciseNameRef + "AverageRef.txt");
                        var time = DateTime.Now;
                        skeletonCompare.DTW();//the dtw algorithm
                        Console.WriteLine(Environment.NewLine + (DateTime.Now - time).ToString());

                    }
                    else
                    {
                        Console.WriteLine("The exercise name is not correct !!!");

                    }
                }
                else
                {
                    Console.WriteLine("The teaching with new reference data completed!");

                }
                Console.WriteLine(Environment.NewLine + "Press enter and try again or press esc and application close" + Environment.NewLine);
                //if press esc, the application close
                if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                {
                    Environment.Exit(0);
                }

            }

        }
    }

}
