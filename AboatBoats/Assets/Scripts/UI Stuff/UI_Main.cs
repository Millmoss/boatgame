using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    public Scoring points_score;
    public float duration = 3f;
    private float waiting_duration = 0f;
    public Print_Text points_text;
    private bool waiting = false;
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            points_score.AddPoints(100);
            points_text.SetText("Looking Cool");
            waiting_duration = 0;
        }

        if (points_score.IsUpdatingScore())
        {
            waiting_duration += Time.deltaTime;

            if (waiting_duration > duration)
            { 
                points_score.DisableScore();
                points_text.RemoveText(true);
            }
        }
    }

}
