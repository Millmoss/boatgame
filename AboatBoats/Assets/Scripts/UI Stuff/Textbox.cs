using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Use this for textbox style animations in the future.
[RequireComponent(typeof(CanvasRenderer))]
public class Textbox : MonoBehaviour, IMeshModifier
{
    private CanvasRenderer cr;
    private Mesh mesh;
    private List<UIVertex> stream;

    //NOTE: Get this from recttransform later.
    public float rect_width = 550;
    private float total_time = 0;
    private bool animating = false;
    public delegate void delegate_fun();
    public delegate_fun fin_anim_func;
    

    void Start()
    {
        mesh = new Mesh();
        stream = new List<UIVertex>();
        cr = GetComponent<CanvasRenderer>();
    }

    //This one is obsolete?
    public void ModifyMesh(Mesh mesh)
    {
        print("One");
        //throw new System.NotImplementedException();
    }

    public void ModifyMesh(VertexHelper verts)
    {
        verts.GetUIVertexStream(stream);
    }


    public void Animate()
    {
        animating = true;
        total_time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
            Animate();
        if(animating)
        {
            //We done here boys.
            if (total_time * 3 > (1 + ((stream[stream.Count-1].position.x / rect_width)) / 2) + 3.14 * 2)
            {
                fin_anim_func();
            }
            total_time += Time.deltaTime * 2;
            mesh.Clear();
            VertexHelper verts = new VertexHelper();
            for (int i = 0; i < stream.Count; i++)
            {
                UIVertex v = stream[i];
                //This makes RADICAL effects.
                //v.position.y *= (1 + (v.position.x / rect_width) * tmp);

                if(total_time * 3 > (1 + ((v.position.x / rect_width)) / 2) )
                {
                    if(total_time * 3 < (1 + ((v.position.x / rect_width)) / 2) + 3.14 * 2)
                    { 
                        v.position.y += (Mathf.Sin(total_time * 3 - (1 + ((v.position.x / rect_width)) / 2)) * 45);
                    }
                }
                verts.AddVert(v);
            }
            for (int i = 0; i < stream.Count; i += 3)
            {
                verts.AddTriangle(i, i + 1, i + 2);
            }

            verts.FillMesh(mesh);
            cr.SetMesh(mesh);
        }
    }
}
