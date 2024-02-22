using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Sprinkler : Building{

    // public Sprinkler(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) : base(position, basePosition, tilemap) {
    //     Init();
    // }

    // public Sprinkler() : base() {
    //     Init();
    // }

    // public Sprinkler(Vector3Int position, Tilemap tilemap) : base(new Vector3Int[] { position }, new Vector3Int[] { position }, tilemap) {
    //     Init();
    // }

    protected override void Init(){
        name = GetType().Name;
        baseHeight = 1;
    }
    
}
