using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Isso aqui é a plataforma móvel
public class MoveablePlatform : MonoBehaviour
{

    public GameObject start; //ponto de inicio
    public GameObject end; //ponto de fim
    public GameObject playerSnap; //posição de onde o player tem q ficar ao ficar "Preso" na plataforma

    float prealpha = -1; //isso aqui é um calculo maluco pra fazer o alpha ficar fazendo um "Ping-pong" entre 0 e 1
    public float speed;
    public bool on;

    void Update()
    {
        if(on) prealpha += Time.deltaTime * speed;
        if (prealpha > 1) prealpha = -1;
        float alpha = Mathf.Abs(prealpha); //exercício: tenta entender como isso funciona

        transform.position = Vector3.Lerp(start.transform.position, end.transform.position, alpha); //faz um lerp entre as duas posições
        //usando esse alpha calculado de ping pong
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(start.transform.position, end.transform.position);
    }

    public void SetOn(bool on)
    {
        this.on = on;
    }
}
