using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;

public abstract class Barn : Building {

    protected override void Init(){
        baseHeight = 4;
        _buildingInteractions = new ButtonTypes[]{
            ButtonTypes.TIER_ONE,
            ButtonTypes.TIER_TWO,
            ButtonTypes.TIER_THREE,
            ButtonTypes.ENTER,
            ButtonTypes.PAINT
        };
    }

    public new void Start(){
        Init();
        base.Start();
    }

    public void ChangeToTierOne(){
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.ClearAllTiles();
        Texture2D texture = Resources.Load<Texture>("BarnTierOne") as Texture2D;
        Tile[] tiles = SplitSprite(texture ,true);
    }
}
