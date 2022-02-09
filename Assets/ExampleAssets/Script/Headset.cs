using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Headset : MonoBehaviour
{

    private Rigidbody m_RigidBody = null;
    //private SteamVR_Behaviour_Pose m_Headset = null;

    // Start is called before the first frame update
    void Start()
    {
        //m_Headset = GetComponent<SteamVR_Behaviour_Pose>();
        m_RigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //print(m_Headset.GetAngularVelocity().sqrMagnitude);
        //print(m_RigidBody.angularVelocity.sqrMagnitude);
        print(m_RigidBody.velocity.sqrMagnitude);
    }
}
