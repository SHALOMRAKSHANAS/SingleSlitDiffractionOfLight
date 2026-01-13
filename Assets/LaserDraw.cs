using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserDraw : MonoBehaviour
{
    [Header("Laser Objects")]
    public Transform laserSource;   // base of laser
    public Transform singleSlit;    // slit center
    public Transform screen;        // screen plane
    public Transform laserTip;      // small sphere placed on screen

    [Header("Diffraction Images")]
    public Texture2D[] diffractionImages; // 1m → 9m
    [Range(1, 9)]
    public int distance = 1; // current distance input

    private LineRenderer lr;

    [Header("Tip Offset (above screen)")]
    public float tipOffset = 0.01f; // 1 cm above screen

    void Awake()
    {
        lr = GetComponent<LineRenderer>();

        // LineRenderer settings (force straight laser look)
        lr.positionCount = 3;
        lr.useWorldSpace = true;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.alignment = LineAlignment.View;
        lr.textureMode = LineTextureMode.Stretch;

        Material m = new Material(Shader.Find("Unlit/Color"));
        m.color = Color.red;
        lr.material = m;
    }

    void Update()
    {
        if (laserSource == null || singleSlit == null || screen == null)
            return;

        // --- Laser line ---
        Vector3 p0 = laserSource.position;
        Vector3 p1 = singleSlit.position;
        Vector3 dir = (p1 - p0).normalized;

        float screenDistance = Vector3.Dot(screen.position - p0, dir);
        Vector3 p2 = p0 + dir * screenDistance;

        lr.SetPosition(0, p0);
        lr.SetPosition(1, p1);
        lr.SetPosition(2, p2);

        // --- Move laser tip to bright fringe ---
        if (laserTip != null && diffractionImages.Length > 0)
        {
            int index = Mathf.Clamp(distance - 1, 0, diffractionImages.Length - 1);
            Texture2D tex = diffractionImages[index];

            // Find the brightest horizontal column (central fringe)
            int centerX = tex.width / 2;
            float maxIntensity = 0f;

            for (int x = 0; x < tex.width; x++)
            {
                float colIntensity = 0f;
                for (int y = 0; y < tex.height; y++)
                {
                    Color c = tex.GetPixel(x, y);
                    colIntensity += c.r + c.g + c.b;
                }

                if (colIntensity > maxIntensity)
                {
                    maxIntensity = colIntensity;
                    centerX = x;
                }
            }

            // Map texture pixel → local screen coords
            float px = ((float)centerX / tex.width - 0.5f) * screen.localScale.x;
            Vector3 localPos = new Vector3(px, 0f, 0f); // y centered

            // Transform to world position and add offset
            Vector3 tipPos = screen.TransformPoint(localPos) + screen.forward * tipOffset;

            // Move tip
            laserTip.position = tipPos;
        }
    }
}
