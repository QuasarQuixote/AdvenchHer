using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float health;
    public bool isDead = false;
    public bool wasAttacked = false;
    public float invulnerabiltyDuration = 0.1f;

    private bool invulnerable=false;
    private float lastHit;

    public virtual void Attack(float damage)
    {
        if (invulnerable) return;
        lastHit = Time.fixedTime;
        health -= damage;
        wasAttacked = true;
        if (health < 0f)
        {
            isDead = true;
            health = 0f;
        }
    }

    public void Heal(float healVal)
    {
        health += healVal;
        if (health>maxHealth) health = maxHealth;
    }

    void Update()
    {
        if(invulnerable && Time.fixedTime>lastHit+invulnerabiltyDuration) invulnerable = false;
    }
}
