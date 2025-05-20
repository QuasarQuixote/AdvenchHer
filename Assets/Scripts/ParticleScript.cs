using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public Vector2 velocity = new Vector2(0,0);
    public float rotationSpeed = 0f;
    public float lifeTime = 0.5f;

    private float spawnTime;
    private Vector3 realVelocity;

    // Start is called before the first frame update
    void Start()
    {
        spawnTime = Time.fixedTime;
        realVelocity = new Vector3(velocity.x, velocity.y, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + realVelocity * Time.deltaTime;
        transform.RotateAround(transform.position, Vector3.forward, rotationSpeed * Time.deltaTime);
        if (Time.fixedTime > spawnTime + lifeTime) Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
