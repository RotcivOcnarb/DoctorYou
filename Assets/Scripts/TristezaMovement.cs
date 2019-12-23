using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TristezaMovement : MonoBehaviour
{

    enum TristezaEstado
    {
        Idle,
        Gritando,
        Chorando
    }

    TristezaEstado estado = TristezaEstado.Idle;

    struct Transicao
    {
        public TristezaEstado from;
        public TristezaEstado to;
        public Func<bool> transition;
    }

    List<Transicao> transicoes;

    /* */
    float idleTimer = 0;
    public float speed;
    Vector2 targetVelocity;
    int direction = 1;
    Animator animator;
    Rigidbody2D body;
    Vector2 originalScale;
    public LayerMask layerMask;
    public ContactFilter2D damageMask;
    public ParticleSystem exclamation;
    float choroTimer = 0;
    float lagrimaTimer = 0;
    public GameObject lagrimaPrefab;

    // Start is called before the first frame update
    void Start()
    {
        targetVelocity = new Vector2();
        animator = GetComponent<Animator>();
        transicoes = new List<Transicao>();
        body = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;

        /* Transições */

        transicoes.Add(new Transicao() {
            from = TristezaEstado.Idle,
            to = TristezaEstado.Gritando,
            transition = () => {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2, layerMask);
                RaycastHit2D hit2 = Physics2D.Raycast(transform.position, new Vector2(-direction, 0), 2, layerMask);
                bool avistouPlayer = (hit.collider != null && hit.collider.tag == "Player") || (hit2.collider != null && hit2.collider.tag == "Player");

                if (avistouPlayer) {
                    exclamation.Play();
                    animator.SetTrigger("Grito");
                }

                if (hit2.collider != null && hit2.collider.tag == "Player")
                    direction = -direction;

                return avistouPlayer;
            }
        });
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transicao t in transicoes) {
            if (t.from == estado) {
                if (t.transition()) {
                    estado = t.to;
                }

            }
        }

        switch (estado) {
            case TristezaEstado.Idle:

                idleTimer -= Time.deltaTime;
                if (idleTimer < 0) {
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
            case TristezaEstado.Chorando:
                choroTimer += Time.deltaTime;

                targetVelocity.x = 0;
                animator.SetBool("Moving", false);
                animator.SetBool("Chorando", true);

                if (choroTimer > 5) {
                    choroTimer = 0;
                    estado = TristezaEstado.Idle;
                    animator.SetBool("Chorando", false);
                }

                lagrimaTimer += Time.deltaTime;
                if(lagrimaTimer > 0.1f) {
                    lagrimaTimer = 0;

                    for (int i = -1; i <= 1; i += 2) {
                        GameObject lagrima = Instantiate(lagrimaPrefab);
                        lagrima.transform.position = transform.position;
                        Vector3 direction = new Vector3(.3f * i, .7f, 0);
                        direction.Normalize();

                        direction = Rotate(direction, UnityEngine.Random.Range(-3f, 3f));

                        lagrima.GetComponent<Rigidbody2D>().velocity = direction * 4;
                    }
                }

                break;
            case TristezaEstado.Gritando:
                targetVelocity.x = 0;
                animator.SetBool("Moving", false);
                break;
        }

        //Atualiza a velocidade
        Vector2 vel = body.velocity;
        vel.x += (targetVelocity.x - vel.x) * 0.6f;
        body.velocity = vel;
        transform.localScale = Vector3.Scale(originalScale, new Vector3(-direction, 1, 1));

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

    public void DamageArea()
    {
        List<RaycastHit2D> hitList = new List<RaycastHit2D>();
        Physics2D.CircleCast(transform.position, 3, new Vector2(1, 0), damageMask, hitList, 0);

        foreach(RaycastHit2D hit in hitList) {
            if(hit.collider.tag == "Player") {
                hit.collider.GetComponent<PlayerMovement>().Kill();
            }
        }
    }

    public void FinishGrito()
    {
        estado = TristezaEstado.Chorando;
    }

}
