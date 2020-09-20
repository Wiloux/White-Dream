using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    public float jumpForce;
    public float FallMultiplier;
    public float lowJumpForce;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
       anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    bool jump;
    public bool canJump;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            jump = true;
            canJump = false;
        }

        //  rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, 0);

        if(Input.GetAxis("Horizontal") > 0.1 || Input.GetAxis("Horizontal") < -0.1)
        { 
            anim.SetBool("Walk", true);
            anim.SetBool("Idle", false);
        }
        else
        {
            anim.SetBool("Idle", true);
            anim.SetBool("Walk", false);
        }

        if(Input.GetAxis("Horizontal") > 0)
        {
            sprite.flipX = false;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            sprite.flipX = true;

        }
        transform.Translate(new Vector2(Input.GetAxis("Horizontal") * speed, 0) * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.CompareTag("Ground"))
        {
            canJump = true;
        }

    }
    private void FixedUpdate()
    {
        if (jump)
        {
            // rb.velocity += Vector2.up * jumpForce;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }
        anim.SetFloat("VelocityY", rb.velocity.y);
        HandleGravityJump();

    }

    void HandleGravityJump()
    {

        if (rb.velocity.y < 0)
        {
            rb.gravityScale = FallMultiplier;
        }
        //else if (rb.velocity.y > 0 && Input.GetKey(KeyCode.Space))
        //{
        //   // rb.gravityScale = lowJumpForce;
        //}
        else
        {
            rb.gravityScale = 1f;
        }
    }
}


