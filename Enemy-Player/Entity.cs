using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Stats permanentStats;
    public TemporaryStats temporaryStats;
    public float GetDamage()
    {
        return (temporaryStats.damageFlat + permanentStats.damageFlat) * (temporaryStats.damageMul + permanentStats.damageMul);
    }
    
    public float GetSpeed()
    {
        return (temporaryStats.speedFlat + permanentStats.speedFlat) * (temporaryStats.speedMul + permanentStats.speedMul);
    }

    public float GetShotSpeed()
    {
        return (temporaryStats.shotSpeedFlat + permanentStats.shotSpeedFlat) * (temporaryStats.shotSpeedMul + permanentStats.shotSpeedMul);
    }
    public float GetReloadReduction()
    {
        return (temporaryStats.reloadTimeReduction + permanentStats.reloadTimeReduction);
    }

    public virtual void Heal(GameObject source, float amount = 0)
    {

    }
    public virtual void Hurt(GameObject source, float amount = 0)
    {

    }

    public virtual void Die(GameObject source)
    {
        
    }

    public virtual void OnHealed(GameObject source)
    {

    }

    public virtual void OnEnemyHurt(GameObject affectedEntity)
    {

    }

    public virtual void OnEnemyKilled(GameObject affectedEntity)
    {

    }

   
}
