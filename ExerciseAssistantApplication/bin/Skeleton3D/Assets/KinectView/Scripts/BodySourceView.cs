using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System;
using System.Linq;
using UnityEngine.UI;
using System.IO;

//this class is in Unity Pro packages from microsoft page
/// <summary>
/// The class displays the body
/// </summary>
public class BodySourceView : MonoBehaviour
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
    /// Result textbox
    /// </summary>
    public Text result;
    /// <summary>
    /// In this object is bodies.
    /// </summary>
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    /// <summary>
    /// A manager witch manage the body frame
    /// </summary>
    private BodySourceManager _BodyManager;


    /// <summary>
    /// The path in which is the reference skeleton data
    /// </summary>
    private string path = "";// @"C:\Users\Izabella\Documents\Visual Studio 2015\Projects\ExerciseAssistantWithKinectV2\TornagyakorlatokElemzese\ReferenceData\gugolas2.txt";


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
    };

    /// <summary>
    /// Reference skeleton list
    /// </summary>
    private List<Vector4> reference = new List<Vector4>();

    /// <summary>
    /// User skeleton
    /// </summary>
    private List<Vector4> userSkeleton = new List<Vector4>();

    /// <summary>
    /// Get the path from input
    /// </summary>
    /// <param name="path">The path in which is skeleton data</param>
    public void GetInput(string path)
    {
        path = @"..\..\..\ReferenceData\" + path + ".txt";

        Debug.Log("The path is BodySourceView " + path);

        //processing the skeleton data
        ReferenceSkeleton();
    }

    void Start()
    {
        System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\skeleton.txt", "X              " + "Y         " + "Z     " + " JointType " + Environment.NewLine);
        //read the reference skeleton from a file
        
    }


    /// <summary>
    /// Update frame
    /// </summary>
    void Update()
    {
        if (path != "" && File.Exists(path))
        {
            if (BodySourceManager == null)
            {
                return;
            }

            //get the component
            _BodyManager = BodySourceManager.GetComponent<BodySourceManager>();
            if (_BodyManager == null)
            {
                return;
            }
            //get the body data
            Kinect.Body[] data = _BodyManager.GetData();
            if (data == null)
            {
                return;
            }

            List<ulong> trackedIds = new List<ulong>();// body id
                                                       //if there is more body
            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    trackedIds.Add(body.TrackingId);
                }
            }

            List<ulong> knownIds = new List<ulong>(_Bodies.Keys);

            // First delete untracked bodies
            foreach (ulong trackingId in knownIds)
            {
                if (!trackedIds.Contains(trackingId))
                {
                    Destroy(_Bodies[trackingId]);
                    _Bodies.Remove(trackingId);
                }
            }

            foreach (var body in data)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    if (!_Bodies.ContainsKey(body.TrackingId))
                    {
                        //if the body is oke, create the body object
                        _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    }
                    //refresh the body object
                    RefreshBodyObject(body, _Bodies[body.TrackingId]);
                }
            }
        }else
        {
            //default path
            path = @"..\..\..\ReferenceData\allas.txt";

        }
    }

    /// <summary>
    /// Create the body object, and displays the body's joints
    /// </summary>
    /// <param name="id">Body id</param>
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
            lr.SetWidth(0.2f, 0.2f); //cube's size

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString(); // the game object's name is the joint's name
            jointObj.transform.parent = body.transform;
        }

        return body;
    }

    /// <summary>
    /// Refresh the body object
    /// </summary>
    /// <param name="body">The tracked body</param>
    /// <param name="bodyObject">The body object</param>
    private void RefreshBodyObject(Kinect.Body body, GameObject bodyObject)
    {
        //it walks across the joints
        for (Kinect.JointType jt = Kinect.JointType.SpineBase; jt <= Kinect.JointType.ThumbRight; jt++)
        {
            //jt is a joint from body joint
            Kinect.Joint sourceJoint = body.Joints[jt];
            //the jt's pair
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                //the jt's pair from the bone map
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            // the game object's joint
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            //body joint position
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            //drawing bone
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                //source joint
                lr.SetPosition(0, jointObj.localPosition);
                //target joint
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                //size
                lr.SetWidth(0.2f, 0.2f);
                //get target joint's position
                Vector3 v = GetVector3FromJoint(targetJoint.Value);
                //System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\koordinate.txt", v.x + " " + v.y + " " + v.z + Environment.NewLine);
                //the joint's color, red (not tracked) or green(tracked) or black
                lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                //size
                lr.SetWidth(0.2f, 0.2f);
                if (targetJoint.Value.TrackingState == Kinect.TrackingState.Tracked)
                {
                    System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\skeleton.txt", sourceJoint.Position.X + " " + sourceJoint.Position.Y + " " + sourceJoint.Position.Z + " " + (int)jt + " " + jt.ToString() + Environment.NewLine);
                    Vector3 pos = new Vector3(sourceJoint.Position.X, sourceJoint.Position.Y, sourceJoint.Position.Z);
                    j++;//skeleton's position counter
                    Vector4 position = new Vector4();
                    position.w = (int)jt;
                    position.x = pos.x;
                    position.y = pos.y;
                    position.z = pos.z;
                    userSkeleton.Add(position);

                }
            }
            else
            {
                lr.enabled = false;
            }
        }

        if (reference.Count != 0) {
            int dtw = DTWDistance(reference, userSkeleton, n);
            result.text = dtw.ToString();
            result.color = Color.green;
        }
           userSkeleton = new List<Vector4>();
                
    }

    /// <summary>
    /// Get pen's color
    /// </summary>
    /// <param name="state">Tracked or not tracked</param>
    /// <returns></returns>
    private static Color GetColorForState(Kinect.TrackingState state)
    {
        switch (state)
        {
            case Kinect.TrackingState.Tracked:
                return Color.green; // the tracked joint is green

            case Kinect.TrackingState.Inferred:
                return Color.red; // the inferred joint is red

            default:
                return Color.black; // defeault joint is black
        }
    }

    /// <summary>
    /// Get position of a joint
    /// </summary>
    /// <param name="joint"></param>
    /// <returns></returns>
    private static Vector3 GetVector3FromJoint(Kinect.Joint joint)
    {
        return new Vector3(joint.Position.X * 10, joint.Position.Y * 10 - 2, 15);
    }

    /// <summary>
    /// Reference skeleton's position count
    /// </summary>
    private int n = 0;
    /// <summary>
    /// User skeleton's position count
    /// </summary>
    private static int j = 0;

    private void ReferenceSkeleton()
    {
        System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\skeletonRef.txt", "   X       Y         Z      JointType" + Environment.NewLine);
        string[] lines = System.IO.File.ReadAllLines(path);//reference     

        int type = 0;
        try
        {
            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                if (line != lines[0])
                {
                    Vector4 position = new Vector4();
                    position.w = int.Parse(words[5]);
                    position.z = float.Parse(words[0]);
                    position.x = float.Parse(words[1]);
                    position.y = float.Parse(words[2]);

                    reference.Add(position);
                    System.IO.File.AppendAllText(@"C:\Users\Izabella\Desktop\skeletonRef.txt", position.x + " " + position.y + " " + position.z + " " + position.w + Environment.NewLine);

                    if (int.Parse(words[5]) <= type)
                    {
                        n++;
                    }
                    type = int.Parse(words[5]);
                }

            }
        }
        catch (Exception e)
        {

        }
    }

    /// <summary>
    /// Dtw algorithm matrix
    /// </summary>
    //private static  DTW;

    public static int DTWDistance(List<Vector4> s, List<Vector4> t, int n)
    {
        //one reference skeleton position(24 joint)
        List<Vector4> skeletonRef = new List<Vector4>();
        List<Vector4> skeleton = new List<Vector4>();

        int m = 2;
        j = 1;

        int[,] DTW = new int[n, m];
        int cost;

        for (int i = 0; i < n; ++i)
        {
            DTW[i, 0] = 100000;
        }
        for (int i = 0; i < m; ++i)
        {
            DTW[0, i] = 100000;
        }
        DTW[0, 0] = 0;

        for (int i = 1; i < n; ++i)
        {
            //for (int j = 1; j < m; ++j)
            //{
            skeletonRef = new List<Vector4>();
            skeleton = new List<Vector4>();
            int type = (int)s[0].w;
           
            for (int k = 0; k < 25; ++k)
            {
                if ((int)s[k].w < type)
                {  
                    break;
                }
                skeletonRef.Add(s[k]);
                type =(int) s[k].w;
            }
            int type2 = (int)t[0].w;
            for (int k = 0; k < 25; ++k)
            {
                if ((int)t[k].w < type2)
                {
                    break;
                }
                skeleton.Add(t[k]);
                type2 = (int)t[k].w;
            }
            cost = distance(skeletonRef, skeleton);
            int min = Math.Min(DTW[i - 1, j], DTW[i, j - 1]);
            DTW[i, j] = cost + Math.Min(min, DTW[i - 1, j - 1]);

            
            //}

        }
        return DTW[n-1, m-1];
    }

    private static int distance(List<Vector4> skeletonRef, List<Vector4> skeleton)
    {
        int diferenceJoint = 0;
        int k = 0;
        for (int i = 0; i < skeletonRef.Count; ++i)
        {
            for (int j = k; j <= (int)skeletonRef[i].w; ++j)
            {
                if (j < skeleton.Count)
                {
                    //ha ugyanaz a csomopont
                    if ((int)skeletonRef[i].w == (int)skeleton[j].w)
                    {
                        float z = Math.Abs(skeletonRef[i].z - skeleton[j].z);
                        float x = Math.Abs(skeletonRef[i].x - skeleton[j].x);
                        float y = Math.Abs(skeletonRef[i].y - skeleton[j].y);
                        if (x > 0.4 || y > 0.4 || z > 0.4)
                        {
                            diferenceJoint++;
                        }
                        k = j + 1;
                    }
                }
            }
        }

        return diferenceJoint;
    }
}
