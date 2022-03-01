using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CustomHand : MonoBehaviour
{
    public SteamVR_Action_Boolean m_GrabAction = null;
    public SteamVR_Action_Boolean m_ButtonAction = null;
    public RSVP m_RSVP = null;
    public Pointer m_Pointer = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private FixedJoint m_Joint = null;
    private SteamVR_RenderModel m_Controller = null;
    private GameObject m_PointerObject = null;
    private bool isRightHand = true;

    public CustomInteractable m_CurrentInteractable = null;
    //Public for debugging purposes in unity.
    public List<CustomInteractable> m_ContactInteractables = new List<CustomInteractable>();

    private void Awake()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Joint = GetComponent<FixedJoint>();
        m_Controller = GetComponentInChildren<SteamVR_RenderModel>();
        m_PointerObject = GameObject.Find("PR_Pointer");
        //Check if this is the right hand (The pointer)
        if (gameObject.name.Equals("Controller (left)"))
        {
            isRightHand = false;
        }
    }

    private void Update()
    {
        //Grip Down
        if (m_GrabAction.GetStateDown(m_Pose.inputSource))
        {
            //print(m_Pose.inputSource + "Grip Down");
            Pickup();
        }

        //Enable RSVP by pointing at it and press the trigger button
        if (isRightHand 
            && m_ButtonAction.GetStateDown(m_Pose.inputSource) 
            && m_Pointer.m_HitInteractable
            && !m_CurrentInteractable)
        {
            m_RSVP.EnableRSVP(m_Pointer.m_HitInteractable.GetText());
        }

        //If item is being held and trigger is down
        if (m_CurrentInteractable 
            && m_ButtonAction.GetStateDown(m_Pose.inputSource))
        {
            m_RSVP.EnableRSVP(m_CurrentInteractable.GetText());

        }

        //If controller isnt pointing or grabbing anything, toggle between visibility.
        if (!m_CurrentInteractable 
            && !m_RSVP.isContentEmpty()
            && !m_Pointer.m_HitInteractable 
            && m_ButtonAction.GetStateDown(m_Pose.inputSource))
        {
            m_RSVP.ToggleVisibility();
        }

        //Grip Up
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

        bool distanceGrab = false;
        //Check if there is a paper near your hand
        if (!m_CurrentInteractable)
        {
            //Check if the pointer is empty or the grab is left hand
            if (!m_Pointer.m_HitInteractable || !isRightHand)
            {
                return;
            }
            distanceGrab = true;
            m_CurrentInteractable = m_Pointer.m_HitInteractable;
        }

        //Check if it is already held
        if (m_CurrentInteractable.m_ActiveHand)
        {
            m_CurrentInteractable.m_ActiveHand.Drop();
        } 
        
        //If there isnt paper near hand but there is at the distance, grab that
        if (distanceGrab)
        {
            m_CurrentInteractable.transform.position = transform.position;
        } 

        //Set hands invisible
        m_Controller.SetMeshRendererState(false);

        //If it is right hand, then turn the pointer invisible
        if (isRightHand)
        {
            m_PointerObject.SetActive(false);
        }

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
        //Look at the Angular Momentum formula, it returns the square value instead of root value
        int flickThreshold = 50;
        if(m_Pose.GetAngularVelocity().sqrMagnitude > flickThreshold)
        {
            //print("Flick");
            m_CurrentInteractable.transform.SetPositionAndRotation(m_CurrentInteractable.startingPoint, m_CurrentInteractable.startingRotation);
        }

        //Set hands and pointer to visible
        m_Controller.SetMeshRendererState(true);
        m_PointerObject.SetActive(true);

        //Detach
        m_Joint.connectedBody = null;

        //Clear
        m_CurrentInteractable.m_ActiveHand = null;
        m_CurrentInteractable = null;

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
