using UnityEngine;

/// <summary>
/// Represents the behavior of an enemy in the game.
/// </summary>
/// <remarks>
/// This is a very simple and naive implementation of enemy behavior. It should have
/// been implemented via a proper state machine.
/// </remarks>
public class EnemyBehaviour : MonoBehaviour, IDamageable
{
#region Defintions
    /// <summary>
    /// Possible states of the enemy.
    /// </summary>
    public enum EnemyState
    {
        /// <summary>
        /// The enemy stays in place.
        /// </summary>
        Idle,

        /// <summary>
        /// The enemy moves towards a position.
        /// </summary>
        Move,

        /// <summary>
        /// The enemy pics a random position and moves towards it.
        /// </summary>
        // TODO: Implement this state.
        Wander,

        /// <summary>
        /// The enemy attacks the player.
        /// </summary>
        Attack,

        /// <summary>
        /// The enemy chases the player.
        /// </summary>
        Chase,

        /// <summary>
        /// The enemy returns to its original position.
        /// </summary>
        Return
    }
#endregion

#region Serializable Fields
    /// <summary>
    /// Current state of the enemy.
    /// </summary>
    public EnemyState CurrentState;
    
    /// <summary>
    /// Distance from the player at which the enemy will attack.
    /// </summary>
    public int AttackDistance = 5;
    
    /// <summary>
    /// Interval between enemy attacks.
    /// </summary>
    public float AttackInterval = 2;
    
    /// <summary>
    /// Speed at which the enemy moves forward.
    /// </summary>
    public float SpeedForward = 3f;
    
    /// <summary>
    /// Health of the enemy.
    /// </summary>
    [SerializeField]
    private int health = 100;
    
    /// <summary>
    /// Gets or sets the health of the enemy.
    /// </summary>
    public int Health
    {
        get
        {
            return this.health;
        }
        set
        {
            this.health = value;
        }
    }
#endregion

#region Private Fields
    /// <summary>
    /// Sight sensor of the enemy.
    /// </summary>
    private SightSensor cmp_sightSensor;
    
    /// <summary>
    /// Position to return to when the enemy loses sight of the player.
    /// </summary>
    private Vector3 returnPosition;
    
    /// <summary>
    /// Rotation to return to when the enemy loses sight of the player.
    /// </summary>
    private Quaternion returnRotation;

    /// <summary>
    /// Current position to move to.
    /// </summary>
    private Vector3 movePosition;
    
    /// <summary>
    /// Current time since last attack.
    /// </summary>
    private float currentAttackTime;

    /// <summary>
    /// Whether the enemy can attack.
    /// </summary>
    private bool canAttack = true;
    
    /// <summary>
    /// Gun of the enemy.
    /// </summary>
    private GunBehaviour gun = null;
#endregion

#region Unity Callbacks
    void Awake()
    {
        this.cmp_sightSensor = GetComponent<SightSensor>();
        this.gun = GetComponentInChildren<GunBehaviour>();
    }

    void Update()
    {
        switch (CurrentState)
        {
            case EnemyState.Idle:
            {
                IdleState_Update();
                break;
            }
            case EnemyState.Chase:
            {
                ChaseState_Update();
                break;
            }
            case EnemyState.Return:
            {
                ReturnState_Update();
                break;
            }
            case EnemyState.Move:
            {
                MoveState_Update();
                break;
            }
            case EnemyState.Attack:
            {
                AttackState_Update();
                break;
            }
            default:
            {
                IdleState_Update();
                break;
            }
        }
    }
#endregion

#region FSM
    /// <summary>
    /// Idle state update.
    /// </summary>
    /// <remarks>
    /// The enemy will transition to <see cref="EnemyState.Chase"/> if the player is detected.
    /// </remarks>
    private void IdleState_Update()
    {
        if (this.cmp_sightSensor.DetectedObject != null)
        {
            this.CurrentState = EnemyState.Chase;
            this.returnPosition = this.transform.position;
            this.returnRotation = this.transform.rotation;
        }
    }

    /// <summary>
    /// Chase state update.
    /// </summary>
    /// <remarks>
    /// The enemy will transition to <see cref="EnemyState.Return"/> if the player is lost.
    /// The enemy will transition to <see cref="EnemyState.Attack"/> if the player is in range.
    /// </remarks>
    private void ChaseState_Update()
    {
        if (this.cmp_sightSensor.DetectedObject == null)
        {
            this.CurrentState = EnemyState.Return;
        }
        else
        {
            Rotate(this.cmp_sightSensor.DetectedObject.transform.position);

            var inRange = Vector3.Distance(
                this.transform.position, 
                this.cmp_sightSensor.DetectedObject.transform.position) < this.AttackDistance;
            if (inRange)
            {
                this.CurrentState = EnemyState.Attack;
            }
            else            {
                this.transform.Translate(
                    new Vector3(this.transform.forward.x, 0, this.transform.forward.z) * SpeedForward * Time.deltaTime, Space.World);
            }
        }
    }

