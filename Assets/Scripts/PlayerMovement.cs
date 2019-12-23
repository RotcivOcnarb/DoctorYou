using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    Vector2 targetVelocity;
    Vector2 vel;
    Rigidbody2D body;
    public float speed;
    [Range(0.01f, 1)]
    public float acceleration;
    public float jumpHeight;
    [HideInInspector]
    public int direction = 1;
    public float feetPosition;
    Animator animator;
    int jumpFrameDelay = 0;
    bool dead;

    MoveablePlatform platform;

    RaycastHit2D footHit;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
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
        vel.x += (targetVelocity.x - vel.x) * acceleration;

        jumpFrameDelay--;

        if (Input.GetKeyDown(KeyCode.W) && !dead) {
            if (OnGround()) {
                vel.y = jumpHeight;
                animator.SetTrigger("Jump");
                jumpFrameDelay = 5;
                platform = null;
            }
        }

        animator.SetBool("OnGround", OnGround() && jumpFrameDelay <= 0);

        body.velocity = vel;
        footHit = Physics2D.Raycast(transform.position + new Vector3(0, feetPosition, 0), new Vector3(0, -1, 0));

        if(platform != null) {
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

    public void Kill()
    {
        dead = true;
        animator.SetTrigger("Die");
        body.constraints = RigidbodyConstraints2D.FreezePosition;
    }

    public void FinishKill()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.GetContact(0).normal.y == 1)
        platform = collision.collider.GetComponent<MoveablePlatform>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (platform != null) {
            if (collision.collider.gameObject == platform.gameObject) {
                platform = null;
                Debug.Log("Exit platform");
            }
        }
    }
}
