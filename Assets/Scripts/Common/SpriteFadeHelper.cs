using System;
using System.Collections;
using UnityEngine;

public static class SpriteFadeHelper
{
    public const float DefaultFadeDuration = 1.5f;

    public static IEnumerator FadeSpriteRenderer(SpriteRenderer spriteRenderer, float duration, Action onComplete = null)
    {
        if (spriteRenderer == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        Color startColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        onComplete?.Invoke();
    }

    public static IEnumerator FadeMaterialPropertyBlock(
        SpriteRenderer spriteRenderer,
        MaterialPropertyBlock propertyBlock,
        float duration,
        Action onComplete = null)
    {
        if (spriteRenderer == null)
        {
            onComplete?.Invoke();
            yield break;
        }

        Color startColor = spriteRenderer.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            Color newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);

            spriteRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", newColor);
            spriteRenderer.SetPropertyBlock(propertyBlock);

            yield return null;
        }

        onComplete?.Invoke();
    }
}
