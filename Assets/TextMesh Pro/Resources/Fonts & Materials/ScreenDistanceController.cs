using UnityEngine;
using TMPro;

public class ScreenDistanceController : MonoBehaviour
{
    [Header("References")]
    public Transform singleSlit;
    public Transform screen;
    public Transform holder;
    public Transform rod;
    public Transform basePlate;
    public Transform railDirection; // Direction along which objects move
    public TMP_InputField distanceInput;

    [Header("Settings")]
    public float minDistance = 0.5f;
    public float maxDistance = 10f;
    public float smoothSpeed = 5f;

    private float targetDistance;

    void Start()
    {
        // Initial distance
        targetDistance = Vector3.Distance(screen.position, singleSlit.position);

        if (distanceInput != null)
            distanceInput.onEndEdit.AddListener(OnDistanceInputChanged);
    }

    void Update()
    {
        MoveObjects();
    }

    void MoveObjects()
    {
        if (railDirection == null) return;

        Vector3 startPos = singleSlit.position;
        Vector3 dir = (railDirection.position - singleSlit.position).normalized;

        Vector3 targetPos = startPos + dir * targetDistance;

        // Smooth move each object individually
        if (screen != null)
            screen.position = Vector3.Lerp(screen.position, targetPos, Time.deltaTime * smoothSpeed);
        if (holder != null)
            holder.position = Vector3.Lerp(holder.position, targetPos, Time.deltaTime * smoothSpeed);
        if (rod != null)
            rod.position = Vector3.Lerp(rod.position, targetPos, Time.deltaTime * smoothSpeed);
        if (basePlate != null)
            basePlate.position = Vector3.Lerp(basePlate.position, targetPos, Time.deltaTime * smoothSpeed);
    }

    void OnDistanceInputChanged(string input)
    {
        if (float.TryParse(input, out float dist))
        {
            targetDistance = Mathf.Clamp(dist, minDistance, maxDistance);
        }
    }
}
