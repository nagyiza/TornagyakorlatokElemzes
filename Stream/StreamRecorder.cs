using Emgu.CV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Stream
{
    public class StreamRecorder
    {
        /// <summary>
        /// Skeleton frame
        /// </summary>
        public SkeletonStream skeletonStream;
        /// <summary>
        /// Color frame
        /// </summary>
        public ColorStream colorStream;
        /// <summary>
        /// Event
        /// </summary>
        public ElapsedEventHandler saveEventHandler = null;
        /// <summary>
        /// Write images to video format
        /// </summary>
        public VideoWriter videoWriter;
        /// <summary>
        /// Write correct skeleton data in a file
        /// </summary>
        public StreamWriter streamWriter;
        /// <summary>
        /// Write incorrect skeleton data in a file
        /// </summary>
        public StreamWriter streamWriter2;

        /// <summary>
        /// Timer for saving
        /// </summary>
        public System.Timers.Timer saveTimer;

        /// <summary>
        /// This ensures that one thread does not enter a critical section of code while another thread is in the critical section
        /// </summary>
        public System.Object lockWriter = new System.Object();

        public StreamRecorder(SkeletonStream Skeleton, ColorStream Color)
        {
            skeletonStream = Skeleton;
            colorStream = Color;
        }
        /// <summary>
        /// Start recording method. Create the files (txt and avi).
        /// </summary>
        /// <param name="fileName"></param>
        public void StartRecord(string fileName)
        {
            //skeleton recording start
            skeletonStream.IsRecording = true;
            //color recording start
            colorStream.IsRecording = true;

            //skeleton stream saving (tracked and inferred) in 2 file
            streamWriter = new StreamWriter(fileName + ".txt");
            streamWriter2 = new StreamWriter(fileName + "ErrorFrameData.txt");

            // In file is bill head:
            streamWriter.Write("Z           X           Y     Timestamp     Body id     Joint type ColorPointX ColorPointY  DepthPointX DepthPointY  WidthOfDisplay  HeightOfDisplay" + Environment.NewLine);
            streamWriter2.Write("Z           X            Y     Timestamp     Body id     Joint type ColorPointX ColorPointY  DepthPointX DepthPointY  WidthOfDisplay  HeightOfDisplay" + Environment.NewLine);

            //color stream saving event
            if (saveEventHandler == null) saveEventHandler = new ElapsedEventHandler(Record);

            //Frame rate per second
            int fps = 40;

            if (videoWriter == null)
            {
                //filename
                string saveFileName = fileName + ".avi";
                //The size of the frame
                System.Drawing.Size size = new System.Drawing.Size(1920, 1080); //full HD
                videoWriter = new VideoWriter(saveFileName, VideoWriter.Fourcc('M', 'P', '4', '2'), fps, size, true);
            }

            //Timer            
            saveTimer = new System.Timers.Timer();
            saveTimer.Elapsed += saveEventHandler;
            saveTimer.Interval = 1000 / fps;
            saveTimer.AutoReset = true;
            saveTimer.Enabled = true;
            saveTimer.Start();
        }
        /// <summary>
        /// The event for video record
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Record(object sender, ElapsedEventArgs e)
        {
            lock (lockWriter)
            {
                if (videoWriter != null)
                {
                    // the color frame
                    Mat frame = colorStream.CurrentFrame;
                    string correct = skeletonStream.CurrentSkeletonFrameData;//tracked frame data
                    string notCorrect = skeletonStream.CurrentSkeletonFrameData2;// infared or not tracked frame data
                    try
                    {
                        if (frame != null && correct != null)
                        {
                            //write the color frame
                            videoWriter.Write(frame);
                            //write the tracked skeleton
                            streamWriter.Write(correct);
                        }

                        if (frame != null && notCorrect != null)
                        {
                            //write the not tracked skeleton
                            streamWriter2.Write(notCorrect);
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
        }

        /// <summary>
        /// The record is stop
        /// </summary>
        public void StopRecording()
        {
            lock (lockWriter)
            {
                //Stop timer
                saveTimer.Elapsed -= saveEventHandler;
                saveTimer.Enabled = false;
                //Stop record
                if (colorStream.IsRecording)
                {
                    colorStream.IsRecording = false;
                }
                if (skeletonStream.IsRecording)
                {
                    skeletonStream.IsRecording = false;
                }

                //Stop write
                if (streamWriter != null)
                {
                    streamWriter.Close();
                }
                if (videoWriter != null)
                {
                    videoWriter.Dispose();
                    videoWriter = null;
                }
            }
        }
    }
}
