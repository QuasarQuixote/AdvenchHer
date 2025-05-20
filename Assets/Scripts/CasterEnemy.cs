using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasterEnemy : Enemy
{
    private enum State {NEUTRAL, ATTACKING, DEAD};
    private State state;
    public Rigidbody2D thisBody;
    private Vector3 playerDir;
    private RaycastHit2D hit;
    private float attackStart;
    private bool canAttack;
    public SpriteRenderer rendherher;
    private Color brown;
    private Color bred;
    private Projectile projproj;
    private bool hayProj=false;
    private float appliedSpeed;
    private float internalSpeed;
    private float momentOfDeath;
    private float scale = 1f;
    private Vector3 baseVector;

    public float deathDuration = 1f;
    public float speed;
    //public Transform player;
    public float aimTime = 0.75f;
    public float attackCD = 2f;
    public GameObject projectile;

    void Start()
    {
        baseVector = transform.localScale;
        brown = new Color((139f/255f), (69f/255f), (19f/255f));
        bred = new Color(230f/255f, 40f/255f, 10f/255f);
        //rendherher = gameObject.GetComponent<SpriteRenderer>();
        rendherher.color = brown;
        state = State.NEUTRAL;
        //Instantiate(projectile, transform.position, Quaternion.identity);
        //projectile.GetComponent<Projectile>().player = player;

        projectile.GetComponent<Projectile>().player = player.transform;
        projectile.GetComponent<Projectile>().parent = gameObject;
        projectile.GetComponent<Projectile>().length = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        
        thisBody.velocityX = internalSpeed + appliedSpeed;
        if (health.isDead && state != State.DEAD)
        {
            state = State.DEAD;
            collider.enabled = false;
            momentOfDeath = Time.fixedTime;
            rendherher.color = Color.white;
            thisBody.gravityScale = 0f;
            internalSpeed = 0f;
        }
        //Debug.Log(thisBody.velocityX);
        if (appliedSpeed != 0)
        {
            appliedSpeed -= Mathf.Clamp(appliedSpeed, -10f*Time.deltaTime, 10f*Time.deltaTime);
        }

        if (!canAttack && Time.fixedTime>attackStart+attackCD)
        {
            canAttack = true;
        }
        if (state == State.NEUTRAL)
        {
            playerDir = (player.transform.position - transform.position);
            playerDir.Normalize();
            if (playerDir.x>0)
            {
                internalSpeed = speed;
            }
            else if(playerDir.x<0)
            {
                internalSpeed = -1 * speed;
            }

            hit = Physics2D.Raycast(transform.position + 0.5f*playerDir, new Vector2(playerDir.x, playerDir.y));
            if(hit && (hit.collider.name.Equals("Playah")|| hit.collider.name.Equals("feet")) && canAttack)
            {
                rendherher.color = bred;
                state = State.ATTACKING;
                attackStart = Time.fixedTime;
                internalSpeed = 0f;
            } 

        }
        else if (state == State.ATTACKING)
        {
            //thisBody.velocityX = 0;
            if (attackStart + aimTime < Time.fixedTime)
            {
                //Debug.Log("Fire!!!");
                canAttack = false;
                rendherher.color = brown;
                state = State.NEUTRAL;
                //Spawn projectile
                Instantiate(projectile, transform.position, Quaternion.identity);
                
                //Debug.Log(player.position - transform.position);
            }
        }
        else if (state == State.DEAD)
        {
            if (Time.fixedTime > momentOfDeath + deathDuration) Destroy(gameObject);
            transform.RotateAround(transform.position, Vector3.forward, 1000f * Time.deltaTime);
            scale = (momentOfDeath + deathDuration - Time.fixedTime)/deathDuration;
            transform.localScale = baseVector * scale;
        }
    }

    public override void ReceiveHit(float damage, Vector2 knockback)
    {
        health.Attack(damage);
        Debug.Log(knockback);
        thisBody.velocityY += knockback.y;
        appliedSpeed += knockback.x;
    }

}
