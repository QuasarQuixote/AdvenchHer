using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public float maxEnergy = 100f;
    public float damageToEnergyRatio = 2f;
    public float currentEnergy = 0f;
    public bool isDodging = false;

    

    public override void Attack(float damage)
    {
        if(!isDodging) base.Attack(damage);
        else
        {
            IncrementEnergy(damage * damageToEnergyRatio);
        }
        if(health == 0f) if(gameObject.TryGetComponent<PlayahMovement>(out PlayahMovement playah)) playah.Die();
    }
    

    public void IncrementEnergy(float energy)
    {
        float oldEnergy = this.currentEnergy;
        this.currentEnergy = Mathf.Clamp(this.currentEnergy + energy, 0f, this.maxEnergy);
        Debug.Log("Went From "+oldEnergy+" to "+this.currentEnergy);
    }
}
