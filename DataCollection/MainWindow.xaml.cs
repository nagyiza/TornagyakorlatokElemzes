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
        KinectSensor sensor = null;
        ColorStream colorStream = null;

        SkeletonStream skeletonStream = null;

        DrawingImage drawingImage;
        DrawingGroup drawingGroup;

        StreamRecorder streamRecorder = null;
        StreamPlayer streamPlayer = null;
        public string path = "..\\..\\..\\ReferenceData\\";

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = this; //data binding
            colorStreamView.DataContext = colorStream;
            skeletonStreamView.DataContext = skeletonStream;

            drawingGroup = new DrawingGroup();
            drawingImage = new DrawingImage(drawingGroup);

            skeletonReplayImage.Source = drawingImage;
        }

        private void RecordButton_Click(object sender, RoutedEventArgs e)
        {
            ControlKeyEnable(false, false, true);

            string file = Interaction.InputBox("Write the exercise name!", "Exercise save", "");
            bool ckecking = CheckPath(file);
            while (!ckecking) // false
            {
                file = Interaction.InputBox("Write the exercise name!", "Exercise save", "");
                ckecking = CheckPath(file);
            }

            streamRecorder.StartRecord(path);
        }

        private bool CheckPath(string file)
        {
            if (file == null || file == "")
            {
                MessageBox.Show("The exercise name is empty");
                return false;
            }
            else
            {
                path = path + file;
            }
            
            //existing file
            if (File.Exists(path + ".avi") == true || File.Exists(path + ".txt") == true)
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

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (colorStream.IsRecording || skeletonStream.IsRecording)
            {
                streamRecorder.StopRecording();
            }
            else
            {
                streamPlayer.StopPlayback();
            }
            ControlKeyEnable(true, true, false);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            ControlKeyEnable(false, false, true);

            OpenFileDialog playing = new OpenFileDialog();
            playing.Filter = "All files(*.*)|*.*";
            playing.FilterIndex = 2;
            playing.RestoreDirectory = true;
            if (playing.ShowDialog() == true)
            {
                // file name 
                string[] split = playing.FileName.Split('\\');
                split = split[split.Length - 1].Split('.');
                path = "..\\..\\..\\ReferenceData\\" + split[0]; // ReferenceData\filename
            }
            string[] pathSplit = path.Split('\\');
            if (pathSplit[pathSplit.Length - 1] != null && pathSplit[pathSplit.Length - 1] != "")
            {
                streamPlayer.StartPlayback(path);
            }else
            { 
                ControlKeyEnable(true, true, false);
            }
        }

        private void ControlKeyEnable(bool rec, bool play, bool stop)
        {
            btnRecord.IsEnabled = rec;
            btnPlayback.IsEnabled = play;
            btnStop.IsEnabled = stop;
        }

        private void Window_Closed(object sender, EventArgs e)
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
