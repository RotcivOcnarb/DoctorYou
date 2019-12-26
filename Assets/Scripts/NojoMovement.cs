using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NojoMovement : MonoBehaviour
{

    enum NojoState
    {
        Idle,
        Jump,
        Cuspe
    }

    NojoState estado = NojoState.Idle;

    float idleTimer = 2;
    Animator animator;
    Rigidbody2D body;
    int frameDelay = 0;
    public GameObject player; //referencia a player pra ele poder atirar em direção a ele
    int direction = 1;
    Vector3 originalScale;
    public GameObject throwPointRight; //o ponto de lançar acido do lado direito
    public GameObject throwPointLeft; //e esquerdo (próximo da boca dele)
    public GameObject acidoPrefab;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        switch (estado) {
            case NojoState.Idle:

                idleTimer -= Time.deltaTime;
                if (idleTimer < 0) {

                    Vector3 dif = player.transform.position - transform.position; //calcula a direção pra onde o player tá em relação
                    //a mim

                    if (Random.Range(0, 2) == 0 || dif.magnitude > 5) { //aleatóriamente pode pular ou cuspir (isso se o player estiver
                        //próximo o suficiente, senão só pula)
                        estado = NojoState.Jump;
                        animator.SetTrigger("Jump");
                    }
                    else {
                        estado = NojoState.Cuspe;
                        animator.SetTrigger("Cuspe");

                        //Essa conta aqui é meio maluca, tu só precisa saber q ela calcula a direção do nojo
                        //idependente de se ele estiver no teto na parede ou no chão
                        Vector2 normal = new Vector2(
                            Mathf.Cos(Mathf.Deg2Rad * (transform.rotation.eulerAngles.z+90)),
                            Mathf.Sin(Mathf.Deg2Rad * (transform.rotation.eulerAngles.z+90))
                            );

                        if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x)) 
                            direction = norm(dif.x) * norm(normal.y);
                        else 
                            direction = norm(dif.y) * norm(normal.x);
                        
                        if (direction > 0) direction = 1;
                        else direction = -1;
                        direction *= -1;
                    }
                }

                break;
            case NojoState.Jump:
                frameDelay--; //mesmo esquema de delay de pulo do player
                break;
            case NojoState.Cuspe:
                break;
        }

        transform.localScale = Vector3.Scale(originalScale, new Vector3(direction, 1, 1));

    }

    public int norm(float val)
    {
        if(val > 0) return 1;
        return -1;
    }

    public void Impulse() //dá o pulo dele
    {
        Vector2 direction = Rotate(new Vector2(0, 1), transform.rotation.eulerAngles.z);
        direction = Rotate(direction, Random.Range(-45f, 45f));

        body.gravityScale = 0; //desliga a gravidade pra ele continuar na direção sem cair
        body.velocity = direction * 6;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 180);//seta a direção
        frameDelay = 5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Wall" && frameDelay < 0) { //se bateu com uma parede, gruda nela
            float euler = Mathf.Rad2Deg * Mathf.Atan2(collision.GetContact(0).normal.y, collision.GetContact(0).normal.x) - 90;
            body.velocity = new Vector2(0, 0);
            transform.rotation = Quaternion.Euler(0, 0, euler);
            animator.SetTrigger("WallHit");
            estado = NojoState.Idle;
            idleTimer = Random.Range(2f, 5f);
        }
    }

    public static Vector2 Rotate(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    public void Cuspe() //spawna o cuspe
    {
        Vector3 dif = player.transform.position - transform.position;

        GameObject acido;
        if (direction == 1)
            acido = Instantiate(acidoPrefab, throwPointRight.transform.position, acidoPrefab.transform.rotation);
        else
            acido = Instantiate(acidoPrefab, throwPointLeft.transform.position, acidoPrefab.transform.rotation);

        acido.GetComponent<Rigidbody2D>().velocity = dif.normalized * 8;
    }

    public void EndCuspe() //ao terminar a animação de cuspir, volta pro Idle
    {
        estado = NojoState.Idle;
        animator.ResetTrigger("Cuspe");
        idleTimer = Random.Range(2f, 5f);

    }
}
