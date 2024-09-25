using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTurnButton : MonoBehaviour
{

    bool endTurnEnabled = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void EndTurn()
    {
        if (endTurnEnabled)
            GameStageControllerScript._instance.EndActionTurn();
    }

    public void SetEndTurnEnabled(bool endEnableState)
    {
        endTurnEnabled = endEnableState;
    }
}
