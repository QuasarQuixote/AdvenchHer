using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerScript : MonoBehaviour
{
    public GameObject player;
    public List<GameObject> entities;
    // Update is called once per frame

    void Start()
    {
        foreach(GameObject enemy in entities) enemy.GetComponent<Enemy>().player = player;
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("Spwaner collision");
        if (col.gameObject == player)
        {
            //Debug.Log("Spawning!!! ");
            foreach (GameObject enemy in entities) Instantiate(enemy, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
        
    }
}
