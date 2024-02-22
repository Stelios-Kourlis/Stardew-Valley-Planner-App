using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SprinklerT2 : Sprinkler
{
    public SprinklerT2(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public SprinklerT2() : base() {
        Init();
    }

    public SprinklerT2(Vector3Int position, Tilemap tilemap) : base(position, tilemap) {
        Init();
    }

    protected override void Init(){
        texture = Resources.Load("Buildings/Sprinkler2") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.IronBar, 1},
            {Materials.GoldBar, 1},
            {Materials.RefinedQuartz, 1},
        };
        base.Init();
    }
}