using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Health health;
    public Collider2D collider;
    public GameObject player;
    public virtual void Die() 
    {
        Object.Destroy(gameObject);
    }
    public virtual void ReceiveHit(float damage, Vector2 knockback)
    {
        health.Attack(damage);
        Debug.Log(knockback+" Teehee");
    }

}
