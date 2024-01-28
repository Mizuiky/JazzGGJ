using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float timeToStable = 1;

    [Header("References")]
    [SerializeField] Transform bowlSocket;
    [SerializeField] ConfigurableJoint rLegJoint;
    [SerializeField] ConfigurableJoint lLegJoint;

    Rigidbody rb;
    ConfigurableJoint cj;
    IInteractable interactableInterface;

    float stability;
    float initialStability;

    bool dragging;
    bool isMoving;
    bool canWalk = true;
    bool canInteract = true;

    Coroutine legRoutine;
    Coroutine loseStability;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        cj = GetComponent<ConfigurableJoint>();
        initialStability = stability;
    }

    private void OnEnable()
    {
        CanvasManager.enableWalk += DisablePlayer;
        InteractableDragItem.draggableObject += DragItem; 
    }

    private void OnDisable()
    {
        CanvasManager.enableWalk -= DisablePlayer;
        InteractableDragItem.draggableObject -= DragItem;
    }

    private void FixedUpdate()
    {
        Movement();
    }

    private void Update()
    {
        Interact();
    }

    public void DisablePlayer(bool value)
    {
        canWalk = value;
        canInteract = value;
    }

    void Movement()
    {
        if (!canWalk) return;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        Vector3 dir = new Vector3(horizontal, 0, vertical).normalized;

        isMoving = dir.magnitude != 0;

        rb.AddForce(dir * movementSpeed, ForceMode.Impulse);

        if (isMoving && legRoutine == null)
        {
            legRoutine = StartCoroutine("LegAnimation");
        }
        if (isMoving)
        {
            Vector3 rotDir = dir;
            rotDir.x = -dir.x;
            Quaternion rotTarget = Quaternion.LookRotation(rotDir);
            cj.targetRotation = rotTarget;
        }
        else if (legRoutine != null)
        {
            StopCoroutine(legRoutine);
            legRoutine = null;
            lLegJoint.targetRotation = Quaternion.identity;
            rLegJoint.targetRotation = Quaternion.identity;
        }
    }

    IEnumerator LegAnimation()
    {
        while (isMoving)
        {
            lLegJoint.targetRotation = Quaternion.Euler(0, 0, 33);
            rLegJoint.targetRotation = Quaternion.Euler(0, 0, 33);

            yield return new WaitForSeconds(0.3f);

            lLegJoint.targetRotation = Quaternion.Euler(0, 0, -33);
            rLegJoint.targetRotation = Quaternion.Euler(0, 0, -33);

            yield return new WaitForSeconds(.3f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactableInterface = interactable;
            }
        }

        if (other.CompareTag("Obstacle"))
        {
            if (loseStability == null)
                loseStability = StartCoroutine("LoseStability");
            else
            {
                StopCoroutine(loseStability);
                loseStability = null;
                loseStability = StartCoroutine("LoseStability");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactableInterface = null;
        }
    }

    void Interact()
    {
        if (!canInteract || interactableInterface == null) return;

        if (Input.GetKeyDown(KeyCode.E))
            interactableInterface.Interact();
    }

    void DragItem(GameObject item)
    {
        if (dragging) return;
        dragging = true;
        Instantiate(item, bowlSocket);
    }

    public void ReleaseItem()
    {

    }

    IEnumerator LoseStability()
    {
        yield return new WaitForSeconds(timeToStable);

        Explode();
    }

    void Explode()
    {
        Debug.Log("exp");
        Vector3 dir = Vector3.forward + Vector3.up;
        rb.AddForce(dir * 10, ForceMode.Impulse);
    }
}
