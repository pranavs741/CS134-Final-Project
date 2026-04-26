using UnityEngine;

[ExecuteAlways]
public class RimColliderBuilder : MonoBehaviour
{
    [Header("Ring Shape")]
    [Tooltip("Distance from the center of the ring out to the middle of the tube.")]
    [SerializeField] private float ringRadius = 0.23f;

    [Tooltip("Thickness of the rim tube (the metal). Real hoops are ~1.6cm.")]
    [SerializeField] private float tubeRadius = 0.012f;

    [Tooltip("How many capsule segments make up the ring. More = smoother but heavier.")]
    [Range(6, 32)]
    [SerializeField] private int segments = 16;

    [Header("Physics")]
    [SerializeField] private PhysicMaterial physicMaterial;

    [Header("Tagging")]
    [Tooltip("Tag applied to each rim segment so the scoring system can detect rim hits. Must already exist in Tags & Layers.")]
    [SerializeField] private string segmentTag = "Rim";

    [ContextMenu("Build Rim Colliders")]
    private void BuildRim()
    {
        ClearRim();

        float circumference = 2f * Mathf.PI * ringRadius;
        float capsuleLength = circumference / segments + tubeRadius * 0.25f;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 localPos = new Vector3(
                Mathf.Cos(angle) * ringRadius,
                0f,
                Mathf.Sin(angle) * ringRadius);

            Vector3 tangent = new Vector3(-Mathf.Sin(angle), 0f, Mathf.Cos(angle));

            GameObject seg = new GameObject($"RimSegment_{i:00}");
            seg.transform.SetParent(transform, false);
            seg.transform.localPosition = localPos;
            seg.transform.localRotation = Quaternion.LookRotation(tangent, Vector3.up);

            if (!string.IsNullOrEmpty(segmentTag))
            {
                try { seg.tag = segmentTag; }
                catch
                {
                    Debug.LogWarning(
                        $"[RimColliderBuilder] Tag '{segmentTag}' does not exist. " +
                        "Add it under Edit > Project Settings > Tags and Layers, then rebuild.");
                }
            }

            CapsuleCollider cap = seg.AddComponent<CapsuleCollider>();
            cap.direction = 2;
            cap.radius = tubeRadius;
            cap.height = capsuleLength;

            if (physicMaterial != null)
                cap.material = physicMaterial;
        }
    }

    [ContextMenu("Clear Rim Colliders")]
    private void ClearRim()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("RimSegment_"))
            {
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}
