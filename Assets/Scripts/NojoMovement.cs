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
    public GameObject player;
    int direction = 1;
    Vector3 originalScale;
    public GameObject throwPointRight;
    public GameObject throwPointLeft;
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

                    Vector3 dif = player.transform.position - transform.position;


                    if (Random.Range(0, 2) == 0 || dif.magnitude > 5) {
                        estado = NojoState.Jump;
                        animator.SetTrigger("Jump");
                    }
                    else {
                        estado = NojoState.Cuspe;
                        animator.SetTrigger("Cuspe");

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
                frameDelay--;
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

    public void Impulse()
    {
        Vector2 direction = Rotate(new Vector2(0, 1), transform.rotation.eulerAngles.z);
        direction = Rotate(direction, Random.Range(-45f, 45f));

        body.gravityScale = 0;
        body.velocity = direction * 6;

        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan2(direction.y, direction.x) + 180);
        frameDelay = 5;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "Wall" && frameDelay < 0) {
            float euler = Mathf.Rad2Deg * Mathf.Atan2(collision.GetContact(0).normal.y, collision.GetContact(0).normal.x) - 90;
            Debug.Log("Hit with normal: " + collision.GetContact(0).normal + " and euler: " + euler);
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

    public void Cuspe()
    {
        Vector3 dif = player.transform.position - transform.position;

        GameObject acido;
        if (direction == 1)
            acido = Instantiate(acidoPrefab, throwPointRight.transform.position, acidoPrefab.transform.rotation);
        else
            acido = Instantiate(acidoPrefab, throwPointLeft.transform.position, acidoPrefab.transform.rotation);

        acido.GetComponent<Rigidbody2D>().velocity = dif.normalized * 8;
    }

    public void EndCuspe()
    {
        estado = NojoState.Idle;
        animator.ResetTrigger("Cuspe");
        idleTimer = Random.Range(2f, 5f);

    }
}
