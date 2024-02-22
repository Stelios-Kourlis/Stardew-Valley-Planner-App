using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SlimeHutch : Building {
    public SlimeHutch(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 6;
        texture = Resources.Load("Buildings/Slime Hutch") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/Barn1") as Texture2D;
        _buildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 10_000},
            {Materials.Stone, 500},
            {Materials.RefinedQuartz, 10},
            {Materials.IridiumBar, 1},
        };
    }
}