    /// <summary>
    /// Return state update.
    /// </summary>
    /// <remarks>
    /// The enemy will transition to <see cref="EnemyState.Chase"/> if the player is detected.
    /// The enemy will rotate towards <see cref="this.returnPosition"/> and <see cref="EnemyState.Move"/> towards it. 
    /// </remarks>
    private void ReturnState_Update()
    {
        if (this.cmp_sightSensor.DetectedObject != null)
        {
            this.CurrentState = EnemyState.Chase;
        }
        else
        {
            if (!IsFacing(this.returnPosition))
            {
                Rotate(this.returnPosition);
            }

            this.CurrentState = EnemyState.Move;
            this.movePosition = this.returnPosition;
        }
    }

    /// <summary>
    /// Moves the enemy towards <see cref="this.movePosition"/>.
    /// </summary>
    /// <remarks>
    /// The enemy will transition to <see cref="EnemyState.Idle"/> if it reaches <see cref="this.movePosition"/>.
    /// The enemy will transition to <see cref="EnemyState.Chase"/> if the player is detected.
    /// </remarks>
    private void MoveState_Update()
    {
        if (this.cmp_sightSensor.DetectedObject != null)
        {
            this.CurrentState = EnemyState.Chase;
        }
        else 
        {
            var haveGap = Vector3.Distance(transform.position, this.movePosition) > 0;
            var isBehind = Vector3.Dot(transform.forward, this.movePosition - this.transform.position) < 0;

            if (isBehind) 
            {
                this.transform.position = this.movePosition;
                this.transform.rotation = this.returnRotation;
                this.CurrentState = EnemyState.Idle;
            }
            else if (haveGap)
            {
                this.transform.Translate(
                    new Vector3(this.transform.forward.x, 0, this.transform.forward.z) * SpeedForward * Time.deltaTime, Space.World);
            }
        }
    }

    /// <summary>
    /// Attack state update - the enemy will shoot at the player, when in range.
    /// </summary>
    /// <remarks>
    /// The enemy will transition to <see cref="EnemyState.Return"/> if the player is lost.
    /// The enemy will transition to <see cref="EnemyState.Chase"/> if the player is out of range.
    /// </remarks>
    private void AttackState_Update()
    {
        if (this.cmp_sightSensor.DetectedObject == null)
        {
            this.CurrentState = EnemyState.Return;
        }
        else
        {
            Rotate(this.cmp_sightSensor.DetectedObject.transform.position);

            var inRange = Vector3.Distance(
                this.transform.position, 
                this.cmp_sightSensor.DetectedObject.transform.position) < this.AttackDistance;
            if (inRange)
            {
                Shoot();
            }
            else
            {
                this.CurrentState = EnemyState.Chase;
            }
        }
    }
#endregion

#region Actions
    /// <summary>
    /// Shoots the gun of the enemy, with some delay between shots.
    /// </summary>
    private void Shoot()
    {
        if (canAttack) 
        {
            canAttack = false;
            if (gun != null)
            {
                gun.Shoot();
            }
        }
        else if (currentAttackTime < AttackInterval)
        {
            currentAttackTime += Time.deltaTime;
        }
        else
        {
            canAttack = true;
            currentAttackTime = 0;
        }
    }

    /// <summary>
    /// Rotates the enemy towards a world position.
    /// </summary>
    /// <param name="target">World position.</param>
    private void Rotate(Vector3 target)
    {
        this.transform.LookAt(new Vector3(target.x, this.transform.position.y, target.z));
    }

    /// <summary>
    /// Applies damage to the enemy.
    /// </summary>
    /// <remarks>
    /// The enemy will transition to <see cref="EnemyState.Move"/> if the damage origin is not null.
    /// The enemy will die if its health reaches 0.
    /// </remarks>
    /// <param name="damage">Amount of damage.</param>
    /// <param name="damageOrigin">The origin of damage.</param>
    public void Damage(int damage, Vector3? damageOrigin)
    {
        this.Health -= damage;
        if (this.Health <= 0)
        {
            this.Die();
        }

        if (damageOrigin != null)
        {
            switch (CurrentState)
            {
                case EnemyState.Idle:
                case EnemyState.Wander:
                case EnemyState.Return:
                {
                    if (!IsFacing(damageOrigin.Value))
                    {
                        Rotate(damageOrigin.Value);
                    }

                    this.CurrentState = EnemyState.Move;
                    this.movePosition = damageOrigin.Value;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Called when the enemy dies.
    /// </summary>
    /// <remarks>
    /// The enemy is removed from the scene.
    /// </remarks>
    public void Die()
    {
        LevelManager.Instance.EnemyDestroyed();
        Destroy(this.gameObject);
    }
#endregion

#region Helpers
    /// <summary>
    /// Checks whether the enemy is facing a world position.
    /// </summary>
    /// <param name="target">World position.</param>
    /// <returns></returns>
    private bool IsFacing(Vector3 target)
    {
        var direction = Vector3.Normalize(target - this.transform.position);
        var angle = Vector3.Angle(this.transform.forward, direction);
        return angle == 0;
    }
#endregion
}
