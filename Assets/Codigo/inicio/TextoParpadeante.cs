using TMPro;
using UnityEngine;

public class TextoParpadeante : MonoBehaviour
{
    [SerializeField] private float velocidad = 2f;
    private TMP_Text texto;

    private void Awake() => texto = GetComponent<TMP_Text>();

    private void Update()
    {
        float alfa = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f;
        texto.alpha = Mathf.Lerp(0.2f, 1f, alfa);
    }
}