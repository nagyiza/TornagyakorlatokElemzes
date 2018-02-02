using UnityEngine;
using System.Collections;
using Windows.Kinect;

//this class is in Unity Pro packages from microsoft page
/// <summary>
/// The class manage the kinect and the body frame
/// </summary>
public class BodySourceManager : MonoBehaviour 
{
    /// <summary>
    /// Kinect sensor
    /// </summary>
    private KinectSensor _Sensor;
    /// <summary>
    /// A reader with body frame
    /// </summary>
    private BodyFrameReader _Reader;
    /// <summary>
    /// The body data
    /// </summary>
    private Body[] _Data = null;

    /// <summary>
    /// Get the body data
    /// </summary>
    /// <returns></returns>
    public Body[] GetData()
    {
        return _Data;
    }
    
    /// <summary>
    /// Start the kinect and read body frame
    /// </summary>
    void Start () 
    {
        _Sensor = KinectSensor.GetDefault();

        if (_Sensor != null)
        {
            _Reader = _Sensor.BodyFrameSource.OpenReader();
            
            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }   
    }
    
    /// <summary>
    /// Update frame
    /// Read the next frame, and refresh the body data
    /// </summary>
    void Update () 
    {
        if (_Reader != null)
        {
            var frame = _Reader.AcquireLatestFrame();
            if (frame != null)
            {
                if (_Data == null)
                {
                    _Data = new Body[_Sensor.BodyFrameSource.BodyCount];
                }
                
                frame.GetAndRefreshBodyData(_Data);
                
                frame.Dispose();
                frame = null;
            }
        }    
    }
    
    /// <summary>
    /// When the application is quit
    /// Closed the kinect and the reader
    /// </summary>
    void OnApplicationQuit()
    {
        if (_Reader != null)
        {
            _Reader.Dispose();
            _Reader = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }
            
            _Sensor = null;
        }
    }
}
