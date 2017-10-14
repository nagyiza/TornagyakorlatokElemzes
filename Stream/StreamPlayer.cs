using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stream
{
    public class StreamPlayer
    {
        public List<SkeletonFrame> skeletonFrames;
        public int frameCount;
        public Capture video = null;
        public Mat playbackFrame = null;
        
        public delegate void EventHandlerNewFrame(Mat image);
        public event EventHandlerNewFrame NewFrame;

        //color frame size (not video size)
        public float colorDisplayHeight = ColorStream.ColorDisplayHeight;
        public float colorDisplayWidth = ColorStream.ColorDisplayWidth;

        //video (full hd) size
        private float VideoHeight = 1080;
        private float VideoWidth = 1920;
        /// <summary>
        /// Coordinate mapper to map one type of point to another
        /// </summary>
        private CoordinateMapper coordinateMapper = null;

        public StreamPlayer()
        {

        }

        public void StartPlayback(string path)
        {
            skeletonFrames = new List<SkeletonFrame>();
            //construct the frames for the skeleton playback
            ProcessSkeletonData(path + ".txt");

            //video start
            video = new Capture(path + ".avi");
            if (video != null)
            {
                playbackFrame = new Mat();
                video.ImageGrabbed += ProcessPlaybackFrame;//event 
                video.Start();
            }

        }

        /// <summary>
        /// Event for video playing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        public void ProcessPlaybackFrame(object sender, EventArgs arg)
        {
            if (video != null)
            {
                if (video.Ptr != IntPtr.Zero)
                {
                    //next
                    video.Retrieve(playbackFrame, 3);
                    //data processing
                    if (NewFrame != null)
                    {
                        NewFrame(playbackFrame);
                    }
                }
                
            }
        }

        /// <summary>
        /// This method process the skeleton data (from the file)
        /// </summary>
        /// <param name="path"></param>
        private void ProcessSkeletonData(string path)
        {
            string line; // A line in the file
            char[] separators = { ' ' };
            string[] pathSplit = path.Split('\\');
            // Just file name
            if (pathSplit[pathSplit.Length-1] != ".txt")
            {
                StreamReader file = new StreamReader(path);
                SkeletonFrame currentFrame = new SkeletonFrame();
                int jointNumber = 0;
                List<Tuple<double, double>> joints = new List<Tuple<double, double>>();
                //read the first line, because it is the bill head and this not need it
                line = file.ReadLine();
                //read the skeleton data
                while ((line = file.ReadLine()) != null)
                {
                    // split the data 
                    string[] words = line.Split(separators); 
                    // in one line is 10 data from skeleton
                    if (words.Length < 10)
                    {
                        // skeleton has 25 joints data
                        while (joints.Count < 25)
                        {
                            joints.Add(new Tuple<double, double>(0, 0));
                        }
                        //set the list to the frame
                        currentFrame.Joints = joints;
                        //determine bones
                        currentFrame.DetermineBones();
                        //save the last frame
                        skeletonFrames.Add(currentFrame);
                        break;
                    }

                    if (currentFrame.Timestamp == 0) currentFrame.Timestamp = Convert.ToInt64(words[3]);

                    int jointTypeNr = Convert.ToInt32(words[5]); //(int)jointType
                    double X = Convert.ToDouble(words[6]); //colorSpacePoint.X
                    double Y = Convert.ToDouble(words[7]); //colorSpacePoint.Y

                    double skeletonWidth = Convert.ToDouble(words[10]); //skeleton display Width
                    double skeletonHeight = Convert.ToDouble(words[11]); //skeleton display Height

                    // display size -> skeleton size; colorstream size -> the color frame size

                    
                    X = X * skeletonWidth / VideoWidth; // (512 / 1920)
                    Y = Y * skeletonWidth / VideoWidth;// (512 / 1920)


                    if (jointNumber > Convert.ToInt32(words[5]))
                    {
                        //fill the end with (0, 0)
                        while (joints.Count < 25)
                        {
                            //will with (0, 0)
                            joints.Add(new Tuple<double, double>(0, 0));
                        }
                        //set the list to the frame
                        currentFrame.Joints = joints;
                        //determine bones
                        currentFrame.DetermineBones();
                        //put in a list
                        skeletonFrames.Add(currentFrame);
                        //create new frame
                        currentFrame = new SkeletonFrame();
                        joints = new List<Tuple<double, double>>();

                        currentFrame.Timestamp = Convert.ToInt64(words[3]); //timestamp
                    }
                    // skeleton joint type in number
                    jointNumber = Convert.ToInt32(words[5]);
                    while (joints.Count < jointNumber)
                    {
                        //will with (0, 0)
                        joints.Add(new Tuple<double, double>(0, 0));
                    }
                    joints.Add(new Tuple<double, double>(X, Y));
                }
                //frame constructions are finished
                //starting frame
                frameCount = 0;

                file.Close();
            }
        }

        public void StopPlayback()
        {
            if (video != null)
            {
                video.ImageGrabbed -= ProcessPlaybackFrame;
                video.Stop();
                video.Dispose();
                video = null;


            }
        }

        public int FrameCount
        {
            get
            {
                return frameCount;
            }

            set
            {
                frameCount = value;
            }
        }

        internal List<SkeletonFrame> SkeletonFrames
        {
            get
            {
                return skeletonFrames;
            }

            set
            {
                skeletonFrames = value;
            }
        }
    }
}
