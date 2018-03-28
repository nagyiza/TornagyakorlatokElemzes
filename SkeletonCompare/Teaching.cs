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
        private string path = "..\\..\\..\\ReferenceData\\";
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

        public Teaching(string exerciseNameRef)
        {
            Skeletons = new List<Skeleton>();
            average = new List<Skeleton>();
            scatter = new List<Skeleton>();

            exerciseName = exerciseNameRef;
            if (!File.Exists(path + exerciseNameRef + "Average.txt")
                || !File.Exists(path + exerciseNameRef + "AngleAverage.txt")
                || !File.Exists(path + exerciseNameRef + "Scatter.txt")
                || !File.Exists(path + exerciseNameRef + "AngleScatter.txt"))
            {
                filesData = new List<List<Skeleton>>();
                exerciseNamesRef = new List<string>();
                if (File.Exists(path + exerciseNameRef + ".txt"))
                {
                    exerciseNamesRef.Add(exerciseNameRef + ".txt");
                    filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseNameRef + ".txt"));
                    //CalculateSkeletonAngles(filesData[filesData.Count - 1]);

                    for (int i = 1; ; ++i)
                    {
                        if (File.Exists(path + exerciseNameRef + i + ".txt"))
                        {
                            exerciseNamesRef.Add(exerciseNameRef + i + ".txt");
                            filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseNameRef + i + ".txt"));
                            //CalculateSkeletonAngles(filesData[filesData.Count - 1]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }

            }

        }

        /// <summary>
        /// Calculate the averages, the scatter and teach what joints are important
        /// </summary>
        public void TeachingSkeleton()
        {
            //calculate average and the scatter
            CalculateScatter();

            if (filesData != null)
            {
                string[] names = exerciseNamesRef[0].Split('.');
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

        private void CalculateScatter()
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
                    Average();

                    Scatter();
                }
            }
            else
            {
                //read data in file (joint's averages and scatters)
                average = Skeleton.ProcessSkeletonFromFile(path + exerciseName + "Average.txt");
                scatter = Skeleton.ProcessSkeletonFromFile(path + exerciseName + "Scatter.txt");
                // in this list are the joint, the angle and the teaching percent
                List<Tuple<JointType, JointType, double, double>> angleListAverage = Skeleton.ProcessSkeletonAngelsFromFile(path + exerciseName + "AngleAverage.txt");
                List<Tuple<JointType, JointType, double, double>> angleListScatter = Skeleton.ProcessSkeletonAngelsFromFile(path + exerciseName + "AngleScatter.txt");

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

        public void Average()
        {
            List<Skeleton> sum = new List<Skeleton>();

            int counter = 0;
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

                average.Add(AverageSkeletons(sum));
                sum.Clear();

                if (counter == filesData.Count)
                {
                    break;
                }


            }
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
        public void Scatter()
        {
            //string[] names = exerciseNamesRef[0].Split('.');
            //average = Skeleton.ProcessSkeletonFromFile(path + names[0] + "Average.txt");

            List<Skeleton> sum = new List<Skeleton>();
            List<Skeleton> averages = new List<Skeleton>();
            int counter = 0;
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

    }
}
