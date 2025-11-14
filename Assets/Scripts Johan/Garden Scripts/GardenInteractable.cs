using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Makes objects in the garden interactable (tap to interact)
/// Use for: plants, decorations, collectibles, minigames, etc.
/// </summary>
public class GardenInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 1.5f;
    [SerializeField] private bool requirePetNearby = true;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject highlightEffect;
    [SerializeField] private bool showInteractionPrompt = true;
    [SerializeField] private GameObject interactionPromptUI;
    
    [Header("Interaction Type")]
    [SerializeField] private InteractionType interactionType = InteractionType.Custom;
    [SerializeField] private int xpToGive = 10; // XP instead of points
    [SerializeField] private bool destroyAfterInteraction = false;
    
    [Header("Events")]
    public UnityEvent onInteract;
    
    private PetController pet;
    private bool canInteract = false;
    
    public enum InteractionType
    {
        Custom,          // Uses onInteract event
        CollectXP,       // Gives XP and destroys
        Plant,           // Opens plant minigame
        Decoration,      // Shows decoration info
        LevelGate        // Unlocks new area
    }
    
    void Start()
    {
        pet = FindObjectOfType<PetController>();
        
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(false);
        }
        
        if (interactionPromptUI != null)
        {
            interactionPromptUI.SetActive(false);
        }
    }
    
    void Update()
    {
        CheckInteractionRange();
        HandleInteractionInput();
    }
    
    void CheckInteractionRange()
    {
        if (pet == null) return;
        
        float distance = Vector2.Distance(transform.position, pet.transform.position);
        bool wasCanInteract = canInteract;
        canInteract = !requirePetNearby || distance <= interactionRange;
        
        // Visual feedback when in range
        if (canInteract != wasCanInteract)
        {
            if (highlightEffect != null)
            {
                highlightEffect.SetActive(canInteract);
            }
            
            if (interactionPromptUI != null && showInteractionPrompt)
            {
                interactionPromptUI.SetActive(canInteract);
            }
        }
    }
    
    void HandleInteractionInput()
    {
        // Click/tap to interact
        if (canInteract && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
            // Check if clicked on this object
            Collider2D hitCollider = Physics2D.OverlapPoint(mousePos);
            if (hitCollider != null && hitCollider.gameObject == gameObject)
            {
                Interact();
            }
        }
    }
    
    public void Interact()
    {
        Debug.Log($"Interacted with: {gameObject.name}");
        
        // Handle different interaction types
        switch (interactionType)
        {
            case InteractionType.CollectXP:
                CollectXP();
                break;
                
            case InteractionType.Plant:
                OpenPlantMinigame();
                break;
                
            case InteractionType.Decoration:
                ShowDecorationInfo();
                break;
                
            case InteractionType.LevelGate:
                TryUnlockLevel();
                break;
                
            case InteractionType.Custom:
                // Custom behavior via Unity Events
                break;
        }
        
        // Trigger custom events
        onInteract?.Invoke();
        
        // Destroy if set
        if (destroyAfterInteraction)
        {
            Destroy(gameObject);
        }
    }
    
    void CollectXP()
    {
        GardenManager garden = FindObjectOfType<GardenManager>();
        if (garden != null)
        {
            garden.AddXP(xpToGive);
            Debug.Log($"Collected {xpToGive} XP!");
        }
    }
    
    void OpenPlantMinigame()
    {
        Debug.Log("Opening plant minigame...");
        // TODO: Integrate with your plant/watering minigame
    }
    
    void ShowDecorationInfo()
    {
        Debug.Log("Showing decoration info...");
        // TODO: Show UI panel with decoration details
    }
    
    void TryUnlockLevel()
    {
        Debug.Log("Attempting to unlock new level...");
        // TODO: Check requirements and unlock
    }
    
    // Helper to visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
