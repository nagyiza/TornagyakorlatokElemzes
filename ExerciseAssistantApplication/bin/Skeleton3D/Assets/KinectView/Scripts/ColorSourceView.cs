using UnityEngine;
using System.Collections;
using Windows.Kinect;

//this class is in Unity Pro packages from microsoft page
/// <summary>
/// The class displays the color frame
/// </summary>
public class ColorSourceView : MonoBehaviour
{
    /// <summary>
    /// A game object
    /// </summary>
    public GameObject ColorSourceManager;
    /// <summary>
    /// A manager witch manage the color frame
    /// </summary>
    private ColorSourceManager _ColorManager;
    
    /// <summary>
    /// When started gets the component
    /// </summary>
    void Start ()
    {
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(-1, 1));
    }
    /// <summary>
    /// Update frame
    /// </summary>
    void Update()
    {
        if (ColorSourceManager == null)
        {
            return;
        }
        //get the component
        _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
        if (_ColorManager == null)
        {
            return;
        }
        
        //get the texture
        gameObject.GetComponent<Renderer>().material.mainTexture = _ColorManager.GetColorTexture();
    }
}
