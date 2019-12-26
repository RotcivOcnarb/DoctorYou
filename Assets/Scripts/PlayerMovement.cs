using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Vector2 targetVelocity; //Velocidade q a gente deseja chegar
    Vector2 vel; //vector2 temporario pra não ter q ficar instanciando um monte de objeto a cada frame
    Rigidbody2D body;
    public float speed; //velocidade horizontal
    [Range(0.01f, 1)]
    public float acceleration; //a aceleração do player (de parado pra velocidade máxima, e vice versa) (eu uso pro tween)
    public float jumpHeight;
    [HideInInspector]
    public int direction = 1; //Direção q o player tá parado, 1 é direita e -1 é esquerda
    public float feetPosition; //Posição dos pés do player em Y (pra detectar quando ele tá encostando no chão)
    Animator animator;
    int jumpFrameDelay = 0; //Essa parada serve pq quando eu pulo, existe um delayzinho até o código identificar q de
    //fato o player saiu do chão, então eu espero alguns frames pra executar os codigos q executam quando ele tá no ar
    bool dead;

    MoveablePlatform platform; //referencia a qual plataforma móvel ele está em cima no momento (null se não tiver em nenhum)

    RaycastHit2D footHit; //faz um raycast pra baixo pra saber se ele tá no chão

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        targetVelocity.Set(0, 0);
        if (!dead) {
            if (Input.GetKey(KeyCode.D)) {
                targetVelocity.x = speed;
                direction = 1;
            }
            if (Input.GetKey(KeyCode.A)) {
                targetVelocity.x = -speed;
                direction = -1;
            }
        }

        vel = body.velocity;
        vel.x += (targetVelocity.x - vel.x) * acceleration; //faz um tween entre a velocidade normal e a velocidade q a gente quer
        //acceleration é o fator de tween aqui

        jumpFrameDelay--;

        if (Input.GetKeyDown(KeyCode.W) && !dead) {
            if (OnGround()) {
                vel.y = jumpHeight;
                animator.SetTrigger("Jump");
                jumpFrameDelay = 5;
                platform = null;
            }
        }

        animator.SetBool("OnGround", OnGround() && jumpFrameDelay <= 0); //uso o delay de frames aq pra não cagar a animação

        body.velocity = vel;
        //faz o raycast pra baixo a partir da posição do pé pra ver se tem alguma plataforma em baixo
        footHit = Physics2D.Raycast(transform.position + new Vector3(0, feetPosition, 0), new Vector3(0, -1, 0));
        
        if(platform != null) { //se o player estiver em cima de uma plataforma móvel, "trava" ele em cima dela
            //pra evitar do player ficar dando micropulinhos quando a plataforma descer
            Vector3 pos = transform.position;
            pos.y = platform.playerSnap.transform.position.y;
            transform.position = pos;
        }
    }

    public bool OnGround()
    {
        return footHit.distance < 0.01;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, feetPosition, 0));
    }

    public void Kill() //Esse código é de começar a animação de morrer
    {
        dead = true;
        animator.SetTrigger("Die");
        body.constraints = RigidbodyConstraints2D.FreezeAll; //trava o player no ar quando morrer
    }

    public void FinishKill() //esse aqui é quando a animação termina, pra resetar a fase
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.GetContact(0).normal.y == 1) //Detecta a colisão com uma plataforma móvel
            platform = collision.collider.GetComponent<MoveablePlatform>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (platform != null) { //detecta q ele saiu da plataforma móvel
            if (collision.collider.gameObject == platform.gameObject) {
                platform = null;
            }
        }
    }
}
