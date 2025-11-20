using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// FIXED: Updated to give Coins instead of XP
/// Makes objects in the garden interactable (tap to interact)
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
    [SerializeField] private int coinsToGive = 10; // CHANGED: XP -> Coins
    [SerializeField] private bool destroyAfterInteraction = false;
    
    [Header("Events")]
    public UnityEvent onInteract;
    
    private PetController pet;
    private bool canInteract = false;
    
    public enum InteractionType
    {
        Custom,          // Uses onInteract event
        CollectCoins,    // CHANGED: Gives Coins and destroys
        Decoration,      // Shows decoration info
    }
    
    void Start()
    {
        pet = FindObjectOfType<PetController>();
        
        if (highlightEffect != null) highlightEffect.SetActive(false);
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
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
        
        if (canInteract != wasCanInteract)
        {
            if (highlightEffect != null) highlightEffect.SetActive(canInteract);
            if (interactionPromptUI != null && showInteractionPrompt) interactionPromptUI.SetActive(canInteract);
        }
    }
    
    void HandleInteractionInput()
    {
        if (canInteract && Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            
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
        
        switch (interactionType)
        {
            case InteractionType.CollectCoins: // CHANGED
                CollectCoins();
                break;
                
            case InteractionType.Decoration:
                Debug.Log("This is a decoration!");
                break;
                
            case InteractionType.Custom:
                break;
        }
        
        onInteract?.Invoke();
        
        if (destroyAfterInteraction)
        {
            Destroy(gameObject);
        }
    }
    
    void CollectCoins() // CHANGED FROM CollectXP
    {
        GardenManager garden = FindObjectOfType<GardenManager>();
        if (garden != null)
        {
            garden.AddCoins(coinsToGive); // FIXED: Calls AddCoins instead of AddXP
        }
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}