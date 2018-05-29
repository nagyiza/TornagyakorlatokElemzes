using DataCollectionInterface;
using System;
using Microsoft.Kinect;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows;
using System.Runtime.InteropServices;

// In this class I used the example code of Kinect SDK 2.0

namespace Stream
{
    public class DepthStream : IDataCollection<DepthFrameArrivedEventArgs>, INotifyPropertyChanged
    {
        /// <summary>
        /// Map depth range to byte range
        /// </summary>
        private const int MapDepthToByte = 8000 / 256;

        /// <summary>
        /// Reader for depth frames
        /// </summary>
        private DepthFrameReader depthFrameReader = null;

        /// <summary>
        /// Description of the data contained in the depth frame
        /// </summary>
        private FrameDescription depthFrameDescription = null;

        /// <summary>
        /// Bitmap to display
        /// </summary>
        private WriteableBitmap depthBitmap = null;

        /// <summary>
        /// Intermediate storage for frame data converted to color
        /// </summary>
        private byte[] depthPixels = null;

        /// <summary>
        /// Frame width.
        /// </summary>
        public int width;

        /// <summary>
        /// Frame height.
        /// </summary>
        public int height;

        /// <summary>
        /// The depth values.
        /// </summary>
        public ushort[] depthData = null;

        /// <summary>
        /// The body index values.
        /// </summary>
        public byte[] bodyData = null;

        public event PropertyChangedEventHandler PropertyChanged;


        public DepthStream() { }
        public void Stream(KinectSensor kinectSensor)
        {
            if (kinectSensor != null)
            {
                // open the reader for the depth frames
                depthFrameReader = kinectSensor.DepthFrameSource.OpenReader();

                // wire handler for frame arrival
                depthFrameReader.FrameArrived += FrameArrived;

                // get FrameDescription from DepthFrameSource
                depthFrameDescription = kinectSensor.DepthFrameSource.FrameDescription;

                // allocate space to put the pixels being received and converted
                depthPixels = new byte[this.depthFrameDescription.Width * this.depthFrameDescription.Height];

                // create the bitmap to display
                depthBitmap = new WriteableBitmap(this.depthFrameDescription.Width, this.depthFrameDescription.Height, 96.0, 96.0, PixelFormats.Gray8, null);
            }
        }
        /// <summary>
        /// Handles the depth frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void FrameArrived(object sender, DepthFrameArrivedEventArgs e)
        {
            bool depthFrameProcessed = false;

            using (DepthFrame depthFrame = e.FrameReference.AcquireFrame())
            {
                if (depthFrame != null)
                {
                    // the fastest way to process the body index data is to directly access 
                    // the underlying buffer
                    using (Microsoft.Kinect.KinectBuffer depthBuffer = depthFrame.LockImageBuffer())
                    {
                        // verify data and write the color data to the display bitmap
                        if (((depthFrameDescription.Width * depthFrameDescription.Height) == (depthBuffer.Size / depthFrameDescription.BytesPerPixel)) &&
                            (depthFrameDescription.Width == depthBitmap.PixelWidth) && (depthFrameDescription.Height == depthBitmap.PixelHeight))
                        {
                            // Note: In order to see the full range of depth (including the less reliable far field depth)
                            // we are setting maxDepth to the extreme potential depth threshold
                            ushort maxDepth = ushort.MaxValue;

                            // If you wish to filter by reliable depth distance, uncomment the following line:
                            //// maxDepth = depthFrame.DepthMaxReliableDistance

                            ProcessDepthFrameData(depthBuffer.UnderlyingBuffer, depthBuffer.Size, depthFrame.DepthMinReliableDistance, maxDepth);
                            depthFrameProcessed = true;
                        }
                    }
                }
            }
            if (depthFrameProcessed)
            {
                RenderDepthPixels();
            }
        }
        private unsafe void ProcessDepthFrameData(IntPtr depthFrameData, uint depthFrameDataSize, ushort minDepth, ushort maxDepth)
        {
            // depth frame data is a 16 bit value
            ushort* frameData = (ushort*)depthFrameData;

            // convert depth to a visual representation
            for (int i = 0; i < (int)(depthFrameDataSize / this.depthFrameDescription.BytesPerPixel); ++i)
            {
                // Get the depth for this pixel
                ushort depth = frameData[i];

                // To convert to a byte, we're mapping the depth value to the byte range.
                // Values outside the reliable depth range are mapped to 0 (black).
                this.depthPixels[i] = (byte)(depth >= minDepth && depth <= maxDepth ? (depth / MapDepthToByte) : 0);
            }
        }
        /// <summary>
        /// Renders color pixels into the writeableBitmap.
        /// </summary>
        private void RenderDepthPixels()
        {
            this.depthBitmap.WritePixels(
                new Int32Rect(0, 0, this.depthBitmap.PixelWidth, this.depthBitmap.PixelHeight),
                this.depthPixels,
                this.depthBitmap.PixelWidth,
                0);
        }

        // I used the KinectCoordinateMapping project,http://pterneas.com/2014/05/06/understanding-kinect-coordinate-mapping/,  https://github.com/Vangos/kinect-2-coordinate-mapping
        /// <summary>
        /// Converts a depth frame to the corresponding System.Windows.Media.Imaging.BitmapSource.
        /// </summary>
        public BitmapSource ToBitmap(DepthFrame frame, BodyIndexFrame bodyIndexFrame)
        {
            ushort minDepth = frame.DepthMinReliableDistance;
            ushort maxDepth = frame.DepthMaxReliableDistance;

            if (bodyData == null)
            {
                width = frame.FrameDescription.Width;
                height = frame.FrameDescription.Height;
                depthData = new ushort[width * height];
                bodyData = new byte[width * height];
                depthPixels = new byte[width * height * (PixelFormats.Bgr32.BitsPerPixel + 7) / 8];
                depthBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr32, null);
            }

            frame.CopyFrameDataToArray(depthData);
            bodyIndexFrame.CopyFrameDataToArray(bodyData);

            // Convert the depth to RGB.
            for (int depthIndex = 0, colorPixelIndex = 0; depthIndex < depthData.Length && colorPixelIndex < depthPixels.Length; depthIndex++, colorPixelIndex += 4)
            {
                // Get the depth for this pixel
                ushort depth = depthData[depthIndex];
                byte player = bodyData[depthIndex];

                // To convert to a byte, we're discarding the most-significant
                // rather than least-significant bits.
                // We're preserving detail, although the intensity will "wrap."
                // Values outside the reliable depth range are mapped to 0 (black).
                byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);

                if (player != 0xff)
                {
                    depthPixels[colorPixelIndex + 0] = Colors.Gold.B; // Blue
                    depthPixels[colorPixelIndex + 1] = Colors.Gold.G; // Green
                    depthPixels[colorPixelIndex + 2] = Colors.Gold.R; // Red
                }
                else
                {
                    // Color the rest of the image in grayscale.
                    depthPixels[colorPixelIndex + 0] = intensity; // B
                    depthPixels[colorPixelIndex + 1] = intensity; // G
                    depthPixels[colorPixelIndex + 2] = intensity; // R
                }
            }

            depthBitmap.Lock();

            Marshal.Copy(depthPixels, 0, depthBitmap.BackBuffer, depthPixels.Length);
            depthBitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));

            depthBitmap.Unlock();

            return depthBitmap;
        }        
    }
}