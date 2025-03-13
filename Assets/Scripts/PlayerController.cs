using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector2 inputVector;
    Animator animator;
    LevelSystem levelSystem;


    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 720f;
    [SerializeField] float jumpForce = 15f;
    [SerializeField] Camera mainCamera;

    [Header("Animation Settings")]
    [SerializeField] ParticleSystem levelUpParticle;

    [Header("Player Stats")]
    [SerializeField] float attackSpeed;
    [SerializeField] float damage;

    [Header("References")]
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference jumpAction;



    bool isGrounded;
    [SerializeField] float gravityModifier = 4;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        Physics.gravity *= gravityModifier;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }


    public void SetLevelSystem(LevelSystem levelSystem)
    {
        this.levelSystem = levelSystem;

        levelSystem.OnLevelChanged += LevelSystem_OnLevelChanged;
    }

    void LevelSystem_OnLevelChanged(object sender, EventArgs e)
    {
        PlayParticle(levelUpParticle);
        moveSpeed += 0.1f;
        animator.SetFloat("runSpeedMultiplier", animator.GetFloat("runSpeedMultiplier") + 0.05f);

    }

    void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        jumpAction.action.performed += ctx => Jump();
    }

    void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        jumpAction.action.performed -= ctx => Jump();
    }

    void Update()
    {
        inputVector = moveAction.action.ReadValue<Vector2>();

        if (isGrounded)
        {
            animator.SetBool("isJumping_b", false);
        }
    }

    void FixedUpdate()
    {
        Move();
        rb.angularVelocity = Vector3.zero;
    }

    void Move()
    {
        Vector3 moveDirection = new Vector3(inputVector.x, 0, inputVector.y);
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

        animator.SetFloat("speed_f", rb.linearVelocity.magnitude);
    }

    void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            animator.SetTrigger("jump_trig");
            animator.SetBool("isJumping_b", true);
            isGrounded = false;
        }
    }

    void PlayParticle(ParticleSystem particleSystem)
    {
        particleSystem.Play(particleSystem);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping_b", false);
        }
    }
}