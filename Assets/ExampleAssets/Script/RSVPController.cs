using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Valve.VR;

public class RSVPController : MonoBehaviour
{
    public SteamVR_Action_Boolean m_JoyLeft = null;
    public SteamVR_Action_Boolean m_JoyRight = null;
    public TMP_Text wpmText = null;
    public RSVP m_RSVP = null;

    private SteamVR_Behaviour_Pose m_Pose = null;
    private Canvas m_Canvas = null;
    
    private int wpm = 200;
    private bool joystate = false;
    private bool increaseState = false;
    private bool mashJoy = false;
    private bool isVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        m_Pose = GetComponent<SteamVR_Behaviour_Pose>();
        m_Canvas = GetComponentInChildren<Canvas>();

        m_Canvas.enabled = false;

        updateWPM();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_JoyLeft.GetStateDown(m_Pose.inputSource))
        {
            joystate = true;
            mashJoy = true;

            decreaseWPM();
            //To avoid double execution
            if (!isVisible)
            {
                StartCoroutine(visibility());
            }
            
        }

        if (m_JoyLeft.GetStateUp(m_Pose.inputSource))
        {
            joystate = false;
        }

        if (m_JoyRight.GetStateDown(m_Pose.inputSource))
        {
            joystate = true;
            increaseState = true;
            mashJoy = true;

            increaseWPM();
            //To avoid double execution
            if (!isVisible)
            {
                StartCoroutine(visibility());
            }
        }

        if (m_JoyRight.GetStateUp(m_Pose.inputSource))
        {
            joystate = false;
            increaseState = false;
        }
    }

    private void increaseWPM()
    {
        wpm += 10;
        updateWPM();
    }

    private void decreaseWPM()
    {
        if (wpm <= 10)
        {
            return;
        }
        wpm -= 10;
        updateWPM();
    }

    private void updateWPM()
    {
        wpmText.text = wpm + " WPM";
        m_RSVP.speed = 1.0f / (wpm / 60.0f);
    }

    private IEnumerator visibility()
    {
        while (mashJoy)
        {
            m_Canvas.enabled = true;
            mashJoy = false;
            isVisible = true;

            //Little pause before it starts changing
            yield return new WaitForSecondsRealtime(0.2f);

            while (joystate)
            {
                //If the user is holding down the stick
                if (increaseState)
                {
                    increaseWPM();
                }
                else
                {
                    decreaseWPM();
                }
                //The speed at which the wpm is being updated
                yield return new WaitForSecondsRealtime(0.1f);
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }

        isVisible = false;
        m_Canvas.enabled = false;
    }
}
