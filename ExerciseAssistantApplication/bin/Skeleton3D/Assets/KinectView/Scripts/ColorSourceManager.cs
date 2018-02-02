using UnityEngine;
using System.Collections;
using Windows.Kinect;

//this class is in Unity Pro packages from microsoft page
/// <summary>
/// The class manage the kinect's color frame
/// </summary>
public class ColorSourceManager : MonoBehaviour 
{
    /// <summary>
    /// The color frame's width
    /// </summary>
    public int ColorWidth { get; private set; }
    /// <summary> 
    /// The color frame's height
    /// </summary>
    public int ColorHeight { get; private set; }
    /// <summary>
    /// Kinect sensor
    /// </summary>
    private KinectSensor _Sensor;
    /// <summary>
    /// A reader with color frame
    /// </summary>
    private ColorFrameReader _Reader;
    /// <summary>
    /// The texture witch is 2D
    /// </summary>
    private Texture2D _Texture;
    /// <summary>
    /// The data
    /// </summary>
    private byte[] _Data;
    
    /// <summary>
    /// Get the texture
    /// </summary>
    /// <returns></returns>
    public Texture2D GetColorTexture()
    {
        return _Texture;
    }

    /// <summary>
    /// Start the kinect, read color frame and sets up the texture size
    /// </summary>
    void Start()
    {
        _Sensor = KinectSensor.GetDefault();
        
        if (_Sensor != null) 
        {
            _Reader = _Sensor.ColorFrameSource.OpenReader();
            
            var frameDesc = _Sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            ColorWidth = frameDesc.Width;
            ColorHeight = frameDesc.Height;
            
            _Texture = new Texture2D(frameDesc.Width, frameDesc.Height, TextureFormat.RGBA32, false);
            _Data = new byte[frameDesc.BytesPerPixel * frameDesc.LengthInPixels];
            
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
                frame.CopyConvertedFrameDataToArray(_Data, ColorImageFormat.Rgba);
                _Texture.LoadRawTextureData(_Data);
                _Texture.Apply();
                
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
