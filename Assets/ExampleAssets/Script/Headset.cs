using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Headset : MonoBehaviour
{

    public RSVP rsvp = null;

    Vector3 prevAngle;
    Vector3 currentAngle;

    float velocity;

    float[] history = new float[3];
    int currentHistoryIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        //transform.localRotation.ToAngleAxis(out angle, out axis);
        //currentAngle = axis * angle;
        currentAngle = transform.forward;
        prevAngle = currentAngle;
    }

    // Update is called once per frame
    void Update()
    {

        //transform.localRotation.ToAngleAxis(out angle, out axis);
        //currentAngle = axis * angle;
        currentAngle = transform.forward;

        AppendMomentum(Vector3.Angle(prevAngle, currentAngle) / Time.deltaTime);
        velocity = AverageVelocity();

        if (velocity > 275)
        {
            rsvp.VisibilityOff();
        }

        prevAngle = currentAngle;
        
    }

    void AppendMomentum(float velocity)
    {
        if (currentHistoryIndex >= history.Length)
        {
            currentHistoryIndex = 0;
        }

        history[currentHistoryIndex] = velocity;
        currentHistoryIndex++;

    }

    float AverageVelocity()
    {
        float sum = 0;

        for (int i=0; i<history.Length; i++)
        {
            sum += history[i];
        }

        return sum / history.Length;
    }
}
