using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float decrease_rate = 0.0f;
    private bool decrease = false;
    public float bottom;
    private GameObject wtr;


    private void Start()
    {
        wtr = gameObject;
    }

    public void Sink()
    {
        decrease = true;
    }

    private void Update()
    {
        if(decrease)
        {
            if(wtr.transform.position.y > bottom)
            {
                wtr.transform.position = new Vector3(
                    wtr.transform.position.x,
                    wtr.transform.position.y - decrease_rate * Time.deltaTime,
                    wtr.transform.position.z);
            }
        }
    }


}
