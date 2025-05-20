using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlobScript : Enemy
{
    //public GameObject player;
    public float maxSpeed;
    public float acceleration;
    public Sprite deadBlob;
    public float damage = 30f;
    public float playerDashLeniencyTime = 0.5f;

    private Vector2 playerDir;
    private Vector2 movement;
    private Vector3 playerDir3d;
    private Rigidbody2D thisBody;
    private enum State { ALIVE, DEAD };
    private State state;
    private SpriteRenderer rendHerHer;
    private float deathStart;
    private float playerDashThroughStart;
    private Collider2D collider;
    
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        playerDashThroughStart = -1f;
        movement = new Vector2(0, 0);
        thisBody = GetComponent<Rigidbody2D>();
        state = State.ALIVE;
        rendHerHer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((collider.enabled == false) && Time.fixedTime > playerDashThroughStart + playerDashLeniencyTime) collider.enabled = true;
        if(health.isDead && state != State.DEAD)
        {
            state = State.DEAD;
            rendHerHer.sprite = deadBlob;
            GetComponent<Collider2D>().enabled = false;
            deathStart = Time.fixedTime;
        }
        if (state == State.ALIVE)
        {
            playerDir3d = player.transform.position - transform.position;
            playerDir = new Vector2(playerDir3d.x, playerDir3d.y);
            playerDir.Normalize();
            if ((Mathf.Abs(movement.magnitude - maxSpeed) > 0.1f) || (Mathf.Abs(Vector2.Angle(movement, playerDir)) > 90f)) movement += playerDir * acceleration * Time.deltaTime;
            else if (Mathf.Abs(Vector2.Angle(movement, playerDir)) > 1f)
            {
                Vector2 perpendicular = new Vector2(-1 * movement.y, movement.x);
                perpendicular.Normalize();
                if (Mathf.Abs(Vector2.Angle(perpendicular, playerDir)) > 90f) perpendicular *= -1;
                movement += perpendicular * acceleration * Time.deltaTime;
            }
        } else if (state == State.DEAD)
        {
            if (Time.fixedTime > deathStart + 2f) Destroy(gameObject);
        }
        thisBody.velocity = movement;

    }

    public override void ReceiveHit(float damage, Vector2 knockback)
    {
        health.Attack(damage);
        movement += 0.5f*knockback;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        GameObject o = collision.gameObject;
        ContactPoint2D contact = collision.GetContact(0);
        Vector2 normal = contact.normal;
        normal.Normalize();
        Vector2 relativeVelocity = contact.relativeVelocity;
        Vector2 knockback = normal * relativeVelocity.magnitude*Mathf.Cos(Vector2.Angle(relativeVelocity, normal) * (Mathf.PI/180f));
        if (o.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.Attack(damage);
            if(!playerHealth.isDodging)
                o.GetComponent<PlayahMovement>().StoreVelocity(2*knockback);
            else
            {
                collider.enabled = false;
                playerDashThroughStart  = Time.fixedTime;
            }
        } else if (o.TryGetComponent<Health>(out Health health))
        {
            health.Attack(damage);
            movement += 2*knockback;
        }
        else
        {
            movement += 2*knockback;
        }

    }
}
