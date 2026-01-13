using UnityEngine;
using TMPro;

public class DistanceInputHandler : MonoBehaviour
{
    [Header("References")]
    public TMP_InputField distanceInput;
    public SingleSlitImageSplitter splitter;  // your splitter script
    public RailBoundMultiMover railMover;     // your rail mover script

    [Range(1, 9)]
    public int minDistance = 1;
    public int maxDistance = 9;

    // Call this in InputField OnEndEdit
    public void OnDistanceInputChanged(string input)
    {
        if (!int.TryParse(input, out int value))
            return;

        // Clamp distance to valid range
        value = Mathf.Clamp(value, minDistance, maxDistance);

        // 1️⃣ Update SingleSlitImageSplitter
        if (splitter != null)
        {
            splitter.SetDistanceFromInput(value.ToString());
        }

        // 2️⃣ Update RailBoundMultiMover
        if (railMover != null)
        {
            // Scale value to match rail movement
            railMover.distanceInput.text = value.ToString();
            railMover.SetDistance();
        }
    }
}
