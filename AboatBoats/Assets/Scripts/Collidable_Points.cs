using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Assuming there's a tag for the boat
[RequireComponent(typeof(MeshCollider))]
public class Collidable_Points : MonoBehaviour
{
    private static string player_tag = "Player";
    public int point_value;
    public UI_Main score_main;
    public MonoBehaviour disable_onhit;
	public AudioSource ding;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == player_tag)
        {
            string txt = "";
            int rand = Random.Range(0, 3);
            switch (rand)
            {
                case 0:
                    txt = "Woah!";
                    break;
                case 1:
                    txt = "Cool!";
                    break;
                case 2:
                    txt = "Tubular!";
                    break;
            }

            score_main.AddPoints(point_value,txt);
            disable_onhit.enabled = false;
			other.gameObject.GetComponentInParent<Boat>().playDing();
        }
    }
}
