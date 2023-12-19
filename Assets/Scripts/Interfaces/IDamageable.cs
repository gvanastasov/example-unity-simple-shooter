using UnityEngine;

/// <summary>
/// Objects that can be damaged.
/// </summary>
public interface IDamageable
{
    /// <summary>
    /// Max health of the object.
    /// </summary>
    int Health { get; }

    /// <summary>
    /// Describes the behaviour of the object when it takes damage.
    /// </summary>
    /// <param name="damage">Amount of damage taken.</param>
    /// <param name="damageOrigin">Origin location of damage.</param>
    void Damage(int damage, Vector3? damageOrigin = null);
}