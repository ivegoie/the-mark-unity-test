using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;

    [Header("VFX Settings")]
    [SerializeField] ParticleSystem levelUpParticle;

    [Header("Attacking")]
    [SerializeField] SwordAttack swordAttack;

    [Header("References")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference attackAction;

    Camera mainCamera;
    Rigidbody rb;
    Animator animator;
    LevelSystem levelSystem;
    Vector2 inputVector;
    Vector3 moveDirection;
    HealthSystem healthSystem;

    bool attacking = false;
    bool readyToAttack = true;
    float gravityModifier = 4;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Physics.gravity *= gravityModifier;
        mainCamera ??= Camera.main;

        StartCoroutine(HealthRegeneration());
    }

    void OnEnable()
    {
        moveAction.action.Enable();
        attackAction.action.Enable();

        attackAction.action.started += _ => Attack();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        attackAction.action.Disable();
        attackAction.action.started -= _ => Attack();
    }

    void Update()
    {
        inputVector = moveAction.action.ReadValue<Vector2>();

        if (!attacking)
        {
            if (attackAction.action.WasPressedThisFrame()) Attack();
        }

        UpdateAnimationState();

        if (attacking && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            attacking = false;
            readyToAttack = true;
        }
    }


    void FixedUpdate()
    {
        Move();
        rb.angularVelocity = Vector3.zero;
    }

    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;
        levelSystem.OnLevelChanged += LevelUp;
    }

    public void SetHealthSystem(HealthSystem healthSystem)
    {
        this.healthSystem = healthSystem;
    }

    public void GainExperience(int amount)
    {
        levelSystem.AddExperience(amount);
    }

    IEnumerator HealthRegeneration()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);

            if (healthSystem.GetCurrentHealth() < healthSystem.GetMaxHealth())
            {
                healthSystem.Heal(Mathf.RoundToInt(healthSystem.GetRegenRate()));
            }
        }
    }

    void LevelUp(object sender, EventArgs e)
    {
        PlayParticle(levelUpParticle);
        moveSpeed += 0.1f;
        animator.SetFloat("runSpeedMultiplier_f", animator.GetFloat("runSpeedMultiplier_f") + 0.05f);
        healthSystem.IncreaseRegenRate(0.5f);
        healthSystem.IncreaseMaxHealth(50);
    }

    void Move()
    {
        moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
        moveDirection = mainCamera.transform.TransformDirection(moveDirection);
        moveDirection.y = 0;
        moveDirection.Normalize();

        rb.linearVelocity = new Vector3(
            moveDirection.x * moveSpeed,
            rb.linearVelocity.y,
            moveDirection.z * moveSpeed
        );

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            rb.rotation = Quaternion.RotateTowards(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(int damage)
    {
        healthSystem.TakeDamage(damage);

        animator.SetTrigger("isHit_trig");
    }

    void UpdateAnimationState()
    {
        float speed = rb.linearVelocity.magnitude;
        animator.SetFloat("speed_f", speed);
    }

    void Attack()
    {
        if (!readyToAttack || attacking) return;

        attacking = true;
        readyToAttack = false;

        animator.SetTrigger("attack_trig");

        swordAttack.EnableCollider();
        Invoke(nameof(ResetAttack), 1);
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;

        swordAttack.DisableCollider();
    }

    void PlayParticle(ParticleSystem particleSystem)
    {
        particleSystem.Play();
    }
}
