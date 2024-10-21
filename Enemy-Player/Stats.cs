using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats: MonoBehaviour{
    //Using a class instead of struct since classes are easier to work with in Unity
    public float damageFlat;
    public float damageMul; 
    public float speedFlat;
    public float speedMul;
    public float shotSpeedFlat;
    public float shotSpeedMul;
    public float reloadTimeReduction; // %0-%100
    public float maxHealth;
    public float currentHealth;
}
