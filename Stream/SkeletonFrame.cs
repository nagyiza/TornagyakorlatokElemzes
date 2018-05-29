using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Stream
{
    public class SkeletonFrame
    {
        private long timestamp = 0;
        private List<Tuple<JointType, JointType, bool>> bones;
        private List<Tuple<double, double>> joints;

        public SkeletonFrame()
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
                if ((joints[(int)(bones[index].Item1)].Item1 != 0) && joints[(int)(bones[index].Item2)].Item1 != 0)
                {
                    //the bone is valid
                    bones[index] = new Tuple<JointType, JointType, bool>(bones[index].Item1, bones[index].Item2, true);
                }
            }
        }

        public void Play(DrawingGroup drawingGroup)
        {
            //draw                
            DrawingContext drawingContext = drawingGroup.Open();
            drawingContext.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, 512, 424));
            Pen drawPen = new Pen(Brushes.Green, 3);
            //bones
            foreach (var bone in bones)
            {
                if (bone.Item3)
                {
                    Point begin = new Point(joints[(int)bone.Item1].Item1, joints[(int)bone.Item1].Item2);
                    Point end = new Point(joints[(int)bone.Item2].Item1, joints[(int)bone.Item2].Item2);
                    drawingContext.DrawLine(drawPen, begin, end);
                }
            }
            //joints
            foreach (var joint in joints)
            {
                if (joint.Item1 != 0)
                {
                    drawingContext.DrawEllipse(Brushes.LightGreen, null, new Point(joint.Item1, joint.Item2), 3, 3);
                }
            }
            drawingContext.Close();
        }

        public long Timestamp
        {
            get
            {
                return timestamp;
            }

            set
            {
                timestamp = value;
            }
        }

        public List<Tuple<double, double>> Joints
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
    }
}
