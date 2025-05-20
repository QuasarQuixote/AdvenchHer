using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostScript : MonoBehaviour
{
    
    public float lifeTime;
    public GameObject player;
    public float energyPerDamage = 2f;
    public GameObject particle;

    private float currentEnergy;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        currentEnergy = 0;
        startTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.fixedTime > startTime + lifeTime) Expire();
    }

    private void Expire()
    {
        player.GetComponent<PlayerHealth>().IncrementEnergy(currentEnergy);
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        GameObject o = col.gameObject;
        if (o.TryGetComponent<Projectile>(out Projectile projectile))
        {
            if (projectile.dodged) return;
            currentEnergy += projectile.damage * energyPerDamage;
            player.GetComponent<PlayahMovement>().DashReset();
            Instantiate(particle, o.transform.position-(Vector3.forward*0.1f), Quaternion.identity);
        }
    }
}
