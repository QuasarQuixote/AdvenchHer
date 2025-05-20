using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealtBarScript : MonoBehaviour
{
    public bool forHealth;
    public PlayerHealth health;
    public RectTransform thisTransform;
    public float height = 30f;
    public float leftBound;
    public float rightBound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (forHealth)
        {
            thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (health.health / health.maxHealth)*(rightBound-leftBound));
        } else
        {
            thisTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (health.currentEnergy / health.maxEnergy)*(rightBound-leftBound));
        }
    }
}
