using Dialog;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/DialogData", fileName = "TestData")]
public class DialogData : ScriptableObject
{
    public DialogNode[] dialogs;
}

