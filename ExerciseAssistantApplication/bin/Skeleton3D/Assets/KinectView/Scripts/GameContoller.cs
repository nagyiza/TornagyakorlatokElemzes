using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class is a controller, witch controlls the game (the reference skeleton)
/// </summary>
public class GameContoller : MonoBehaviour {
    /// <summary>
    /// This is a input to the path
    /// </summary>
    private InputField input;
    /// <summary>
    /// Game object
    /// </summary>
    private GameObject gameobj;

    /// <summary>
    /// Default path
    /// </summary>
    public string path = @"C:\Users\Izabella\Documents\Visual Studio 2015\Projects\ExerciseAssistantWithKinectV2\TornagyakorlatokElemzese\ReferenceData\gugolas2.txt";

    /// <summary>
    /// Get the input text (the path)
    /// </summary>
    /// <param name="path"></param>
    public void GetInput(string path)
    {
        Debug.Log("The path is " + path);

        //The path is input text
        path = @"C:\Users\Izabella\Documents\Visual Studio 2015\Projects\ExerciseAssistantWithKinectV2\TornagyakorlatokElemzese\ReferenceData\" + path + ".txt";
        
       
    }
}