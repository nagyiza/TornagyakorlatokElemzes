using DataCollectionInterface;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Microsoft.Kinect;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

// In this class I used the example code of Kinect SDK 2.0

namespace Stream
{
    public class ColorStream : IDataCollection<ColorFrameArrivedEventArgs>, INotifyPropertyChanged
    {
        /// <summary>
        /// Reader for color frames
        /// </summary>
        ColorFrameReader colorFrameReader = null;
        FrameDescription frameDescription = null;
        /// <summary>
        /// Bitmap to display
        /// </summary>
        WriteableBitmap colorBitmap = null;

        Mat currentFrame = null;

        private byte[] colorPixels;
        bool isRecording = false;

        /// <summary>
        /// Color display height (not video size)
        /// </summary>
        public static float colorDisplayHeight = 300;
        /// <summary>
        /// Color display width (not video size)
        /// </summary>
        public static float colorDisplayWidth = 600;

        public event PropertyChangedEventHandler PropertyChanged;

        public ColorStream() { }
        
        public void Stream(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                //open the color stream reader
                colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();
                // wire handler for frame arrival
                colorFrameReader.FrameArrived += FrameArrived;
                //frame size
                //frameDescription = kinectSensor.ColorFrameSource.FrameDescription;
                //writeableBitmap = new WriteableBitmap(frameDescription.Width, frameDescription.Height, 96.0, 96.0, PixelFormats.Bgra32, null);

                // create the colorFrameDescription from the ColorFrameSource using Bgra format
                FrameDescription colorFrameDescription = kinectSensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Bgra);

                // create the bitmap to display
                this.colorBitmap = new WriteableBitmap(colorFrameDescription.Width, colorFrameDescription.Height, 96.0, 96.0, PixelFormats.Bgr32, null);
            }
            else
            {
                throw new ArgumentNullException("kinectSensor", "The parameter is null!");
            }
        }

        /// <summary>
        /// Handles the color frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (ColorFrame colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    //drawing
                    FrameDescription colorFrameDescription = colorFrame.FrameDescription;

                    using (KinectBuffer colorBuffer = colorFrame.LockRawImageBuffer())
                    {
                        this.colorBitmap.Lock();

                        // verify data and write the new color frame data to the display bitmap
                        if ((colorFrameDescription.Width == this.colorBitmap.PixelWidth) && (colorFrameDescription.Height == this.colorBitmap.PixelHeight))
                        {
                            colorFrame.CopyConvertedFrameDataToIntPtr(
                                this.colorBitmap.BackBuffer,
                                (uint)(colorFrameDescription.Width * colorFrameDescription.Height * 4),
                                ColorImageFormat.Bgra);

                            this.colorBitmap.AddDirtyRect(new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight));
                        }

                        this.colorBitmap.Unlock();
                    }
                    //image saving
                    if (IsRecording)
                    {
                        VideoSave(colorFrame);
                    }

                }
            }
        }

        private void VideoSave(ColorFrame frame)
        {
            //V2 Mat -> image saveing
            int width = frame.FrameDescription.Width;
            int height = frame.FrameDescription.Height;
            int depth = ((PixelFormats.Bgr32.BitsPerPixel + 7) / 8);
            this.colorPixels = new byte[width * height * depth];

            if (frame.RawColorImageFormat == ColorImageFormat.Bgra)
            {
                frame.CopyRawFrameDataToArray(colorPixels);
            }
            else
            {
                frame.CopyConvertedFrameDataToArray(colorPixels, ColorImageFormat.Bgra);
            }

            int stride = width * PixelFormats.Bgra32.BitsPerPixel / 8;
            BitmapSource bitmapimage = BitmapSource.Create(width, height, 96, 96, PixelFormats.Bgra32, null, colorPixels, stride);
            //convert frame to Mat object
            currentFrame = BitmapSourceConvert.ToMat(bitmapimage);

        }
        public ImageSource colorImage
        {
            get
            {
                return colorBitmap;
            }
        }
        public Mat CurrentFrame
        {
            get
            {
                return currentFrame;
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
        public static float ColorDisplayHeight
        {
            get
            {
                return colorDisplayHeight;
            }
        }
        public static float ColorDisplayWidth
        {
            get
            {
                return colorDisplayWidth;
            }
        }
        public void Dispose()
        {
            if (colorFrameReader != null)
            {
                colorFrameReader.FrameArrived -= FrameArrived;
                colorFrameReader.Dispose();
                colorFrameReader = null;
            }
        }

    }
}