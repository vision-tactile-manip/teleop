using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Quest2RosMsg;

public class habtic_feedback : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<HapticFeedbackMsg>("haptic_feedback", TactiletoHaptic);
        
    }


    void TactiletoHaptic(HapticFeedbackMsg hapticMessage)
    {   
        if (hapticMessage.handid=="right")
            OVRInput.SetControllerVibration(hapticMessage.frequency, hapticMessage.amplitude, OVRInput.Controller.RTouch);
        if (hapticMessage.handid=="left")
            OVRInput.SetControllerVibration(hapticMessage.frequency, hapticMessage.amplitude, OVRInput.Controller.LTouch);
    }

 
}
