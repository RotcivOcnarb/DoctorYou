using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    Animator animator;
    PlayerMovement playerMovement;
    Rigidbody2D body;
    Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
        body = GetComponent<Rigidbody2D>();
        startScale = transform.localScale;

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("Moving", Mathf.Abs(body.velocity.x) > 0.2f);
        transform.localScale = Vector3.Scale(startScale, new Vector3(playerMovement.direction, 1, 1));
    }

 
}
