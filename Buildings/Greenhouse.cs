using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Greenhouse : Building {
    // public Greenhouse(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public Greenhouse() : base(){
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 6;
        texture = Resources.Load("Buildings/Greenhouse") as Texture2D;
        insideAreaTexture = Resources.Load("BuildingInsides/Greenhouse") as Texture2D;
        _buildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
    }
}
