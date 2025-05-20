using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 3f;
    public float lifeTime = 10f;
    private Vector3 direction;
    public GameObject parent;
    public float length;
    public float damage = 10f;
    public Transform player;
    public bool destroyOnCollision = true;
    public GameObject dodgeParticle;
    public bool aims;

    private float start;
    public bool dodged = false;
    // Start is called before the first frame update
    void Start()
    {
        if (aims)
        {
            if (player == null) Destroy(gameObject);
            direction = (player.position - transform.position);
        }
        else
        {
            direction = transform.right;
        }
        direction.Normalize();
        //Debug.Log("Projectile Start");
        //Debug.Log(direction);
        start = Time.fixedTime;
        transform.right = direction;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + direction * speed * Time.deltaTime;
        if (Time.fixedTime > start + lifeTime) Destroy(gameObject);
        if (!dodged)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, length);
            if (hit)
            {
                GameObject o = hit.collider.gameObject;
               
                if (o == parent)
                {
                    return;
                }
                Debug.Log("I entered " + o.name);
                if (o.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
                {
                    playerHealth.Attack(damage);
                    if (playerHealth.isDodging) dodged = true;
                    Instantiate(dodgeParticle, transform.position - Vector3.forward * -0.1f, Quaternion.identity);
                    if (o.TryGetComponent<PlayahMovement>(out PlayahMovement playah)) playah.DashReset();
                }
                else if (o.TryGetComponent<Health>(out Health targetHealth))
                {
                    targetHealth.Attack(damage);
                }
                if (!hit.collider.isTrigger && destroyOnCollision && !dodged) Destroy(gameObject);
            }
        }
    }
    /*
    void OnTriggerEnter2D(Collider2D c)
    {
        
        GameObject o = c.gameObject;
        Debug.Log("I entered "+o.name);
        if (o == parent)
        {
            return;
        }
        if(o.TryGetComponent<Health>(out Health targetHealth))
        {
            targetHealth.Attack(damage);

        }
        if (!c.isTrigger && destroyOnCollision) Destroy(gameObject);
    }
    */
}
