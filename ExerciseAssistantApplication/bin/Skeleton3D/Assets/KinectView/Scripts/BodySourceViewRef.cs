using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System.IO;
using System;
using UnityEngine.UI;

/// <summary>
/// This class displays the reference skeleton
/// </summary>
public class BodySourceViewRef : MonoBehaviour
{
    /// <summary>
    /// The body's bone material
    /// </summary>
    public Material BoneMaterial;
    /// <summary>
    /// A game object
    /// </summary>
    public GameObject BodySourceManager;

    /// <summary>
    /// The path in which is the reference skeleton data
    /// </summary>
    private string path = "";// @"C:\Users\Izabella\Documents\Visual Studio 2015\Projects\ExerciseAssistantWithKinectV2\TornagyakorlatokElemzese\ReferenceData\gugolas2.txt";

    /// <summary>
    /// In this object is bodies.
    /// </summary>
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();

    /// <summary>
    /// In this object is bones (joint pairs)
    /// </summary>
    private Dictionary<Kinect.JointType, Kinect.JointType> _BoneMap = new Dictionary<Kinect.JointType, Kinect.JointType>()
    {
        { Kinect.JointType.FootLeft, Kinect.JointType.AnkleLeft },
        { Kinect.JointType.AnkleLeft, Kinect.JointType.KneeLeft },
        { Kinect.JointType.KneeLeft, Kinect.JointType.HipLeft },
        { Kinect.JointType.HipLeft, Kinect.JointType.SpineBase },

        { Kinect.JointType.FootRight, Kinect.JointType.AnkleRight },
        { Kinect.JointType.AnkleRight, Kinect.JointType.KneeRight },
        { Kinect.JointType.KneeRight, Kinect.JointType.HipRight },
        { Kinect.JointType.HipRight, Kinect.JointType.SpineBase },

        { Kinect.JointType.HandTipLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.ThumbLeft, Kinect.JointType.HandLeft },
        { Kinect.JointType.HandLeft, Kinect.JointType.WristLeft },
        { Kinect.JointType.WristLeft, Kinect.JointType.ElbowLeft },
        { Kinect.JointType.ElbowLeft, Kinect.JointType.ShoulderLeft },
        { Kinect.JointType.ShoulderLeft, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.HandTipRight, Kinect.JointType.HandRight },
        { Kinect.JointType.ThumbRight, Kinect.JointType.HandRight },
        { Kinect.JointType.HandRight, Kinect.JointType.WristRight },
        { Kinect.JointType.WristRight, Kinect.JointType.ElbowRight },
        { Kinect.JointType.ElbowRight, Kinect.JointType.ShoulderRight },
        { Kinect.JointType.ShoulderRight, Kinect.JointType.SpineShoulder },

        { Kinect.JointType.SpineBase, Kinect.JointType.SpineMid },
        { Kinect.JointType.SpineMid, Kinect.JointType.SpineShoulder },
        { Kinect.JointType.SpineShoulder, Kinect.JointType.Neck },
        { Kinect.JointType.Neck, Kinect.JointType.Head },
        { Kinect.JointType.Head, Kinect.JointType.Neck },
    };

    private Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint> joints = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint>();
    /// <summary>
    /// In this object is joints and which joint is enable
    /// </summary>
    private Dictionary<Windows.Kinect.JointType, bool> joinIsValid = new Dictionary<Windows.Kinect.JointType, bool>()
    {
        { Kinect.JointType.FootLeft, false },
        { Kinect.JointType.AnkleLeft, false },
        { Kinect.JointType.KneeLeft, false },
        { Kinect.JointType.HipLeft, false },

        { Kinect.JointType.FootRight, false },
        { Kinect.JointType.AnkleRight,false },
        { Kinect.JointType.KneeRight, false },
        { Kinect.JointType.HipRight, false },

        { Kinect.JointType.HandTipLeft, false },
        { Kinect.JointType.ThumbLeft, false },
        { Kinect.JointType.HandLeft, false },
        { Kinect.JointType.WristLeft, false },
        { Kinect.JointType.ElbowLeft, false },
        { Kinect.JointType.ShoulderLeft,false },

        { Kinect.JointType.HandTipRight, false },
        { Kinect.JointType.ThumbRight, false },
        { Kinect.JointType.HandRight, false },
        { Kinect.JointType.WristRight, false },
        { Kinect.JointType.ElbowRight, false},
        { Kinect.JointType.ShoulderRight,false },

        { Kinect.JointType.SpineBase, false},
        { Kinect.JointType.SpineMid,false },
        { Kinect.JointType.SpineShoulder,false },
        { Kinect.JointType.Neck, false },
        { Kinect.JointType.Head, false },
    };
    /// <summary>
    /// The path's lines
    /// </summary>
    private string[] lines;
    /// <summary>
    /// It is the line counter
    /// </summary>
    private long pos = 1; // 1, because first row is the bill head and this not need it
    /// <summary>
    /// The first jointType
    /// </summary>
    private int jointType = 0; // for processing skeleton data

