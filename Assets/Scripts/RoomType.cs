using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum roomType { LR, LRB, LRU, LRBU };
public class RoomType : MonoBehaviour
{


    [Tooltip("Select the value by seeing which doors are open on your prefab. L is Left, R right, U Up and B bottom.")]
    public roomType thisRoomType;

    private void Start()
    {
        
    }

    public void RoomDestruction()
    {
        Destroy(gameObject);
    }
}
