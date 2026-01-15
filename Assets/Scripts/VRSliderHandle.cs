using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

[RequireComponent(typeof(XRGrabInteractable))]
public class VRSliderHandle : MonoBehaviour
{
    [Header("Slider Settings")]
    public Slider slider;             // The Unity UI Slider
    public TMP_InputField inputField; // Optional: input field for manual value
    public Transform minPoint;        // Left end of slider track
    public Transform maxPoint;        // Right end of slider track

    [Header("Handle Colors")]
    public Color idleColor = Color.blue;
    public Color hoverColor = Color.white;

    private Renderer handleRenderer;
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        handleRenderer = GetComponent<Renderer>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        // Initial color
        handleRenderer.material.color = idleColor;

        // Subscribe to XR events
        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Subscribe to input field change
        if (inputField != null)
            inputField.onEndEdit.AddListener(OnInputChanged);
    }

    private void Update()
    {
        if (grabInteractable.isSelected)
        {
            UpdateSliderValueFromHandle();
        }
    }

    // Update Slider.value from handle position
    private void UpdateSliderValueFromHandle()
    {
        if (slider == null || minPoint == null || maxPoint == null) return;

        Vector3 localPos = transform.localPosition;
        float minX = minPoint.localPosition.x;
        float maxX = maxPoint.localPosition.x;

        // Clamp X
        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
        transform.localPosition = localPos;

        // Map to slider value
        float t = Mathf.InverseLerp(minX, maxX, localPos.x);
        slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, t);

        // Update input field if exists
        if (inputField != null)
            inputField.text = slider.value.ToString("F2");
    }

    // Update handle position if slider value changes (from input field)
    private void UpdateHandlePositionFromSlider()
    {
        // SAFETY GUARDS (do not remove)
        if (!this) return;
        if (!gameObject.activeInHierarchy) return;
        if (slider == null) return;
        if (minPoint == null || maxPoint == null) return;

        float t = Mathf.InverseLerp(slider.minValue, slider.maxValue, slider.value);

        Vector3 localPos = transform.localPosition;
        localPos.x = Mathf.Lerp(minPoint.localPosition.x, maxPoint.localPosition.x, t);
        transform.localPosition = localPos;
    }

    private void OnInputChanged(string value)
    {
        if (!gameObject.activeInHierarchy) return;
        if (slider == null) return;

        if (float.TryParse(value, out float val))
        {
            slider.value = Mathf.Clamp(val, slider.minValue, slider.maxValue);
            UpdateHandlePositionFromSlider();
        }
    }
    private void OnDestroy()
    {
        if (inputField != null)
            inputField.onEndEdit.RemoveListener(OnInputChanged);

        if (grabInteractable != null)
        {
            grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
            grabInteractable.hoverExited.RemoveListener(OnHoverExit);
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }



    #region XR Events
    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        handleRenderer.material.color = hoverColor;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (!grabInteractable.isSelected)
            handleRenderer.material.color = idleColor;
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        handleRenderer.material.color = hoverColor;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        handleRenderer.material.color = idleColor;
    }
    #endregion
}
