using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alavanca : MonoBehaviour
{
    public bool on;
    public RotateAround rot; //isso aqui serve pra rodar a imagem da alavanca em volta de outro eixo sem ser o centro
    //infelizmente o unity não tem um helper pra isso e eu tive q escrever o meu

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
                on = true; //liga a alavanca se a raiva passar por ela enquanto estiver correndo
            }
        }
    }
}
