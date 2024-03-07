using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于实现当玩家经过的时候实现该对象的阴影剔除
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    public void FadeOut()
    {
        StartCoroutine(nameof(FadeOutCoroutine));
    }

    public void FadeIn()
    {
        StartCoroutine(nameof(FadeInCoroutine));
    }

    private IEnumerator FadeOutCoroutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = currentAlpha - Settings.targetFadeAlpha;

        while(currentAlpha - Settings.targetFadeAlpha > 0.01f)
        {
            currentAlpha -= distance / Settings.fadeOutTime * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, Settings.targetFadeAlpha);
    }

    private IEnumerator FadeInCoroutine()
    {
        float currentAlpha = spriteRenderer.color.a;
        float distance = 1f - currentAlpha;
        while (1f - currentAlpha > 0.01f)
        {
            currentAlpha += distance / Settings.fadeOutTime * Time.deltaTime;
            spriteRenderer.color = new Color(1f, 1f, 1f, currentAlpha);
            yield return null;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
}
