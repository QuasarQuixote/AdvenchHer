using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public BasicSword parent;
    public PolygonCollider2D collider;
    public SpriteRenderer sprite;
    public int attackID;
    public void AttackStart()
    {
        collider.enabled = true;
        sprite.enabled = true;
    }
    public void AttackEnd()
    {
        collider.enabled = false;
        sprite.enabled = false;
    }
    void OnTriggerEnter2D(Collider2D o)
    {
        //parent.Hit(attackID, o.gameObject);
        //Debug.Log($"I ({attackID}) just hit " + o.name);
        parent.Hit(attackID, o.gameObject);
    }
}
