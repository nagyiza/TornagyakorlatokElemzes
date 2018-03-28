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
    public class Skeleton
    {
        /// <summary>
        /// List of bones
        /// </summary>
        private List<Tuple<JointType, JointType, bool>> bones;
        /// <summary>
        /// List of joints
        /// </summary>
        private List<Vector3D> joints;
        /// <summary>
        /// List of angles
        /// </summary>
        private List<Tuple<JointType, JointType, double>> angleList;
        /// <summary>
        /// List of important joint in percent
        /// </summary>
        private List<double> importanceInPercent;
        /// <summary>
        /// List of important angle in percent
        /// </summary>
        private List<double> importanceAngleInPercent;

        /// <summary>
        /// Constructor, in witch create the bones
        /// </summary>
        public Skeleton()
        {
            joints = new List<Vector3D>();
            angleList = new List<Tuple<JointType, JointType, double>>();
            importanceAngleInPercent = new List<double>();
            importanceInPercent = new List<double>();

            bones = new List<Tuple<JointType, JointType, bool>>();
            //torso
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.Head, JointType.Neck, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.Neck, JointType.SpineShoulder, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.SpineShoulder, JointType.SpineMid, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.SpineMid, JointType.SpineBase, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.SpineShoulder, JointType.ShoulderRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.SpineShoulder, JointType.ShoulderLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.SpineBase, JointType.HipRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.SpineBase, JointType.HipLeft, false));
            //rightArm
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.ShoulderRight, JointType.ElbowRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.ElbowRight, JointType.WristRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.WristRight, JointType.HandRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.HandRight, JointType.HandTipRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.WristRight, JointType.ThumbRight, false));
            //leftArm
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.ShoulderLeft, JointType.ElbowLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.ElbowLeft, JointType.WristLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.WristLeft, JointType.HandLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.HandLeft, JointType.HandTipLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.WristLeft, JointType.ThumbLeft, false));
            //rightLeg
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.HipRight, JointType.KneeRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.KneeRight, JointType.AnkleRight, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.AnkleRight, JointType.FootRight, false));
            //leftLeg
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.HipLeft, JointType.KneeLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.KneeLeft, JointType.AnkleLeft, false));
            bones.Add(new Tuple<JointType, JointType, bool>(JointType.AnkleLeft, JointType.FootLeft, false));

        }

        /// <summary>
        /// Determine bones
        /// </summary>
        public void DetermineBones()
        {
            for (int index = 0; index < bones.Count; index++)
            {
                if ((joints[(int)(bones[index].Item1)].X != 0) && joints[(int)(bones[index].Item2)].X != 0)
                {
                    //the bone is valid
                    bones[index] = new Tuple<JointType, JointType, bool>(bones[index].Item1, bones[index].Item2, true);
                }
            }
        }

        /// <summary>
        /// Calculate and save the angles in a list
        /// </summary>
        public void SkeletonAngle()
        {
            angleList = new List<Tuple<JointType, JointType, double>>();
            for (int i = 0; i < bones.Count; ++i)
            {
                var bone = bones[i];

                JointType first = bone.Item1;
                JointType center = bone.Item2;
                //search the first joint's pair
                for (int j = i + 1; j < bones.Count; ++j)
                {
                    if (bones[j].Item1 == center) // if found the the first joint's pair
                    {
                        JointType second = bones[j].Item2;
                        //if the bones (first-center, center-second) is tracked 
                        if (bone.Item3 && bones[j].Item3)
                        {
                            // three vector
                            Vector3D firstVector = joints[(int)(bone.Item1)];
                            Vector3D centerVector = joints[(int)(bone.Item2)];
                            Vector3D secondVector = joints[(int)(bones[j].Item2)];
                            //calculate angle
                            double angle = AngleBetweenTwoVectors(firstVector, centerVector, secondVector);
                            //save angels in the list
                            angleList.Add(new Tuple<JointType, JointType, double>(first, second, angle));
                        }
                        else
                        {
                            //save angels in the list
                            //if the bones are not tracked, the angle going to be -1
                            angleList.Add(new Tuple<JointType, JointType, double>(first, second, -1));
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Calculate the angels beetween 2 joints
        /// </summary>
        /// <param name="a"> First joint's coordinate</param>
        /// <param name="b"> Have to be origo </param>
        /// <param name="c"> Second joint's coordinate</param>
        /// <returns> Return the angles beetween a and c points</returns>
        public double AngleBetweenTwoVectors(Vector3D a, Vector3D b, Vector3D c)
        {
            //b going to be origo
            a.X = a.X - b.X;
            a.Y = a.Y - b.Y;
            a.Z = a.Z - b.Z;

            c.X = c.X - b.X;
            c.Y = c.Y - b.Y;
            c.Z = c.Z - c.Z;

            double dotProduct;
            a.Normalize();
            c.Normalize();
            dotProduct = Vector3D.DotProduct(a, c);

            return (double)Math.Acos(dotProduct) / Math.PI * 180; ;
        }


        /// <summary>
        /// Process skeleton data from a file
        /// </summary>
        /// <param name="pathName"> File </param>
        /// <returns>List of skeletons</returns>
        public static List<Skeleton> ProcessSkeletonFromFile(string pathName)
        {
            int jointCount = 25;
            List<Skeleton> Skeletons = new List<Skeleton>();
            string line = ""; // A line in the file
            char[] separators = { ' ' };
            string[] pathSplit = pathName.Split('\\');
            // Just file name
            if (pathSplit[pathSplit.Length - 1] != ".txt")
            {
                StreamReader file;
                if (File.Exists(pathName))
                {
                    file = new StreamReader(pathName);
                }
                else
                {
                    return null;
                }
                Skeleton currentFrame = new Skeleton();
                int jointNumber = 0;
                List<Vector3D> joints = new List<Vector3D>();
                List<double> percents = new List<double>();
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


                    int jointTypeNr = Convert.ToInt32(words[5]); //(int)jointType
                    float X = Convert.ToSingle(words[8]); //unitySpacePoint.X
                    float Y = Convert.ToSingle(words[9]); //unitySpacePoint.Y
                    float Z = 15; //unitySpacePoint.Z


                    if (jointNumber > Convert.ToInt32(words[5]))
                    {
                        //fill the end with (0, 0)
                        while (joints.Count < 25)
                        {
                            //will with (0, 0)
                            joints.Add(new Vector3D(0, 0, 0));
                        }
                        //set the list to the frame
                        currentFrame.Joints = joints;
                        //set the teaching percent
                        currentFrame.ImportanceInPercent = percents;
                        //determine bones
                        currentFrame.DetermineBones();
                        //put in a list
                        Skeletons.Add(currentFrame);
                        //create new frame
                        currentFrame = new Skeleton();
                        percents = new List<double>();
                        joints = new List<Vector3D>();

                    }
                    // skeleton joint type in number
                    jointNumber = Convert.ToInt32(words[5]);
                    while (joints.Count < jointNumber)
                    {
                        //will with (0, 0)
                        joints.Add(new Vector3D(0, 0, 0));
                    }
                    joints.Add(new Vector3D(X, Y, Z));

                    //if is in a file the teaching percent
                    if (words.Length > 12 && words[12] != "")
                    {
                        percents.Add(Convert.ToDouble(words[12]));
                    }

                }
                if (Skeletons.Count == 0)
                {
                    //set the list to the frame
                    currentFrame.Joints = joints;
                    //set the teaching percent
                    currentFrame.ImportanceInPercent = percents;
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

        /// <summary>
        /// Process skeleton data from a file
        /// </summary>
        /// <param name="pathName"></param>
        /// <returns></returns>
        public static List<Tuple<JointType, JointType, double, double>> ProcessSkeletonAngelsFromFile(string pathName)
        {
            List<Tuple<JointType, JointType, double, double>> Angles = new List<Tuple<JointType, JointType, double, double>>();
            string line = ""; // A line in the file
            char[] separators = { ' ' };
            string[] pathSplit = pathName.Split('\\');
            // Just file name
            if (pathSplit[pathSplit.Length - 1] != ".txt")
            {
                StreamReader file;
                if (File.Exists(pathName))
                {
                    file = new StreamReader(pathName);
                }
                else
                {
                    return null;
                }

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


                    //2 joint type, whitch beetween is te angle
                    JointType jointType1 = GetJointType(Convert.ToInt32(words[0]));
                    JointType jointType2 = GetJointType(Convert.ToInt32(words[1]));

                    double angle = Convert.ToDouble(words[2]);

                    

                    //if is in a file the teaching percent
                    if (words.Length > 3)
                    {
                        Angles.Add(new Tuple<JointType, JointType, double, double>(jointType1, jointType2, angle, Convert.ToDouble(words[3])));
                    }
                    else
                    {
                        Angles.Add(new Tuple<JointType, JointType, double, double>(jointType1, jointType2, angle, 0));
                    }

                }
                file.Close();
                return Angles;


            }
            return null;
        }


        /// <summary>
        /// Print skeletons in a file
        /// </summary>
        /// <param name="skeletonList"></param>
        /// <param name="path"></param>
        public static void SkeletonPrint(List<Skeleton> skeletonList, string path)
        {
            int jointCount = 25;
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write("               JointType               X                    Y  WidthOfDisplay  HeightOfDisplay" + Environment.NewLine);

            foreach (Skeleton skeleton in skeletonList)
            {
                for (int i = 0; i < jointCount; ++i)
                {
                    double X = skeleton.Joints[i].X;
                    double Y = skeleton.Joints[i].Y;
                    double Z = skeleton.Joints[i].Z;
                    streamWriter.Write("0 0 0 0 0 " + i + " 0 0 " + X + " " + Y + " 512 424" + Environment.NewLine);
                }
            }
            streamWriter.Close();
        }
        /// <summary>
        /// Print skeleton's angels in a file
        /// </summary>
        /// <param name="skeletonList"></param>
        /// <param name="path"></param>
        public static void SkeletonAnglePrint(List<Skeleton> skeletonList, string path)
        {
            int angelCount = 23;
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write("First JointType Second JointType  Angel" + Environment.NewLine);

            foreach (Skeleton skeleton in skeletonList)
            {
                for (int i = 0; i < angelCount; ++i)
                {
                    JointType first = skeleton.AngleList[i].Item1;
                    JointType second = skeleton.AngleList[i].Item2;
                    double angle = skeleton.AngleList[i].Item3;
                    streamWriter.Write((int)first + " " + (int)second + " " + angle + Environment.NewLine);
                }
            }
            streamWriter.Close();
        }
        /// <summary>
        /// Print skeletons in a file
        /// </summary>
        /// <param name="skeletonList"></param>
        /// <param name="path"></param>
        public static void ScatterPrint(List<Skeleton> skeletonList, string path)
        {
            int jointCount = 25;
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write("               JointType               X                    Y  WidthOfDisplay  HeightOfDisplay    IsImportant?(%)" + Environment.NewLine);

            for (int i = 0; i < skeletonList.Count; ++i)
            {
                for (int j = 0; j < jointCount; ++j)
                {
                    double X = skeletonList[i].Joints[j].X;
                    double Y = skeletonList[i].Joints[j].Y;
                    double Z = skeletonList[i].Joints[j].Z;
                    streamWriter.Write("0 0 0 0 0 " + j + " 0 0 " + X + " " + Y + " 512 424 " + skeletonList[i].ImportanceInPercent[j] + Environment.NewLine);
                }
            }
            streamWriter.Close();
        }
        /// <summary>
        /// Print skeleton's angels in a file
        /// </summary>
        /// <param name="skeletonList"></param>
        /// <param name="path"></param>
        public static void ScatterAnglePrint(List<Skeleton> skeletonList, string path)
        {
            int angelCount = 23;
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.Write("First JointType Second JointType  Angle   IsImportant?(%)" + Environment.NewLine);

            for(int i =0; i < skeletonList.Count; ++i)
            {
                for (int j = 0; j < angelCount; ++j)
                {
                    JointType first = skeletonList[i].AngleList[j].Item1;
                    JointType second = skeletonList[i].AngleList[j].Item2;
                    double angle = skeletonList[i].AngleList[j].Item3;
                    streamWriter.Write((int)first + " " + (int)second + " " + angle + " " + skeletonList[i].ImportanceAngleInPercent[j] + Environment.NewLine);
                }
            }
            streamWriter.Close();
        }
        /// <summary>
        /// Get and set the skeleton's joints list
        /// </summary>
        public List<Vector3D> Joints
        {
            get
            {
                return joints;
            }

            set
            {
                joints = value;
            }
        }
        /// <summary>
        /// Get and set the skeleton's joints bones list
        /// </summary>
        public List<Tuple<JointType, JointType, bool>> Bones
        {
            get
            {
                return bones;
            }

            set
            {
                bones = value;
            }
        }
        /// <summary>
        /// Get and set the skeleton's angles list
        /// </summary>
        public List<Tuple<JointType, JointType, double>> AngleList
        {
            get
            {
                return angleList;
            }

            set
            {
                angleList = value;
            }
        }
        /// <summary>
        /// Get and set the important joint list
        /// </summary>
        public List<double> ImportanceInPercent
        {
            get
            {
                return importanceInPercent;
            }

            set
            {
                importanceInPercent = value;
            }
        }
        /// <summary>
        /// Get and set the important angle list
        /// </summary>
        public List<double> ImportanceAngleInPercent
        {
            get
            {
                return importanceAngleInPercent;
            }

            set
            {
                importanceAngleInPercent = value;
            }
        }

        /// <summary>
        /// Get the joint type in format JointType
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static JointType GetJointType(int jointTypeInt)
        {
            switch (jointTypeInt)
            {
                case 0: return JointType.SpineBase;
                case 1: return JointType.SpineMid;
                case 2: return JointType.Neck;
                case 3: return JointType.Head;
                case 4: return JointType.ShoulderLeft;
                case 5: return JointType.ElbowLeft;
                case 6: return JointType.WristLeft;
                case 7: return JointType.HandLeft;
                case 8: return JointType.ShoulderRight;
                case 9: return JointType.ElbowRight;
                case 10: return JointType.WristRight;
                case 11: return JointType.HandRight;
                case 12: return JointType.HipLeft;
                case 13: return JointType.KneeLeft;
                case 14: return JointType.AnkleLeft;
                case 15: return JointType.FootLeft;
                case 16: return JointType.HipRight;
                case 17: return JointType.KneeRight;
                case 18: return JointType.AnkleRight;
                case 19: return JointType.FootRight;
                case 20: return JointType.SpineShoulder;
                case 21: return JointType.HandTipLeft;
                case 22: return JointType.ThumbLeft;
                case 23: return JointType.HandTipRight;
                case 24: return JointType.ThumbRight;
                default: return JointType.SpineBase;
            }
        }

        
    }
}
