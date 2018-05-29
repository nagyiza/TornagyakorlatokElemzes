using System;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System.Linq;
using System.Text;
using UnityEngine;
using System.IO;

namespace Assets.KinectView.Scripts
{

    public class Skeleton
    {
        public Dictionary<Kinect.JointType, Kinect.JointType> bones = new Dictionary<Kinect.JointType, Kinect.JointType>();
        public Dictionary<Kinect.JointType, bool> joinIsValid = new Dictionary<Kinect.JointType, bool>();
        private List<Vector3> joints;
        private List<Vector4> angleList; // JointType, JointType, double

        /// <summary>
        /// Constructor, in witch create the bones
        /// </summary>
        public Skeleton()
        {
            bones = new Dictionary<Kinect.JointType, Kinect.JointType>()
            {
                 { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
        { Kinect.JointType.Head, Kinect.JointType.Neck },

            };


            joinIsValid = new Dictionary<Kinect.JointType, bool>()
            {
                 { Kinect.JointType.FootLeft, false },
        { Kinect.JointType.AnkleLeft, false },
        { Kinect.JointType.KneeLeft, false },
        { Kinect.JointType.HipLeft, false },

        { Kinect.JointType.FootRight, false },
        { Kinect.JointType.AnkleRight,false },
        { Kinect.JointType.KneeRight, false },
        { Kinect.JointType.HipRight, false },

        { Kinect.JointType.HandTipLeft, false },
        { Kinect.JointType.ThumbLeft, false },
        { Kinect.JointType.HandLeft, false },
        { Kinect.JointType.WristLeft, false },
        { Kinect.JointType.ElbowLeft, false },
        { Kinect.JointType.ShoulderLeft,false },

        { Kinect.JointType.HandTipRight, false },
        { Kinect.JointType.ThumbRight, false },
        { Kinect.JointType.HandRight, false },
        { Kinect.JointType.WristRight, false },
        { Kinect.JointType.ElbowRight, false},
        { Kinect.JointType.ShoulderRight,false },

        { Kinect.JointType.SpineBase, false},
        { Kinect.JointType.SpineMid,false },
        { Kinect.JointType.SpineShoulder,false },
        { Kinect.JointType.Neck, false },
        { Kinect.JointType.Head, false },

            };

        }

        /// <summary>
        /// Determine bones
        /// </summary>
        public void DetermineBones()
        {
            /*for (int index = 0; index < bones.Count; index++)
            {
                if ((joints[(int)(bones[index].Item1)].X != 0) && joints[(int)(bones[index].Item2)].X != 0)
                {
                    //the bone is valid
                    bones[index] = new Tuple<JointType, JointType, bool>(bones[index].Item1, bones[index].Item2, true);
                }
            }*/
        }

        /// <summary>
        /// Calculate and save the angles in a list
        /// </summary>
        public void SkeletonAngle()
        {
            angleList = new List<Vector4>();
            for (int i = 0; i < 24; ++i)
            {
                var bone = bones[getJoinType(i)];

                Kinect.JointType first = getJoinType(i);
                Kinect.JointType center = bone;
                Kinect.JointType second = bones[center];

                //if the bones (first-center, center-second) is tracked 
                if (joinIsValid[first] && joinIsValid[second] && joinIsValid[center])
                {
                    // three vector
                    Vector3 firstVector = joints[(int)(first)];
                    Vector3 centerVector = joints[(int)(center)];
                    Vector3 secondVector = joints[(int)(second)];
                    //calculate angle
                    float angle = AngleBetweenTwoVectors(firstVector, centerVector, secondVector);
                    //Debug.Log("angle: "+angle);
                    //save angels in the list
                    angleList.Add(new Vector3((int)first, (int)second, angle));
                }
                else
                {
                    //save angels in the list
                    //if the bones are not tracked, the angle going to be -1
                    angleList.Add(new Vector3((int)first, (int)second, -1));
                }



            }
        }

        /// <summary>
        /// Get joinType string
        /// </summary>
        /// <param name="jointType"></param>
        /// <returns></returns>
        public Kinect.JointType getJoinType(int jointType)
        {
            switch (jointType)
            {
                case 0: return Windows.Kinect.JointType.SpineBase;
                case 1: return Windows.Kinect.JointType.SpineMid;
                case 2: return Windows.Kinect.JointType.Neck;
                case 3: return Windows.Kinect.JointType.Head;
                case 4: return Windows.Kinect.JointType.ShoulderLeft;
                case 5: return Windows.Kinect.JointType.ElbowLeft;
                case 6: return Windows.Kinect.JointType.WristLeft;
                case 7: return Windows.Kinect.JointType.HandLeft;
                case 8: return Windows.Kinect.JointType.ShoulderRight;
                case 9: return Windows.Kinect.JointType.ElbowRight;
                case 10: return Windows.Kinect.JointType.WristRight;
                case 11: return Windows.Kinect.JointType.HandRight;
                case 12: return Windows.Kinect.JointType.HipLeft;
                case 13: return Windows.Kinect.JointType.KneeLeft;
                case 14: return Windows.Kinect.JointType.AnkleLeft;
                case 15: return Windows.Kinect.JointType.FootLeft;
                case 16: return Windows.Kinect.JointType.HipRight;
                case 17: return Windows.Kinect.JointType.KneeRight;
                case 18: return Windows.Kinect.JointType.AnkleRight;
                case 19: return Windows.Kinect.JointType.FootRight;
                case 20: return Windows.Kinect.JointType.SpineShoulder;
                case 21: return Windows.Kinect.JointType.HandTipLeft;
                case 22: return Windows.Kinect.JointType.ThumbLeft;
                case 23: return Windows.Kinect.JointType.HandTipRight;
                case 24: return Windows.Kinect.JointType.ThumbRight;
                default: return Kinect.JointType.SpineBase;
            }
        }

        /// <summary>
        /// Calculate the angels beetween 2 joints
        /// </summary>
        /// <param name="a"> First joint's coordinate</param>
        /// <param name="b"> Have to be origo </param>
        /// <param name="c"> Second joint's coordinate</param>
        /// <returns> Return the angles beetween a and c points</returns>
        public float AngleBetweenTwoVectors(Vector3 a, Vector3 b, Vector3 c)
        {
            //b going to be origo
            a.x = a.x - b.x;
            a.y = a.y - b.y;
            a.z = a.z - b.z;

            c.x = c.x - b.x;
            c.y = c.y - b.y;
            c.z = c.z - b.z;

            double dotProduct;
            a.Normalize();
            c.Normalize();
            dotProduct = a.x * c.x + a.y * c.y + a.z * c.z;

            float result = Convert.ToSingle(Math.Acos(dotProduct) / Math.PI * 180);
            return result;
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
            List<double> percents = new List<double>();
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
                List<Vector3> joints = new List<Vector3>();
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
                            joints.Add(new Vector3(0, 0, 0));
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
                            joints.Add(new Vector3(0, 0, 0));
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
                        joints = new List<Vector3>();

                    }
                    // skeleton joint type in number
                    jointNumber = Convert.ToInt32(words[5]);
                    while (joints.Count < jointNumber)
                    {
                        //will with (0, 0)
                        joints.Add(new Vector3(0, 0, 0));
                    }
                    joints.Add(new Vector3(X, Y, Z));

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
        public static List<Vector4> ProcessSkeletonAngelsFromFile(string pathName)
        {
            //jointtype - jointtype - angle - percent
            List<Vector4> Angles = new List<Vector4>();
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
                    Kinect.JointType jointType1 = GetJointType(Convert.ToInt32(words[0]));
                    Kinect.JointType jointType2 = GetJointType(Convert.ToInt32(words[1]));

                    double angle = Convert.ToDouble(words[2]);



                    //if is in a file the teaching percent
                    if (words.Length > 3)
                    {
                        Vector4 ang = new Vector4();
                        ang.x = (int)jointType1;
                        ang.y = (int)jointType2;
                        ang.z = (float)angle;
                        ang.w = Convert.ToSingle(words[3]);
                        Angles.Add(ang);
                    }
                    else
                    {
                        Vector4 ang = new Vector4();
                        ang.x = (int)jointType1;
                        ang.y = (int)jointType2;
                        ang.z = (float)angle;
                        ang.w = 0;
                        Angles.Add(ang);
                    }

                }
                file.Close();
                return Angles;


            }
            return null;
        }
        /// <summary>
        /// Get and set the skeleton's joints list
        /// </summary>
        public List<Vector3> Joints
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
        /// Get and set the skeleton's angles list
        /// </summary>
        public List<Vector4> AngleList
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
        /// List of important joint in percent
        /// </summary>
        private List<double> importanceInPercent;
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
        /// Get the joint type in format JointType
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private static Kinect.JointType GetJointType(int jointTypeInt)
        {
            switch (jointTypeInt)
            {
                case 0: return Kinect.JointType.SpineBase;
                case 1: return Kinect.JointType.SpineMid;
                case 2: return Kinect.JointType.Neck;
                case 3: return Kinect.JointType.Head;
                case 4: return Kinect.JointType.ShoulderLeft;
                case 5: return Kinect.JointType.ElbowLeft;
                case 6: return Kinect.JointType.WristLeft;
                case 7: return Kinect.JointType.HandLeft;
                case 8: return Kinect.JointType.ShoulderRight;
                case 9: return Kinect.JointType.ElbowRight;
                case 10: return Kinect.JointType.WristRight;
                case 11: return Kinect.JointType.HandRight;
                case 12: return Kinect.JointType.HipLeft;
                case 13: return Kinect.JointType.KneeLeft;
                case 14: return Kinect.JointType.AnkleLeft;
                case 15: return Kinect.JointType.FootLeft;
                case 16: return Kinect.JointType.HipRight;
                case 17: return Kinect.JointType.KneeRight;
                case 18: return Kinect.JointType.AnkleRight;
                case 19: return Kinect.JointType.FootRight;
                case 20: return Kinect.JointType.SpineShoulder;
                case 21: return Kinect.JointType.HandTipLeft;
                case 22: return Kinect.JointType.ThumbLeft;
                case 23: return Kinect.JointType.HandTipRight;
                case 24: return Kinect.JointType.ThumbRight;
                default: return Kinect.JointType.SpineBase;
            }
        }
    }
}
