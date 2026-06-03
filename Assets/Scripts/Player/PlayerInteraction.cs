using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;

    private IInteractable currentTarget;
    private IInteractable previousTarget;
    private IInputProvider inputProvider;
    private Vector3 currentTargetPosition;

    private Collider2D[] hitColliders = new Collider2D[10];
    private ContactFilter2D interactableFilter;

    private void Start()
    {
        inputProvider = InputProviderBootstrap.EnsureInputProvider(gameObject);

        interactableFilter = new ContactFilter2D();
        interactableFilter.useLayerMask = true;
        interactableFilter.SetLayerMask(interactableLayer);
        interactableFilter.useTriggers = true; 
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameplayActive())
        {
            ClearInteraction();
            return;
        }

        FindClosestInteractable();
        HandleInteractionUI();
        HandleInput();
    }

    private void FindClosestInteractable()
    {
        int hitCount = Physics2D.OverlapCircle(transform.position, interactRange, interactableFilter, hitColliders);

        IInteractable closestInteractable = null;
        float minimumDistance = interactRange;

        for (int i = 0; i < hitCount; i++)
        {
            IInteractable interactable = hitColliders[i].GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, hitColliders[i].transform.position);
                if (distance < minimumDistance)
                {
                    minimumDistance = distance;
                    closestInteractable = interactable;
                    currentTargetPosition = hitColliders[i].transform.position;
                }
            }
        }

        currentTarget = closestInteractable;
    }

    private void HandleInteractionUI()
    {
        if (currentTarget != null)
        {
            InteractionUI.Instance?.ShowHint(currentTarget.GetHintText(), currentTargetPosition);
        }
        else if (previousTarget != null)
        {
            InteractionUI.Instance?.HideHint();
        }

        previousTarget = currentTarget;
    }

    private void HandleInput()
    {
        if (currentTarget != null && inputProvider.GetButtonDown("Interact"))
        {
            currentTarget.Interact();
            ClearInteraction();
        }
    }

    private void ClearInteraction()
    {
        if (currentTarget != null || previousTarget != null)
        {
            InteractionUI.Instance?.HideHint();
            currentTarget = null;
            previousTarget = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}