using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Sword 
{
    public bool CanSwing();
    public void PrimarySwing();
    public void PowerSwing();
    public void HealSwing();
    public void Hit(int attackID, GameObject target);

}
