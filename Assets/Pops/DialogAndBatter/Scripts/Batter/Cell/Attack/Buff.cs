using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public string Name { get; private set; }
    public float Duration { get; set; }

    public Buff(string name, float duration)
    {
        Name = name;
        Duration = duration;
    }

    public abstract void ApplyEffect();
    public abstract void RemoveEffect();
}
