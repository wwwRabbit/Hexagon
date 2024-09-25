using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMaintainer : MonoBehaviour
{
    public SpriteRenderer mainRender;
    public SpriteRenderer leftMinorRender;
    public SpriteRenderer rightMinorRender;

    UnitBuff transformBuff;
    List<UnitBuff> minorBuffs = new List<UnitBuff>();

    // Start is called before the first frame update
    void Start()
    {
        //enBuff(new buff_HealthUp());
        //enBuff(new buff_TCLookHealth(buffTagForEffectType.friendly));
        //enBuff(new buff_ZXThrow(buffTagForEffectType.friendly));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void enBuff(UnitBuff newBuff) //这是接口
    {
        if (newBuff.ifTransform())
        {
            transformBuff = newBuff;
            mainRender.enabled = true;
            mainRender.sprite = BuffDPFactory._instance.buffDPFactory(newBuff);
        }
        else
        {
            minorBuffs.Add(newBuff);
            while (minorBuffs.Count > 2)
            {
                minorBuffs.RemoveAt(0);
            }
            leftMinorRender.enabled = true;
            leftMinorRender.sprite = BuffDPFactory._instance.buffDPFactory(minorBuffs[0]);
            rightMinorRender.enabled = minorBuffs.Count == 1 ? false : true;
            rightMinorRender.sprite = minorBuffs.Count == 1 ? null :BuffDPFactory._instance.buffDPFactory(minorBuffs[1]);
        }

        if (transformBuff == null)
            mainRender.enabled = false;
        if (minorBuffs.Count < 2)
            rightMinorRender.enabled = false;
        if (minorBuffs.Count < 1)
            leftMinorRender.enabled = false;
    }

    public List<GeneralBuff> getBuffs()
    {
        List<GeneralBuff> toReturn = new List<GeneralBuff>();
        if (transformBuff != null) toReturn.Add(transformBuff as GeneralBuff);
        foreach(UnitBuff uB in minorBuffs)
        {
            toReturn.Add(uB as GeneralBuff);
        }
        return toReturn;
    }

    public UnitBuff returnMain() { return transformBuff; }
    public List<UnitBuff> getSideBuff() { return minorBuffs; }

    public void UseMain() {transformBuff = null; }


    public void switchBuff(UnitBuff bf1, UnitBuff bfn)
    {
        if (transformBuff.returnMessage() == bf1.returnMessage()) transformBuff = null;
        else
        {
            UnitBuff toRemove = null;
            foreach (UnitBuff bf in minorBuffs)
            {
                if (bf.returnMessage() == bf1.returnMessage())
                {
                    toRemove = bf;
                    break;
                }
            }
            if (toRemove != null)
            {
                minorBuffs.Remove(toRemove);
            }
        }
        Debug.Log("here switch");
        enBuff(bfn);
        Debug.Log(transformBuff == null);
    }
}
