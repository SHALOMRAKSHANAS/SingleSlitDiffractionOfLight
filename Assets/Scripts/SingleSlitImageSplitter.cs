using UnityEngine;
using UnityEngine.UI;

public class SingleSlitImageSplitter : MonoBehaviour
{
    public RailBoundMultiMover railMover;

    [Header("Diffraction Images (1m → 9m)")]
    public Texture2D[] diffractionImages;

    [Header("Screen Renderer (mesh on screen)")]
    public Renderer screenRenderer;

    [Header("UI")]
    public Slider distanceSlider;

    [Range(1, 9)]
    public int distance = 1;

    int lastDistance = -1;
    bool isSyncingUI = false; // 🔥 loop breaker

    void Start()
    {
        ApplyDistance();

    }

    void Update()
    {
        if (distance != lastDistance)
        {
            ApplyDistance();
        }
    }

    // ================= CORE APPLY =================

    void ApplyDistance()
    {
        lastDistance = distance;

        // update image
        int index = Mathf.Clamp(distance - 1, 0, diffractionImages.Length - 1);
        screenRenderer.material.SetTexture("_MainTex", diffractionImages[index]);

        // update physical screen
        if (railMover != null)
            railMover.SetDistanceFromImageIndex(distance);



        Debug.Log("✅ Image + Screen updated to distance " + distance);
    }

    // ================= UI SYNC (NO CALLBACK) =================



    // ================= UI INPUTS =================

    // UI SLIDER
    public void SetDistanceFromSlider(float value)
    {
        if (isSyncingUI) return; // 🔥 STOP LOOP

        distance = Mathf.RoundToInt(value);
        ApplyDistance();

        Debug.Log("🎯 UI SLIDER MOVED → " + distance);
    }

    // INPUT FIELD
    public void SetDistanceFromInput(string input)
    {
        if (!int.TryParse(input, out int value)) return;

        distance = Mathf.Clamp(value, 1, 9);
        ApplyDistance();

        Debug.Log("⌨ INPUT SET → " + distance);
    }
}
