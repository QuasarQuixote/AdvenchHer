using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayahMovement : MonoBehaviour
{
    //public vars
    public Rigidbody2D thisBody;
    public footScript feet;
    public float jumpPower;
    public float jumpDuration;
    public float downAccel;
    public float horizontalSpeed;
    public float dashSpeed;
    public float dashDuration;
    public float dashCD = 0.25f;
    public float airFriction;
    public float floorFrictionBonus;
    public float counterForce = 10f;
    public PlayerHealth health;
    public GameObject ghost;
    public GameManagerScript gameManager;

    //private vars
    private float appliedHorizVel;
    private float externalHorizVel=0f;
    private float jumpStart;
    private enum SwordState {Neutral, Dash, Nair, Pull, Dead}
    private SwordState state;
    private bool canDash;
    private Vector2 dashDir;
    private float dashStart;
    private Vector2 pullVect;
    private float pullDur;
    private float pullStart;
    private float storedVertical;

    // Start is called before the first frame update
    void Start()
    {
        state = SwordState.Neutral;
        ghost.GetComponent<GhostScript>().player = gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        //Neutral state actions
        if (state == SwordState.Neutral)
        {
            // Jump
            if (feet.grounded && Input.GetKey("space"))
            {
                thisBody.velocityY = jumpPower;
                jumpStart = Time.fixedTime;
            }
            if (Input.GetKey("space") && Time.fixedTime < jumpStart + jumpDuration)
            {
                thisBody.velocityY += jumpPower * Time.deltaTime * 0.75f / jumpDuration;
            }

            // Fast Falling
            if (Input.GetKey("s") && !feet.grounded) thisBody.velocityY -= downAccel * Time.deltaTime;

            // left and right movement
            appliedHorizVel = Input.GetAxis("Horizontal") * horizontalSpeed;
            if (externalHorizVel != 0)
            {
                thisBody.velocityX = appliedHorizVel + externalHorizVel;
            }
            else thisBody.velocityX = appliedHorizVel;

            if (externalHorizVel != 0)
            {
                int sign;
                if (externalHorizVel < 0) sign = -1;
                else sign = 1;
                int onGround = 0;
                if(feet.grounded) onGround = 1;
                int playerOpposing = 0;
                if((Input.GetAxis("Horizontal")>0&&externalHorizVel<0)||(Input.GetAxis("Horizontal") < 0 && externalHorizVel > 0)) playerOpposing = 1;

                externalHorizVel = Mathf.Clamp(externalHorizVel - (Time.deltaTime * sign * (airFriction + (floorFrictionBonus * onGround) + (counterForce*playerOpposing))), (-10f + (10f * sign)), (10f + (10f * sign)));
            }

            if (storedVertical != 0)
            {
                thisBody.velocityY += storedVertical;
                storedVertical = 0;
            }

            // Start dash
            if (Input.GetKey("left shift") && canDash)
            {
                Debug.Log("Dash!");
                canDash = false;
                state = SwordState.Dash;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dashDir.x = mousePos.x - thisBody.position.x;
                dashDir.y = mousePos.y - thisBody.position.y;
                dashDir.Normalize();
                dashStart = Time.fixedTime;
                health.isDodging = true;
                
                Instantiate(ghost, transform.position, Quaternion.identity);
            }

            //Reset ground dashes
            if(Time.fixedTime>dashStart+dashDuration+dashCD && feet.grounded)
            {
                canDash = true;
            }
        }
        //Dash mechanics
        else if (state == SwordState.Dash)
        {
            if(Time.fixedTime < dashStart + dashDuration)
            {
                thisBody.velocity = dashDir * dashSpeed;
            }
            else
            {
                state = SwordState.Neutral;
                thisBody.velocityY = 0.33f * thisBody.velocityY;
                externalHorizVel = 0.33f * thisBody.velocityX;
                health.isDodging = false;
            }
        }

        else if (state == SwordState.Pull)
        {
            thisBody.velocity = pullVect / pullDur;
            if (Time.fixedTime > pullStart + pullDur)
            {
                gameObject.GetComponent<PolygonCollider2D>().isTrigger = false;
                state = SwordState.Neutral;
                thisBody.gravityScale = 1f;
                thisBody.velocity = new Vector2(0f, 0f);
            }
        }

    }

    public void DashReset()
    {
        canDash = true;
    }

    public void PullTo(Vector2 dropPos, float pullDuration)
    {
        Debug.Log("Pulling to: " + dropPos);
        pullVect = (dropPos - (new Vector2(transform.position.x, transform.position.y)));
        pullStart = Time.fixedTime;
        pullDur = pullDuration;
        state = SwordState.Pull;
        gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        thisBody.gravityScale = 0f;
    }

    public void StoreVelocity(Vector2 velocity)
    {
        storedVertical += velocity.y;
        externalHorizVel += velocity.x;
    }

    public void Die()
    {
        Debug.Log("Player Died!");
        gameManager.Lose();
    }
}
