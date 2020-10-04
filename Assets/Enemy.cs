using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField]
    Transform target;

    [SerializeField]
    float h;
    public float h2;

    Rigidbody2D rb;
    Animator anim;

    public bool DebugDrawLine;
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        timerDurIdle = Random.Range(0.5f, 10f);
        timerDurIdleStop = Random.Range(0.5f, 5f);
        timer = timerDurIdle;
        timerStop = timerDurIdleStop;
        gravity = Physics2D.gravity.magnitude * -1;
        rb = GetComponent<Rigidbody2D>();
        if (DebugDrawLine)
        {
            DrawPath();
        }
    }

    public enum States { chase, idle }
    public States currentstate;

    public float MaxDistance;

    public float timer;
    public float timerDur;

    private void FixedUpdate()
    {
        if (currentstate == States.chase)
        {
            if (!ChargesJump && !isJumping && Vector3.Distance(transform.position, target.transform.position) > MinDistance + 0.2f)
            {
                    Debug.Log("Chasing");
                rb.velocity = new Vector2(speed * dir, rb.velocity.y);
            }
            else if (!isJumping && !ChargesJump && !NoInterf)
            {
                rb.velocity = Vector2.zero;
                Debug.Log("Stops");
            }
        }
    }

    private void Update()
    {
//        isJumping = !CheckGround();


        //Debug
        if (Input.GetKeyDown(KeyCode.P))
        {
             rb.velocity = CalculateLaunchData(target).InitialVelocity;
        }

        //Chase
        if (currentstate == States.chase)
        {
            if (CheckWall() && !isJumping && !ChargesJump)
            {
                rb.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
      //          isJumping = true;
                timer = timerDur / 2;
            }


            if (TargetIsLeft(target))
            {
                if (!isLeft)
                {
                    Flip();
                }
            }
            else
            {
                if (isLeft)
                {
                    Flip();
                }
            }

            if (timer >= 0)
            {
                timer -= Time.deltaTime;
            }
            else if (!isJumping && !ChargesJump && Vector3.Distance(transform.position, target.transform.position) < MaxDistance && timer <= 0 && Vector3.Distance(transform.position, target.transform.position) >= MinDistance - 0.1f)
            {
                ChargesJump = true;
                JumpTarget = target;
                StartCoroutine(JumpCoro(JumpTarget));
            }
        }
        else if (currentstate == States.idle)
        {
            CheckIfChase();
            if (timer >= 0)
            {
                timer -= Time.deltaTime;
                rb.velocity = new Vector2((speed / 3) * dir, rb.velocity.y);
                anim.SetBool("Stop", false);
            }
            else
            {
                if (timerStop >= 0)
                {
                    anim.SetBool("Stop", true);
                    timerStop -= Time.deltaTime;
                    rb.velocity = Vector2.zero;
                }
                else
                {
                    Flip();
                    timerDurIdle = Random.Range(0.5f, 10f);
                    timerDurIdleStop = Random.Range(0.5f, 5f);
                    timerStop = timerDurIdleStop;
                    timer = timerDurIdle;
                }
            }

            if (CheckWall())
            {
                Flip();
                timerDurIdle = Random.Range(0.5f, 10f);
                timer = timerDurIdle;
            }

        }



        //Stops the jump
        //if (Vector3.Distance(transform.position, target.transform.position) < 0.5f && isJumping)
        //{
        //    rb.velocity = Vector2.zero;
        //    Debug.Log("Stop");
        //}
    }
    public float timerDurIdle;
    public float timerStop;
    public float timerDurIdleStop;
    Transform JumpTarget;
    public float JumpCharge;
    public bool ChargesJump;
    bool NoInterf;

    IEnumerator JumpCoro(Transform JumpTarget)
    {
        yield return new WaitForSeconds(JumpCharge);
        ChargesJump = false;
      //  isJumping = true;
        NoInterf = true;
        rb.velocity = CalculateLaunchData(JumpTarget).InitialVelocity;
        timer = timerDur;
        yield return new WaitForSeconds(0.5f);
        NoInterf = false;

    }

    public float gravity;
    public float speed;

    public bool isLeft;
    float dir = 1;

    void Flip()
    {
        isLeft = !isLeft;
        transform.localScale = new Vector3(transform.localScale.x * -1, 1);
        dir *= -1f;
    }
    bool TargetIsLeft(Transform target)
    {
        if (target.position.x >= transform.position.x)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    Vector3 CalculateTarget(Transform p)
    {
        Vector3 p2;
        if (p.position.x - transform.position.x > 0)
        {
            p2 = new Vector3(transform.GetComponent<BoxCollider2D>().size.x + (p.GetComponent<Collider2D>().bounds.center.x - p.GetComponent<Collider2D>().bounds.extents.x), p.GetComponent<Collider2D>().bounds.center.y + p.GetComponent<Collider2D>().bounds.extents.y, 0);
        }
        else
        {
            p2 = new Vector3(-transform.GetComponent<BoxCollider2D>().size.x + (p.GetComponent<Collider2D>().bounds.center.x + p.GetComponent<Collider2D>().bounds.extents.x), p.GetComponent<Collider2D>().bounds.center.y + p.GetComponent<Collider2D>().bounds.extents.y, 0);
        }

        return p2;
    }
    //Vector3 previous;
    LaunchData CalculateLaunchData(Transform p)
    {
        //  Vector3 p2 = CalculateTarget(p);
        Vector3 p2 = p.transform.position;
        float DisplacementY = p2.y - transform.position.y;
        //   float targetvelocity = (p.transform.position.x - previous.x) / Time.deltaTime;
        //  previous = p.transform.position;
        //  Debug.Log("Pred: " +p2.x + targetvelocity * Time.deltaTime+ ", Curr:" +(p2.x));
        //   Debug.Log(p2.x + " + " + (p2.x + targetvelocity * Time.deltaTime));
        if (DisplacementY < 3)
        {
            Vector3 DisplacementXZ = new Vector3(p2.x - transform.position.x, 0, p2.z - transform.position.z);
            float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (DisplacementY - h) / gravity);
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
            Vector3 velocityXZ = DisplacementXZ / time;
            return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
        }
        else
        {

            Vector3 DisplacementXZ = new Vector3(p2.x - transform.position.x, 0, p2.z - transform.position.z);
            float time = Mathf.Sqrt(-2 * (DisplacementY * h2) / gravity) + Mathf.Sqrt(2 * (DisplacementY - DisplacementY * h2) / gravity);
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * (DisplacementY * h2));
            Vector3 velocityXZ = DisplacementXZ / time;
            return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
        }

        //   return new LaunchData(velocityXZ + velocityY, time);
        //   Selected angle in radians
        //float angle = h * Mathf.Deg2Rad;

        //// Positions of this object and the target on the same plane
        //Vector3 planarTarget = new Vector3(p.position.x, 0, p.position.z);
        //Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);

        //// Planar distance between objects
        //float distance = Vector3.Distance(planarTarget, planarPostion);
        //// Distance along the y axis between objects
        //float yOffset = transform.position.y - p.position.y;

        //float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        //Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        //// Rotate our velocity to match the direction between the two objects
        //float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.position.x > transform.position.x ? 1 : -1);
        //Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        //return new LaunchData(finalVelocity, 5);
        //   rigid.velocity = finalVelocity;

    }

    void DrawPath()
    {
        LaunchData ld;
        if (isJumping)
        {
            ld = CalculateLaunchData(JumpTarget);
        }
        else
        {
            ld = CalculateLaunchData(target);
        }
        Vector3 previousdrawPoint = transform.position;
        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simTime = i / (float)resolution * ld.timeToTarget;
            Vector3 displacement = ld.InitialVelocity * simTime + Vector3.up * gravity * simTime * simTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            Debug.DrawLine(previousdrawPoint, drawPoint, Color.red);
            previousdrawPoint = drawPoint;
        }
    }

    public bool isJumping;
    public bool isWall;
    public Transform CheckWallTransform;
    public Transform CheckGroundTransform;
    public float circleRadius;
    public LayerMask groundLayer;
    bool CheckGround()
    {
       return Physics2D.OverlapCircle(CheckGroundTransform.position, circleRadius, groundLayer);
    }
    bool CheckWall()
    {
        return Physics2D.OverlapCircle(CheckWallTransform.position, circleRadius, groundLayer);
    }

    public float circleChaseRadius;
    public LayerMask PlayerLayer;
    public LayerMask LineMask;
    void CheckIfChase()
    {
        if(Physics2D.OverlapCircle(CheckWallTransform.position, circleChaseRadius, PlayerLayer)){
            if(!Physics2D.Linecast(transform.position, target.transform.position, LineMask))
            {
                timer = timerDur;
                rb.velocity = Vector2.zero;
                currentstate = States.chase;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("lol");
        isJumping = false;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("lol");
        isJumping = true;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if(collision.transform.gameObject.tag == "Ground")
        //{
        //    isJumping = false;
        //}
        if (collision.transform.gameObject.tag == "Player")
        { 
            //Hit!
        }
    }
    public float MinDistance;

    void OnDrawGizmos()
    {
        if (Vector3.Distance(transform.position, target.transform.position) < MaxDistance && Vector3.Distance(transform.position, target.transform.position) > MinDistance)
            DrawPath();

        Gizmos.DrawWireSphere(CheckGroundTransform.position, circleRadius);
        Gizmos.DrawWireSphere(CheckWallTransform.position, circleRadius);

        if(currentstate == States.idle)
        {
            Gizmos.DrawWireSphere(transform.position, circleChaseRadius);
            if (!Physics2D.Linecast(transform.position, target.transform.position, LineMask))
            {
                Gizmos.DrawLine(transform.position, target.transform.position);
            }
        }

    }
    struct LaunchData
    {
        public readonly Vector3 InitialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 InitialVelocity, float timeToTarget)
        {
            this.InitialVelocity = InitialVelocity;
            this.timeToTarget = timeToTarget;

        }
    }
}
