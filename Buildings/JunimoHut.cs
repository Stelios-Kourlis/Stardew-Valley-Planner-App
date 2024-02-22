using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class JunimoHut : Building {
    public JunimoHut(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
        Init();
    }

    public JunimoHut() : base() {
        Init();
    }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 2;
        texture = Resources.Load("Buildings/Junimo Hut") as Texture2D;
        _materialsNeeded = new Dictionary<Materials, int>(){
            {Materials.Coins, 20_000},
            {Materials.Stone, 200},
            {Materials.Starfruit, 9},
            {Materials.Fiber, 100}
        };
    }
}