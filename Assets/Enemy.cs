using System.Collections;
using System.Collections.Generic;
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

    public bool DebugDrawLine;
    void Start()
    {
        gravity = Physics2D.gravity.magnitude * -1;
        rb = GetComponent<Rigidbody2D>();
        if (DebugDrawLine)
        {
            DrawPath();
        }
    }

    public bool isJumping;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isJumping = true; rb.velocity = CalculateLaunchData(target).InitialVelocity;

        }

        if (Vector3.Distance(transform.position, target.transform.position) < 1f && isJumping)
        {
            isJumping = false;
            rb.velocity = Vector2.zero;
            Debug.Log("Stop");
        }
    }

    public float gravity;

    Vector3 CalculateTarget(Transform p)
    {
        Vector3 p2;
        if(p.position.x - transform.position.x > 0) {
            p2 = new Vector3(transform.GetComponent<BoxCollider2D>().size.x + (p.GetComponent<Collider2D>().bounds.center.x - p.GetComponent<Collider2D>().bounds.extents.x), p.GetComponent<Collider2D>().bounds.center.y + p.GetComponent<Collider2D>().bounds.extents.y, 0);
        }
        else
        {         
            p2 = new Vector3(-transform.GetComponent<BoxCollider2D>().size.x + (p.GetComponent<Collider2D>().bounds.center.x + p.GetComponent<Collider2D>().bounds.extents.x), p.GetComponent<Collider2D>().bounds.center.y + p.GetComponent<Collider2D>().bounds.extents.y, 0);
        }

        return p2;
    }
    LaunchData CalculateLaunchData(Transform p)
    {
        Vector3 p2 = CalculateTarget(p);
        float DisplacementY = p2.y - transform.position.y;

        if (DisplacementY < 3)
        {
            Vector3 DisplacementXZ = new Vector3(p2.x - transform.position.x, 0, p2.z - transform.position.z);
            float time = Mathf.Sqrt(-2 * h / gravity) + Mathf.Sqrt(2 * (DisplacementY - h) / gravity);
            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * h);
            Vector3 velocityXZ = DisplacementXZ / time;
            return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), time);
        }
        else {

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
        LaunchData ld = CalculateLaunchData(target);
        Vector3 previousdrawPoint = transform.position;
        int resolution = 30;
        for (int i = 1; i <= resolution; i++)
        {
            float simTime = i / (float)resolution * ld.timeToTarget;
            Vector3 displacement = ld.InitialVelocity * simTime + Vector3.up * gravity * simTime * simTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            Debug.DrawLine(previousdrawPoint, drawPoint, Color.green);
            previousdrawPoint = drawPoint;
        }
    }

    void OnDrawGizmosSelected()
    {
        DrawPath();
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
