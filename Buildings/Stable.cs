using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Stable : Building {
    public Stable(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public Stable() : base(){
        Init();
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 2;
        texture = Resources.Load("Buildings/Stable") as Texture2D;
        _buildingInteractions = new ButtonTypes[]{
            ButtonTypes.PAINT
        };
        _materialsNeeded =  new Dictionary<Materials, int>(){
            {Materials.Coins, 10_000},
            {Materials.Hardwood, 100},
            {Materials.IronBar, 5},
        };
    }
}
