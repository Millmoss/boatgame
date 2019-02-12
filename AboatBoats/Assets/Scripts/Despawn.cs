using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawn : MonoBehaviour
{
    public float despawn = 5f;

    public void Deactivate()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(despawn);
        gameObject.SetActive(false);
        yield return true;
    }

}