    //System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\koordinateRef.txt", line + Environment.NewLine);

    
    /// <summary>
    /// Get the path from input
    /// </summary>
    /// <param name="path">The path in which is skeleton data</param>
    public void GetInput(string path)
    {
        path = @"..\..\..\ReferenceData\" + path + ".txt";

        Debug.Log("The path is " + path);
        joinIsValid = jointsIsNotValid();

        //read the all lines in the path
        lines = File.ReadAllLines(path);
        //processing the skeleton data
        SkeletonData();

    }

    /// <summary>
    /// Read the all lines in the path and processing the skeleton data
    /// </summary>
    void Start()
    {
        if (path != "" && File.Exists(path))
        {
            lines = File.ReadAllLines(path);
            Debug.Log("The Start " + path);
            SkeletonData();
        }

    }
    /// <summary>
    /// Update reference skeleton frame
    /// </summary>
    void Update()
    {
        if (path != "" && File.Exists(path))
        {
            if (BodySourceManager == null)
            {
                return;
            }

            //The reference body id is 0
            if (!_Bodies.ContainsKey(0))
            {
                //create the reference body
                _Bodies[0] = CreateBodyObject(0);
            }
            //refresh the reference body
            RefreshBodyObject(_Bodies[0]);
        }
        else
        {
            //default path
            path = @"C:\Users\Izabella\Documents\Visual Studio 2015\Projects\ExerciseAssistantWithKinectV2\TornagyakorlatokElemzese\ReferenceData\gugolas2.txt";

        }

    }
    /// <summary>
    /// Create the reference body object
    /// </summary>
    /// <param name="id">Body id (is 0)</param>
    /// <returns></returns>
    private GameObject CreateBodyObject(ulong id)
    {
        //Create the body object
        GameObject body = new GameObject("Body:" + id);

        //it walks across the joints
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            //create a body's joint, witch is a cube
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //displays the joint (the cube)
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.1f, 0.1f); //cube's size

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString(); // the game object's name is the joint's name
            jointObj.transform.parent = body.transform;
        }

        return body;
    }
    /// <summary>
    /// Refresh the reference body object
    /// </summary>
    /// <param name="bodyObject">The reference body gameobject</param>
    private void RefreshBodyObject(GameObject bodyObject)
    {
        //it walks across the joints
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            if (joinIsValid[jt])
            {
                if (joinIsValid[_BoneMap[jt]])
                {

                    Kinect.Joint sourceJoint = joints[jt];
                    Kinect.Joint? targetJoint = null;

                    if (_BoneMap.ContainsKey(jt))
                    {
                        targetJoint = joints[_BoneMap[jt]];
                    }

                    Transform jointObj = bodyObject.transform.Find(jt.ToString());
                    jointObj.localPosition = GetVector3FromJoint(sourceJoint);

                    LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                    if (targetJoint.HasValue)
                    {
                        Vector3 v = GetVector3FromJoint(targetJoint.Value);
                        lr.SetPosition(0, jointObj.localPosition);
                        lr.SetPosition(1, v);
                        lr.SetColors(Color.green, Color.green);
                        lr.SetWidth(0.2f, 0.2f);

                        System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\koordinateRef.txt", v.x + " " + v.y + " " + v.z + Environment.NewLine);

                    }
                    else
                    {
                        lr.enabled = false;

                    }
                }

            }
        }
        joints = new Dictionary<Windows.Kinect.JointType, Windows.Kinect.Joint>();
        joinIsValid = jointsIsNotValid(); // all joint is false

        //processing the skeleton data
        SkeletonData();

    }
    

    /// <summary>
    /// Get the joint's position
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
    }

    /// <summary>
    /// Processing skeleton data (processing 24 joint)
    /// </summary>
    private void SkeletonData()
    {
        string line; // A line in the file
        char[] separators = { ' ' };
        string[] pathSplit = path.Split('\\');
        // Just file name
        if (pathSplit[pathSplit.Length - 1] != ".txt")
        {
            int jointNumber = 0;

            Kinect.Joint? targetJoint = null;

            //read the skeleton data
            if (lines != null) {
                while (lines.Length != pos)
                {
                    line = lines[pos];
                    // split the data 
                    string[] words = line.Split(separators);

                    if (jointType > Convert.ToInt32(words[5])) // jointType
                    {
                        jointType = Convert.ToInt32(words[5]);
                        break;
                    }

                    jointType = Convert.ToInt32(words[5]);

                    //skeleton display Width
                    double skeletonWidth = Convert.ToDouble(words[10]);
                    //skeleton display Height
                    double skeletonHeight = Convert.ToDouble(words[11]);

                    //colorSpacePoint.X
                    double X = Convert.ToDouble(words[8]);
                    // colorSpacePoint.Y
                    double Y = Convert.ToDouble(words[9]);
                    double Z = 15;

                    X = X * skeletonWidth / 1920; // (512 / 1920)
                    Y = Y * skeletonWidth / 1920;// (512 / 1920)
                                                 // shift, small
                    X = X * 0.2 + 5;
                    Y = (Y * 0.2 - 10) * (-1);


                    Kinect.Joint jt = new Kinect.Joint();
                    Kinect.CameraSpacePoint point = new Kinect.CameraSpacePoint();
                    //Create the camera space point
                    point.X = (float)X;
                    point.Y = (float)Y;
                    point.Z = (float)Z;
                    jt.Position = point;

                    if (_BoneMap.ContainsKey(getJoinType(jointType)))
                    {
                        //add the joint and enabling the joint
                        joints.Add(getJoinType(jointType), jt);
                        targetJoint = jt;
                        joinIsValid[getJoinType(jointType)] = true;
                    }

                    ++pos;

                }
                //if the path finished, replay the reference skeleton
                if (pos == lines.Length)
                {
                    pos = 1;
                }
            }

            
        }

    }

    /// <summary>
    /// Get joinType string
    /// </summary>
    /// <param name="jointType"></param>
    /// <returns></returns>
    private Kinect.JointType getJoinType(int jointType)
    {
        switch (jointType)
        {
            case 0: return Windows.Kinect.JointType.SpineBase;
            case 1: return Windows.Kinect.JointType.SpineMid;
            case 2: return Windows.Kinect.JointType.Neck;
            case 3: return Windows.Kinect.JointType.Head;
            case 4: return Windows.Kinect.JointType.ShoulderLeft;
            case 5: return Windows.Kinect.JointType.ElbowLeft;
            case 6: return Windows.Kinect.JointType.WristLeft;
            case 7: return Windows.Kinect.JointType.HandLeft;
            case 8: return Windows.Kinect.JointType.ShoulderRight;
            case 9: return Windows.Kinect.JointType.ElbowRight;
            case 10: return Windows.Kinect.JointType.WristRight;
            case 11: return Windows.Kinect.JointType.HandRight;
            case 12: return Windows.Kinect.JointType.HipLeft;
            case 13: return Windows.Kinect.JointType.KneeLeft;
            case 14: return Windows.Kinect.JointType.AnkleLeft;
            case 15: return Windows.Kinect.JointType.FootLeft;
            case 16: return Windows.Kinect.JointType.HipRight;
            case 17: return Windows.Kinect.JointType.KneeRight;
            case 18: return Windows.Kinect.JointType.AnkleRight;
            case 19: return Windows.Kinect.JointType.FootRight;
            case 20: return Windows.Kinect.JointType.SpineShoulder;
            case 21: return Windows.Kinect.JointType.HandTipLeft;
            case 22: return Windows.Kinect.JointType.ThumbLeft;
            case 23: return Windows.Kinect.JointType.HandTipRight;
            case 24: return Windows.Kinect.JointType.ThumbRight;
            default: return Kinect.JointType.SpineBase;
        }
    }
    /// <summary>
    /// The joints enable sets up false
    /// </summary>
    /// <returns></returns>
    private Dictionary<Kinect.JointType, bool> jointsIsNotValid()
    {
        Dictionary<Kinect.JointType, bool> notValid = new Dictionary<Windows.Kinect.JointType, bool>()
        {
            { Kinect.JointType.FootLeft, false },
            { Kinect.JointType.AnkleLeft, false },
            { Kinect.JointType.KneeLeft, false },
            { Kinect.JointType.HipLeft, false },

            { Kinect.JointType.FootRight, false },
            { Kinect.JointType.AnkleRight,false },
            { Kinect.JointType.KneeRight, false },
            { Kinect.JointType.HipRight, false },

            { Kinect.JointType.HandTipLeft, false },
            { Kinect.JointType.ThumbLeft, false },
            { Kinect.JointType.HandLeft, false },
            { Kinect.JointType.WristLeft, false },
            { Kinect.JointType.ElbowLeft, false },
            { Kinect.JointType.ShoulderLeft,false },

            { Kinect.JointType.HandTipRight, false },
            { Kinect.JointType.ThumbRight, false },
            { Kinect.JointType.HandRight, false },
            { Kinect.JointType.WristRight, false },
            { Kinect.JointType.ElbowRight, false},
            { Kinect.JointType.ShoulderRight,false },

            { Kinect.JointType.SpineBase, false},
            { Kinect.JointType.SpineMid,false },
            { Kinect.JointType.SpineShoulder,false },
            { Kinect.JointType.Neck, false },
            { Kinect.JointType.Head, false },
        };
        return notValid;
    }
}
