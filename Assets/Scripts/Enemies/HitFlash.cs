using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float duration = 0.12f;

    private Color original;
    private Coroutine routine;

    private void Awake()
    {
        if (sprite == null) sprite = GetComponentInChildren<SpriteRenderer>();
        original = sprite.color;
    }

    public void Flash()
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        sprite.color = flashColor;
        yield return new WaitForSeconds(duration);
        sprite.color = original;
        routine = null;
    }
}