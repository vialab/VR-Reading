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
            decreaseWPM();
        }

        if (m_JoyRight.GetStateDown(m_Pose.inputSource))
        {
            increaseWPM();
        }
    }

    private void increaseWPM()
    {
        wpm += 10;
        updateWPM();
        StartCoroutine(visibility());
    }

    private void decreaseWPM()
    {
        if (wpm <= 10)
        {
            return;
        }
        wpm -= 10;
        updateWPM();
        StartCoroutine(visibility());
    }

    private void updateWPM()
    {
        wpmText.text = wpm + " WPM";
        m_RSVP.speed = 1.0f / (wpm / 60.0f);
    }

    private IEnumerator visibility()
    {
        m_Canvas.enabled = true;
        yield return new WaitForSecondsRealtime(1.0f);
        m_Canvas.enabled = false;
    }
}
