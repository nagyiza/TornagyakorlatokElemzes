using Microsoft.Kinect;
using System;
using System.Runtime.InteropServices;

namespace DataCollectionInterface
{
    [ComVisible(true)]
    public interface IDataCollection<T> 
    {
        void Stream(KinectSensor sensor);
        void FrameArrived(object sender, T e); //event

    }
}
