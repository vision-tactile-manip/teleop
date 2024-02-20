using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Geometry;

public class cube_move : MonoBehaviour
{
    GameObject test_box;
    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<TwistMsg>("cube_vel", cube_teleop);
        test_box = GameObject.Find("test_box");
        
    }


    void cube_teleop(TwistMsg cube_vel)
    {   
        test_box.GetComponent<Rigidbody>().velocity = new Vector3((float)cube_vel.linear.x, (float)cube_vel.linear.y, (float)cube_vel.linear.z);
        test_box.GetComponent<Rigidbody>().angularVelocity = new Vector3((float)cube_vel.angular.x, (float)cube_vel.angular.y, (float)cube_vel.angular.z);
    }
}
