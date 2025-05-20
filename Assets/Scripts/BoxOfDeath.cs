using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOfDeath : MonoBehaviour
{
    private List<GameObject> recentHits;
    private List<float> hitTime;
    public Vector2 normal;
    public float power;
    public float graceTime = 0.1f;
    public float damage = 500f;

    void Start()
    {
        hitTime =  new List<float>();
        recentHits = new List<GameObject>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        
        GameObject o = col.gameObject;
        Debug.Log("I hit "+o.name);
        if (o.TryGetComponent<BossScript>(out BossScript boss)) return;
        if(o.TryGetComponent<footScript>(out footScript feet)) o = feet.gameObject.transform.parent.gameObject;
        int hitIndex = recentHits.IndexOf(o);
        if (hitIndex != -1 && hitTime[hitIndex] < Time.fixedTime - graceTime)
        {
            hitTime.RemoveAt(hitIndex);
            recentHits.RemoveAt(hitIndex);
        }
        else if (hitIndex != -1) return;
        if(o.TryGetComponent<Health>(out Health health))
        {
            recentHits.Add(o);
            hitTime.Add(Time.fixedTime);
            health.Attack(damage);
        }
        if(o.TryGetComponent<PlayahMovement>(out PlayahMovement playah))
        {
            playah.StoreVelocity(normal * power);
            playah.DashReset();
        }
    }

}
