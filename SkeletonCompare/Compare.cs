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
    public class Compare
    {
        private int jointCount = 25;
        private List<Skeleton> skeletons;
        private List<Skeleton> skeletonsRef;
        private String path = "";

        private Tuple<double, double, double> bodyDistance;
        /// <summary>
        /// The skeletons's coordinate's averages
        /// </summary>
        private List<Tuple<double, double, double>> averages;
        /// <summary>
        /// The skeletons's coordinate's scatters
        /// </summary>
        private List<Skeleton> scattersSkeleton;

        /// <summary>
        /// The joints's angle
        /// </summary>
        List<Tuple<JointType, double>> anglesList;

        /// <summary>
        /// The count of angles
        /// </summary>
        private int angleCount = 0;
        
        /// <summary>
        /// The count of joints
        /// </summary>
        private int scatterCount = 0;

        public Compare(String path, String userSkeletonData, String refSkeletonData)
        {
            this.path = path;
            skeletons = new List<Skeleton>();
            skeletonsRef = new List<Skeleton>();
            skeletons = ProcessSkeletonData(path + userSkeletonData);
            skeletonsRef = ProcessSkeletonData(path + refSkeletonData);

            //DTW compare algorithm
            DTW();

        }

        #region Process skeleton data
        /// <summary>
        /// This method process the skeleton data (from the file)
        /// </summary>
        /// <param name="path"></param>
        public List<Skeleton> ProcessSkeletonData(string path)
        {
            List<Skeleton> Skeletons = new List<Skeleton>();
            string line = ""; // A line in the file
            char[] separators = { ' ' };
            string[] pathSplit = path.Split('\\');
            // Just file name
            if (pathSplit[pathSplit.Length - 1] != ".txt")
            {
                StreamReader file;
                if (File.Exists(path))
                {
                    file = new StreamReader(path);
                }
                else
                {
                    return null;
                }
                Skeleton currentFrame = new Skeleton();
                int jointNumber = 0;
                List<Vector3D> joints = new List<Vector3D>();
                //read the first line, because it is the bill head and this not need it
                line = file.ReadLine();
                //read the skeleton data
                while ((line = file.ReadLine()) != null)
                {
                    // split the data 
                    string[] words = line.Split(separators);

                    //if the line is a first line
                    double number;
                    if (!Double.TryParse(words[0], out number))
                    {
                        break;
                    }
                    
                    // in one line is 10 data from skeleton
                    if (words.Length < 4)
                    {
                        // skeleton has 25 joints data
                        while (joints.Count < jointCount)
                        {
                            joints.Add(new Vector3D(0, 0, 0));
                        }

                        //set the list to the frame
                        currentFrame.Joints = joints;
                        //determine bones
                        currentFrame.DetermineBones();
                        //save the last frame
                        Skeletons.Add(currentFrame);
                        break;
                    }


                    int jointTypeNr = Convert.ToInt32(words[3]); //(int)jointType
                    double X = Convert.ToDouble(words[0]); //unitySpacePoint.X
                    double Y = Convert.ToDouble(words[1]); //unitySpacePoint.Y
                    double Z = Convert.ToDouble(words[2]); //unitySpacePoint.Z


                    if (jointNumber > Convert.ToInt32(words[3]))
                    {
                        //fill the end with (0, 0)
                        while (joints.Count < 25)
                        {
                            //will with (0, 0)
                            joints.Add(new Vector3D(0, 0, 0));
                        }
                        //set the list to the frame
                        currentFrame.Joints = joints;
                        //determine bones
                        currentFrame.DetermineBones();
                        //put in a list
                        Skeletons.Add(currentFrame);
                        //create new frame
                        currentFrame = new Skeleton();
                        joints = new List<Vector3D>();

                    }
                    // skeleton joint type in number
                    jointNumber = Convert.ToInt32(words[3]);
                    while (joints.Count < jointNumber)
                    {
                        //will with (0, 0)
                        joints.Add(new Vector3D(0, 0, 0));
                    }
                    joints.Add(new Vector3D(X, Y, Z));

                }
                if (Skeletons.Count == 0)
                {
                    //set the list to the frame
                    currentFrame.Joints = joints;
                    //determine bones
                    currentFrame.DetermineBones();
                    //put in a list
                    Skeletons.Add(currentFrame);
                }
                file.Close();
                return Skeletons;

            }
            else
            {
                return null;
            }
        }
        #endregion

        #region DTW algorithm
        public void DTW()
        {
            try
            {
                if (skeletonsRef.Count > 0 && skeletons.Count > 0)
                {
                    //scatter of reference skeleton
                    Scatter(skeletonsRef);
                    //print scatter
                    SkeletonPrint(scattersSkeleton, path + @"\scatterSkeletonRef.txt");

                    //calculat angles
                    CalculateSkeletonAngles(skeletonsRef);
                    CalculateSkeletonAngles(skeletons);


                    int[] dtw = DTWDistance(skeletonsRef, skeletons);
                    double scatterPercent = (scatterCount - dtw[0]) * 100 / (double)scatterCount;
                    double anglePercent = (angleCount - dtw[1]) * 100 / (double)angleCount;
                    Console.WriteLine("Scatter: " + dtw[0] + Environment.NewLine + "Angles: " + dtw[1] + Environment.NewLine);
                    Console.WriteLine("Scatter (%): " + scatterPercent + Environment.NewLine + "Angles (%): " + anglePercent);

                    Console.WriteLine(Environment.NewLine + "scatter count: " + scatterCount + Environment.NewLine + "angles count: " + angleCount);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exeption " + e.Message);
            }
        }

        public int[] DTWDistance(List<Skeleton> s, List<Skeleton> t)
        {
            //List<Tuple<double, double, double>> skeletonRef = new List<Tuple<double, double, double>>();
            //List<Tuple<double, double, double>> skeleton = new List<Tuple<double, double, double>>();

            int n = s.Count; //reference
            int m = t.Count; //user

            int[,] DTW = new int[n, m]; //scatter 
            int[,] DTW2 = new int[n, m];//angles
            int cost;

            for (int i = 0; i < n; ++i)
            {
                DTW[i, 0] = 100000;
                DTW2[i, 0] = 100000;
            }
            for (int i = 0; i < m; ++i)
            {
                DTW[0, i] = 100000;
                DTW2[0, i] = 100000;
            }
            DTW[0, 0] = 0;
            DTW2[0, 0] = 0;
            for (int i = 1; i < n; ++i) // for the reference skeleton
            {
                for (int j = 1; j < m; ++j) // for the real skeleton
                {
                    //DTW with scatters
                    cost = CompareSkeletonWithScatters(skeletonsRef[i], skeletons[j], scattersSkeleton[i]);
                    int min = Math.Min(DTW[i - 1, j], DTW[i, j - 1]);
                    DTW[i, j] = cost + Math.Min(min, DTW[i - 1, j - 1]);
                    //Console.Write(DTW[i, j] + " ");

                    //compare skeletons with angles
                    cost = CompareSkeletonWithAngles(skeletonsRef[i], skeletons[j]);
                    int min2 = Math.Min(DTW2[i - 1, j], DTW2[i, j - 1]);
                    DTW2[i, j] = cost + Math.Min(min2, DTW2[i - 1, j - 1]);
                }
                //Console.WriteLine();

            }

            int[] result = new int[2];
            result[0] = DTW[n - 1, m - 1];
            result[1] = DTW2[n - 1, m - 1];
            return result;
        }
        #endregion

        #region For scatter
        //atlag
        /// <summary>
        /// This method calculates the coordinates's averages
        /// </summary>
        /// <param name="Skeletons"></param>
        public void Average(List<Skeleton> Skeletons)
        {
            //sum coordinate
            List<Tuple<double, double, double>> sums = new List<Tuple<double, double, double>>(jointCount);//osszegek
            List<Tuple<double, double, double>> sums2 = new List<Tuple<double, double, double>>(jointCount);//seged valtozo
            List<int> count = new List<int>(jointCount) { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            if (Skeletons.Count > 0) {
                foreach (Skeleton skeleton in Skeletons)
                {
                    if (skeleton == Skeletons[0])
                    {
                        for (int i = 0; i < jointCount; ++i)
                        {
                            sums.Add(new Tuple<double, double, double>(skeleton.Joints[i].X, skeleton.Joints[i].Y, skeleton.Joints[i].Z));
                        }
                    }
                    //for joints
                    for (int i = 0; i < jointCount; ++i)
                    {
                        if (skeleton.Joints[i].X != 0)
                        {
                            count[i]++;

                            double X = sums[i].Item1 + skeleton.Joints[i].X;
                            double Y = sums[i].Item2 + skeleton.Joints[i].Y;
                            double Z = sums[i].Item3 + skeleton.Joints[i].Z;
                            sums2.Add(new Tuple<double, double, double>(X, Y, Z)); // x,y,z
                        }
                        else
                        {
                            sums2.Add(new Tuple<double, double, double>(sums[i].Item1, sums[i].Item2, sums[i].Item3));
                        }

                    }
                    sums = sums2;
                    sums2 = new List<Tuple<double, double, double>>(25);

                }
                //average
                averages = new List<Tuple<double, double, double>>();//atlagok
                for (int i = 0; i < jointCount; ++i)
                {
                    if (count[i] != 0)
                    {
                        averages.Add(new Tuple<double, double, double>(sums[i].Item1 / count[i], sums[i].Item2 / count[i], sums[i].Item3 / count[i]));
                    }
                    else
                    {
                        averages.Add(new Tuple<double, double, double>(sums[i].Item1, sums[i].Item2, sums[i].Item3));
                    }
                }
            }
        }

        //szoras
        public void Scatter(List<Skeleton> Skeletons)
        {
            //calculates the averages
            Average(Skeletons);

            //calculates the scetters
            List<Vector3D> scatters = new List<Vector3D>();//atlagok
            scattersSkeleton = new List<Skeleton>();
            foreach (Skeleton skeleton in Skeletons)
            {
                Skeleton scatterSkeleton = new Skeleton();
                //for joints
                for (int i = 0; i < jointCount; ++i)
                {
                    //scatter = coordinate - average
                    double X = Math.Abs(skeleton.Joints[i].X - averages[i].Item1);
                    double Y = Math.Abs(skeleton.Joints[i].Y - averages[i].Item2);
                    double Z = Math.Abs(skeleton.Joints[i].Z - averages[i].Item3);
                    scatters.Add(new Vector3D(X, Y, Z));
                }
                scatterSkeleton.Joints = scatters;
                scatters = new List<Vector3D>();
                //add the new Skeleton data
                scattersSkeleton.Add(scatterSkeleton);
            }
        }
        #endregion

        #region Compare skeletons
        /// <summary>
        /// Calculate the distance beetween skeletonRef coordinates and skeleton coordinates with scatters
        /// skeletonRef.x - skeleton.x ...
        /// </summary>
        /// <param name="skeletonRef"></param>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private int CompareSkeletonWithScatters(Skeleton skeletonRef, Skeleton skeleton, Skeleton scatterSkeleton)
        {
            //distance beetween reference body and real body
            bodyDistance = new Tuple<double, double, double>(
                Math.Abs(skeleton.Joints[0].X - skeleton.Joints[0].X),
                Math.Abs(skeleton.Joints[0].Y - skeleton.Joints[0].Y),
                Math.Abs(skeleton.Joints[0].Z - skeleton.Joints[0].Z)); // distance beetwen bodys's spine base (joinType = 0)

            int diferenceJoint = 0;
            for (int i = 0; i < jointCount; ++i) // for skeleton joint; one for, because the skeletonRef and skeleton array lenght are same
            {
                if (isJointTracked(skeletonRef, i)
                    && isJointTracked(skeleton, i))
                {

                    double x = Math.Abs(skeletonRef.Joints[i].X - skeleton.Joints[i].X - bodyDistance.Item1);
                    double y = Math.Abs(skeletonRef.Joints[i].Y - skeleton.Joints[i].Y - bodyDistance.Item2);
                    double z = Math.Abs(skeletonRef.Joints[i].Z - skeleton.Joints[i].Z - bodyDistance.Item3);
                    //count the joints
                    scatterCount++;
                    if (x > scatterSkeleton.Joints[i].X || y > scatterSkeleton.Joints[i].Y || z > scatterSkeleton.Joints[i].Z)
                    {
                        diferenceJoint++;
                    }
                }

            }

            return diferenceJoint;
        }

        /// <summary>
        /// Compare skeletons with angles
        /// </summary>
        /// <param name="skeletonRef">Reference skeleton</param>
        /// <param name="skeleton">User skeleton</param>
        /// <returns></returns>
        private int CompareSkeletonWithAngles(Skeleton skeletonRef, Skeleton skeleton)
        {
            int diferenceAngel = 0;
            for (int i = 0; i < skeletonRef.AngleList.Count; ++i) // for skeleton angles; one for, because the skeletonRef's angels and skeleton's angles array lenght are same
            {
                if (isJointTracked(skeletonRef, i)
                    && isJointTracked(skeleton, i)
                    && skeletonRef.AngleList[i].Item3 > 0
                    && skeleton.AngleList[i].Item3 > 0
                    )
                {
                    // without hand tip and without foot, because these are not important
                    if ((int)skeletonRef.AngleList[i].Item1 != 23 // HandTipRight
                        && (int)skeletonRef.AngleList[i].Item1 != 24 // ThumbRight
                        && (int)skeletonRef.AngleList[i].Item1 != 21 // HandTipLeft
                        && (int)skeletonRef.AngleList[i].Item1 != 22 // ThumbLeft
                        && (int)skeletonRef.AngleList[i].Item1 != 19 // FootRight
                        && (int)skeletonRef.AngleList[i].Item1 != 15) // FootLeft
                    {
                        double angleDistance = Math.Abs(skeletonRef.AngleList[i].Item3 - skeleton.AngleList[i].Item3);

                        if (angleDistance > 0)  
                        {
                            diferenceAngel++;
                        }
                        //count the angles
                        angleCount++;
                    }
                }

            }

            return diferenceAngel;
        }
        

        /// <summary>
        /// This method tell if the bone is valid
        /// Tell the joint was tracked
        /// </summary>
        /// <param name="skeleton"></param>
        /// <param name="jointType"></param>
        /// <returns></returns>
        public bool isJointTracked(Skeleton skeleton, int jointType)
        {
            for (int i = 0; i < skeleton.Bones.Count; ++i)
            {
                if (jointType == (int)skeleton.Bones[i].Item1 || jointType == (int)skeleton.Bones[i].Item2)
                {
                    if (skeleton.Bones[i].Item3)
                    {
                        return true;
                    }
                    else
                    {
                        if (skeleton.Joints[jointType].X == 0 && skeleton.Joints[jointType].Y == 0 && skeleton.Joints[jointType].Z == 0)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                }
            }
            return false;
        }

        /// <summary>
        /// Calculate the angles beetween skeleton's bones
        /// </summary>
        /// <param name="skeleton"> List of skeletons</param>
        public void CalculateSkeletonAngles(List<Skeleton> skeleton)
        {
            if (skeleton.Count > 0) {
                foreach (var skl in skeleton)
                {
                    skl.SkeletonAngle();
                }
            }
        }
        #endregion

        #region Skeleton print
        public void SkeletonPrint(List<Skeleton> skeletonList, string path)
        {
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write("  X                    Y                  Z                   JointType" + Environment.NewLine);

            foreach (Skeleton skeleton in skeletonList)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    double X = skeleton.Joints[i].X;
                    double Y = skeleton.Joints[i].Y;
                    double Z = skeleton.Joints[i].Z;
                    streamWriter.Write(X + " " + Y + " " + Z + "      " + i + Environment.NewLine);
                }
            }
        }
        #endregion
    }
}
