using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ChangeBuildingTierbutton : BuildingActionsButton
{
    public override void Initialize(){
        position = new Vector3Int(0, 0, 0);
        icon = Resources.Load("UI/Button1") as Texture2D;
        sizeScale = 1.0f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
