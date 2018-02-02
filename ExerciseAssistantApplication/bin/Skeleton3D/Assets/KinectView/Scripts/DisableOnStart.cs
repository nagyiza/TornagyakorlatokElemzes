using UnityEngine;
using System.Collections;

//this class is in Unity Pro packages from microsoft page
public class DisableOnStart : MonoBehaviour {

    // Use this for initialization
    void Start () 
    {
        //The game object is hidden
        gameObject.SetActive(false);
    }
}
