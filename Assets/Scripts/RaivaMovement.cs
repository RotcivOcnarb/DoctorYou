using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaivaMovement : MonoBehaviour
{
    public enum RaivaEstado
    {
        Idle,
        Avistou,
        Dash,
        Hit
    }

    [HideInInspector]
    public RaivaEstado estado = RaivaEstado.Idle;

    struct Transicao
    {
        public RaivaEstado from;
        public RaivaEstado to;
        public Func<bool> transition;
    }

    List<Transicao> transicoes;


    //Propriedades de movimento
    Vector2 targetVelocity;
    public float speed;
    public float dashSpeed;
    Rigidbody2D body;
    int direction = 1;
    Vector3 originalScale;
    float idleTimer = 0;
    Animator animator;
    public LayerMask layerMask;
    float avistouTimer = 0;
    public ParticleSystem exclamation;

    void Start()
    {
        transicoes = new List<Transicao>();
        targetVelocity = new Vector2();
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        animator = GetComponent<Animator>();

        //MAQUINA DE ESTADOS

        transicoes.Add(new Transicao() {
            from = RaivaEstado.Idle,
            to = RaivaEstado.Avistou,
            transition = () => {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 5, layerMask);
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector2(-direction, 0), 5, layerMask);
                bool ret = (hit.collider != null && hit.collider.tag == "Player") || (hit2.collider != null && hit2.collider.tag == "Player");

                if (ret)
                    exclamation.Play();

                if (hit2.collider != null && hit2.collider.tag == "Player")
                    direction = -direction;

                return ret;
            }
        });

        transicoes.Add(new Transicao() {
            from = RaivaEstado.Avistou,
            to = RaivaEstado.Dash,
            transition = () => {
                if(avistouTimer > 1) {
                    avistouTimer = 0;
                    animator.SetTrigger("Dash");
                    return true;
                }
                return false;
            }
        });
    }

    void Update()
    {
        foreach(Transicao t in transicoes) {
            if(t.from == estado) {
                if (t.transition()) {
                    estado = t.to;
                }
            }
        }

        switch (estado) {
            case RaivaEstado.Idle: 
                idleTimer -= Time.deltaTime;
                if(idleTimer < 0) {
                    idleTimer = UnityEngine.Random.Range(0.5f, 1.5f);

                    int rnd = UnityEngine.Random.Range(0, 3);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(1, 0), .3f, LayerMask.GetMask("Wall"));

                    while (rnd == 0 && hit.collider != null) { //Evita de ir pra direita se tiver uma parede
                        rnd = UnityEngine.Random.Range(0, 3);
                        hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(1, 0), 0.3f, LayerMask.GetMask("Wall"));
                    }

                    hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(-1, 0), 0.3f, LayerMask.GetMask("Wall"));

                    while (rnd == 1 && hit.collider != null) { //Evita de ir pra esquerda se tiver uma parede
                        rnd = UnityEngine.Random.Range(0, 3);
                        hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(-1, 0), 0.3f, LayerMask.GetMask("Wall"));
                    }

                    if (rnd == 0) {//anda pra direita
                        targetVelocity.x = speed;
                        direction = 1;
                        animator.SetBool("Moving", true);
                    }
                    else if (rnd == 1) { // anda pra esquerda
                        targetVelocity.x = -speed;
                        direction = -1;
                        animator.SetBool("Moving", true);

                    }
                    else {//fica parado
                        targetVelocity.x = 0;
                        animator.SetBool("Moving", false);

                    }
                }
                break;
            case RaivaEstado.Avistou:
                avistouTimer += Time.deltaTime;
                targetVelocity.x = 0;
                animator.SetBool("Moving", false);
                break;
            case RaivaEstado.Dash:
                targetVelocity.x = dashSpeed * direction;
                break;
            case RaivaEstado.Hit:
                targetVelocity.x = 0;
                animator.SetBool("Moving", false);
                break;
        }

        //Atualiza a velocidade
        Vector2 vel = body.velocity;
        vel.x += (targetVelocity.x - vel.x) * 0.6f;
        body.velocity = vel;

        transform.localScale = Vector3.Scale(originalScale, new Vector3(direction, 1, 1));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (estado == RaivaEstado.Dash) {
            animator.SetTrigger("Hit");
            estado = RaivaEstado.Hit;
            if (collision.collider.tag == "Player") {
                collision.collider.GetComponent<PlayerMovement>().Kill();
            }
            if(collision.collider.tag == "Box") {
                collision.collider.GetComponent<MovableBox>().MoveBox(direction);
            }
            if(collision.collider.tag == "Button") {
                collision.collider.GetComponent<ButtonController>().PressButton();
            }
        }
    }


    public void TransitionBackToIdle()
    {
        estado = RaivaEstado.Idle;
    }

}
