using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Quest2RosMsg;

public class teleop : MonoBehaviour
{
    ROSConnection ros;
    public string ovr_inputs_topicName = "ovr_inputs";
    public string ovr_right_hand_vel_topicName = "ovr_right_hand_vel";    
    public string ovr_left_hand_vel_topicName = "ovr_left_hand_vel";

    public float publishMessageFrequency = 0.001f; //limited by the VR framerate
    private float timeElapsed;

    GameObject test_box;
    GameObject OVRPlayerController;
    GameObject OVRCameraRig;
    GameObject RightControllerAnchor;
    GameObject LeftControllerAnchor;
    Transform baseTransform;

    // Start is called before the first frame update
    void Start()
    {
        test_box = GameObject.Find("test_box");
        OVRPlayerController = GameObject.Find("OVRPlayerController");
        OVRCameraRig = GameObject.Find("OVRCameraRig");
        RightControllerAnchor = GameObject.Find("RightControllerAnchor");        
        LeftControllerAnchor = GameObject.Find("LeftControllerAnchor");
        baseTransform = OVRCameraRig.transform;

    // start the ROS connection
        ros = ROSConnection.GetOrCreateInstance();
        ros.RegisterPublisher<OVRbuttonsMsg>(ovr_inputs_topicName);
        ros.RegisterPublisher<OVRHandPosRotVelMsg>(ovr_right_hand_vel_topicName);
        ros.RegisterPublisher<OVRHandPosRotVelMsg>(ovr_left_hand_vel_topicName);       


    }

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > publishMessageFrequency)
        {            

            //send contler inputs to ros
            detect_controller_inputs();

            //send right hand position and rotation (euler) to ros
            right_controler_vel();
            left_controler_vel();
            timeElapsed=0;

            
        }


        // reset the cube to initial position by pressing A & B (on right controler)
         if ((OVRInput.Get(OVRInput.Button.One)) && (OVRInput.Get(OVRInput.Button.Two)))
            {   
              test_box.transform.position = new Vector3(0, 1, 0.5f);
            }

     
        
    }


    void detect_controller_inputs()
    {   
        //right hand
        bool rh_one_a = OVRInput.Get(OVRInput.Button.One);
        bool rh_two_b = OVRInput.Get(OVRInput.Button.Two);
        var rh_as = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick); 
        float rh_asx = rh_as[0];
        float rh_asy = rh_as[1];
        float rh_st = OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger); 
        float rh_lt = OVRInput.Get(OVRInput.Axis1D.SecondaryHandTrigger); 
        //left hand
        bool lh_three_x = OVRInput.Get(OVRInput.Button.Three);
        bool lh_four_y = OVRInput.Get(OVRInput.Button.Four);
        var lh_as = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick); 
        float lh_asx = lh_as[0];
        float lh_asy = lh_as[1];
        float lh_st = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger);
        float lh_lt = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger);

        OVRbuttonsMsg iovr_inputs = new OVRbuttonsMsg(
                rh_one_a,
                rh_two_b,
                rh_asx,
                rh_asy,
                rh_st,
                rh_lt,
                lh_three_x,
                lh_four_y,
                lh_asx,
                lh_asy,
                lh_st,
                lh_lt
                );

        //puplish the inputs
        ros.Publish(ovr_inputs_topicName, iovr_inputs);

    }

  

     void right_controler_vel()
    {

        var rh_vel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch);
        rh_vel = baseTransform.TransformDirection(rh_vel);
        var rh_vel_ang = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.RTouch);
        rh_vel_ang = baseTransform.TransformDirection(rh_vel_ang);
        OVRHandPosRotVelMsg right_hand_vel = new OVRHandPosRotVelMsg(
                 rh_vel[0],
                rh_vel[1],
                rh_vel[2],
                rh_vel_ang[0],
                rh_vel_ang[1],
                rh_vel_ang[2]
                );
        // Finally send the message to server_endpoint.py running in ROS
        ros.Publish(ovr_right_hand_vel_topicName, right_hand_vel);

    }

     void left_controler_vel()
    {

        var lh_vel = OVRInput.GetLocalControllerVelocity(OVRInput.Controller.LTouch);
        lh_vel = baseTransform.TransformDirection(lh_vel);
        var lh_vel_ang = OVRInput.GetLocalControllerAngularVelocity(OVRInput.Controller.LTouch);
        lh_vel_ang = baseTransform.TransformDirection(lh_vel_ang);
        OVRHandPosRotVelMsg left_hand_vel = new OVRHandPosRotVelMsg(
                 lh_vel[0],
                lh_vel[1],
                lh_vel[2],
                lh_vel_ang[0],
                lh_vel_ang[1],
                lh_vel_ang[2]
                );
        // Finally send the message to server_endpoint.py running in ROS
        ros.Publish(ovr_left_hand_vel_topicName, left_hand_vel);

    }
}
