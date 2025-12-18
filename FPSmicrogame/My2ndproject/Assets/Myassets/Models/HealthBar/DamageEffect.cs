using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffect : MonoBehaviour
{
    [Header("Damage Flash")]
    public Image damageImage; 
    public float flashSpeed = 5f;
    public Color flashColor = new Color(1f, 0f, 0f, 0.3f); 

    [Header("Death Fade")]
    public Image deathFadeImage; 
    public float deathFadeDuration = 2f;

    private Color transparent = new Color(0f, 0f, 0f, 0f);
    private bool isDamageFlashing = false;

    void Start()
    {
        
        if (damageImage != null)
        {
            damageImage.color = transparent;
        }
        if (deathFadeImage != null)
        {
            deathFadeImage.color = transparent;
        }
    }

    
    public void ShowDamageEffect()
    {
        if (damageImage != null && !isDamageFlashing)
        {
            StartCoroutine(DamageFlash());
        }
    }

   
    public void ShowDeathEffect()
    {
        if (deathFadeImage != null)
        {
            StartCoroutine(DeathFade());
        }
    }

    private IEnumerator DamageFlash()
    {
        isDamageFlashing = true;

     
        damageImage.color = flashColor;

      
        while (damageImage.color.a > 0.0f)
        {
            Color currentColor = damageImage.color;
            currentColor.a -= flashSpeed * Time.deltaTime;
            damageImage.color = currentColor;
            yield return null;
        }

        damageImage.color = transparent;
        isDamageFlashing = false;
    }

    private IEnumerator DeathFade()
    {
        float elapsedTime = 0f;
        Color startColor = transparent;
        Color endColor = Color.black; 

        while (elapsedTime < deathFadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / deathFadeDuration);
            deathFadeImage.color = Color.Lerp(startColor, endColor, alpha);
            yield return null;
        }

        deathFadeImage.color = endColor;
    }
}