using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicSword : MonoBehaviour, Sword
{
    public float attackCD = 1f;
    public float attackDuration = 0.25f;
    public PlayerAttack basicHitbox;
    public PlayerAttack powerHitbox;
    public PlayerAttack healHitbox;
    public GameObject player;
    public float basicDamage;
    public float powerDamage;
    public float healDamage;
    public float healVal;
    public float basicKnockback = 3f;
    public float powerKnockback = 8f;
    public float powerSwingCost = 35f;
    public float healSwingCost = 50f;

    private PlayerHealth playerHealth;
    private float attackStart;
    private enum State {rest, primary, power, heal}
    private State state;
    void Start()
    {
        playerHealth = player.GetComponent<PlayerHealth>();
        attackStart = Time.fixedTime - 1f;
        basicHitbox.AttackEnd();
        powerHitbox.AttackEnd();
        healHitbox.AttackEnd();
    }
    public bool CanSwing()
    {
        return attackStart < Time.fixedTime - attackCD;
    }
    public void PrimarySwing()
    {
        attackStart = Time.fixedTime;
        state = State.primary;
        basicHitbox.AttackStart();
    }
    public void PowerSwing()
    {
        attackStart = Time.fixedTime;
        state = State.power;
        powerHitbox.AttackStart();
    }
    public void HealSwing()
    {
        attackStart = Time.fixedTime;
        state = State.heal;
        healHitbox.AttackStart();
    }

    public void Hit(int attackId, GameObject target)
    {
        //Debug.Log("Hit "+target.name);
        if (target == player) return;
        if (target.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Debug.Log("I hit an enemy");
            if (attackId == 0)
            {
                playerHealth.IncrementEnergy(5f);
                Vector3 mousePosition = Input.mousePosition;
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
                Vector2 knockback = new Vector2(worldPoint.x-transform.position.x, worldPoint.y-transform.position.y);
                knockback.Normalize();
                knockback *= basicKnockback;
                enemy.ReceiveHit(basicDamage, knockback);
                //Debug.Log("Knock back:" + knockback);
            }
            if (attackId == 1)
            {
                Vector2 knockback = new Vector2(enemy.transform.position.x - player.transform.position.x, enemy.transform.position.y - player.transform.position.y);
                knockback.Normalize();
                knockback *= powerKnockback;
                enemy.ReceiveHit(powerDamage, knockback);
            }
            if (attackId == 2)
            {
                enemy.ReceiveHit(healDamage, Vector2.zero);
                player.GetComponent<Health>().Heal(healVal);
            }
        }
        if (target.TryGetComponent<Hook>(out Hook hook))
        {
            Debug.Log("I hit a hook");
            if (attackId == 0)
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
                Vector2 dir = new Vector2(worldPoint.x-transform.position.x, worldPoint.y-transform.position.y);
                dir.Normalize();
                //Debug.Log(dir);
                if (hook.CanDashThrough(dir, player.transform)) hook.BigBoom(dir, player);
                else hook.BigBonk(dir, player);
                //Debug.Log("Knock back:" + knockback);
            }
            
        }
    }

    void Update()
    {
        if(state != State.rest && Time.fixedTime > attackStart + attackDuration)
        {
            state = State.rest;
            basicHitbox.AttackEnd();
            powerHitbox.AttackEnd();
            healHitbox.AttackEnd();
        }

        if(Input.GetMouseButton(0) && state == State.rest && CanSwing())
        {
            //Debug.Log("Click!");
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
            transform.right = new Vector2(worldPoint.x - transform.position.x, worldPoint.y - transform.position.y);
            PrimarySwing();
        }
        if (Input.GetMouseButton(1) && state == State.rest && CanSwing() && playerHealth.currentEnergy>(powerSwingCost))
        {
            //Debug.Log("Right Click!");
            PowerSwing();
            playerHealth.IncrementEnergy(-1f*powerSwingCost);
        }
        if (Input.GetMouseButton(2) && state == State.rest && CanSwing() && playerHealth.currentEnergy > (healSwingCost))
        {
            //Debug.Log("Middle Click!");
            Vector3 mousePosition = Input.mousePosition;
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0f));
            transform.right = new Vector2(worldPoint.x - transform.position.x, worldPoint.y - transform.position.y);
            HealSwing();
            playerHealth.IncrementEnergy(-1f * healSwingCost);
        }
        transform.position = player.transform.position;

        if(state == State.power)
        {
            transform.RotateAround(transform.position, Vector3.forward, 1000f * Time.deltaTime);
        }
    }

    

}
