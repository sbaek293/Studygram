using UnityEngine;

/// <summary>
/// Smooth camera that follows the pet around the garden
/// Supports boundaries so camera doesn't show outside the garden
/// </summary>
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // The pet to follow
    
    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    
    [Header("Boundaries (Optional)")]
    [SerializeField] private bool useBoundaries = false;
    [SerializeField] private float minX = -10f;
    [SerializeField] private float maxX = 10f;
    [SerializeField] private float minY = -10f;
    [SerializeField] private float maxY = 10f;
    
    private Camera cam;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Auto-find pet if not assigned
        if (target == null)
        {
            PetController pet = FindObjectOfType<PetController>();
            if (pet != null)
            {
                target = pet.transform;
            }
        }
    }
    
    void LateUpdate()
    {
        if (target == null) return;
        
        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // Smoothly move camera
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
    
    // Helper to set boundaries based on garden size
    public void SetBoundaries(float width, float height, Vector2 center)
    {
        useBoundaries = true;
        
        // Calculate camera view size
        float cameraHeight = cam.orthographicSize * 2;
        float cameraWidth = cameraHeight * cam.aspect;
        
        // Set boundaries so camera doesn't show outside garden
        minX = center.x - (width / 2) + (cameraWidth / 2);
        maxX = center.x + (width / 2) - (cameraWidth / 2);
        minY = center.y - (height / 2) + (cameraHeight / 2);
        maxY = center.y + (height / 2) - (cameraHeight / 2);
    }
}
