﻿using Microsoft.Kinect;
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
        private List<Tuple<JointType, JointType, bool>> bones;
        private List<Vector3D> joints;
        private List<Tuple<JointType, JointType, double>> angleList;

        public Skeleton()
        {
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

        public void SkeletonAngle()
        {
            angleList = new List<Tuple<JointType, JointType, double>>();
            for (int i = 0; i < bones.Count; ++i)
            {
                var bone = bones[i];
                //if (bone.Item3) {
               
                JointType first = bone.Item1;
                JointType center = bone.Item2;
                for (int j = i + 1; j < bones.Count; ++j)
                {
                    if (bones[j].Item1 == center)
                    {
                        JointType second = bones[j].Item2;
                        if (bone.Item3 && bones[j].Item3)
                        {
                            // three vector
                            Vector3D firstVector = joints[(int)(bone.Item1)];
                            Vector3D centerVector = joints[(int)(bone.Item2)];
                            Vector3D secondVector = joints[(int)(bones[j].Item2)];
                            //calculate angle
                            double angle = AngleBetweenTwoVectors(firstVector, centerVector, secondVector);
                            angleList.Add(new Tuple<JointType, JointType, double>(first, second, angle));
                        }
                        else
                        {
                            angleList.Add(new Tuple<JointType, JointType, double>(first, second, -1));
                        }

                    }
                }
                //}
            }
        }
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
    }
}