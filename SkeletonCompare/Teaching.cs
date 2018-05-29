using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SkeletonCompare
{
    public class Teaching
    {
        /// <summary>
        /// Path in witch are the reference videos and skeletons
        /// </summary>
<<<<<<< HEAD
        private string referencePath;
=======
        private string path = "..\\..\\..\\ReferenceData\\";
>>>>>>> parent of d3daa5c... comment + database
        /// <summary>
        /// Name of exercise (the first)
        /// </summary>
        private string exerciseName;
        /// <summary>
        /// Name of input
        /// </summary>
        private string input;
        /// <summary>
        /// Name of reference exercise
        /// </summary>
        private List<string> exerciseNamesRef;
        /// <summary>
        /// List of Skeletons
        /// </summary>
        private List<Skeleton> Skeletons;
        /// <summary>
        /// List of the data, witch read in the files
        /// </summary>
        private List<List<Skeleton>> filesData;
        /// <summary>
        /// List of the avarage
        /// </summary>
        private List<Skeleton> average;
        /// <summary>
        /// List of the scatter
        /// </summary>
        private List<Skeleton> scatter;

<<<<<<< HEAD
        private int newRefExerciseIndex;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exerciseNameRef"> reference file name (without number)</param>
        /// <param name="path">where is the file</param>
        /// <param name="isNew">True - is new reference data, False - is a first reference data</param>
        public Teaching(string input, string exerciseNameRef, string path, bool isNew)
        {
            this.input = input;
            this.referencePath = path;
=======
        public Teaching(string exerciseNameRef)
        {
>>>>>>> parent of d3daa5c... comment + database
            Skeletons = new List<Skeleton>();
            average = new List<Skeleton>();
            scatter = new List<Skeleton>();

            exerciseName = exerciseNameRef;
            //-----------
            //if (!isNew)
            //{
            //if the input is new reference data or if not exist the files
            if (!File.Exists(referencePath + exerciseName + "Average.txt")
                || !File.Exists(referencePath + exerciseName + "AngleAverage.txt")
                || !File.Exists(referencePath + exerciseName + "Scatter.txt")
                || !File.Exists(referencePath + exerciseName + "AngleScatter.txt")
                || isNew)
            {
                filesData = new List<List<Skeleton>>();
                exerciseNamesRef = new List<string>();
                if (File.Exists(referencePath + exerciseName + ".txt"))
                {
                    exerciseNamesRef.Add(exerciseName + ".txt");
                    filesData.Add(Skeleton.ProcessSkeletonFromFile(referencePath + exerciseName + ".txt"));
                    //CalculateSkeletonAngles(filesData[filesData.Count - 1]);

                    for (int i = 1; ; ++i)
                    {
                        if (File.Exists(referencePath + exerciseName + i + ".txt"))
                        {
                            exerciseNamesRef.Add(exerciseName + i + ".txt");
                            filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseName + i + ".txt"));
                            //CalculateSkeletonAngles(filesData[filesData.Count - 1]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            
            TeachingSkeleton(exerciseNameRef, isNew);
        }

        /// <summary>
        /// Calculate the averages, the scatter and teach what joints are important
        /// </summary>
        public void TeachingSkeleton(string name, bool isNew)
        {
            //calculate average and the scatter
            CalculateScatter(isNew);

            if (filesData != null && filesData.Count != 0)
            {
                if (isNew)
                {
                    if (File.Exists(referencePath + name + "Average.txt")
                     || File.Exists(referencePath + name + "AngleAverage.txt")
                     || File.Exists(referencePath + name + "Scatter.txt")
                     || File.Exists(referencePath + name + "AngleScatter.txt"))
                    {
                        File.Delete(referencePath + name + "Average.txt");
                        File.Delete(referencePath + name + "AngleAverage.txt");
                        File.Delete(referencePath + name + "Scatter.txt");
                        File.Delete(referencePath + name + "AngleScatter.txt");

                    }
                }
                Skeleton.SkeletonPrint(average, referencePath + name + "Average.txt");
                Skeleton.SkeletonAnglePrint(average, referencePath + name + "AngleAverage.txt");

                for (int i = 0; i < scatter.Count; ++i)
                {
                    for (int j = 0; j < scatter[i].Joints.Count; ++j)
                    {
                        int counter = 0;
                        for (int k = 0; k < filesData.Count; ++k)
                        {
                            if (i < filesData[k].Count)
                            {
                                if (filesData[k][i].Joints[j].X <= average[i].Joints[j].X + scatter[i].Joints[j].X
                                    && filesData[k][i].Joints[j].X >= average[i].Joints[j].X - scatter[i].Joints[j].X
                                    && filesData[k][i].Joints[j].Y <= average[i].Joints[j].Y + scatter[i].Joints[j].Y
                                    && filesData[k][i].Joints[j].Y >= average[i].Joints[j].Y - scatter[i].Joints[j].Y
                                    && filesData[k][i].Joints[j].Z <= average[i].Joints[j].Z + scatter[i].Joints[j].Z
                                    && filesData[k][i].Joints[j].Z >= average[i].Joints[j].Z - scatter[i].Joints[j].Z
                                    )
                                {
                                    counter++;
                                }
                            }
                        }
                        scatter[i].ImportanceInPercent.Add(counter / (double)filesData.Count() * 100);// in %

                    }
                    //angles
                    for (int j = 0; j < scatter[i].AngleList.Count; ++j)
                    {
                        int counter = 0;
                        for (int k = 0; k < filesData.Count; ++k)
                        {
                            if (i < filesData[k].Count)
                            {
                                if (filesData[k][i].AngleList[j].Item1 == average[i].AngleList[j].Item1
                                    && filesData[k][i].AngleList[j].Item2 == average[i].AngleList[j].Item2
                                    && filesData[k][i].AngleList[j].Item3 <= average[i].AngleList[j].Item3 + scatter[i].AngleList[j].Item3
                                    && filesData[k][i].AngleList[j].Item3 >= average[i].AngleList[j].Item3 - scatter[i].AngleList[j].Item3
                                    )
                                {
                                    counter++;
                                }
                            }
                        }

                        scatter[i].ImportanceAngleInPercent.Add(counter / (double)filesData.Count() * 100);// in %

                    }
                }

                Skeleton.ScatterPrint(scatter, referencePath + name + "Scatter.txt");
                Skeleton.ScatterAnglePrint(scatter, referencePath + name + "AngleScatter.txt");
            }

        }
<<<<<<< HEAD
        /// <summary>
        /// Calculate the scatter of reference moves
        /// </summary>
        private void CalculateScatter(bool isNew)
=======

        private void CalculateScatter()
>>>>>>> parent of d3daa5c... comment + database
        {
            if (filesData == null
                || isNew)
            {
                //read from files
                if (File.Exists(referencePath + exerciseName + "Average.txt")
                     || File.Exists(referencePath + exerciseName + "AngleAverage.txt")
                     || File.Exists(referencePath + exerciseName + "Scatter.txt")
                     || File.Exists(referencePath + exerciseName + "AngleScatter.txt"))
                {
                    //read data in file (joint's averages and scatters)
                    average = Skeleton.ProcessSkeletonFromFile(referencePath + exerciseName + "Average.txt");
                    scatter = Skeleton.ProcessSkeletonFromFile(referencePath + exerciseName + "Scatter.txt");
                    // in this list are the joint, the angle and the teaching percent
                    List<Tuple<JointType, JointType, double, double>> angleListAverage = Skeleton.ProcessSkeletonAngelsFromFile(referencePath + exerciseName + "AngleAverage.txt");
                    List<Tuple<JointType, JointType, double, double>> angleListScatter = Skeleton.ProcessSkeletonAngelsFromFile(referencePath + exerciseName + "AngleScatter.txt");

                    for (int i = 0; i < average.Count; ++i)
                    {
                        List<Tuple<JointType, JointType, double>> angleList = new List<Tuple<JointType, JointType, double>>();
                        List<Tuple<JointType, JointType, double>> scatterList = new List<Tuple<JointType, JointType, double>>();
                        List<double> percentAngle = new List<double>();
                        for (int j = 0; j < 23; ++j)
                        {
                            angleList.Add(new Tuple<JointType, JointType, double>(angleListAverage[j + i * 23].Item1, angleListAverage[j + i * 23].Item2, angleListAverage[j + i * 23].Item3));
                            scatterList.Add(new Tuple<JointType, JointType, double>(angleListScatter[j + i * 23].Item1, angleListScatter[j + i * 23].Item2, angleListScatter[j + i * 23].Item3));
                            percentAngle.Add(angleListScatter[j + i * 23].Item4);
                        }
                        average[i].AngleList = angleList;
                        scatter[i].AngleList = scatterList;
                        scatter[i].ImportanceAngleInPercent = percentAngle;
                    }
                }
                else
                {
                    if (filesData != null)
                    {
                        for (int i = 0; i < filesData.Count; ++i)
                        {
                            CalculateSkeletonAngles(filesData[i]);
                        }
                        if (filesData.Count == 1)
                        {
                            for (int i = 0; i < filesData[0].Count; ++i)
                            {
                                average.Add(filesData[0][i]);
                            }
                            for (int i = 0; i < average.Count; ++i)
                            {
                                scatter.Add(SkeletonCompare.CalculateScatter.scatterSkeletons(average, average[i]));
                            }
                        }
                        else
                        {
                            //false - calculate first average and first scatter
                            Average(false);
                            Scatter(false);
                        }
                    }
                    return;
                }
            }
<<<<<<< HEAD
=======
        }

        public void Average()
        {
            List<Skeleton> sum = new List<Skeleton>();
>>>>>>> parent of d3daa5c... comment + database

            if (!isNew)
            {
                if (filesData != null)
                {
                    for (int i = 0; i < filesData.Count; ++i)
                    {
                        CalculateSkeletonAngles(filesData[i]);
                    }
                    if (filesData.Count == 1)
                    {
                        for (int i = 0; i < filesData[0].Count; ++i)
                        {
                            average.Add(filesData[0][i]);
                        }
                        for (int i = 0; i < average.Count; ++i)
                        {
                            scatter.Add(SkeletonCompare.CalculateScatter.scatterSkeletons(average, average[i]));
                        }
                    }
                    else
                    {
                        //false - calculate first average and first scatter
                        Average(false);
                        Scatter(false);
                    }
                }
            }
<<<<<<< HEAD
            else
=======
        }

        private Skeleton AverageSkeletons(List<Skeleton> skeletons)
        {
            Skeleton average = new Skeleton();
            int[] div = new int[25];
            int[] div2 = new int[25];
            average.Joints = skeletons[0].Joints;
            average.AngleList = new List<Tuple<JointType, JointType, double>>();
            double[] anglesSum = new double[25];
            for (int i = 1; i < skeletons.Count; ++i)
>>>>>>> parent of d3daa5c... comment + database
            {
                for (int i = 0; i < filesData.Count; ++i)
                {
                    CalculateSkeletonAngles(filesData[i]);
                }
                // UJATLAG
                string ind = input.Substring(exerciseName.Length);
                if (!Int32.TryParse(ind, out newRefExerciseIndex))
                {
                    newRefExerciseIndex = 0;
                }

                //regi, at kell alakitani ugy h ujat is tudjon szamolni
                //true - calculate new average and new scatter with old average and old scatter
                Average(true);
                Scatter(true);
            }

        }


        /// <summary>
        /// Calculate the average
        /// </summary>
        public void Average(bool isNew)
        {
            List<Skeleton> sum = new List<Skeleton>();

            int counter = 0;
            if (!isNew)
            {
                for (int j = 0; ; j++)
                {
                    for (int i = 0; i < filesData.Count; ++i)
                    {
                        if (filesData[i].Count > j)
                        {
                            sum.Add(filesData[i][j]);
                        }
                        else
                        {
                            counter++;
                        }
                    }
                    //if this is first teaching

                    average.Add(SkeletonCompare.CalculateScatter.AverageSkeletons(sum));

                    sum.Clear();

                    if (counter == filesData.Count)
                    {
                        break;
                    }
                }
            }
            else
            {
                average = SkeletonCompare.CalculateScatter.NewAverageSkeletons(filesData[newRefExerciseIndex], average);
            }
        }


<<<<<<< HEAD
        /// <summary>
        /// Scatter
        /// </summary>
        public void Scatter(bool isNew)
=======
        }

        private Skeleton scatterSkeletons(List<Skeleton> skeletons, Skeleton average)
>>>>>>> parent of d3daa5c... comment + database
        {
            List<Skeleton> sum = new List<Skeleton>();
            List<Skeleton> averages = new List<Skeleton>();
            int counter = 0;

            if (!isNew)
            {
                for (int j = 0; ; j++)
                {
                    for (int i = 0; i < filesData.Count; ++i)
                    {
                        if (filesData[i].Count > j)
                        {
                            sum.Add(filesData[i][j]);

                        }
                        else
                        {
                            counter++;
                        }
                    }

                    scatter.Add(SkeletonCompare.CalculateScatter.scatterSkeletons(sum, average[j]));
                    sum.Clear();
                    averages.Clear();

                    if (counter == filesData.Count)
                    {
                        break;
                    }

                }
            }
            else
            {
                scatter = SkeletonCompare.CalculateScatter.NewScatterSkeletons(filesData[newRefExerciseIndex], average, scatter);
            }


        }

        /// <summary>
        /// Calculate the angles beetween skeleton's bones
        /// </summary>
        /// <param name="skeleton"> List of skeletons</param>
        public void CalculateSkeletonAngles(List<Skeleton> skeleton)
        {
            if (skeleton.Count > 0)
            {
                foreach (var skl in skeleton)
                {
                    skl.SkeletonAngle();
                }
            }
        }

        public List<Skeleton> GetAverages
        {
            get
            {
                return average;
            }

        }
        public List<Skeleton> GetScatters
        {
            get
            {
                return scatter;
            }

        }


        private string GetName(string exerciseName)
        {
            string name = "";
            int isInt;
            //check the last character
            if (Int32.TryParse(exerciseName[exerciseName.Length - 1].ToString(), out isInt))
            {
                //delete the number in the exercise name
                foreach (char c in exerciseName)
                {
                    if (!Int32.TryParse(c.ToString(), out isInt))
                    {
                        name = name + c.ToString();
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else
            {
                name = exerciseName;
            }

            return name;
        }

    }
}
