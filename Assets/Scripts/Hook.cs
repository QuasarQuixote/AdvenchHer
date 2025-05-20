using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour
{
    public float power = 20f;
    private float boomStart;
    public bool CanDashThrough(Vector2 dir, Transform player)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        dir.Normalize();
        Vector2 player2D = new Vector2(player.position.x, player.position.y);
        float radius = ((Vector2)(origin - player2D)).magnitude + Mathf.Sqrt(2f)*transform.localScale.x/2f;
        RaycastHit2D hit = Physics2D.Raycast(player2D+dir*radius, dir, 0.5f);
        Debug.DrawRay(player2D + dir * radius, dir);
        if (hit) return false;
        return true;
    }

    public void BigBoom(Vector2 dir, GameObject player)
    {
        PlayahMovement playah = player.GetComponent<PlayahMovement>();
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Vector2 player2D = new Vector2(player.transform.position.x, player.transform.position.y);
        float radius = ((Vector2)(origin - player2D)).magnitude + Mathf.Sqrt(2f) * transform.localScale.x;

        Vector2 launchPoint = player2D + dir * radius;
        Vector2 storeVelocity = dir * power;

        playah.PullTo(launchPoint, 0.1f);
        playah.StoreVelocity(storeVelocity);
        //gameObject.GetComponent<Collider2D>().enabled = false;
        boomStart = Time.fixedTime;
    }

    public void BigBonk(Vector2 dir, GameObject player)
    {
        PlayahMovement playah = player.GetComponent<PlayahMovement>();
        Vector2 veloz = dir * -1.5f * power;
        playah.StoreVelocity(veloz);
        playah.DashReset();
    }

    //void Update()
    //{
    //    if (Time.fixedTime > boomStart + 0.25f) gameObject.GetComponent<Collider2D>().enabled=true;
    //}
}
