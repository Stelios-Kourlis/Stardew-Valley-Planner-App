using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.ClassManager;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using System.Linq;

public class JunimoHut : Building, IRangeEffectBuilding {//todo add range interface

    private Tile greenTile;
    public override string TooltipMessage => "";

    public RangeEffectBuilding RangeEffectBuildingComponent {get; private set;}

    public override void OnAwake(){
        name = GetType().Name;
        BaseHeight = 2;
        base.OnAwake();
        RangeEffectBuildingComponent = new RangeEffectBuilding(this);
        greenTile = LoadTile("GreenTile");
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
    return new List<MaterialInfo>(){
        new(20000, Materials.Coins),
        new(200, Materials.Stone),
        new(9, Materials.Starfruit),
        new(100, Materials.Fiber)
    };
}

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        base.PlacePreview(position);
        RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(new Vector3Int(position.x-7, position.y-8, 0), 17, 17).ToArray());
    }

    public override void Place(Vector3Int position){
        base.Place(position);
        RangeEffectBuildingComponent.HideEffectRange();
    }

    protected override void OnMouseEnter(){
        Vector3Int lowerLeftCorner = BaseCoordinates[0];
        RangeEffectBuildingComponent.ShowEffectRange(GetAreaAroundPosition(new Vector3Int(lowerLeftCorner.x-7, lowerLeftCorner.y-8, 0), 17, 17).ToArray());
    }

    protected override void OnMouseExit(){
        RangeEffectBuildingComponent.HideEffectRange();
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        OnAwake();
        Place(new Vector3Int(x,y,0));
    }

    public void ShowEffectRange(Vector3Int[] RangeArea) => RangeEffectBuildingComponent.ShowEffectRange(RangeArea);
    public void HideEffectRange() => RangeEffectBuildingComponent.HideEffectRange();
}