using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CustomHand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_ButtonAction = null;
    public RSVP m_RSVP = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;
    private SteamVR_RenderModel m_Controller = null;
    private GameObject m_Pointer = null;

    private CustomInteractable m_CurrentInteractable = null;
    //Public for debugging purposes in unity.
    public List<CustomInteractable> m_ContactInteractables = new List<CustomInteractable>();

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
        m_Controller = GetComponentInChildren<SteamVR_RenderModel>();
        m_Pointer = GameObject.Find("PR_Pointer");
    }

    private void Update()
    {
        //Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            //print(m_Pose.inputSource + "Grip Down");
            Pickup();
        }

        //If item is being held and trigger is down
        if (m_CurrentInteractable != null && m_ButtonAction.GetStateDown(m_Pose.inputSource))
        {
            //Contact the RSVP class
            m_RSVP.ToggleVisibility();
            m_RSVP.SplitText(m_CurrentInteractable.GetText());

        }

        //Up
        if (m_GrabAction.GetStateUp(m_Pose.inputSource))
        {
            //print(m_Pose.inputSource + "Grip Up");
            Drop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
        {
            return;
        }

        m_ContactInteractables.Add(other.gameObject.GetComponent<CustomInteractable>());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.CompareTag("Interactable"))
        {
            return;
        }

        m_ContactInteractables.Remove(other.gameObject.GetComponent<CustomInteractable>());
    }

    public void Pickup()
    {
        //Get Nearest
        m_CurrentInteractable = GetNearestInteractable();
        
        //Null Check
        if (!m_CurrentInteractable)
        {
            return;
        }

        //Already Held, check
        if (m_CurrentInteractable.m_ActiveHand)
        {
            m_CurrentInteractable.m_ActiveHand.Drop();
        }

        //Position (if you need to move the object to the center)
        /*
        m_CurrentInteractable.transform.position = transform.position;
        */
        
        //If the paper is too far, teleports it to your hand
        float distance = (m_CurrentInteractable.transform.position - transform.position).sqrMagnitude;
        //The distance needs to be tweaked.
        if (distance > 0.2f)
        {
            m_CurrentInteractable.transform.position = transform.position;
        }
        

        //Set hands invisible
        m_Controller.SetMeshRendererState(false);
        m_Pointer.SetActive(false);

        //Attach
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        m_Joint.connectedBody = targetBody;

        //Set active hand
        m_CurrentInteractable.m_ActiveHand = this;
    }

    public void Drop()
    {
        //Null check
        if (!m_CurrentInteractable)
        {
            return;
        }

        //Apply Velocity (if you want to add physics velocity)
        /*
        Rigidbody targetBody = m_CurrentInteractable.GetComponent<Rigidbody>();
        targetBody.velocity = m_Pose.GetVelocity();
        targetBody.angularVelocity = m_Pose.GetAngularVelocity();
        */

        //Flick function to send the paper back to its original
        if(m_Pose.GetAngularVelocity().magnitude > 7)
        {
            //print("Flick");
            m_CurrentInteractable.transform.SetPositionAndRotation(m_CurrentInteractable.startingPoint, m_CurrentInteractable.startingRotation);
        }

        //Set hands and pointer to visible
        m_Controller.SetMeshRendererState(true);
        m_Pointer.SetActive(true);

        //Detach
        m_Joint.connectedBody = null;

        //Clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;

        //This is just a patch. NEED TO FIND A BETTER SOLUTION.
        m_ContactInteractables.Clear();
    }

    private CustomInteractable GetNearestInteractable()
    {
        CustomInteractable nearest = null;
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(CustomInteractable interactable in m_ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }

        return nearest;
    }
}
