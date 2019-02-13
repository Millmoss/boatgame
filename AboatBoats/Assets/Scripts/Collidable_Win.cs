using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

//Assuming there's a tag for the boat
[RequireComponent(typeof(MeshCollider))]
public class Collidable_Win : MonoBehaviour
{
    private static string player_tag = "Player";
    public AudioSource ding;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == player_tag)
        {
            SceneManager.LoadScene("win");
            other.gameObject.GetComponentInParent<Boat>().playDing();
        }
    }
}
