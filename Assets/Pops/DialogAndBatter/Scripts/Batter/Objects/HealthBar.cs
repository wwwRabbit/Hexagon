using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image hpImag;
    public Image hpEffectImg;
    

    float maxHp;
    float currentHp;
    float buffTime = 0.5f;

    Coroutine coroutine;

    public bool isFollow;

    private void Awake()
    {
        hpImag = this.transform.Find("ImgHealthBarHp").GetComponent<Image>();
        hpEffectImg = this.transform.Find("ImgHealthBarHpEffect").GetComponent<Image>();        
    }

    private void Start()
    {
        UpdateState();
    }

    void UpdateState()
    {
        hpImag.fillAmount = currentHp / maxHp;

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(UpdateHpEffect());
    }

    public void SetHealth(float blood, float max)
    {
        currentHp = blood;
        maxHp = max;
        UpdateState();
    }

    IEnumerator UpdateHpEffect()
    {
        float effectLength = hpEffectImg.fillAmount - hpImag.fillAmount;
        float elapsedTime = 0;

        while(elapsedTime < buffTime && effectLength != 0)
        {
            elapsedTime += Time.deltaTime;
            hpEffectImg.fillAmount = Mathf.Lerp(hpImag.fillAmount + effectLength, hpImag.fillAmount, elapsedTime / buffTime);
            yield return null;
        }

        hpEffectImg.fillAmount = hpImag.fillAmount;
    }
}
