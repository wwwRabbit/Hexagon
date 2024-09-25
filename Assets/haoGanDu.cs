using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class haoGanDu : CardBasic
{
    public Image selfImage;
    Color dark = Color.gray;
    Color lightWhite = Color.white;
    Coroutine dk;

    public Sprite xueyeDown;
    public Sprite gusuiDown;
    public Sprite lingbaDown;
    public Sprite nianmoDown;
    public void Confirm()
    {
        selfImage.color = lightWhite;
        if(dk != null )StopCoroutine(dk);
        dk = StartCoroutine(reDark());
    }

    internal override void Update()
    {
        base.Update();
    }

    IEnumerator reDark()
    {
        yield return new WaitForSeconds(0.3f);
        SetTar(exitPos);
        yield return new WaitForSeconds(1);
        selfImage.color = dark;
    }

    public void SetHaoganTar(Moniter m)
    {
        switch (m)
        {
            case Moniter.xueye:
                selfImage.sprite = xueyeDown;
                break;

            case Moniter.gusui:
                selfImage.sprite = gusuiDown;
                break;

            case Moniter.lingba:
                selfImage.sprite = lingbaDown;
                break;

            case Moniter.nianmo:
                selfImage.sprite = nianmoDown;
                break;
        }
    }
}

public enum Moniter
{
    xueye,
    gusui,
    lingba,
    nianmo
}
