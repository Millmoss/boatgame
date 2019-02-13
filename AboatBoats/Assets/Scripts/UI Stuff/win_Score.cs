using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class win_Score : MonoBehaviour
{
    public Text txt;

    private void Start()
    {
        txt.text = "winr is you\nSCORE IS " +UI_Main.total_score;
    }

}
