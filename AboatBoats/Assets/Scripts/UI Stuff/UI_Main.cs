using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Main : MonoBehaviour
{
    public Scoring total_points;
    public Scoring_Two_Parts cur_points;
    public Textbox txbx;
    public float score_duration = 3f, comment_duration = 3f;
    public Print_Text points_text;
    private bool waiting_score_c = false, waiting_score_t = false, waiting_comment = false, adding_scores = false;
    private float waiting_s_duration = 0f, waiting_c_duration = 0f,points_to_add = 0;

    private void Start()
    {
        txbx.fin_anim_func = setWaitingScoreT;
    }

    public void setWaitingScoreT()
    {
        waiting_score_t = true;
    }

    public void AddPoints(int points, string text)
    {
        cur_points.reset_score();
        total_points.AddPoints(0);
        cur_points.AddPoints(points);
        points_text.SetText(text);
        waiting_s_duration = 0;
        waiting_c_duration = 0;
        waiting_score_c = true;
        waiting_score_t = false;
        waiting_comment = true;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L	))
        {
            AddPoints(100,"LOOKING COOL JOKER");
        }
        //Handle waiting for the text.
        if(waiting_comment)
        {
            if(points_text.IsFinished())
            { 
                waiting_c_duration += Time.deltaTime;
                if(waiting_c_duration >= comment_duration)
                {
                    points_text.RemoveText(true);
                    waiting_comment = false;
                }
            }
        }

        if (waiting_score_c)
        {
            if (cur_points.IsUpdatingScore())
            {
                waiting_s_duration += Time.deltaTime;

                if (waiting_s_duration > score_duration)
                {
                    cur_points.DisableScore();
                    waiting_score_c = false;
                    waiting_s_duration = 0;
                    adding_scores = true;
                }
            }
        }
        else if (adding_scores && !cur_points.IsOnScreen())
        {
            total_points.AddPoints(100);
            txbx.Animate();
            adding_scores = false;
        }
        //Handle waiting for the score total
        else if(waiting_score_t)
        { 
            if (total_points.IsUpdatingScore())
            {
                waiting_s_duration += Time.deltaTime;

                if (waiting_s_duration > score_duration)
                {
                    total_points.DisableScore();
                    waiting_score_t = false;
                }
            }
        }
    }

}
