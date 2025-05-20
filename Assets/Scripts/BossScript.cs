using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : Enemy
{
    public float leftBound;
    public float rightBound;
    public float lowerBound;
    public float maxStamina;
    public Rigidbody2D thisBody;

    public float dashSpeed;
    public float dashWarning = 0.75f;
    public float dashCost;

    public float thwompDur = 4f;
    public float thwompSpeed = 10f;
    public float thwompCost;

    public float quakeDur;
    public GameObject quakeDeathBox;
    public float quakeToFloor;
    public float quakeCost;

    public float burstProjInterval;
    public GameObject burstProjectile;
    public float burstDur;
    public float burstCost;

    public float sweepDur;
    public float sweepCost;
    public GameObject sweepDeathBox;

    public float restDur;

    public SpriteRenderer face;
    public Sprite dashFace;
    public float dashFaceScale;
    public Sprite thwompFace;
    public float thwompFaceScale;
    public Sprite quakeFace;
    public float quakeFaceScale;
    public Sprite sweepFace;
    public float sweepFaceScale;
    public Sprite burstFace;
    public float burstFaceScale;
    public Sprite restFace;
    public float restFaceScale;

    public float deathDuration = 1f;


    private GameManagerScript gameManager;

    private Vector3 baseVector = new Vector3(1f, 1f, 1f);
    private float scale;
    private float currentStamina;
    private Transform spriteTrans;
    private float stateStart;
    private Collider2D thisCollider;
    private bool quaked = false;
    private bool hasDied = false;
    private enum State { THWOMP, DASH, QUAKE, BURST, SWEEP, REST, UNDETERMINED, DEAD};
    private State state;
    private Vector3 spawnPoint;
    private GameObject virtualQuake;
    private Quaternion projAngle;
    private int projectilesShot;
    private GameObject sweeper;
    private float dir;

    void Start()
    {
        burstProjectile.GetComponent<Projectile>().parent = gameObject;
        //burstProjectile.GetComponent<Projectile>().player = player;
        spawnPoint = gameObject.transform.position;
        spriteTrans = face.gameObject.transform;
        currentStamina = maxStamina;
        thisCollider = GetComponent<Collider2D>();
        scale = transform.localScale.x;
        gameManager = player.GetComponent<PlayahMovement>().gameManager;
    }

    // Update is called once per frame
    void Update()
    {
        if (health.isDead&&!hasDied)
        {
            gameManager.Victory();
            state = State.DEAD;
            stateStart = Time.fixedTime;
            hasDied = true;
        }
        if(gameObject.transform.position.x<leftBound-1 || gameObject.transform.position.x > rightBound + 1 || gameObject.transform.position.y < lowerBound)
        {
            gameObject.transform.position = spawnPoint;
            if(state == State.DASH)
            {
                state = State.UNDETERMINED;
                thisCollider.isTrigger = false;
                thisBody.gravityScale = 1f;
            }
        }

        if(state == State.UNDETERMINED)
        {
            stateStart = Time.fixedTime;
            if (currentStamina <= 0)
            {
                Rest();
            }
            else
            {
                int num = (int)Random.Range(0, 4.999f);
                if (num == 0) Thwomp();
                if (num == 1) Dash();
                if (num == 2) Quake();
                if (num == 3) Burst();
                if (num == 4) Sweep();
            }
        }

        if(state == State.DASH)
        {
            if (gameObject.transform.position.x < rightBound && gameObject.transform.position.x > leftBound && Time.fixedTime > stateStart + dashWarning) 
            {
                thisBody.velocityX = dashSpeed * dir;
            } else if(Time.fixedTime > stateStart + dashWarning)
            {
                state = State.UNDETERMINED;
                thisCollider.isTrigger = false;
                thisBody.gravityScale = 1f;
            }
        }

        if(state == State.THWOMP)
        {
            if (Time.fixedTime < stateStart + (thwompDur/2f))
            {
                Vector2 playerDir = new Vector2(player.transform.position.x - gameObject.transform.position.x, (player.transform.position.y+6f) - gameObject.transform.position.y);
                playerDir.Normalize();
                thisBody.velocity = playerDir * thwompSpeed;
            } else if(Time.fixedTime < stateStart + thwompDur) {
                if (!thisCollider.enabled)
                {
                    thisCollider.enabled = true;
                    thisCollider.isTrigger = true;
                }
                thisBody.velocity = Vector2.down * thwompSpeed;
            } else
            {
                state = State.UNDETERMINED;
                thisCollider.isTrigger = false;
            }
        }

        if(state == State.QUAKE)
        {
            if(Time.fixedTime > stateStart + (quakeDur * 0.8f) && !quaked)
            {
                virtualQuake = Instantiate(quakeDeathBox, spawnPoint - Vector3.up*quakeToFloor, Quaternion.identity);
                quaked = true;
            } else if (Time.fixedTime > stateStart + quakeDur)
            {
                Destroy(virtualQuake);
                quaked = false;
                state = State.UNDETERMINED;
            }
        }

        if(state == State.BURST)
        {
            if(projectilesShot == 0 && Time.fixedTime>stateStart + burstDur/2f)
            {
                for(int i=0; i<(int)(360f/burstProjInterval); i++)
                {
                    Instantiate(burstProjectile, gameObject.transform.position, projAngle);
                    projAngle.eulerAngles = projAngle.eulerAngles + Vector3.forward * burstProjInterval;
                    projectilesShot++;
                }
            }
            if (Time.fixedTime > stateStart + burstDur)
            {
                state = State.UNDETERMINED;
            }
        }

        if(state == State.SWEEP)
        {
            if(Time.fixedTime > stateStart + sweepDur)
            {
                Destroy(sweeper);
                state = State.UNDETERMINED;
            }
            if(Time.fixedTime > stateStart + sweepDur * 0.5f)
            {
                sweeper.transform.RotateAround(transform.position, Vector3.forward, (720f / sweepDur) * Time.deltaTime);
            }
        }

        if(state == State.REST)
        {
            if(Time.fixedTime > stateStart + restDur)
            {
                state = State.UNDETERMINED;
            }
        }

        if(state == State.DEAD)
        {
            if (Time.fixedTime > stateStart + deathDuration) Destroy(gameObject);
            transform.RotateAround(transform.position, Vector3.forward, 1000f * Time.deltaTime);
            scale = (stateStart + deathDuration - Time.fixedTime) / deathDuration;
            transform.localScale = baseVector * scale;
        }
    }

    void Dash()
    {
        thisBody.gravityScale = 0f;
        dir = player.transform.position.x - gameObject.transform.position.x;
        dir /= Mathf.Abs(dir);
        state = State.DASH;
        currentStamina -= dashCost;
        face.sprite = dashFace;
        spriteTrans.localScale = new Vector3(dashFaceScale, dashFaceScale, 1);
        thisCollider.isTrigger = true;
    }

    void Thwomp()
    {
        thisCollider.enabled = false;
        state = State.THWOMP;
        currentStamina -= thwompCost;
        face.sprite = thwompFace;
        spriteTrans.localScale = new Vector3(thwompFaceScale, thwompFaceScale, 1);
    }

    void Quake()
    {
        state = State.QUAKE;
        currentStamina -= quakeCost;
        face.sprite = quakeFace;
        spriteTrans.localScale = new Vector3(quakeFaceScale, quakeFaceScale, 1);
    }

    void Burst()
    {
        state = State.BURST;
        currentStamina -= burstCost;
        face.sprite = burstFace;
        spriteTrans.localScale = new Vector3(burstFaceScale, burstFaceScale, 1);
        projAngle = Quaternion.identity;
        projectilesShot = 0;
    }

    void Sweep()
    {
        state = State.SWEEP;
        currentStamina -= sweepCost;
        face.sprite = sweepFace;
        spriteTrans.localScale = new Vector3(sweepFaceScale, sweepFaceScale, 1);
        float dist = Mathf.Sqrt(Mathf.Pow((player.transform.position.x - gameObject.transform.position.x),2)+ Mathf.Pow((player.transform.position.y - gameObject.transform.position.y), 2));
        sweeper = Instantiate(sweepDeathBox, transform.position + Vector3.down * dist, Quaternion.identity);
    }

    void Rest()
    {
        state = State.REST;
        currentStamina = maxStamina;
        face.sprite = restFace;
        spriteTrans.localScale = new Vector3(restFaceScale, restFaceScale, 1);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o == player)
        {
            if(state == State.DASH || state == State.THWOMP) o.GetComponent<PlayerHealth>().Attack(40f);
        }
        if(!o.TryGetComponent<Health>(out Health health) && state == State.THWOMP)
        {
            state = State.UNDETERMINED;
            thisCollider.isTrigger = false;
        }
    }
}
