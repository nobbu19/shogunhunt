using UnityEngine;
using UnityEngine.UI;

public class DetectionHUD : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private bool fillWithDetection = false; // true: la barra se llena al ser detectado
    [SerializeField] private Color safeColor = Color.green;
    [SerializeField] private Color dangerColor = Color.red;

    void Update()
    {
        if (DetectionMeter.Instance == null) return;

        float life = DetectionMeter.Instance.Normalized;

        fillImage.fillAmount = fillWithDetection ? 1f - life : life;
        fillImage.color = Color.Lerp(dangerColor, safeColor, life);
    }
}