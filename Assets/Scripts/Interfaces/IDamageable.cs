using UnityEngine;

public interface IDamageable
{
    int Health { get; }
    void Damage(int damage, Vector3? damageOrigin = null);
}