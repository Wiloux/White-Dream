using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGeneration : MonoBehaviour
{
    public Transform[] startingPositions;
    public GameObject[] rooms; // index 0 LR, index 1 LRB, index 2 LRT, index 3 LRBU

    public LayerMask room;
    private int direction;
    public float spaceToMove;
    private float timeBtwRoom;
    public float startTimeBtwRoom = 0.25f;
    public float minX, maxX;
    public float minY, maxY;
    bool stopGeneration;

    public static GameObject lastRoomGenerated;

    void Start()
    {
        int randStartingPos = Random.Range(0, startingPositions.Length);
        transform.position = startingPositions[randStartingPos].position;
        Instantiate(rooms[0], transform.position, Quaternion.identity);

        direction = Random.Range(1, 6);
    }

    private void Update()
    {
        if (timeBtwRoom <= 0 && stopGeneration == false)
        {
            Move();
            timeBtwRoom = startTimeBtwRoom;
        }
        else
        {
            timeBtwRoom -= Time.deltaTime;
        }
    }

    private void Move()
    {

        if (direction == 1 || direction == 2)
        {
            if (transform.position.x < maxX)
            {
                Vector3 newPos = new Vector3(transform.position.x + spaceToMove, transform.position.y, transform.position.z);
                transform.position = newPos;
                //Moves right

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);
                if (direction == 3)
                {
                    direction = 2;
                }
                else if (direction == 4)
                {
                    direction = 5;
                }
            }
            else
            {
                direction = 5;
            }

        }
        else if (direction == 3 || direction == 4)
        {
            if (transform.position.x > minX)
            {
                Vector3 newPos = new Vector3(transform.position.x - spaceToMove, transform.position.y, transform.position.z);
                transform.position = newPos;
                //Moves left

                int rand = Random.Range(0, rooms.Length);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(3, 6);
            }
            else
            {
                direction = 5;
            }

        }
        else if (direction == 5)
        {
            if (transform.position.y > minY)
            {
                Collider2D roomDetection = Physics2D.OverlapCircle(transform.position, 1, room);
                if (lastRoomGenerated.GetComponent<RoomType>().thisRoomType != roomType.LR && lastRoomGenerated.GetComponent<RoomType>().thisRoomType != roomType.LRU)
                {
                    lastRoomGenerated.GetComponent<RoomType>().RoomDestruction();

                    int randBottomRoom = Random.Range(1, 4);
                    if(randBottomRoom == 2)
                    {
                        randBottomRoom = 1;
                    }
                    Instantiate(rooms[randBottomRoom], transform.position, Quaternion.identity);
                }

                Vector3 newPos = new Vector3(transform.position.x, transform.position.y - spaceToMove, transform.position.z);
                transform.position = newPos;
                //Moves down

                int rand = Random.Range(2, 4);
                Instantiate(rooms[rand], transform.position, Quaternion.identity);

                direction = Random.Range(1, 6);

            }
            else
            {
                Debug.Log("Generation is over");
                stopGeneration = true;
            }
        }

        //Instantiate(rooms[0], transform.position, Quaternion.identity);

    }
}
