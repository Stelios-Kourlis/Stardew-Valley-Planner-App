using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Silo : Building {

    // public Silo(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public Silo() : base() {
    //     Init();
    // }

    public new void Start(){
        Init();
        base.Start();
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 3;
        texture = Resources.Load("Buildings/Silo") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 100},
            {Materials.Stone, 100},
            {Materials.Clay, 10},
            {Materials.CopperBar, 5}
        };
    }
}
