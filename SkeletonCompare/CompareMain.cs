using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeletonCompare
{
    public class CompareMain
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
        /// Offline result
        /// </summary>
        public static string Result;

        public CompareMain()
        {

        }
        public static void Compare(string exerciseName, bool isNewReference)
        {
            String exerciseNameRef = "";
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

            if (!isNewReference) {
                //if the exercise is exist
                if (File.Exists(unityPath + "User\\" + exerciseName + ".txt")
                    && File.Exists(unityPath + "Reference\\" + exerciseNameRef + "AverageRef.txt"))
                {
                    //compare the user and the reference skeletons
                    skeletonCompare = new Compare(teaching, unityPath, "User\\" + exerciseName + ".txt", "Reference\\" + exerciseNameRef + "AverageRef.txt");
                    skeletonCompare.DTW();//the dtw algorithm
                    Result = skeletonCompare.anglePercent.ToString();//or scatterPercent
                }
            }          
        
        }

    }
}
