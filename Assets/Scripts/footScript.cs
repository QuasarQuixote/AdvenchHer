using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class footScript : MonoBehaviour
{
    public bool grounded;
    public PlayahMovement playah;
    private int collidingItemCount;

    void Start()
    {
        collidingItemCount = 0;
        grounded = false;
    }

    void OnTriggerEnter2D(Collider2D c)
    {
        if(c.isTrigger) return;
        grounded = true;
        collidingItemCount++;
        playah.DashReset();
    }

    void OnTriggerExit2D(Collider2D c)
    {
        if(c.isTrigger) return;
        collidingItemCount--;
        if(collidingItemCount==0) grounded = false;
    }

}
