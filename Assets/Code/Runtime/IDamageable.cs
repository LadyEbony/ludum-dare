using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageSource
{
    PLAYER,
    ENEMY
}

[System.Serializable]
public struct DamageArgs
{
    public float damage;            // Amount of damage
    public DamageSource source;     // What character entity is inflicting damage?
}

public interface IDamageable
{
    void TakeDamage(DamageArgs args);
}
