using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;

    [Header("VFX Settings")]
    [SerializeField] ParticleSystem levelUpParticle;

    [Header("Attacking")]
    [SerializeField] SwordAttack swordAttack;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] float attackDamage = 10f;

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
    PlayerSkills playerSkills;

    bool attacking = false;
    bool readyToAttack = true;
    float gravityModifier = 4;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        playerSkills = new PlayerSkills();

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

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            UseSpinwild();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            UseDash();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            UseBuff();
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            UseFasterAttackSpeed();
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
        playerSkills.AddSkillPoint();
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

        animator.SetFloat("attackSpeedMultiplier", attackSpeed);
        animator.SetTrigger("attack_trig");

        swordAttack.EnableCollider();
        Invoke(nameof(ResetAttack), 1f / attackSpeed);
    }


    public float GetAttackSpeed() { return attackSpeed; }
    public float GetAttackDamage() { return attackDamage; }

    void UseSpinwild()
    {
        if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Spinwild))
        {
            Debug.Log("Spinwild is not unlocked yet!");
            return;
        }

        animator.ResetTrigger("attack_trig");
        animator.SetTrigger("spinwild_trig");

        swordAttack.EnableCollider();
        Invoke(nameof(ResetAttack), 1.5f);
    }

    void UseDash()
    {
        if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Dash))
        {
            Debug.Log("Dash is not unlocked yet!");
            return;
        }

        Vector3 dashPosition = transform.position + transform.forward * 5f;
        transform.position = dashPosition;

    }

    bool buffActive = false;
    void UseBuff()
    {
        if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.Buff) || buffActive)
        {
            Debug.Log("Buff is not available!");
            return;
        }

        buffActive = true;
        attackDamage *= 1.5f;
        healthSystem.TakeDamage(-10);


        Invoke(nameof(ResetBuff), 10f);
    }

    void ResetBuff()
    {
        buffActive = false;
        attackDamage /= 1.5f;
        Debug.Log("Buff expired!");
    }

    bool fasterAttackActive = false;
    void UseFasterAttackSpeed()
    {
        if (!playerSkills.IsSkillUnlocked(PlayerSkills.SkillType.FasterAttackSpeed) || fasterAttackActive)
        {
            Debug.Log("Faster Attack Speed is not available!");
            return;
        }

        fasterAttackActive = true;
        attackSpeed *= 1.5f;
        animator.SetFloat("attackSpeedMultiplier", attackSpeed);


        Invoke(nameof(ResetAttackSpeed), 5f);
    }

    void ResetAttackSpeed()
    {
        fasterAttackActive = false;
        attackSpeed /= 1.5f;
        animator.SetFloat("attackSpeedMultiplier", attackSpeed);
    }


    void ResetAttack()
    {
        attacking = false;
        readyToAttack = true;

        swordAttack.DisableCollider();
    }

    public PlayerSkills GetPlayerSkills()
    {
        return playerSkills;
    }

    void PlayParticle(ParticleSystem particleSystem)
    {
        particleSystem.Play();
    }
}
