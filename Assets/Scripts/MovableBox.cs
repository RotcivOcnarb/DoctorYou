using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableBox : MonoBehaviour
{
    float timer = 0; //esse timer serve pra quando a caixa estiver sendo empurrada, deixar a física mover ela
    //mas deixar ela "estática" depois de um tempo

    void Update()
    {
        timer -= Time.deltaTime;

        if(timer > 0) {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation; //tá movendo a caixa, só trava a rotação
        }
        else {
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; 
            //caixa ta parada, trava a porra toda (só o X na real, pra deixar a caixa cair)
        }
    }

    public void MoveBox(int direction) //é chamado quando a raiva bate na caixa com a animação de correr tocando
    {
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        GetComponent<Rigidbody2D>().AddForceAtPosition(new Vector2(direction * 5f, 0), transform.position, ForceMode2D.Impulse);
        //aplica um impulso na direção q a raiva estiver na hora q ela bateu na caixa
        timer = 2; //depois de 2 segundos a fisica da caixa vai travar de novo
    }
}
