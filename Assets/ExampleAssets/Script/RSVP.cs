using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RSVP : MonoBehaviour
{
    private string content;
    private bool isVisible = false;
    private string[] word;
    private int iteration;
    private int maxIteration;

    private TMP_Text m_textMeshPro;
    private Canvas m_Canvas;

    

    // Start is called before the first frame update
    void Start()
    {
        m_Canvas = GetComponent<Canvas>();
        m_textMeshPro = GetComponentInChildren<TMP_Text>();

        m_Canvas.enabled = isVisible;

        InvokeRepeating("RapidText", 0.03f, 0.3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Split the text from each word. Each word is then added to the list.
    public void SplitText(string text)
    {
        //If the content is the same, dont do anything.
        if (text.Equals(content))
        {
            return;
        }

        content = text;
        word = text.Split(null);
        m_textMeshPro.text = word[0];
        iteration = 0;
        maxIteration = word.Length - 1;
    }

    public void ToggleVisibility()
    {
        isVisible = !isVisible;
        m_Canvas.enabled = isVisible;
    }

    public void RapidText()
    {
        //If already at max, stop.
        if (iteration == maxIteration)
        {
            return;
        }
        iteration += 1;
        m_textMeshPro.text = word[iteration];
    }
}
