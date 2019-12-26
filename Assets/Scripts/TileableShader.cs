using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Isso aqui serve pra deixar o shader de tile funcionando, com uma textura repetível idependente do tamanho
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
        //Eu to usando um shader chamado Tileable, tá lá na pasta Assets > Materials
        GetComponent<SpriteRenderer>().material.SetFloat("_ScaleX", transform.localScale.x);
        GetComponent<SpriteRenderer>().material.SetFloat("_ScaleY", transform.localScale.y);
    }
}
