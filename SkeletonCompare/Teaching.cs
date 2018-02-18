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
        private string path = "..\\..\\..\\ReferenceData\\";
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
        public Teaching(string exerciseNameRef)
        {
            if (!File.Exists(path + exerciseNameRef + "Average.txt"))
            {
                exerciseNamesRef = new List<string>();
                Skeletons = new List<Skeleton>();
                filesData = new List<List<Skeleton>>();
                average = new List<Skeleton>();
                if (File.Exists(path + exerciseNameRef + ".txt"))
                {
                    exerciseNamesRef.Add(exerciseNameRef + ".txt");
                    filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseNameRef + ".txt"));
                    CalculateSkeletonAngles(filesData[filesData.Count - 1]);

                    for (int i = 1; ; ++i)
                    {
                        if (File.Exists(path + exerciseNameRef + i + ".txt"))
                        {
                            exerciseNamesRef.Add(exerciseNameRef + i + ".txt");
                            filesData.Add(Skeleton.ProcessSkeletonFromFile(path + exerciseNameRef + i + ".txt"));
                            CalculateSkeletonAngles(filesData[filesData.Count - 1]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Average();
                string[] names = exerciseNamesRef[0].Split('.');
                Skeleton.SkeletonPrint(average, path + names[0] + "Average.txt");
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
                    if (skeletons[i].Joints[j].X != 0 && skeletons[i].Joints[j].Y != 0 && skeletons[i].Joints[j].Z != 0) {
                        div[j]++;
                    }
                    else
                    {
                        int l =6;
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
            string[] names = exerciseNamesRef[0].Split('.');
            average = Skeleton.ProcessSkeletonFromFile(path + names[0] + "Average.txt");


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

    }
}
