using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Classe que faz a camera seguir o gameobject selecionado (no caso eu to usando o player)
public class CameraFollow : MonoBehaviour
{

    public GameObject toFollow;
    public Rect limit; //limita a posição da camera pra um "Bounds" especifico
    Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.y += (toFollow.transform.position.y - position.y) / 10f; //faz um Tween pro gameobject
        position.z = -10; //mas não afeta o Z senão da bosta
        transform.position = position;

        if(transform.position.y - mainCamera.orthographicSize < limit.position.y - limit.height / 2f) { //Não deixa a camera
            //passar do limite em Y
            Vector3 v = transform.position;
            v.y = limit.position.y - limit.height / 2f + mainCamera.orthographicSize;
            transform.position = v;
        }
        if (transform.position.y + mainCamera.orthographicSize > limit.position.y + limit.height / 2f) {
            //Não deixa a camera passar do limite em X
            Vector3 v = transform.position;
            v.y = limit.position.y + limit.height / 2f - mainCamera.orthographicSize;
            transform.position = v;
        }

    }

    private void OnDrawGizmosSelected()
    {
        //Aqui é só pra desenhar o quadrado de limite no editor
        Vector2 c0 = limit.position + new Vector2(-limit.width / 2f, -limit.height / 2f);
        Vector2 c1 = limit.position + new Vector2(limit.width / 2f, -limit.height / 2f);
        Vector2 c2 = limit.position + new Vector2(limit.width / 2f, limit.height / 2f);
        Vector2 c3 = limit.position + new Vector2(-limit.width / 2f, limit.height / 2f);

        Gizmos.DrawLine(c0, c1);
        Gizmos.DrawLine(c1, c2);
        Gizmos.DrawLine(c2, c3);
        Gizmos.DrawLine(c3, c0);

    }
}
