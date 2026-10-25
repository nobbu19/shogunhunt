using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color startColor;
    private float duration;
    private float timer;

    public void Init(SpriteRenderer source, Color color, float fadeTime)
    {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = source.sprite;
        sr.flipX = source.flipX;
        sr.sortingLayerID = source.sortingLayerID;
        sr.sortingOrder = source.sortingOrder - 1;
        sr.color = color;

        transform.position = source.transform.position;
        transform.rotation = source.transform.rotation;
        transform.localScale = source.transform.lossyScale;

        startColor = color;
        duration = fadeTime;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float a = 1f - timer / duration;
        sr.color = new Color(startColor.r, startColor.g, startColor.b, startColor.a * a);

        if (timer >= duration) Destroy(gameObject);
    }
}