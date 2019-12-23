﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidoController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Player") {
            collision.collider.GetComponent<PlayerMovement>().Kill();
        }
        Destroy(gameObject);
    }
}
