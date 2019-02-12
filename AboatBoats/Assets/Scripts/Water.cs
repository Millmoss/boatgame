using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float decrease_rate = 0.0f;
	public float seconds_cap = 10f;
    private bool decrease = false;
    public float bottom;
    private GameObject wtr;
	private float decrease_rate_rate;

    private void Start()
    {
        wtr = gameObject;
		decrease = true;
		decrease_rate_rate = 0;
	}

    public void Sink()
    {
        decrease = true;
    }

    void FixedUpdate()
    {
        if(decrease)
        {
			if (decrease_rate_rate < seconds_cap)
				decrease_rate_rate += Time.deltaTime;

			if (wtr.transform.position.y > bottom)
            {
                wtr.transform.position = new Vector3(
                    wtr.transform.position.x,
                    wtr.transform.position.y - (decrease_rate * Time.deltaTime * decrease_rate_rate / seconds_cap),
                    wtr.transform.position.z);
            }
        }
    }


}
