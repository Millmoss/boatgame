using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on boat. Update's the water shader's "Boat is at" value.
public class Boat_Water : MonoBehaviour
{
    public Material mat;

    // Update is called once per frame
    void Update()
    {
        Vector3 tmp = new Vector3(
            transform.position.x,
//            -9999,
            transform.position.y - .04f,
            transform.position.z);
        mat.SetVector("_BoatPos", tmp);
    }
}
