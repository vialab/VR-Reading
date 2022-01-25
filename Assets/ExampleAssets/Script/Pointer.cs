using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pointer : MonoBehaviour
{
    public float m_DefaultLength = 5.0f;
    public GameObject m_Dot;
    //public VRInputModule m_inputModule;

    private LineRenderer m_LineRenderer = null;

    private void Awake()
    {
        m_LineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLine();
    }

    private void UpdateLine()
    {
        //use default or distance
        float targetLength = m_DefaultLength;

        //Set invisible if it doesnt hit anything
        m_Dot.GetComponent<MeshRenderer>().enabled = false;

        //Raycast
        RaycastHit hit = CreateRaycast(targetLength);

        //Default
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        //Or based on hit
        if(hit.collider != null)
        {
            endPosition = hit.point;

            //Set to visible if it hits
            m_Dot.GetComponent<MeshRenderer>().enabled = true;
        }

        //Set Position of the dot
        m_Dot.transform.position = endPosition;

        //Set line renderer
        m_LineRenderer.SetPosition(0, transform.position);
        m_LineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, m_DefaultLength);

        return hit;
    }
}
