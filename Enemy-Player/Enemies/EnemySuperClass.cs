using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Enemy : Entity
{

//THIS CLASS IS DESIGNED TO BE INHERITED FROM
//Using coroutines for attacks is probably better

    protected List<Node> path;
    protected int currentWaypointIndex = 0;
    protected bool isAllowedToMove = true;
    protected bool hasSight = false;
    [SerializeField] protected bool canFly;
    [SerializeField] protected bool doesContactDamage;
    protected float contactDamageCooldown = 1;
    protected float lastContactDamageTime = 0;
    protected Rigidbody2D rb;
    protected Animator animator;
    protected EnemySight sight;
    [SerializeField] protected float quickAttackCooldown;
    [SerializeField] protected float longAttackCooldown;
    [SerializeField] protected float longAttackDuration;
    protected bool isDoingLongAttack = false;
    protected bool isInChargeAnimation = false;
    protected float lastQuickAttackTime = 0;
    protected float lastLongAttackTime = 0;
    protected float longAttackStartTime = 0;
    [SerializeField] private int wavePoint;
    public int WavePoint(){return wavePoint;}


    public void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        sight = GetComponent<EnemySight>();
        Collider2D playerCollider = GetComponent<Collider2D>();
        Collider2D enemyCollider = MainManager.instance.playerRef.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(playerCollider, enemyCollider);

    }

    public override void Hurt(GameObject source, float amount = 0)
    {
        if(permanentStats.currentHealth - amount < 0)
        {
            Die(source);
        }
        else
        {
            permanentStats.currentHealth -= amount;
            if (source.TryGetComponent<Entity>(out var damageSource))
            {
                damageSource.OnEnemyHurt(gameObject);
            }
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (doesContactDamage && collision.gameObject.TryGetComponent<Player>(out Player playerScript))
        {
            if (Time.time - lastContactDamageTime > contactDamageCooldown)
            {
                playerScript.Hurt(gameObject);
                lastContactDamageTime = Time.time;
            }
        }
    }

    public override void Die(GameObject source)
    {
        //play death animation
        if (source.TryGetComponent<Entity>(out var damageSource))
        {
            damageSource.OnEnemyKilled(gameObject);
        }
        Destroy(gameObject);
    }

    public override void Heal(GameObject source, float amount = 0)
    {
        if(permanentStats.currentHealth + amount >= permanentStats.maxHealth)
        {
            permanentStats.currentHealth = permanentStats.maxHealth;
            OnHealed(source);
        }
        else
        {
            permanentStats.currentHealth += amount;
            OnHealed(source);
        }
    }


    protected virtual void QuickAttack()
    {


        lastLongAttackTime = Time.time;
    }

    protected virtual void LongAttack()
    {
        // check for isDoingLongAttack at the start of update and redirect to this if it is true
        if (!isDoingLongAttack)
        {
            //Start the long attack
            isDoingLongAttack = true;
            longAttackStartTime = Time.time;
        }
        else
        {
            if(longAttackStartTime  + longAttackDuration >= Time.time)
            {
                //finish the long attack
                isDoingLongAttack = false;
                lastLongAttackTime = Time.time;
            }
            else
            {
                // do the things that are done during long attack
            }
        }
    }

    protected virtual void StartChargeAnimation()
    {
        isInChargeAnimation = true;
    }

    protected void GetAStarPathToPlayer()
    {
        path = AStarManager.instance.GeneratePath(transform.position, MainManager.instance.playerPos);
        currentWaypointIndex = 0;
    }

    protected void GetAStarPathToNode(Node destinationNode)
    {
        Node startNode = AStarManager.instance.GetClosestNode(transform.position);
        AStarManager.instance.GeneratePath(startNode, destinationNode);
    }

    protected void GetAStarPathToRandomNode()
    {
        Node destinationNode = AStarManager.instance.GetRandomNode();
        GetAStarPathToNode(destinationNode);
    }

    protected void GetAStarPathToRandomNodeInSight()
    {
        Node destinationNode = AStarManager.instance.GetRandomNodeInSight();
        if (destinationNode != null)
        {
            GetAStarPathToNode(destinationNode);
        }
    }

    protected void GetAStarPathToRandomNodeOutOfSight()
    {
        Node destinationNode = AStarManager.instance.GetRandomNodeOutOfSight();
        if (destinationNode != null)
        {
            GetAStarPathToNode(destinationNode);
        }
    }

    protected void GoToAStarPath()
    {
        Vector3 targetWaypoint = path[currentWaypointIndex].transform.position;
        
        

        if (Vector3.Distance(transform.position, targetWaypoint) < 0.1f)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= path.Count)
            {
                path = null; // stop moving
                rb.velocity = Vector3.zero;
                return;
            }
            else
            {
                targetWaypoint = path[currentWaypointIndex].transform.position;
            }
        }

        Vector3 direction = (targetWaypoint - transform.position).normalized;
        SetVelocityConsideringWalls(direction);
    }

    protected void GoToPlayerDirectly()
    {
        Vector3 target = MainManager.instance.playerPos;
        Vector3 direction = (target - transform.position).normalized;
        SetVelocityConsideringWalls(direction);
        
    }

    protected bool IsReadyToAttack()
    {
        return Time.time >= lastLongAttackTime + quickAttackCooldown;
    }

    public void OnSightGained()
    {
        hasSight = true;
    }

    public void OnSightLost()
    {
        hasSight = false;
        GetAStarPathToPlayer();
    }
    public void StopMoving()
    {
        isAllowedToMove = false;
    }

    public void ResumeMoving()
    {
        isAllowedToMove = true;
    }

    protected virtual void FixedUpdate()
    {
      // Enemy logic here
    }

    protected void SetVelocityConsideringWalls(Vector2 direction)
    {
        Vector3 horizontalDirection = direction.x > 0f ? Vector3.right : Vector3.left;
        Vector3 verticalDirection = direction.y > 0f ? Vector3.up : Vector3.down;
        bool verticalWallCheck = direction.y > 0f ? sight.upWallCheck : sight.downWallCheck;
        bool horizontalWallCheck = direction.x > 0f ? sight.rightWallCheck : sight.leftWallCheck;
        if (verticalWallCheck && horizontalWallCheck)
        {
            rb.velocity = Vector2.zero;
        }
        else if (verticalWallCheck)
        {
            rb.velocity =  GetSpeed() * horizontalDirection;
        }
        else if (horizontalWallCheck)
        {
            rb.velocity = GetSpeed() * verticalDirection;
        }
        else
        {
            rb.velocity = GetSpeed() * direction; 
        }
    }
}
