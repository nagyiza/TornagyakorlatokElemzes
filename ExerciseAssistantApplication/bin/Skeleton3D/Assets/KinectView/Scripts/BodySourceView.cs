using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kinect = Windows.Kinect;
using System;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using Assets.KinectView.Scripts;

//this class is in Unity Pro packages from microsoft page
/// <summary>
/// The class displays the body
/// </summary>
public class BodySourceView : MonoBehaviour
{
    /// <summary>
    /// The body'skRef bone material
    /// </summary>
    public Material BoneMaterial;
    /// <summary>
    /// A game object
    /// </summary>
    public GameObject BodySourceManager;
    /// <summary>
    /// This is a body with skeleton'skRef joints
    /// </summary>
    public Kinect.Body body;
    /// <summary>
    /// This is a game object
    /// </summary>
    public GameObject bodyObject;
    /// <summary>
    /// Result textbox for result with scatter method
    /// </summary>
    public Text result;
    /// <summary>
    /// Result textbox for result with angles method
    /// </summary>
    public Text result2;
    /// <summary>
    /// The bone witch is not correct (scatter mathod)
    /// </summary>
    public string errorjointType = "";
    /// <summary>
    /// The bone witch is not correct (angles method)
    /// </summary>
    public string errorjointTypeAngles = "";
    /// <summary>
    /// In this object is bodies.
    /// </summary>
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    /// <summary>
    /// A manager witch manage the body frame
    /// </summary>
    private BodySourceManager _BodyManager;

    /// <summary>
    /// Name of the exercise witch get from input.
    /// </summary>
    private String exerciseName = "";
    /// <summary>
    /// Name of the exercise in witch write unity data.
    /// </summary>
    private String lastExerciseName = "";

    /// <summary>
    /// The path in which is the reference skeleton data
    /// </summary>
    private string path = "";// @"C:\Users\Izabella\Documents\Visual Studio 2015\Projects\ExerciseAssistantWithKinectV2\TornagyakorlatokElemzese\ReferenceData\gugolas2.txt";

    /// <summary>
    /// The path in which write the user koordinates (unity koordinates)
    /// </summary>
    private string writePath = @"..\..\..\UnityData\";

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
        exerciseName = path;

        path = @"..\..\..\ReferenceData\" + path + ".txt";

        Debug.Log("The path is BodySourceView " + path);

