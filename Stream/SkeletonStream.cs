using DataCollectionInterface;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

// In this class I used the example code of Kinect SDK 2.0

namespace Stream
{
    public class SkeletonStream : IDataCollection<BodyFrameArrivedEventArgs>
    {
        /// <summary>
        /// Radius of drawn hand circles
        /// </summary>
        private const double HandSize = 30;
        /// <summary>
        /// Thickness of drawn joint lines
        /// </summary>
        private const double JointThickness = 3;
        /// <summary>
        /// Thickness of clip edge rectangles
        /// </summary>
        private const double ClipBoundsThickness = 10;
        /// <summary>
        /// Constant for clamping Z values of camera space points from being negative
        /// </summary>
        private const float InferredZPositionClamp = 0.1f;

        /// <summary>
        /// Brush used for drawing joints that are currently tracked
        /// </summary>
        private readonly Brush trackedJointBrush = new SolidColorBrush(Color.FromArgb(255, 68, 192, 68));

        /// <summary>
        /// Brush used for drawing joints that are currently inferred
        /// </summary>        
        private readonly Brush inferredJointBrush = Brushes.Yellow;

        /// <summary>
        /// Brush used for drawing joints that are not tracked
        /// </summary>        
        private readonly Brush notTrackedJointBrush = Brushes.Red;

        /// <summary>
        /// Pen used for drawing bones that are currently inferred
        /// </summary>        
        private readonly Pen inferredBonePen = new Pen(Brushes.Red, 6);
        /// <summary>
        /// Drawing group for body rendering output
        /// </summary>
        private DrawingGroup drawingGroup;
        /// <summary>
        /// Drawing image that we will display
        /// </summary>
        private DrawingImage imageSource;
        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;
        /// <summary>
        /// Reader for body frames
        /// </summary>
        private BodyFrameReader bodyFrameReader = null;

        /// <summary>
        /// Array for the bodies
        /// </summary>
        private Body[] bodies = null;

        /// <summary>
        /// definition of bones
        /// </summary>
        private List<Tuple<JointType, JointType>> bones;
        /// <summary>
        /// Width of sekeleton stream  (depth space)
        /// </summary>
        private int SkeletonStreamWidth;

        /// <summary>
        /// Height of sekeleton stream (depth space)
        /// </summary>
        private int SkeletonStreamHeight;

        /// <summary>
        /// List of colors for each body tracked
        /// </summary>
        private List<Pen> bodyColors;

        private Pen drawPen = new Pen(Brushes.Green, 6);

        /// <summary>
        /// init current skeleton frame data
        /// </summary>
        private string currentSkeletonFrameData;
        /// <summary>
        /// init current skeleton frame data if it is inferred or not tracked
        /// </summary>
        private string currentSkeletonFrameData2;
        private bool isRecording = false;

