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
    private bool pause = true;
    private bool isRunning = false;
    private bool pauseGap = true;

    private TMP_Text m_textMeshPro;
    private Canvas m_Canvas;

    public float speed = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        m_Canvas = GetComponent<Canvas>();
        m_textMeshPro = GetComponentInChildren<TMP_Text>();

        m_Canvas.enabled = isVisible;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Split the text from each word. Each word is then added to the list.
    public void EnableRSVP(string text)
    {
        //If the content is the same.
        if (text.Equals(content))
        {

            pauseGap = true;
            ToggleVisibility();

            return;
        }

        pauseGap = true;
        SplitText(text);
        VisibilityOn();

        //Stop routine if its already running (Otherwise it will double the speed)
        if (!isRunning)
        {
            StartCoroutine(RapidText());
        }
    }

    public void ToggleVisibility()
    {
        pause = !pause;
        isVisible = !isVisible;
        m_Canvas.enabled = isVisible;
    }

    public void VisibilityOff()
    {
        pause = true;
        isVisible = false;
        m_Canvas.enabled = isVisible;
    }

    public void VisibilityOn()
    {
        pause = false;
        isVisible = true;
        m_Canvas.enabled = isVisible;
    }

    private void SplitText(string text)
    {
        //Variable assignments
        content = text;
        word = text.Split(null);
        m_textMeshPro.text = word[0];
        iteration = 0;
        maxIteration = word.Length - 1;
    }

    private IEnumerator RapidText()
    {
        //Inidicate the rapidtext is running
        isRunning = true;

        while (iteration < maxIteration)
        {
            //if it is paused, then wait.
            while (pause)
            {
                yield return new WaitForEndOfFrame();
            }

            //Introduce a little of time for the user to adjust to the location of RSVP.
            if (pauseGap)
            {
                pauseGap = false;
                yield return new WaitForSecondsRealtime(1.0f);
            }

            
            iteration += 1;
            m_textMeshPro.text = word[iteration];
            yield return new WaitForSecondsRealtime(speed);


        }

        //Once the text reaches the end, indicate it is already stopped.
        isRunning = false;
    }
}