        //processing the skeleton data
        ReferenceSkeleton();
    }

    void Start()
    {

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
                        if (File.Exists(writePath + "User\\" + exerciseName + ".txt"))
                        {
                            for (int i = 1; ; ++i)
                            {
                                if (!File.Exists(writePath + "User\\" + exerciseName + i + ".txt"))
                                {
                                    lastExerciseName = exerciseName + i;
                                    break;
                                }
                            }
                        }
                        File.WriteAllText(writePath + "User\\" + lastExerciseName + ".txt", "X              " + "Y         " + "Z     " + " JointType " + Environment.NewLine);

                        //if the body is oke, create the body object
                        _Bodies[body.TrackingId] = CreateBodyObject(body.TrackingId);
                    }
                    //refresh the body object
                    RefreshBodyObject(body, _Bodies[body.TrackingId]);
                    this.body = body;
                    this.bodyObject = _Bodies[body.TrackingId];
                }
            }
        }
        else
        {
            //default path
            path = @"..\..\..\ReferenceData\allas.txt";

        }
    }

    /// <summary>
    /// Create the body object, and displays the body'skRef joints
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
            //create a body'skRef joint, witch is a cube
            GameObject jointObj = GameObject.CreatePrimitive(PrimitiveType.Cube);

            //displays the joint (the cube)
            LineRenderer lr = jointObj.AddComponent<LineRenderer>();
            lr.SetVertexCount(2);
            lr.material = BoneMaterial;
            lr.SetWidth(0.2f, 0.2f); //cube'skRef size

            jointObj.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            jointObj.name = jt.ToString(); // the game object'skRef name is the joint'skRef name
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
            //the jt'skRef pair
            Kinect.Joint? targetJoint = null;

            if (_BoneMap.ContainsKey(jt))
            {
                //the jt'skRef pair from the bone map
                targetJoint = body.Joints[_BoneMap[jt]];
            }

            // the game object'skRef joint
            Transform jointObj = bodyObject.transform.Find(jt.ToString());
            //body joint position
            jointObj.localPosition = GetVector3FromJoint(sourceJoint);

            //drawing bone
            LineRenderer lr = jointObj.GetComponent<LineRenderer>();
            if (targetJoint.HasValue)
            {
                if (errorjointType == jt.ToString() || errorjointTypeAngles == jt.ToString())
                {
                    lr.SetColors(Color.red, Color.red);
                    //size
                    lr.SetWidth(0.5f, 0.5f);
                }
                else
                {
                    //the joint'skRef color, red (not tracked) or green(tracked) or black
                    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                    //size
                    lr.SetWidth(0.2f, 0.2f);
                }
                //source joint
                lr.SetPosition(0, jointObj.localPosition);
                //target joint
                lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));

                //get target joint'skRef position
                Vector3 v = GetVector3FromJoint(targetJoint.Value);

                if (targetJoint.Value.TrackingState == Kinect.TrackingState.Tracked)
                {
                    //write user coordinates in a file
                    File.AppendAllText(writePath + "User\\" + lastExerciseName + ".txt", sourceJoint.Position.X + " " + sourceJoint.Position.Y + " " + sourceJoint.Position.Z + " " + (int)jt + " " + jt.ToString() + Environment.NewLine);

                    Vector3 pos = new Vector3(sourceJoint.Position.X, sourceJoint.Position.Y, sourceJoint.Position.Z);
                    j++;//skeleton'skRef position counter
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

        if (reference.Count != 0)
        {
            Compare cmp = new Compare(reference, userSkeleton, exerciseName);
            
            if (cmp.dtwResult[0] == 0) // dtwResult[0] - result with scatter
            {
                result.text = "Good";
                result.color = Color.green;
                errorjointType = "";
            }else
            {
                result.text = cmp.errorjointType;
                result.color = Color.red;
            }

            if (cmp.dtwResult[1] == 0) // dtwResult[0] - result with scatter
            {
                result2.text = "Good";
                result2.color = Color.green;
                errorjointTypeAngles = "";
            }
            else
            {
                result2.text = cmp.errorjointTypeAngles;
                result2.color = Color.red;
            }

        }
        userSkeleton = new List<Vector4>();

    }

    /// <summary>
    /// Get pen'skRef color
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
    /// Reference skeleton'skRef position count
    /// </summary>
    private int skeletonRefCount = 0;
    /// <summary>
    /// User skeleton'skRef position count
    /// </summary>
    private static int j = 0;

    private void ReferenceSkeleton()
    {
        File.WriteAllText(writePath + "Reference\\" + exerciseName + "Ref.txt", "   X       Y         Z      JointType" + Environment.NewLine);
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

                    //write reference coordinates in a file
                    File.AppendAllText(writePath + "Reference\\" + exerciseName + "Ref.txt", position.x + " " + position.y + " " + position.z + " " + position.w + Environment.NewLine);

                    if (int.Parse(words[5]) <= type)
                    {
                        skeletonRefCount++;
                    }
                    type = int.Parse(words[5]);
                }

            }
        }
        catch (Exception e)
        {

        }
    }

    ///// <summary>
    ///// Dtw algorithm matrix
    ///// </summary>
    ////private static  DTW;
    //public int DTWDistance(List<Vector4> skRef, List<Vector4> sk, int skeletonRefCount)
    //{
    //    //one reference skeleton position(24 joint)
    //    List<Vector4> skeletonRef = new List<Vector4>();
    //    List<Vector4> skeleton = new List<Vector4>();

    //    int userSkeletonCount = 2;
    //    j = 1;

    //    int[,] DTW = new int[skeletonRefCount, userSkeletonCount];
    //    int cost;

    //    for (int i = 0; i < skeletonRefCount; ++i)
    //    {
    //        DTW[i, 0] = 100000;
    //    }
    //    for (int i = 0; i < userSkeletonCount; ++i)
    //    {
    //        DTW[0, i] = 100000;
    //    }
    //    DTW[0, 0] = 0;

    //    for (int i = 1; i < skeletonRefCount; ++i)
    //    {
    //        //for (int j = 1; j < userSkeletonCount; ++j)
    //        //{
    //        skeletonRef = new List<Vector4>();
    //        skeleton = new List<Vector4>();
    //        int type = (int)skRef[0].w;

    //        for (int k = 0; k < 25; ++k)
    //        {
    //            if ((int)skRef[k].w < type)
    //            {
    //                break;
    //            }
    //            skeletonRef.Add(skRef[k]);
    //            type = (int)skRef[k].w;
    //        }
    //        int type2 = (int)sk[0].w;
    //        for (int k = 0; k < 25; ++k)
    //        {
    //            if ((int)sk[k].w < type2)
    //            {
    //                break;
    //            }
    //            skeleton.Add(sk[k]);
    //            type2 = (int)sk[k].w;
    //        }
    //        cost = distance(skeletonRef, skeleton);
    //        int min = Math.Min(DTW[i - 1, j], DTW[i, j - 1]);
    //        DTW[i, j] = cost + Math.Min(min, DTW[i - 1, j - 1]);


    //        //}

    //    }
    //    return DTW[skeletonRefCount - 1, userSkeletonCount - 1];
    //}

    //private int distance(List<Vector4> skeletonRef, List<Vector4> skeleton)
    //{
    //    int diferenceJoint = 0;
    //    int k = 0;
    //    for (int i = 0; i < skeletonRef.Count; ++i)
    //    {
    //        for (int j = k; j <= (int)skeletonRef[i].w; ++j)
    //        {
    //            if (j < skeleton.Count)
    //            {
    //                //ha ugyanaz a csomopont
    //                if ((int)skeletonRef[i].w == (int)skeleton[j].w)
    //                {
    //                    float z = Math.Abs(skeletonRef[i].z - skeleton[j].z);
    //                    float x = Math.Abs(skeletonRef[i].x - skeleton[j].x);
    //                    float y = Math.Abs(skeletonRef[i].y - skeleton[j].y);
    //                    if (x > 0.2 || y > 0.2 || z > 0.2)
    //                    {
    //                        diferenceJoint++;
    //                        // without hand tip and without foot, because these are not important
    //                        if ((int)skeleton[j].w != 23 // HandTipRight
    //                        && (int)skeleton[j].w != 24 // ThumbRight
    //                        && (int)skeleton[j].w != 21 // HandTipLeft
    //                        && (int)skeleton[j].w != 22 // ThumbLeft
    //                        && (int)skeleton[j].w != 19 // FootRight
    //                        && (int)skeleton[j].w != 15  // FootLeft
    //                        && (int)skeleton[j].w != 7
    //                        && (int)skeleton[j].w != 11
    //                        && (int)skeleton[j].w != 18
    //                        && (int)skeleton[j].w != 14)
    //                        {
    //                            errorjointType = getJoinType((int)skeleton[j].w).ToString();

    //                            result.text = errorjointType;
    //                            result.color = Color.red;
    //                        }
    //                    }
    //                    k = j + 1;
    //                }
    //            }
    //        }
    //    }

    //    return diferenceJoint;
    //}


    /// <summary>
    /// Get joinType string
    /// </summary>
    /// <param name="jointType"></param>
    /// <returns></returns>
    private static Kinect.JointType getJoinType(int jointType)
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
}
