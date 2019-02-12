using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//PUT THIS ON SPHERES!
//This assumes that the given object's LocalScale is the same in X/Y and that Z is 1.
[RequireComponent(typeof(MeshRenderer))]
public class Fish_circle : MonoBehaviour
{
    public GameObject obj, parent;
    public float fish_num = 4;
    public float radius = 0.5f;
    List<GameObject> fishes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        float r = radius * gameObject.transform.localScale.x;
        float theta = 0;
        
        for (int i = 0; i < fish_num; i++)
        {
            theta = (i * (2 * Mathf.PI / fish_num));
            float x = gameObject.transform.position.x + r * Mathf.Cos(theta);
            float y = gameObject.transform.position.y + r * Mathf.Sin(theta);
            GameObject o =
                Instantiate(obj, new Vector3(x, y, transform.position.z)
                ,Quaternion.identity,parent.transform);
            fishes.Add(o);
        }
        for (int i = 0;i < fishes.Count - 1;i++)
        {
            fishes[i].transform.LookAt(fishes[i + 1].transform);
        }
        fishes[fishes.Count - 1].transform.LookAt(fishes[0].transform);
        parent.transform.rotation = transform.rotation;
    }

    private void OnDisable()
    {
        foreach (GameObject fish in fishes)
        {
            fish.GetComponent<Rigidbody>().isKinematic = false;
            fish.GetComponent<Rigidbody>().useGravity = true;
            fish.GetComponent<BoxCollider>().enabled = true;
            fish.GetComponent<Despawn>().Deactivate();
        }
    }

    private void Update()
    {
        foreach(GameObject fish in fishes)
        {
            fish.transform.RotateAround(transform.position, 
                parent.transform.forward, 
                20 * Time.deltaTime);
        }
    }
}
