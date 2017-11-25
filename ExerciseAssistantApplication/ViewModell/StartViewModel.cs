using Emgu.CV;
using ExerciseAssistantApplication.Common;
using Microsoft.Kinect;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using Stream;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace ExerciseAssistantApplication.ViewModell
{
    public class StartViewModel : ViewModelBase, IDisposable
    {
        
        KinectSensor sensor = null;
        ColorStream colorStream = null;
        ColorStream referenceStream = null;

        DrawingImage drawingImage;
        DrawingGroup drawingGroup;

        public string path = "..\\..\\..\\ReferenceData\\";

        public ColorStream colorStreamView;
        public ColorStream referenceStreamView;

        public DrawingImage skeletonReplayImage;
        public string referenceVideo = "C:\\Users\\Izabella\\Documents\\Visual Studio 2015\\Projects\\ExerciseAssistantWithKinectV2\\TornagyakorlatokElemzese\\ReferenceData\\anyu.avi";

        public RelayCommand SelectExercise { get; set; }
        //public RelayCommand Replay { get; set; }
        //public RelayCommand Stop { get; set; }

        public StartViewModel()
        {
            sensor = KinectSensor.GetDefault();
            sensor.Open();
            colorStream = new ColorStream();
            colorStream.Stream(sensor);



            this.SelectExercise = new RelayCommand(SelectExercise_Click, SelectExercise_Cancel);
            //this.Replay = new RelayCommand(PlayButton_Click, PlayButton_Cancel);
            //this.Stop = new RelayCommand(StopButton_Click, StopButton_Cancel);

            referenceStream = new ColorStream();
            referenceStream.Stream(sensor);

            ColorStreamView = colorStream;
            ReferenceStreamView = referenceStream;

            drawingGroup = new DrawingGroup();
            drawingImage = new DrawingImage(drawingGroup);

            SkeletonReplayImage = drawingImage;


           


        }
        public ColorStream ColorStreamView
        {
            get { return colorStreamView; }
            set { colorStreamView = value; }
        }
        public ColorStream ReferenceStreamView
        {
            get { return referenceStreamView; }
            set { referenceStreamView = value; }
        }
        public DrawingImage SkeletonReplayImage
        {
            get { return skeletonReplayImage; }
            set
            {
                skeletonReplayImage = value;
                OnPropertyChanged("skeletonReplayImage");
            }
        }
        public string ReferenceVideo
        {
            get { return referenceVideo; }
            set { referenceVideo = value;
                OnPropertyChanged("referenceVideo");
            }
        }
        private void SelectExercise_Click()
        {
            //ControlKeyEnable(false, false, true);
            OpenFileDialog playing = new OpenFileDialog();
            playing.Filter = "All files(*.*)|*.*";
            playing.FilterIndex = 2;
            playing.RestoreDirectory = true;
            if (playing.ShowDialog() == true)
            {
                // file name 
                ReferenceVideo = playing.FileName;
            }
            
        }
        public bool SelectExercise_Cancel()
        {
            return true;
        }

       
        //private void StopButton_Click()
        //{
        //    if (colorStream.IsRecording || skeletonStream.IsRecording)
        //    {
        //        streamRecorder.StopRecording();
        //    }
        //    else
        //    {
        //        streamPlayer.StopPlayback();
        //    }
        //    //ControlKeyEnable(true, true, false);
        //}
        //public bool StopButton_Cancel()
        //{
        //    return true;
        //}

        //private void PlayButton_Click()
        //{
        //    //ControlKeyEnable(false, false, true);

        //    OpenFileDialog playing = new OpenFileDialog();
        //    playing.Filter = "All files(*.*)|*.*";
        //    playing.FilterIndex = 2;
        //    playing.RestoreDirectory = true;
        //    if (playing.ShowDialog() == true)
        //    {
        //        // file name 
        //        string[] split = playing.FileName.Split('\\');
        //        split = split[split.Length - 1].Split('.');
        //        path = "..\\..\\..\\ReferenceData\\" + split[0]; // ReferenceData\filename
        //    }
        //    if (path != null && path != "")
        //    {
        //        streamPlayer.StartPlayback(path);
        //    }
        //}
        //public bool PlayButton_Cancel()
        //{
        //    return true;
        //}

        //public bool BtnRecord;
        //public bool BtnPlayback;
        //public bool BtnStop;
        //public bool btnRecord
        //{
        //    get { return BtnRecord; }
        //    set
        //    {
        //        BtnRecord = value;
        //        RaisePropertyChanged("BtnRecord");
        //    }
        //}
        //public bool btnPlayback
        //{
        //    get { return BtnPlayback; }
        //    set
        //    {
        //        BtnPlayback = value;
        //        RaisePropertyChanged("BtnPlayback");
        //    }
        //}
        //public bool btnStop
        //{
        //    get { return BtnStop; }
        //    set
        //    {
        //        BtnStop = value;
        //        RaisePropertyChanged("BtnStop");
        //    }
        //}
        //public void ControlKeyEnable(bool rec, bool play, bool stop)
        //{
        //    btnRecord = rec;
        //    btnPlayback = play;
        //    btnStop = stop;
        //}

        public void Dispose()
        {
            if (colorStream != null)
            {
                colorStream.Dispose();
                colorStream = null;
            }
            if (referenceStream != null)
            {
                referenceStream.Dispose();
                referenceStream = null;
            }
            if (sensor != null)
            {
                sensor.Close();
                sensor = null;
            }
        }

    }
}