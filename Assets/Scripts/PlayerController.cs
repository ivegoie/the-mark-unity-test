using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;
    [SerializeField] float health = 100;
    [SerializeField] float healthRegeneration = 10;

    [Header("VFX Settings")]
    [SerializeField] ParticleSystem levelUpParticle;

    [Header("Attacking")]
    [SerializeField] float attackDistance = 3f;
    [SerializeField] float attackDelay = 0.5f;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float attackDamage = 1f;

    [Header("References")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference attackAction;


    const string IDLE = "Idle";
    const string RUNNING = "RunForward";
    const string ATTACK_1 = "MeeleeAttack_OneHanded";

    Camera mainCamera;
    Rigidbody rb;
    Animator animator;
    LevelSystem levelSystem;
    Vector2 inputVector;
    Vector3 moveDirection;
    HealthSystem healthSystem;

    bool attacking = false;
    bool readyToAttack = true;
    bool isGrounded;
    int attackCount;
    float gravityModifier = 4;
    string currentAnimationState;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        Physics.gravity *= gravityModifier;
        mainCamera ??= Camera.main;
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

        if (attackAction.action.IsPressed()) Attack();

        UpdateAnimationState();
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

    void LevelUp(object sender, EventArgs e)
    {
        PlayParticle(levelUpParticle);
        moveSpeed += 0.1f;
        animator.SetFloat("runSpeedMultiplier", animator.GetFloat("runSpeedMultiplier") + 0.05f);
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

    void UpdateAnimationState()
    {
        if (!attacking)
        {
            ChangeAnimationState(rb.linearVelocity.magnitude < 0.1f ? IDLE : RUNNING);
        }
    }

    void ChangeAnimationState(string newState)
    {
        if (currentAnimationState == newState) return;
        currentAnimationState = newState;
        animator.CrossFadeInFixedTime(currentAnimationState, 0.2f);
    }

    void Attack()
    {
        if (!readyToAttack || attacking) return;

        attacking = true;
        readyToAttack = false;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(PerformAttackRaycast), attackDelay);

        ChangeAnimationState(ATTACK_1);
        attackCount = (attackCount + 1) % 2;
    }

    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;
    }

    void PerformAttackRaycast()
    {
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out RaycastHit hit, attackDistance))
        {
            HitTarget(hit.point);
        }
    }

    void HitTarget(Vector3 pos)
    {
        Debug.Log($"Hit target at {pos}");
    }

    void PlayParticle(ParticleSystem particleSystem)
    {
        particleSystem.Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