        public SkeletonStream() { }
        public void Stream(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                // get the coordinate mapper
                this.coordinateMapper = kinectSensor.CoordinateMapper;

                // get the depth (display) extents
                FrameDescription frameDescription = kinectSensor.DepthFrameSource.FrameDescription;

                // get size of joint space (skeleton frame size)
                this.SkeletonStreamWidth = frameDescription.Width;
                this.SkeletonStreamHeight = frameDescription.Height;

                // open the reader for the body frames
                this.bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();

                // a bone defined as a line between two joints
                this.bones = BonesForSkeleton();

                // Create the drawing group we'll use for drawing
                this.drawingGroup = new DrawingGroup();

                // Create an image source that we can use in our image control
                this.imageSource = new DrawingImage(this.drawingGroup);
                // wire handler for frame arrival
                if (this.bodyFrameReader != null)
                {
                    this.bodyFrameReader.FrameArrived += this.FrameArrived;
                }
            }
            else
            {
                throw new ArgumentNullException("kinectSensor", "The parameter is null!");
            }

        }
        /// <summary>
        /// Create bones for skeleton
        /// </summary>
        /// <returns></returns>
        private List<Tuple<JointType, JointType>> BonesForSkeleton()
        {
            List<Tuple<JointType, JointType>> bones = new List<Tuple<JointType, JointType>>();
            //torso
            bones.Add(new Tuple<JointType, JointType>(JointType.Head, JointType.Neck));
            bones.Add(new Tuple<JointType, JointType>(JointType.Neck, JointType.SpineShoulder));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.SpineMid));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineMid, JointType.SpineBase));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineShoulder, JointType.ShoulderLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.SpineBase, JointType.HipLeft));
            //rightArm
            bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderRight, JointType.ElbowRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.ElbowRight, JointType.WristRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.HandRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.HandRight, JointType.HandTipRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristRight, JointType.ThumbRight));
            //leftArm
            bones.Add(new Tuple<JointType, JointType>(JointType.ShoulderLeft, JointType.ElbowLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.ElbowLeft, JointType.WristLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.HandLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.HandLeft, JointType.HandTipLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.WristLeft, JointType.ThumbLeft));
            //rightLeg
            bones.Add(new Tuple<JointType, JointType>(JointType.HipRight, JointType.KneeRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.KneeRight, JointType.AnkleRight));
            bones.Add(new Tuple<JointType, JointType>(JointType.AnkleRight, JointType.FootRight));
            //leftLeg
            bones.Add(new Tuple<JointType, JointType>(JointType.HipLeft, JointType.KneeLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.KneeLeft, JointType.AnkleLeft));
            bones.Add(new Tuple<JointType, JointType>(JointType.AnkleLeft, JointType.FootLeft));
            return bones;
        }
        /// <summary>
        /// Handles the body frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;
            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (this.bodies == null)
                    {
                        this.bodies = new Body[bodyFrame.BodyCount];
                    }
                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(this.bodies);
                    dataReceived = true;
                }
            }
            //do the drawing
            if (dataReceived)
            {
                ProcessData();
            }
        }
        private void ProcessData()
        {
            using (DrawingContext drawingContext = drawingGroup.Open())
            {
                //draw a black background
                drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, SkeletonStreamWidth, SkeletonStreamHeight));
                foreach (Body body in bodies)
                {
                    if (body.IsTracked)
                    {
                        IReadOnlyDictionary<JointType, Joint> joints = body.Joints;
                        Dictionary<JointType, Point> jointPoints = new Dictionary<JointType, Point>();
                        long timestamp = (long)(DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond); // timestamp for saveing
                        currentSkeletonFrameData = ""; //init current skeleton frame data
                        currentSkeletonFrameData2 = ""; //init current skeleton frame data if it is inferred
                        foreach (JointType jointType in joints.Keys)
                        {
                            CameraSpacePoint position = joints[jointType].Position;
                            if (position.Z < 0)
                            {
                                position.Z = 0.1f;
                            }
                            DepthSpacePoint depthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(position);
                            ColorSpacePoint colorSpacePoint = coordinateMapper.MapCameraPointToColorSpace(position);

                            jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y); // to drawing

                            //coordinate from cordinate mapping
                            //source: http://pterneas.com/2014/05/06/understanding-kinect-coordinate-mapping/
                            float ColorX = float.IsInfinity(colorSpacePoint.X) ? 0 : colorSpacePoint.X;
                            float ColorY = float.IsInfinity(colorSpacePoint.Y) ? 0 : colorSpacePoint.Y;

                            float DepthX = float.IsInfinity(depthSpacePoint.X) ? 0 : depthSpacePoint.X;
                            float DepthY = float.IsInfinity(depthSpacePoint.Y) ? 0 : depthSpacePoint.Y;


                            if (joints[jointType].TrackingState == TrackingState.Tracked && isRecording)
                            {
                                {
                                    //fill current skeleton frame data
                                    currentSkeletonFrameData += position.Z + " " + position.X + " " + position.Y + " "
                                        + timestamp + " "
                                        + body.TrackingId + " " + (int)jointType + " " + ColorX + " "
                                        + ColorY + " " + DepthX + " " + DepthY + " " + SkeletonStreamWidth + " "
                                        + SkeletonStreamHeight + " " + Environment.NewLine;
                                }
                            }else if (joints[jointType].TrackingState == TrackingState.Inferred && isRecording)
                            {
                                {
                                    //fill current skeleton frame data
                                    currentSkeletonFrameData2 += position.Z + " " + position.X + " " + position.Y + " "
                                        + timestamp + " "
                                        + body.TrackingId + " " + (int)jointType + " " + ColorX + " "
                                        + ColorY + " " + DepthX + " " + DepthY + " " + SkeletonStreamWidth + " "
                                        + SkeletonStreamHeight + " " + Environment.NewLine;
                                }
                            }
                        }
                        //draw the body
                        DrawBody(joints, jointPoints, drawingContext);
                    }
                }
                // prevent drawing outside of our render area
                drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, SkeletonStreamWidth, SkeletonStreamHeight));
            }

        }
        
        /// <summary>
        /// Draws a body
        /// </summary>
        /// <param name="joints"></param>
        /// <param name="jointPoints"></param>
        /// <param name="drawingContext"></param>
        /// <param name="drawingPen"></param>
        private void DrawBody(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, DrawingContext drawingContext)
        {
            // Draw the bones
            foreach (var bone in this.bones)
            {
                this.DrawBone(joints, jointPoints, bone.Item1, bone.Item2, drawingContext);
            }

            // Draw the joints
            foreach (JointType jointType in joints.Keys)
            {
                Brush drawBrush = null;

                TrackingState trackingState = joints[jointType].TrackingState;

                if (trackingState == TrackingState.Tracked)
                {
                    drawBrush = this.trackedJointBrush; //green
                }
                else if (trackingState == TrackingState.Inferred)
                {
                    drawBrush = this.inferredJointBrush; // yellow
                }
                else if (trackingState == TrackingState.NotTracked)
                {
                    drawBrush = this.notTrackedJointBrush; // red
                }

                if (drawBrush != null)
                {
                    drawingContext.DrawEllipse(drawBrush, null, jointPoints[jointType], JointThickness, JointThickness);
                }
            }
        }

        /// <summary>
        /// Draws one bone of a body (joint to joint)
        /// </summary>
        /// <param name="joints"></param>
        /// <param name="jointPoints"></param>
        /// <param name="jointType0"></param>
        /// <param name="jointType1"></param>
        /// <param name="drawingContext"></param>
        /// <param name="drawingPen"></param>
        private void DrawBone(IReadOnlyDictionary<JointType, Joint> joints, IDictionary<JointType, Point> jointPoints, JointType jointType0, JointType jointType1, DrawingContext drawingContext)
        {
            Joint joint0 = joints[jointType0];
            Joint joint1 = joints[jointType1];
            // We assume all drawn bones are inferred unless BOTH joints are tracked
            Pen drawPen = this.inferredBonePen; // red

            // If we can't find either of these joints, exit
            if (joint0.TrackingState == TrackingState.NotTracked ||
                joint1.TrackingState == TrackingState.NotTracked)
            {
                return;
            }

            //if both joints are tracked the bone is correct
            if ((joint0.TrackingState == TrackingState.Tracked) && (joint1.TrackingState == TrackingState.Tracked))
            {
                drawPen = new Pen(Brushes.Green, 6); //green
            }
            else
            {
                if ((joint0.TrackingState == TrackingState.Inferred) && (joint1.TrackingState == TrackingState.Inferred))
                {
                    drawPen = new Pen(Brushes.Yellow, 6); // yellow
                }
            }

            //draw the bone
            drawingContext.DrawLine(drawPen, jointPoints[jointType0], jointPoints[jointType1]);
        }

        /// <summary>
        /// Draws indicators to show which edges are clipping body data
        /// </summary>
        /// <param name="body">body to draw clipping information for</param>
        /// <param name="drawingContext">drawing context to draw to</param>
        private void DrawClippedEdges(Body body, DrawingContext drawingContext)
        {
            FrameEdges clippedEdges = body.ClippedEdges;

            if (clippedEdges.HasFlag(FrameEdges.Bottom))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, this.SkeletonStreamHeight - ClipBoundsThickness, this.SkeletonStreamWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Top))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, this.SkeletonStreamWidth, ClipBoundsThickness));
            }

            if (clippedEdges.HasFlag(FrameEdges.Left))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(0, 0, ClipBoundsThickness, this.SkeletonStreamHeight));
            }

            if (clippedEdges.HasFlag(FrameEdges.Right))
            {
                drawingContext.DrawRectangle(
                    Brushes.Red,
                    null,
                    new Rect(this.SkeletonStreamWidth - ClipBoundsThickness, 0, ClipBoundsThickness, this.SkeletonStreamHeight));
            }
        }
        public DrawingImage skeletonImage
        {
            get
            {
                return imageSource;
            }
        }

        public bool IsRecording
        {
            get
            {
                return isRecording;
            }

            set
            {
                isRecording = value;
            }
        }

        public string CurrentSkeletonFrameData
        {
            get
            {
                //skeleton data in the current frame
                return currentSkeletonFrameData;
            }
        }
        public string CurrentSkeletonFrameData2
        {
            get
            {
                //skeleton data in the current frame
                return currentSkeletonFrameData2;
            }
        }


        
        public void Dispose()
        {
            if (bodyFrameReader != null)
            {
                bodyFrameReader.FrameArrived -= FrameArrived;
                bodyFrameReader.Dispose();
                bodyFrameReader = null;
            }
        }
    }
}
