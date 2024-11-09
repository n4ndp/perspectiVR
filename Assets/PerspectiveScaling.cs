using UnityEngine;

public class SuperliminalVR : MonoBehaviour
{
    public Transform target;
    public LayerMask targetMask;
    public LayerMask ignoreTargetMask;
    public float offsetFactor;

    private float originalDistance;
    private float originalScale;
    private Vector3 targetScale;
    public Transform playerCamera; // La cámara VR (CenterEyeAnchor).

    void Start()
    {
        originalDistance = 0f;
        originalScale = 0f;
        targetScale = Vector3.one;
    }

    void Update()
    {
        HandleInput();
        ResizeTarget();
    }

    void HandleInput()
    {
        // Usa OVR Input para detectar si se presiona el botón de agarre en el controlador derecho.
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            if (target == null)
            {
                RaycastHit hit;
                // Realiza un Raycast desde la cámara VR.
                if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, Mathf.Infinity, targetMask))
                {
                    target = hit.transform;
                    target.GetComponent<Rigidbody>().isKinematic = true;
                    originalDistance = Vector3.Distance(playerCamera.position, target.position);
                    originalScale = target.localScale.x;
                    targetScale = target.localScale;
                }
            }
            else
            {
                target.GetComponent<Rigidbody>().isKinematic = false;
                target = null;
            }
        }
    }

    void ResizeTarget()
    {
        if (target == null) return;

        RaycastHit hit;
        // Realiza un Raycast para ajustar la posición y calcular el escalado.
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, Mathf.Infinity, ignoreTargetMask))
        {
            target.position = hit.point - playerCamera.forward * offsetFactor * targetScale.x;

            float currentDistance = Vector3.Distance(playerCamera.position, target.position);
            float scaleMultiplier = currentDistance / originalDistance;
            targetScale.x = targetScale.y = targetScale.z = scaleMultiplier;
            target.localScale = targetScale * originalScale;
        }
    }
}