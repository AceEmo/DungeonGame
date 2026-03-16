using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 1.5f;
    private IInteractable currentTarget;

    private void Update()
    {
        FindClosestInteractable();

        if (currentTarget != null)
        {
            InteractionUI.Instance.ShowHint(
                currentTarget.GetHintText(),
                ((MonoBehaviour)currentTarget).transform.position
            );

            if (Input.GetKeyDown(KeyCode.E))
            {
                currentTarget.Interact();
                currentTarget = null;
                InteractionUI.Instance.HideHint();
            }
        }
        else
        {
            InteractionUI.Instance.HideHint();
        }
    }

    void FindClosestInteractable()
    {
        IInteractable closest = null;
        float minDist = interactRange;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRange);

        foreach (var hit in hits)
        {
            IInteractable interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float dist = Vector2.Distance(transform.position, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = interactable;
                }
            }
        }

        currentTarget = closest;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}