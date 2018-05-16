using Microsoft.Kinect;
using System;
using System.Runtime.InteropServices;

namespace DataCollectionInterface
{
    /// <summary>
    /// Interface for stream by Kinect
    /// Get the data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [ComVisible(true)]
    public interface IDataCollection<T> 
    {
        /// <summary>
        /// Display the frame by Kinect
        /// </summary>
        /// <param name="sensor">The Kinect sensor</param>
        void Stream(KinectSensor sensor);
        /// <summary>
        /// Handles the frame data arriving from the sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FrameArrived(object sender, T e); //event
    }
}
