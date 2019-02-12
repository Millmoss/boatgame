using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Assuming there's a tag for the boat
[RequireComponent(typeof(MeshCollider))]
public class Collidable_Water : MonoBehaviour
{
    private static string player_tag = "Player";
    public Water wt;
    private MeshCollider disable_onhit;

    private void Start()
    {
        disable_onhit = GetComponent<MeshCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == player_tag)
        {
            wt.Sink();
            disable_onhit.enabled = false;
        }
    }
}
