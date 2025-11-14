using UnityEngine;

/// <summary>
/// Controls pet movement in the garden
/// Supports: Click-to-move, Keyboard (WASD/Arrows), and can add joystick later
/// </summary>
public class PetController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Control Options")]
    [SerializeField] private bool enableClickToMove = true;
    [SerializeField] private bool enableKeyboardControl = true;
    
    [Header("Animation (Optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private string walkAnimationBool = "isWalking";
    
    private Vector3 targetPosition;
    private bool isMovingToTarget = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        targetPosition = transform.position;
        
        // Setup rigidbody for smooth movement
        if (rb != null)
        {
            rb.gravityScale = 0; // No gravity for top-down
            rb.freezeRotation = true;
        }
    }
    
    void Update()
    {
        HandleInput();
        
        // Update animation if animator exists
        if (animator != null)
        {
            bool isMoving = rb.linearVelocity.magnitude > 0.1f;
            animator.SetBool(walkAnimationBool, isMoving);
        }
    }
    
    void FixedUpdate()
    {
        if (isMovingToTarget)
        {
            MoveToTarget();
        }
    }
    
    void HandleInput()
    {
        // Click-to-move (mobile friendly)
        if (enableClickToMove && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            // Check if we didn't click on UI
            if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                SetTargetPosition(mousePos);
            }
        }
        
        // Keyboard control (PC/testing)
        if (enableKeyboardControl)
        {
            Vector2 input = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
            
            if (input.magnitude > 0)
            {
                // Cancel click-to-move if using keyboard
                isMovingToTarget = false;
                
                // Direct keyboard movement
                Vector2 movement = input * moveSpeed;
                rb.linearVelocity = movement;
                
                // Flip sprite based on direction
                if (input.x != 0)
                {
                    spriteRenderer.flipX = input.x < 0;
                }
            }
            else if (!isMovingToTarget)
            {
                // Stop if no input and not moving to target
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
    
    void SetTargetPosition(Vector3 newTarget)
    {
        targetPosition = newTarget;
        isMovingToTarget = true;
    }
    
    void MoveToTarget()
    {
        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
        
        // Close enough? Stop moving
        if (distanceToTarget < 0.1f)
        {
            isMovingToTarget = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }
        
        // Move towards target
        Vector2 direction = (targetPosition - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        
        // Flip sprite based on direction
        if (direction.x != 0)
        {
            spriteRenderer.flipX = direction.x < 0;
        }
    }
    
    // Public methods for external control (e.g., cutscenes, scripted events)
    public void SetMoveSpeed(float newSpeed)
    {
        moveSpeed = newSpeed;
    }
    
    public void StopMovement()
    {
        isMovingToTarget = false;
        rb.linearVelocity = Vector2.zero;
    }
    
    public bool IsMoving()
    {
        return rb.linearVelocity.magnitude > 0.1f;
    }
}
