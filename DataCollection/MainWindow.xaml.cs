using System.Windows;
using System;
using System.Windows.Media;
using Microsoft.Kinect;
using Emgu.CV;
using System.Threading.Tasks;
using Stream;
using Microsoft.Win32;
using Microsoft.VisualBasic;
using System.IO;


namespace ReferenceDataCollection
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IDisposable
    {
        /// <summary>
        /// Kinect sensor
        /// </summary>
        KinectSensor sensor = null;
        /// <summary>
        /// Stream for color frame
        /// </summary>
        ColorStream colorStream = null;

        /// <summary>
        /// Stream for skeleton frame
        /// </summary>
        SkeletonStream skeletonStream = null;
        /// <summary>
        /// Drawing image for skeleton 
        /// </summary>
        DrawingImage drawingImage;
        /// <summary>
        /// Drawing group for skeleton 
        /// </summary>
        DrawingGroup drawingGroup;

        /// <summary>
        /// Record the video and skeleton
        /// </summary>
        StreamRecorder streamRecorder = null;
        /// <summary>
        /// Play the stream by replay button
        /// </summary>
        StreamPlayer streamPlayer = null;
        /// <summary>
        /// Path for data, save the video and skeleton in this path
        /// </summary>
        public string path = "..\\..\\..\\ReferenceData\\";
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            sensor = KinectSensor.GetDefault();
            sensor.Open();
            colorStream = new ColorStream();
            colorStream.Stream(sensor);
            skeletonStream = new SkeletonStream();
            skeletonStream.Stream(sensor);
            streamRecorder = new StreamRecorder(skeletonStream, colorStream);
            streamPlayer = new StreamPlayer();
            streamPlayer.NewFrame += ShowFrameEvent;

            InitializeComponent();
        }
        /// <summary>
        /// When load the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this; //data binding
            colorStreamView.DataContext = colorStream;
            skeletonStreamView.DataContext = skeletonStream;

            drawingGroup = new DrawingGroup();
            drawingImage = new DrawingImage(drawingGroup);

            skeletonReplayImage.Source = drawingImage;
        }
        /// <summary>
        /// Event for record button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {   
            ControlKeyEnable(false, false, true);

            string file = "";
            //check the file
            bool ckecking = false;
            while (!ckecking) // false
            {
                file = Interaction.InputBox("Write the exercise name!", "Exercise save", "");
                ckecking = CheckPath(file);
            }
            //start the recorder
            streamRecorder.StartRecord(path + file);
        }
        /// <summary>
        /// Check the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns>true - the file is correct and false - the file is not correct</returns>
        private bool CheckPath(string file)
        {
            //if file is null or empty, return
            if (file == null || file == "")
            {
                MessageBox.Show("The exercise name is empty");
                return false;
            }
            
            //existing file
            if (File.Exists(path + file + ".avi") == true || File.Exists(path + file + ".txt") == true)
            {
                MessageBoxResult result = MessageBox.Show("The exercise exists! Are you sure you want to replace?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result.ToString() == "Yes")
                {
                    return true;
                }
                else
                {
                    path = "..\\..\\..\\ReferenceData\\";
                    return false;
                }

            }else
            {
                return true;
            }

        }
        /// <summary>
        /// Event for stop button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            //if the recorder is in process
            if (colorStream.IsRecording || skeletonStream.IsRecording)
            {
                //stop the recorder
                streamRecorder.StopRecording();
            }
            else
            {
                //stop the replay
                streamPlayer.StopPlayback();
            }
            
            ControlKeyEnable(true, true, false);
        }
        /// <summary>
        /// Event for play button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ControlKeyEnable(false, false, true);
            //open the file explorer
            OpenFileDialog playing = new OpenFileDialog();
            playing.Filter = "All files(*.*)|*.*";
            playing.FilterIndex = 2;
            playing.RestoreDirectory = true;

            if (playing.ShowDialog() == true)
            {
                //get the file name 
                string[] split = playing.FileName.Split('\\');
                split = split[split.Length - 1].Split('.');
                path = "..\\..\\..\\ReferenceData\\" + split[0]; // ReferenceData\filename
            }
            string[] pathSplit = path.Split('\\');
            if (pathSplit[pathSplit.Length - 1] != null && pathSplit[pathSplit.Length - 1] != "")
            {
                streamPlayer.StartPlayback(path);
            }
            else
            { 
                ControlKeyEnable(true, true, false);
            }

        }
        /// <summary>
        /// Control the button enable
        /// </summary>
        /// <param name="rec"></param>
        /// <param name="play"></param>
        /// <param name="stop"></param>
        private void ControlKeyEnable(bool rec, bool play, bool stop)
        {
            btnRecord.IsEnabled = rec;
            btnPlayback.IsEnabled = play;
            btnStop.IsEnabled = stop;
        }
        /// <summary>
        /// When close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Dispose();
        }
        public void Dispose()
        {
            if (colorStream != null)
            {
                colorStream.Dispose();
                colorStream = null;
            }
            if (skeletonStream != null)
            {
                skeletonStream.Dispose();
                skeletonStream = null;
            }
            if (sensor != null)
            {
                sensor.Close();
                sensor = null;
            }
        }
        /// <summary>
        /// Show the frame in the UI
        /// </summary>
        /// <param name="image"></param>
        public void ShowFrameEvent(Mat image)
        {
            if (image != null)
            {
                try
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (streamPlayer.FrameCount < streamPlayer.skeletonFrames.Count)
                        {
                            //put the color frame on the UI
                            colorPlayback.Source = BitmapSourceConvert.BitmapToBitmapSource(image.Bitmap);
                            //put the skeleton frame on the UI                        
                            streamPlayer.skeletonFrames[streamPlayer.FrameCount].Play(drawingGroup);
                            streamPlayer.FrameCount++;
                        }
                    });
                }
                catch (TaskCanceledException ex)
                {
                    streamPlayer.NewFrame -= ShowFrameEvent;
                }
            }
        }
    }
}
