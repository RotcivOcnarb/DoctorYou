using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RaivaMovement : MonoBehaviour
{
    //O comportamento dos inimigos funciona usando Máquina de Estados
    //dps da uma pesquisada pra ver como q funciona pq é uma parada mais avançada (mas ainda é bem facil de entender)
    public enum RaivaEstado
    {
        Idle,
        Avistou,
        Dash,
        Hit
    }

    [HideInInspector]
    public RaivaEstado estado = RaivaEstado.Idle;

    struct Transicao //define as transições q vão rolar entre os estados
    {
        public RaivaEstado from;
        public RaivaEstado to;
        public Func<bool> transition;
    }

    List<Transicao> transicoes;

    //Propriedades de movimento
    Vector2 targetVelocity; //mesmo esquema de tween do player
    public float speed;
    public float dashSpeed; //velocidade da raiva correndo
    Rigidbody2D body;
    int direction = 1;
    Vector3 originalScale; //mesmo esquema de scale do player q pode ser trocado por SpriteRenderer.FlipX
    float idleTimer = 0; //um contador q conta quantos segundos q ele fica em Idle
    Animator animator;
    public LayerMask layerMask; //a mascara q define quais objetos o inimigo "encherga" na frente dele
    float avistouTimer = 0; //um contador q conta quantos segundos se passou depois do inimigo avistar o player
    public ParticleSystem exclamation; // o efeito de exclamação saido da cabeça dele é um particle system

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
                //Faz dois raycasts, um pra frente e um pra trás detectando se ele encontra um player
                //é como se fosse os "olhos" do inimigo
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 5, layerMask);
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector2(-direction, 0), 5, layerMask);

                //detecta se avistou algum player
                bool ret = (hit.collider != null && hit.collider.tag == "Player") || (hit2.collider != null && hit2.collider.tag == "Player");

                if (ret)
                    exclamation.Play();

                if (hit2.collider != null && hit2.collider.tag == "Player")
                    direction = -direction; //se tiver achado o player atrás dele, vira ao contrário antes de correr

                return ret;
            }
        });

        transicoes.Add(new Transicao() {
            from = RaivaEstado.Avistou,
            to = RaivaEstado.Dash,
            transition = () => {
                if(avistouTimer > 1) { //passa do estado de avistou o player pra correr atrás dele depois de 1 segundo
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
        foreach(Transicao t in transicoes) { //pra cada transição q existe, chega se dá pra transicionar pro próximo estado
            if(t.from == estado) {
                if (t.transition()) {
                    estado = t.to;
                }
            }
        }

        switch (estado) { //o comportamento de cada estado
            case RaivaEstado.Idle: 
                idleTimer -= Time.deltaTime;
                if(idleTimer < 0) {
                    idleTimer = UnityEngine.Random.Range(0.5f, 1.5f); //fica no movimento por um valor entre meio segundo e 1 segundo e meio

                    int rnd = UnityEngine.Random.Range(0, 3);
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(1, 0), .3f, LayerMask.GetMask("Wall"));
                    //olha pra ver se não vai bater numa parede

                    while (rnd == 0 && hit.collider != null) { //Evita de andar pra direita se tiver uma parede
                        rnd = UnityEngine.Random.Range(0, 3);
                        hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(1, 0), 0.3f, LayerMask.GetMask("Wall"));
                    }

                    hit = Physics2D.Raycast(transform.position, transform.position + new Vector3(-1, 0), 0.3f, LayerMask.GetMask("Wall"));

                    while (rnd == 1 && hit.collider != null) { //Evita de andar pra esquerda se tiver uma parede
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
                targetVelocity.x = 0; //deixa ele parado
                animator.SetBool("Moving", false);
                break;
            case RaivaEstado.Dash:
                targetVelocity.x = dashSpeed * direction;
                break;
            case RaivaEstado.Hit:
                targetVelocity.x = 0; //bateu em alguma coisa
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
        if (estado == RaivaEstado.Dash) { //Detecta se a raiva bateu em alguma coisa enquanto estava correndo
            animator.SetTrigger("Hit");
            estado = RaivaEstado.Hit;
            if (collision.collider.tag == "Player") { //se for o player mata ele
                collision.collider.GetComponent<PlayerMovement>().Kill();
            }
            if(collision.collider.tag == "Box") {// se for uma caixa empurra ela
                collision.collider.GetComponent<MovableBox>().MoveBox(direction);
            }
            if(collision.collider.tag == "Button") { //se for um botão pressiona ele
                collision.collider.GetComponent<ButtonController>().PressButton();
            }
        }
    }


    public void TransitionBackToIdle() //isso aqui é chamado no final da animação de "hit" pra ele voltar ao idle
    {
        estado = RaivaEstado.Idle;
    }

}
