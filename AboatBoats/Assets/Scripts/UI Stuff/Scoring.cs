using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Text))]
public class Scoring : MonoBehaviour
{
    private Text display;
    public float speed = 1f;
    private bool on_screen = false, moving = false, waiting = false, active = false;
    public Vector3 end_pos;
    private Vector3 init_pos;
    private float leaving_duration = 0f;

    private int current_score = 0, wanted_score = 0;

    private void Start()
    {
        display = GetComponent<Text>();
        init_pos = display.gameObject.GetComponent<RectTransform>().anchoredPosition;
    }
    public void AddPoints(int points)
    {
        leaving_duration = 0;
        wanted_score += points;
        active = true;
        if (moving && on_screen == true)
            on_screen = false;
        if (!on_screen)
            moving = true;
    }

    public void DisableScore()
    {
        if(on_screen == true)
            moving = true;
    }

    public bool IsUpdatingScore()
    {
        return current_score == wanted_score;
    }

    private void Update()
    {
        if (active)
        {
            if (current_score < wanted_score)
                current_score++;
            else if (current_score > wanted_score)
                current_score--;
            display.text = current_score + " Points";

        }

        if(moving)
        {
            if(!on_screen)
            {
                display.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(
                    display.gameObject.GetComponent<RectTransform>().anchoredPosition,
                    end_pos,
                    Time.deltaTime * speed);

                if(Vector3.Distance(display.gameObject.GetComponent<RectTransform>().anchoredPosition, end_pos) < 0.35f)
                {
                    moving = false;
                    on_screen = true;
                    display.gameObject.GetComponent<RectTransform>().anchoredPosition = end_pos;
                    waiting = true;
                }
            }
            else
            {
                leaving_duration += Time.deltaTime / 2.5f;
                display.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(
                    display.gameObject.GetComponent<RectTransform>().anchoredPosition,
                    init_pos,
                    leaving_duration);

                if (Vector3.Distance(display.gameObject.GetComponent<RectTransform>().anchoredPosition, init_pos) < 0.35f)
                {
                    moving = false;
                    on_screen = false;
                    display.gameObject.GetComponent<RectTransform>().anchoredPosition = init_pos;
                    waiting = false;
                    active = false;
                }
                
            }
                
        }
    }


}
