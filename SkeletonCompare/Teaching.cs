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
    /// <summary>
    /// Calculate the average and scatter of reference move, and teaching the joints and angles
    /// </summary>
    public class Teaching
    {
        /// <summary>
        /// Path in witch are the reference videos and skeletons
        /// </summary>
        private string path;
        /// <summary>
        /// Name of exercise (the first)
        /// </summary>
        private string exerciseName;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exerciseNameRef"> reference file name</param>
        /// <param name="path">where is the file</param>
        /// <param name="isNew">True - is new reference data, False - is a first reference data</param>
        public Teaching(string exerciseNameRef, string path, bool isNew)
        {
            this.path = path;
            Skeletons = new List<Skeleton>();
            average = new List<Skeleton>();
            scatter = new List<Skeleton>();

            exerciseName = GetName(exerciseNameRef);
            if (!isNew)
            {
                if (!File.Exists(path + exerciseName + "Average.txt")
                    || !File.Exists(path + exerciseName + "AngleAverage.txt")
                    || !File.Exists(path + exerciseName + "Scatter.txt")
                    || !File.Exists(path + exerciseName + "AngleScatter.txt"))
                {
                    filesData = new List<List<Skeleton>>();
                    exerciseNamesRef = new List<string>();
                    if (File.Exists(path + exerciseName + ".txt"))
                    {
                        exerciseNamesRef.Add(exerciseName + ".txt");
                        filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseName + ".txt"));
                        //CalculateSkeletonAngles(filesData[filesData.Count - 1]);

                        for (int i = 1; ; ++i)
                        {
                            if (File.Exists(path + exerciseName + i + ".txt"))
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
                    TeachingSkeleton(exerciseNamesRef[0], isNew);
                }
                else
                {
                    TeachingSkeleton(exerciseNameRef, isNew);
                }
            }
            else
            {
                filesData = new List<List<Skeleton>>();
                exerciseNamesRef = new List<string>();
                if (File.Exists(path + exerciseName + ".txt"))
                {
                    exerciseNamesRef.Add(exerciseName + ".txt");
                    filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseName + ".txt"));
                    //CalculateSkeletonAngles(filesData[filesData.Count - 1]);

                    for (int i = 1; ; ++i)
                    {
                        if (File.Exists(path + exerciseName + i + ".txt"))
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
                TeachingSkeleton(exerciseNamesRef[0], isNew);
            }
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
                string[] names = name.Split('.');
                names[0] = GetName(names[0]);
                if (isNew)
                {
                    if (File.Exists(path + names[0] + "Average.txt")
                     || File.Exists(path + names[0] + "AngleAverage.txt")
                     || File.Exists(path + names[0] + "Scatter.txt")
                     || File.Exists(path + names[0] + "AngleScatter.txt"))
                    {
                        File.Delete(path + names[0] + "Average.txt");
                        File.Delete(path + names[0] + "AngleAverage.txt");
                        File.Delete(path + names[0] + "Scatter.txt");
                        File.Delete(path + names[0] + "AngleScatter.txt");

                    }
                }
                Skeleton.SkeletonPrint(average, path + names[0] + "Average.txt");
                Skeleton.SkeletonAnglePrint(average, path + names[0] + "AngleAverage.txt");

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

                Skeleton.ScatterPrint(scatter, path + names[0] + "Scatter.txt");
                Skeleton.ScatterAnglePrint(scatter, path + names[0] + "AngleScatter.txt");
            }

        }
        /// <summary>
        /// Calculate the scatter of reference moves
        /// </summary>
        private void CalculateScatter(bool isNew)
        {
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
                            scatter.Add(scatterSkeletons(average, average[i]));
                        }
                    }
                    else
                    {
                        //false - calculate first average and first scatter
                        Average(false);
                        Scatter(false);
                    }
                }
                else
                {
                    string name = GetName(exerciseName);
                    //read data in file (joint's averages and scatters)
                    average = Skeleton.ProcessSkeletonFromFile(path + name + "Average.txt");
                    scatter = Skeleton.ProcessSkeletonFromFile(path + name + "Scatter.txt");
                    // in this list are the joint, the angle and the teaching percent
                    List<Tuple<JointType, JointType, double, double>> angleListAverage = Skeleton.ProcessSkeletonAngelsFromFile(path + name + "AngleAverage.txt");
                    List<Tuple<JointType, JointType, double, double>> angleListScatter = Skeleton.ProcessSkeletonAngelsFromFile(path + name + "AngleScatter.txt");

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
            }
            else
            {
                string name = GetName(exerciseName);
                //read data in file (joint's averages and scatters)
                average = Skeleton.ProcessSkeletonFromFile(path + name + "Average.txt");
                scatter = Skeleton.ProcessSkeletonFromFile(path + name + "Scatter.txt");
                // in this list are the joint, the angle and the teaching percent
                List<Tuple<JointType, JointType, double, double>> angleListAverage = Skeleton.ProcessSkeletonAngelsFromFile(path + name + "AngleAverage.txt");
                List<Tuple<JointType, JointType, double, double>> angleListScatter = Skeleton.ProcessSkeletonAngelsFromFile(path + name + "AngleScatter.txt");

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

                // UJATLAG
                List<Skeleton> newRefExercise = Skeleton.ProcessSkeletonFromFile(path + exerciseName + ".txt");

                /*for (int i = 0; i < filesData.Count; ++i)
                {
                    CalculateSkeletonAngles(filesData[i]);
                }*/

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

                    average.Add(AverageSkeletons(sum));


                    sum.Clear();

                    if (counter == filesData.Count)
                    {
                        break;
                    }
                }
            }
            else
            {
                average = NewAverageSkeletons(filesData[filesData.Count() - 1]);
            }
        }
        /// <summary>
        /// New average skeleton
        /// When have average, but came new reference exercise
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns></returns>
        private List<Skeleton> NewAverageSkeletons(List<Skeleton> skeletons)
        {
            int n = average[0].Joints.Count;//count for joints
            int m = average[0].AngleList.Count;//count for angles
            int count = skeletons.Count < average.Count ? skeletons.Count : average.Count;
            List<Skeleton> newAverage = new List<Skeleton>(count);
            // the count is 2, because 1-old average, 2-new reference data
            for (int i = 0; i < count; ++i)
            {
                newAverage.Add(new Skeleton());
                newAverage[i].Joints = new List<Vector3D>();
                newAverage[i].AngleList = new List<Tuple<JointType, JointType, double>>();
                //joint
                for (int j = 0; j < 25; ++j) //for joint
                {
                    double x = (average[i].Joints[j].X * (n - 1) + skeletons[i].Joints[j].X) / n;
                    double y = (average[i].Joints[j].Y * (n - 1) + skeletons[i].Joints[j].Y) / n;
                    double z = (average[i].Joints[j].Z * (n - 1) + skeletons[i].Joints[j].Z) / n;

                    newAverage[i].Joints.Add(new Vector3D(x, y, z));
                }

                //angles
                for (int j = 0; j < 23; ++j) //for angles
                {
                    JointType first = average[0].AngleList[j].Item1;
                    JointType second = average[0].AngleList[j].Item2;
                    double av = (average[i].AngleList[j].Item3 * (m - 1) + skeletons[i].AngleList[j].Item3) / m;
                    newAverage[i].AngleList.Add(new Tuple<JointType, JointType, double>(first, second, av));
                }
            }

            return newAverage;
        }
        /// <summary>
        /// Average skeleton
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns></returns>
        private Skeleton AverageSkeletons(List<Skeleton> skeletons)
        {
            Skeleton average = new Skeleton();
            int[] div = new int[25];
            int[] div2 = new int[25];
            average.Joints = skeletons[0].Joints;
            average.AngleList = new List<Tuple<JointType, JointType, double>>();
            double[] anglesSum = new double[25];
            for (int i = 1; i < skeletons.Count; ++i)
            {
                //joint
                for (int j = 0; j < 25; ++j) //for joint
                {
                    Vector3D v = new Vector3D();
                    v = average.Joints[j];
                    v.X += skeletons[i].Joints[j].X;
                    v.Y += skeletons[i].Joints[j].Y;
                    v.Z += skeletons[i].Joints[j].Z;
                    average.Joints[j] = v;
                    if (skeletons[i].Joints[j].X != 0 && skeletons[i].Joints[j].Y != 0 && skeletons[i].Joints[j].Z != 0)
                    {
                        div[j]++;
                    }
                }

                //angles
                for (int j = 0; j < 23; ++j) //for angles
                {
                    anglesSum[j] += skeletons[i].AngleList[j].Item3;
                    div2[j]++;
                }

            }

            for (int j = 0; j < 25; ++j)
            {
                average.Joints[j] = average.Joints[j] / div[j];
            }
            for (int j = 0; j < 23; ++j)
            {
                JointType first = skeletons[0].AngleList[j].Item1;
                JointType second = skeletons[0].AngleList[j].Item2;
                double av = anglesSum[j] / div2[j];
                average.AngleList.Add(new Tuple<JointType, JointType, double>(first, second, av));
            }

            return average;
        }

        /// <summary>
        /// Scatter
        /// </summary>
        public void Scatter(bool isNew)
        {
            //string[] names = exerciseNamesRef[0].Split('.');
            //average = Skeleton.ProcessSkeletonFromFile(path + names[0] + "Average.txt");

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

                    scatter.Add(scatterSkeletons(sum, average[j]));
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
                scatter = NewScatterSkeletons(filesData[filesData.Count() - 1]);
            }


        }
        /// <summary>
        /// Scatter of skeleton
        /// </summary>
        /// <param name="skeletons"></param>
        /// <param name="average"></param>
        /// <returns></returns>
        private Skeleton scatterSkeletons(List<Skeleton> skeletons, Skeleton average)
        {
            Skeleton scatter = new Skeleton();
            scatter.Joints = new List<Vector3D>();
            int[] div = new int[25];
            int[] div2 = new int[25];
            for (int i = 0; i < 25; ++i)
            {
                scatter.Joints.Add(new Vector3D(0, 0, 0));
            }
            scatter.AngleList = new List<Tuple<JointType, JointType, double>>();
            double[] anglesSum = new double[25];
            for (int i = 0; i < skeletons.Count; ++i)
            {
                //joint
                for (int j = 0; j < 25; ++j) //for joint
                {
                    //(x1-A)^2 + (x2-A)^2 + ...
                    double x = scatter.Joints[j].X + (skeletons[i].Joints[j].X - average.Joints[j].X) * (skeletons[i].Joints[j].X - average.Joints[j].X);
                    double y = scatter.Joints[j].Y + (skeletons[i].Joints[j].Y - average.Joints[j].Y) * (skeletons[i].Joints[j].Y - average.Joints[j].Y);
                    double z = scatter.Joints[j].Z + (skeletons[i].Joints[j].Z - average.Joints[j].Z) * (skeletons[i].Joints[j].Z - average.Joints[j].Z);
                    scatter.Joints[j] = new Vector3D(x, y, z);
                    if (x != 0 && y != 0 && z != 0)
                    {
                        div[j]++;
                    }

                }

                //angles
                for (int j = 0; j < 23; ++j) //for angles
                {
                    //(x1-A)^2 + (x2-A)^2 + ...
                    anglesSum[j] += (skeletons[i].AngleList[j].Item3 - average.AngleList[j].Item3) * (skeletons[i].AngleList[j].Item3 - average.AngleList[j].Item3);
                    if ((skeletons[i].AngleList[j].Item3 - average.AngleList[j].Item3) != 0)
                    {
                        div2[j]++;
                    }
                }

            }

            for (int j = 0; j < 25; ++j)
            {
                Vector3D v = new Vector3D();
                if (div[j] == 0)
                {
                    div[j]++;
                }
                v.X = Math.Sqrt(scatter.Joints[j].X / div[j]);
                v.Y = Math.Sqrt(scatter.Joints[j].Y / div[j]);
                v.Z = Math.Sqrt(scatter.Joints[j].Z / div[j]);

                scatter.Joints[j] = v;
            }
            for (int j = 0; j < 23; ++j)
            {
                if (div2[j] == 0)
                {
                    div2[j]++;
                }
                JointType first = skeletons[0].AngleList[j].Item1;
                JointType second = skeletons[0].AngleList[j].Item2;
                double av = Math.Sqrt(anglesSum[j] / div2[j]);
                scatter.AngleList.Add(new Tuple<JointType, JointType, double>(first, second, av));
            }
            return scatter;
        }
        /// <summary>
        /// Scatter of skeleton
        /// </summary>
        /// <param name="skeletons"></param>
        /// <param name="average"></param>
        /// <returns></returns>
        private List<Skeleton> NewScatterSkeletons(List<Skeleton> skeletons)
        {
            List<Skeleton> newScatter = new List<Skeleton>();

            double n = skeletons[0].Joints.Count;
            double m = skeletons[0].AngleList.Count;
            int count = skeletons.Count < scatter.Count ? skeletons.Count : scatter.Count;

            for (int i = 0; i < count; ++i)
            {
                newScatter.Add(new Skeleton());
                newScatter[i].Joints = new List<Vector3D>();
                newScatter[i].AngleList = new List<Tuple<JointType, JointType, double>>();
                newScatter[i].ImportanceInPercent = new List<double>();
                newScatter[i].ImportanceAngleInPercent = new List<double>();
                //joint
                for (int j = 0; j < 25; ++j) //for joint
                {
                    //newScatter = sqrt((n-1)/n * oldScatter^2 + 1/n * (x - A)^2)
                    double x = Math.Sqrt(((n - 1) / n * (scatter[i].Joints[j].X * scatter[i].Joints[j].X)) + (1 / n * (skeletons[i].Joints[j].X - average[i].Joints[j].X)));
                    double y = Math.Sqrt((n - 1) / n * (scatter[i].Joints[j].Y * scatter[i].Joints[j].Y) + 1 / n * (skeletons[i].Joints[j].Y - average[i].Joints[j].Y));
                    double z = Math.Sqrt((n - 1) / n * (scatter[i].Joints[j].Z * scatter[i].Joints[j].Z) + 1 / n * (skeletons[i].Joints[j].Z - average[i].Joints[j].Z));

                    newScatter[i].Joints.Add(new Vector3D(x, y, z));

                }
                newScatter[i].ImportanceInPercent = scatter[i].ImportanceInPercent;

                //angles
                for (int j = 0; j < 23; ++j) //for angles
                {
                    JointType first = skeletons[0].AngleList[j].Item1;
                    JointType second = skeletons[0].AngleList[j].Item2;
                    //newScatter = sqrt((n-1)/n * oldScatter^2 + 1/n * (x - A)^2)
                    double av = Math.Sqrt((n - 1) / n * scatter[i].AngleList[j].Item3 + 1 / n * (skeletons[i].AngleList[j].Item3 - average[i].AngleList[j].Item3));
                    newScatter[i].AngleList.Add(new Tuple<JointType, JointType, double>(first, second, av));
                }
                newScatter[i].ImportanceAngleInPercent = scatter[i].ImportanceAngleInPercent;
            }
            return newScatter;
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
