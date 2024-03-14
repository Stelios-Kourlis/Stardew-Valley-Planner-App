using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTooltip : TooltipableGameObject
{
    public override string TooltipMessage {get{
            return gameObject.name switch{
                "settingsButton" => "Open The Setting Menu",
                "PickupButton" => "Pickup Buildings",
                "PlaceButton" => "Place Buildings",
                "DeleteButton" => "Delete Buildings",
                "ShowTotalMaterials" => "Show Total Materials Needed for Farm",
                "ArrowButton" => "Choose Building",
                "ToggleSelfVisibility" => "Toggle Bar",
                "ShowInvalidTiles" => "Show Invalid Tiles",
                _ => "",
            };
        }
    }

    public override void OnAwake(){}

    public override void OnUpdate(){}
}
