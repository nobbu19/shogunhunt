using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DashDistortion : MonoBehaviour
{
    public static DashDistortion Instance { get; private set; }

    [SerializeField] private Volume volume;
    [SerializeField] private float intensity = 0.8f;
    [SerializeField] private float fadeSpeed = 6f;

    private ChromaticAberration chromatic;
    private float current;

    void Awake()
    {
        Instance = this;
        volume.profile.TryGet(out chromatic);
    }

    public void Pulse()
    {
        current = intensity;
    }

    void Update()
    {
        if (chromatic == null) return;

        current = Mathf.MoveTowards(current, 0f, fadeSpeed * Time.deltaTime);
        chromatic.intensity.value = current;
    }
}