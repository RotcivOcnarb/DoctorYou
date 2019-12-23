using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alavanca : MonoBehaviour
{
    public bool on;
    public RotateAround rot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (on) {
            rot.angle = 170;
        }
        else {
            rot.angle = 90;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<RaivaMovement>() != null) {
            if(collision.GetComponent<RaivaMovement>().estado == RaivaMovement.RaivaEstado.Dash){
                on = true;
            }
        }
    }
}
