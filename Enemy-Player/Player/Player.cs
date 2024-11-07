using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

//NOT FINISHED!!

public class Player : Entity
{
    private PlayerInputs playerControls;
    [SerializeField] private InteractCollider interacter;
    [SerializeField] private GameObject firePoint;
    [SerializeField] private GameObject bulletPrefab;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private InputAction move;
    private InputAction fire;
    private InputAction interact;
    Vector2 handPosition;
    Vector2 moveDirection = Vector2.zero;
    Vector2 mousePosition = Vector2.zero;
    private Rigidbody2D rb;

    private void Awake()
    {
        playerControls = new PlayerInputs();
    }
    private void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();

        fire = playerControls.Player.Fire;
        fire.Enable();
        fire.performed += Fire;
        
        interact = playerControls.Player.Interact;
        interact.Enable();
        interact.performed += Interact;

    }
    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        //animator = GetComponent<Animator>();

    }

    void FixedUpdate()
    {
        moveDirection = move.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        rb.velocity = new Vector2(moveDirection.x * GetSpeed(), moveDirection.y * GetSpeed());
        Vector2 characterDirection = mousePosition - (Vector2)(transform.position);
        float characterAngle = Mathf.Atan2(characterDirection.x,characterDirection.y) * Mathf.Rad2Deg;
        rb.rotation = -characterAngle;
        
    }

    private void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("shot");
        Shoot();
    }

    protected virtual void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position, firePoint.transform.rotation);
        ProjectileBase projectileScript = bullet.GetComponent<ProjectileBase>();
        projectileScript.Damage = GetDamage();
        projectileScript.IsFriendly = true;
        projectileScript.Source = gameObject;
        projectileScript.Bounce = 0; // add bounce and passthrough into stats and pass them instead
        projectileScript.PassThrough = 0;
        bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.transform.up * GetShotSpeed(), ForceMode2D.Impulse);
    }

    private void Interact(InputAction.CallbackContext context)
    {
        if (interacter.ReturnHighlighted() != null)
        {
            interacter.ReturnHighlighted().GetComponent<InteractableInterface>().Interact();
        }
    }

    public void SetStats(Stats newStats)
    {
        permanentStats = newStats;
    }

    public override void Hurt(GameObject source, float amount = 0)
    {
        // Always deals 1 damage to player
        if(permanentStats.currentHealth != 1)
        {
            permanentStats.currentHealth--;
            Entity damageSource = source.GetComponent<Entity>();
            if (damageSource != null) 
            {
                damageSource.OnEnemyHurt(gameObject);
            }

            
        }
        else
        {
            permanentStats.currentHealth = 0;
            Die(source);
        }
    }

    public override void Die(GameObject source)
    {
        Entity damageSource = source.GetComponent<Entity>();
        if (damageSource != null)
        {
            damageSource.OnEnemyKilled(gameObject);
        }
        // do the rest of death screen here
    }

    public override void Heal(GameObject source, float amount = 0)
    {
        amount = Mathf.Round(amount);
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


}
