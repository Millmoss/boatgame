using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Print_Text : MonoBehaviour
{

    private Text textbox;
    private bool unwrite_l2r = false;
    bool text_finished_editing = true;
    private string leftover_text = "";

    private void Start()
    {
        textbox = GetComponent<Text>();
    }

    //Returns if it finishes editing the text.
    public bool IsFinished()
    {
        return text_finished_editing;
    }

    private IEnumerator writeText()
    {
        char tmp = leftover_text[0];
        textbox.text += leftover_text[0];
        leftover_text = leftover_text.Substring(1);

        if (tmp == '\n')
        {
            yield return new WaitForSeconds(0.6525f);
        }
        else
        {
            yield return new WaitForSeconds(0.1225f);
        }

        if (leftover_text.Length > 0)
        {
            yield return writeText();
        }
        else
        {
            text_finished_editing = true;
        }
    }

    private IEnumerator unwriteText()
    {
        
        if(!unwrite_l2r)
        {
            textbox.text = textbox.text.Substring(0, textbox.text.Length - 2);
        }
        else
        {
            textbox.text = textbox.text.Substring(1, textbox.text.Length-1);
        }

        yield return new WaitForSeconds(0.6525f);


        if (textbox.text.Length > 0)
        {
            yield return unwriteText();
        }
        else
        {
            text_finished_editing = true;
        }
    }

    //bool : Should the text be removed left to right or vise versa?
    public void RemoveText(bool isL2R)
    {
        StopAllCoroutines();
        unwrite_l2r = isL2R;
        //StartCoroutine(unwriteText());
    }

    public void SetText(string txt)
    {
        StopAllCoroutines();
        if (txt == "")
        { 
            textbox.text = "";
            return;
        }
        if (!text_finished_editing)
        {
            StopCoroutine("writeText");
            textbox.text += leftover_text;
            leftover_text = "";
            text_finished_editing = true;
        }
        else
        {
            leftover_text = txt;
            textbox.text = "";
            text_finished_editing = false;
            StartCoroutine("writeText");
        }
    }

}
