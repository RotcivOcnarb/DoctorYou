using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileableShader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().material.SetFloat("_ScaleX", transform.localScale.x);
        GetComponent<SpriteRenderer>().material.SetFloat("_ScaleY", transform.localScale.y);

    }
}
