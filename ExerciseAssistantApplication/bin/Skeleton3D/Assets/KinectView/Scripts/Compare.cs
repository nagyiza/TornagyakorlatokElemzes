using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.KinectView.Scripts
{
    public class Compare
    {
        private int jointCount = 25;
        private List<Skeleton> skeletons;
        private List<Skeleton> skeletonsRef;
        private String unityPath = "..\\..\\..\\UnityData\\";
        private String refDataPath = "..\\..\\..\\ReferenceData\\";

        private Vector3 bodyDistance;
        /// <summary>
        /// The skeletons's coordinate's averages
        /// </summary>
        private List<Skeleton> averages;
        /// <summary>
        /// The skeletons's coordinate's scatters
        /// </summary>
        private List<Skeleton> scattersSkeleton;

        /// <summary>
        /// The joints's angle
        /// </summary>
        List<Skeleton> anglesList;

        /// <summary>
        /// The count of angles
        /// </summary>
        private int angleCount = 0;

        /// <summary>
        /// The count of joints
        /// </summary>
        private int scatterCount = 0;

        /// <summary>
        /// The joint witch is not correct
        /// </summary>
        public string errorjointType = "";
        /// <summary>
        /// The joint witch is not correct in angles method
        /// </summary>
        public string errorjointTypeAngles = "";
        /// <summary>
        /// Result of dtw algorithm
        /// 0 - result with scatter
        /// 1 - result with angles
        /// </summary>
        public int[] dtwResult;
        /// <summary>
        /// Average file name of exercise
        /// </summary>
        private string exerciseNameAverage = "";
        /// <summary>
        /// Average file name of exercise
        /// </summary>
        private string exerciseNameScatter = "";

        public Compare(List<Vector4> reference, List<Vector4> user, string exercise)
        {
            if (exercise.Contains("Average"))
            {
                exerciseNameAverage = exercise;
            }
            else
            {
                exerciseNameAverage = exercise + "Average";
            }
            //exerciseNameScatter = "scatterSkeletonRefUnity";

            skeletons = ProcessSkeletonData(user);
            skeletonsRef = ProcessSkeletonData(reference);

            //DTW compare algorithm
            DTW();

        }

        #region Process skeleton data
        /// <summary>
        /// This method process the skeleton data (from the file)
        /// </summary>
        /// <param name="path"></param>
        public List<Skeleton> ProcessSkeletonData(List<Vector4> reference)
        {
            List<Skeleton> Skeletons = new List<Skeleton>();

            Skeleton currentFrame = new Skeleton();
            int jointNumber = 0;
            List<Vector3> joints = new List<Vector3>();
            //read the skeleton data
            foreach (Vector4 line in reference)
            {
                int jointTypeNr = Convert.ToInt32(line.w); //(int)jointType
                float X = line.x; //unitySpacePoint.X
                float Y = line.y; //unitySpacePoint.Y
                float Z = line.z; //unitySpacePoint.Z

                currentFrame.joinIsValid[currentFrame.getJoinType(jointTypeNr)] = true;

                if (jointNumber > jointTypeNr)
                {
                    //fill the end with (0, 0)
                    while (joints.Count < 25)
                    {
                        //will with (0, 0)
                        joints.Add(new Vector3(0, 0, 0));
                    }
                    //set the list to the frame
                    currentFrame.Joints = joints;
                    //determine bones
                    currentFrame.DetermineBones();
                    //put in a list
                    Skeletons.Add(currentFrame);
                    //create new frame
                    currentFrame = new Skeleton();
                    joints = new List<Vector3>();

                }
                // skeleton joint type in number
                jointNumber = Convert.ToInt32(line.w);
                while (joints.Count < jointNumber)
                {
                    //will with (0, 0)
                    joints.Add(new Vector3(0, 0, 0));
                }
                joints.Add(new Vector3(X, Y, Z));


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
            return Skeletons;
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
                    //Scatter(skeletonsRef);
                    //print scatter
                    //SkeletonPrint(scattersSkeleton, unityPath + @"\scatterSkeletonRefUnity.txt");

                    //calculat angles
                    CalculateSkeletonAngles(skeletonsRef);
                    CalculateSkeletonAngles(skeletons);


                    dtwResult = DTWDistance(skeletonsRef, skeletons);

                    File.WriteAllText(unityPath + "result.txt", dtwResult[0] + " " + dtwResult[1] + Environment.NewLine);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exeption " + e.Message);
            }
        }

        public int[] DTWDistance(List<Skeleton> s, List<Skeleton> t)
        {
            //one reference skeleton position(24 joint)

            int skeletonRefCount = s.Count; // reference
            int userSkeletonCount = 2; //user
            int j = 1; //user

            int[,] DTW = new int[skeletonRefCount, userSkeletonCount];
            int[,] DTW2 = new int[skeletonRefCount, userSkeletonCount];
            int cost;

            for (int i = 0; i < skeletonRefCount; ++i)
            {
                DTW[i, 0] = 100000;
                DTW2[i, 0] = 100000;
            }
            for (int i = 0; i < userSkeletonCount; ++i)
            {
                DTW[0, i] = 100000;
                DTW2[0, i] = 100000;
            }
            DTW[0, 0] = 0;
            DTW2[0, 0] = 0;
            for (int i = 1; i < skeletonRefCount; ++i) // for the reference skeleton
            {
                //DTW with scatters
                cost = CompareSkeletonWithScatters(skeletonsRef[i], skeletons[j], null);//scattersSkeleton[i]);
                int min = Math.Min(DTW[i - 1, j], DTW[i, j - 1]);
                DTW[i, j] = cost + Math.Min(min, DTW[i - 1, j - 1]);
                
                //compare skeletons with angles
                cost = CompareSkeletonWithAngles(skeletonsRef[i], skeletons[j]);
                int min2 = Math.Min(DTW2[i - 1, j], DTW2[i, j - 1]);
                DTW2[i, j] = cost + Math.Min(min2, DTW2[i - 1, j - 1]);

            }

            int[] result = new int[2];
            result[0] = DTW[skeletonRefCount - 1, userSkeletonCount - 1];
            result[1] = DTW2[skeletonRefCount - 1, userSkeletonCount - 1];

            Debug.Log("Result: " + result[0] + " " + result[1]);
            return result;
        }
        #endregion

        #region For scatter
        //atlag
        /// <summary>
        /// This method read the coordinates's averages in a file
        /// </summary>
        /// <param name="Skeletons"></param>
        public void Average()
        {
            string pathName = refDataPath + exerciseNameAverage + ".txt";
            averages = Skeleton.ProcessSkeletonFromFile(pathName);
        }
        /// <summary>
        /// This method read the coordinates's scatter in a file
        /// </summary>
        /// <param name="Skeletons"></param>
        public void Scatter(List<Skeleton> Skeletons)
        {
            Average();
            //string pathName = refDataPath + exerciseNameScatter + ".txt";
            //scattersSkeleton = Skeleton.ProcessSkeletonFromFile(pathName);
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
            bodyDistance = new Vector3(
                Math.Abs(skeleton.Joints[0].x - skeleton.Joints[0].x),
                Math.Abs(skeleton.Joints[0].y - skeleton.Joints[0].y),
                Math.Abs(skeleton.Joints[0].z - skeleton.Joints[0].z)); // distance beetwen bodys's spine base (joinType = 0)

            int diferenceJoint = 0;
            for (int i = 0; i < jointCount; ++i) // for skeleton joint; one for, because the skeletonRef and skeleton array lenght are same
            {
                // without hand tip and without foot, because these are not important
                if (isJointTracked(skeletonRef, i)
                    && isJointTracked(skeleton, i)
                    && i != 23 // HandTipRight
                    && i != 24 // ThumbRight
                    && i != 21 // HandTipLeft
                    && i != 22 // ThumbLeft
                    && i != 19 // FootRight
                    && i != 15  // FootLeft
                    && i != 7
                    && i != 11
                    && i != 18
                    && i != 14)
                {
                    double x = Math.Abs(skeletonRef.Joints[i].x - skeleton.Joints[i].x - bodyDistance.x);
                    double y = Math.Abs(skeletonRef.Joints[i].y - skeleton.Joints[i].y - bodyDistance.y);
                    double z = Math.Abs(skeletonRef.Joints[i].z - skeleton.Joints[i].z - bodyDistance.z);
                    //count the joints
                    scatterCount++;
                    //TODO
                    if (x > 0.2 || y > 0.2 || z > 0.2)//x > scatterSkeleton.Joints[i].x || y > scatterSkeleton.Joints[i].y || z > scatterSkeleton.Joints[i].z)
                    {
                        diferenceJoint++;
                        errorjointType = skeleton.getJoinType(i).ToString();
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
                    && skeletonRef.AngleList[i].z > 0
                    && skeleton.AngleList[i].z > 0
                    )
                {
                    // without hand tip and without foot, because these are not important
                    if ((int)skeletonRef.AngleList[i].x != 23 // HandTipRight
                        && (int)skeletonRef.AngleList[i].x != 24 // ThumbRight
                        && (int)skeletonRef.AngleList[i].x != 21 // HandTipLeft
                        && (int)skeletonRef.AngleList[i].x != 22 // ThumbLeft
                        && (int)skeletonRef.AngleList[i].x != 19 // FootRight
                        && (int)skeletonRef.AngleList[i].x != 15) // FootLeft
                    {
                        double angleDistance = Math.Abs(skeletonRef.AngleList[i].z - skeleton.AngleList[i].z);

                        if (angleDistance > 0)
                        {
                            diferenceAngel++;

                            errorjointTypeAngles = skeleton.getJoinType(i).ToString();
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
            for (int i = 0; i < 24; ++i) // bones count = 24
            {
                if (jointType == (int)skeleton.getJoinType(i) || jointType == (int)skeleton.bones[skeleton.getJoinType(i)])
                {
                    if (skeleton.joinIsValid[skeleton.getJoinType(i)])
                    {
                        return true;
                    }
                    else
                    {
                        if (skeleton.Joints[jointType].x == 0 && skeleton.Joints[jointType].y == 0 && skeleton.Joints[jointType].z == 0)
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
            if (skeleton.Count > 0)
            {
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
                    double X = skeleton.Joints[i].x;
                    double Y = skeleton.Joints[i].y;
                    double Z = skeleton.Joints[i].z;
                    streamWriter.Write(X + " " + Y + " " + Z + "      " + i + Environment.NewLine);
                }
            }
        }
        #endregion
    }
}
