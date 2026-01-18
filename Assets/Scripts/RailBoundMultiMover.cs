using UnityEngine;
using TMPro;

public class RailBoundMultiMover : MonoBehaviour
{
    Vector3 screenStartPos;
    Vector3 holderStartPos;
    Vector3 rodStartPos;
    Vector3 baseStartPos;
    float initialDistance;


    [Header("Slit Base Reference")]
    public Transform slitBase;   // empty object placed at base front
    [Header("Collision Safe Stop")]
    public Collider screenCollider;
    public Collider slitBaseCollider;

    public float safetyGap = 0.001f; // tiny gap so they never touch
    float slitSafeDistance;
    float baseToScreenDistance;
    public TextMeshProUGUI errorText;


    public Transform singleSlit;
    float originalScreenDistance;
    float slitToBaseDistance;

    // All parts listed separately (NO parenting)
    public Transform screen;
    public Transform holder;
    public Transform rod;
    public Transform basePlate;

    public Transform railDirection; // empty object aligned to rails
    public TMP_InputField distanceInput;

    public float smoothSpeed = 5f;

    float currentDistance;
    float targetDistance;
    

    Vector3 screenOffset;
    Vector3 holderOffset;
    Vector3 rodOffset;
    Vector3 baseOffset;

    void Start()
    {
        screenOffset = screen.position - singleSlit.position;
        holderOffset = holder.position - singleSlit.position;
        rodOffset = rod.position - singleSlit.position;
        baseOffset = basePlate.position - singleSlit.position;

        Vector3 railDir = railDirection.forward.normalized;

        // FRONT face of slit base
        float slitBaseFront =
            Vector3.Dot(slitBaseCollider.bounds.max - singleSlit.position, railDir);

        // FRONT face of screen
        float screenFront =
            Vector3.Dot(screenCollider.bounds.min - singleSlit.position, railDir);

        // Minimum safe distance (NO collision)
        slitSafeDistance = slitBaseFront - safetyGap;

        // Distance from safe stop → original screen position
        baseToScreenDistance = screenFront - slitSafeDistance;

        // Start at original position
        currentDistance = screenFront;
        targetDistance = currentDistance;
        screenStartPos = screen.position;
        holderStartPos = holder.position;
        rodStartPos = rod.position;
        baseStartPos = basePlate.position;

    }



    void Update()
    {
        currentDistance = Mathf.Lerp(
            currentDistance,
            targetDistance,
            Time.deltaTime * smoothSpeed
        );

        Vector3 moveVector = railDirection.forward * (currentDistance - initialDistance);

        screen.position = screenStartPos + moveVector;
        holder.position = holderStartPos + moveVector;
        rod.position = rodStartPos + moveVector;
        basePlate.position = baseStartPos + moveVector;


        holder.position =
            singleSlit.position +
            moveVector +
            Vector3.ProjectOnPlane(holderOffset, railDirection.forward);

        rod.position =
            singleSlit.position +
            moveVector +
            Vector3.ProjectOnPlane(rodOffset, railDirection.forward);

        basePlate.position =
            singleSlit.position +
            moveVector +
            Vector3.ProjectOnPlane(baseOffset, railDirection.forward);
    }

    public void SetDistance()
    {
        if (string.IsNullOrEmpty(distanceInput.text)) return;

        if (!float.TryParse(distanceInput.text, out float d)) return;

        if (d < 0f || d > 10f)
        {
            Debug.LogWarning("Distance must be between 0 and 10");
            return;
        }

        float scaled = (d / 10f) * baseToScreenDistance;

        // FINAL SAFE TARGET
        targetDistance = slitSafeDistance + scaled;
    }
    public void SetDistanceFromImageIndex(int distanceIndex)
    {
        // distanceIndex = 1 to 9
        float normalized = (distanceIndex - 1) / 8f; // 0 → 1
        targetDistance = slitSafeDistance + normalized * baseToScreenDistance;
    }



}
