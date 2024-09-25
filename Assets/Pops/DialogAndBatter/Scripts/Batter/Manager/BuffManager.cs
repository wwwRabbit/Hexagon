using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager 
{
    List<Buff> buffs = new List<Buff>();

    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);
        buff.ApplyEffect();
    }

    public void RemoveBuff(Buff buff) 
    {
        buff.RemoveEffect();
        buffs.Remove(buff);
    }

    public void Update(float deltaTime)
    {
        for(int i = buffs.Count - 1; i >= 0; i--)
        {
            Buff buff = buffs[i];
            buff.Duration -= deltaTime;
            if (buff.Duration <= 0)
            {
                RemoveBuff(buff);
            }
        }
    }
}
