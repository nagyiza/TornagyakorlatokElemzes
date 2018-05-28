using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SkeletonCompare
{
    public class CalculateScatter
    {
        /// <summary>
        /// New average skeleton
        /// When have average, but came new reference exercise
        /// </summary>
        /// <param name="skeletons"></param>
        /// <returns></returns>
        public static List<Skeleton> NewAverageSkeletons(List<Skeleton> skeletons, List<Skeleton> average)
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
        public static Skeleton AverageSkeletons(List<Skeleton> skeletons)
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
        /// Scatter of skeleton
        /// </summary>
        /// <param name="skeletons"></param>
        /// <param name="average"></param>
        /// <returns></returns>
        public static Skeleton scatterSkeletons(List<Skeleton> skeletons, Skeleton average)
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
        public static List<Skeleton> NewScatterSkeletons(List<Skeleton> skeletons, List<Skeleton> average, List<Skeleton> scatter)
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
    }
}
