using Emgu.CV;
using ExerciseAssistantApplication.Common;
using Microsoft.Kinect;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Stream;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;


namespace ExerciseAssistantApplication.ViewModell
{
    public class NewExerciseViewModel : ViewModelBase, IDisposable
    {
        KinectSensor sensor = null;
        ColorStream colorStream = null;
        SkeletonStream skeletonStream = null;

        DrawingImage drawingImage;
        DrawingGroup drawingGroup;

        StreamRecorder streamRecorder = null;
        StreamPlayer streamPlayer = null;
        public string path = "..\\..\\..\\ReferenceData\\";

        public ColorStream colorStreamView;
        public SkeletonStream skeletonStreamView;

        public DrawingImage skeletonReplayImage;
        public BitmapSource colorPlayback;

        public RelayCommand Record { get; set; }
        public RelayCommand Replay { get; set; }
        public RelayCommand Stop { get; set; }
        public NewExerciseViewModel()
        {
            ControlKeyEnable(true,true,false);

            sensor = KinectSensor.GetDefault();
            sensor.Open();
            colorStream = new ColorStream();
            colorStream.Stream(sensor);
            skeletonStream = new SkeletonStream();
            skeletonStream.Stream(sensor);
            streamRecorder = new StreamRecorder(skeletonStream, colorStream);
            streamPlayer = new StreamPlayer();
            streamPlayer.NewFrame += ShowFrameEvent;

            this.Record = new RelayCommand(RecordButton_Click, RecordButton_Cancel);
            this.Replay = new RelayCommand(PlayButton_Click, PlayButton_Cancel);
            this.Stop = new RelayCommand(StopButton_Click, StopButton_Cancel);

            ColorStreamView = colorStream;
            SkeletonStreamView = skeletonStream;

            drawingGroup = new DrawingGroup();
            drawingImage = new DrawingImage(drawingGroup);

            SkeletonReplayImage = drawingImage;
        }

        public ColorStream ColorStreamView
        {
            get { return colorStreamView; }
            set { colorStreamView = value; }
        }
        public SkeletonStream SkeletonStreamView
        {
            get { return skeletonStreamView; }
            set { skeletonStreamView = value; }
        }
        public DrawingImage SkeletonReplayImage
        {
            get { return skeletonReplayImage; }
            set { skeletonReplayImage = value;
                RaisePropertyChanged("skeletonReplayImage");
            }
        }
        public BitmapSource ColorPlayback
        {
            get { return colorPlayback; }
            set { colorPlayback = value; }
        }


        private void RecordButton_Click()
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
        public bool RecordButton_Cancel()
        {
            return true;
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

            }
            else
            {
                return true;
            }

        }

        private void StopButton_Click()
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
        public bool StopButton_Cancel()
        {
            return true;
        }

        private void PlayButton_Click()
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
            if (path != null && path != "")
            {
                streamPlayer.StartPlayback(path);
            }
        }
        public bool PlayButton_Cancel()
        {
            return true;
        }

        public bool BtnRecord;
        public bool BtnPlayback;
        public bool BtnStop;
        public bool btnRecord
        {
            get { return BtnRecord; }
            set { BtnRecord = value;
                RaisePropertyChanged("BtnRecord");
            }
        }
        public bool btnPlayback
        {
            get { return BtnPlayback; }
            set { BtnPlayback = value;
                RaisePropertyChanged("BtnPlayback");
            }
        }
        public bool btnStop
        {
            get { return BtnStop; }
            set { BtnStop = value;
                RaisePropertyChanged("BtnStop");
            }
        }
        public void ControlKeyEnable(bool rec, bool play, bool stop)
        {
            btnRecord = rec;
            btnPlayback = play;
            btnStop = stop;
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
        /// Replay a video
        /// </summary>
        /// <param name="image"></param>
        public void ShowFrameEvent(Mat image)
        {
            if (image != null)
            {
                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (streamPlayer.FrameCount < streamPlayer.skeletonFrames.Count)
                        {
                            //put the color frame on the UI
                            ColorPlayback = BitmapSourceConvert.BitmapToBitmapSource(image.Bitmap);
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
