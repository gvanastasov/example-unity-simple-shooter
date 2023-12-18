using UnityEngine;

public class EnemyBehaviour : MonoBehaviour, IDamageable
{
    public enum EnemyState
    {
        Idle,
        Move,
        Wander,
        Attack,
        Chase,
        Return
    }

    public EnemyState CurrentState;
    public int AttackDistance = 5;
    public float AttackInterval = 2;
    public float SpeedForward = 3f;
    [SerializeField]
    private int health = 100;
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

    private SightSensor cmp_sightSensor;
    private Vector3 returnPosition;
    private Quaternion returnRotation;
    private Vector3 movePosition;
    private float currentAttackTime;
    private bool canAttack = true;
    private GunBehaviour gun = null;

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
                IdleStateUpdate();
                break;
            }
            case EnemyState.Chase:
            {
                ChaseStateUpdate();
                break;
            }
            case EnemyState.Return:
            {
                ReturnStateUpdate();
                break;
            }
            case EnemyState.Move:
            {
                MoveStateUpdate();
                break;
            }
            case EnemyState.Attack:
            {
                AttackStateUpdate();
                break;
            }
            default:
            {
                IdleStateUpdate();
                break;
            }
        }
    }

    void IdleStateUpdate()
    {
        if (this.cmp_sightSensor.detectedObject != null)
        {
            this.CurrentState = EnemyState.Chase;
            this.returnPosition = this.transform.position;
            this.returnRotation = this.transform.rotation;
        }
    }

    void ChaseStateUpdate()
    {
        if (this.cmp_sightSensor.detectedObject == null)
        {
            this.CurrentState = EnemyState.Return;
        }
        else
        {
            RotateTowards(this.cmp_sightSensor.detectedObject.transform.position);

            var inRange = Vector3.Distance(
                this.transform.position, 
                this.cmp_sightSensor.detectedObject.transform.position) < this.AttackDistance;
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

    void ReturnStateUpdate()
    {
        if (this.cmp_sightSensor.detectedObject != null)
        {
            this.CurrentState = EnemyState.Chase;
        }
        else
        {
            if (!IsFacing(this.returnPosition))
            {
                RotateTowards(this.returnPosition);
            }

            this.CurrentState = EnemyState.Move;
            this.movePosition = this.returnPosition;
        }
    }

    void MoveStateUpdate()
    {
        if (this.cmp_sightSensor.detectedObject != null)
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

    void AttackStateUpdate()
    {
        if (this.cmp_sightSensor.detectedObject == null)
        {
            this.CurrentState = EnemyState.Return;
        }
        else
        {
            RotateTowards(this.cmp_sightSensor.detectedObject.transform.position);

            var inRange = Vector3.Distance(
                this.transform.position, 
                this.cmp_sightSensor.detectedObject.transform.position) < this.AttackDistance;
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

    void Shoot()
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

    bool IsFacing(Vector3 target)
    {
        var direction = Vector3.Normalize(target - this.transform.position);
        var angle = Vector3.Angle(this.transform.forward, direction);
        return angle == 0;
    }

    void RotateTowards(Vector3 target)
    {
        this.transform.LookAt(new Vector3(target.x, this.transform.position.y, target.z));
    }

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
                        RotateTowards(damageOrigin.Value);
                    }

                    this.CurrentState = EnemyState.Move;
                    this.movePosition = damageOrigin.Value;
                    break;
                }
            }
        }
    }

    public void Die()
    {
        Destroy(this.gameObject);
    }
}
