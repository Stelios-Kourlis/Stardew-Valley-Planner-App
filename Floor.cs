using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Codice.Client.BaseCommands;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using static Utility.TilemapManager;

public class Floor : Building {
    
    private SpriteAtlas atlas;
    private delegate void FloorPlacedDelegate(Vector3Int position);
    public static event Action<Vector3Int> FloorWasPlaced;
    private static List<Vector3Int> otherFloors = new List<Vector3Int>();
    private FloorType floorType = FloorType.WOOD_FLOOR;
    private new static Tilemap tilemap;

    protected override void Init(){
        baseHeight = 1;
    }

    public new void Start(){
        Init();
        base.Start();
        PlaceBuilding = Place;
        PlacePreview = PlaceMouseoverEffect;
        FloorWasPlaced += AnotherFloorWasPlaced;
        atlas = Resources.Load<SpriteAtlas>("Buildings/FloorAtlas");
        sprite = atlas.GetSprite("Wood0");
        tilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
    }

    public new void Place(Vector3Int position){
        if (otherFloors.Contains(position)) return;
        // baseCoordinates = new Vector3Int[]{position};
        // spriteCoordinates = new Vector3Int[]{position};
        int height = GetFloorFlagsSum(position);
        Sprite floorSprite = atlas.GetSprite($"Wood{height}");
        tilemap.SetTile(position, SplitSprite(floorSprite)[0]);
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50;
        otherFloors.Add(position);
        FloorWasPlaced?.Invoke(position);
        //hasBeenPlaced = true;
        
        //InvokeBuildingWasPlaced();
    }

    public new void PlaceMouseoverEffect(){
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        int height = GetFloorFlagsSum(currentCell);
        Sprite floorSprite = atlas.GetSprite($"Wood{height}");
        if (otherFloors.Contains(currentCell)) GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        GetComponent<Tilemap>().ClearAllTiles();
        GetComponent<Tilemap>().SetTile(currentCell, SplitSprite(floorSprite)[0]);
    }

    private void AnotherFloorWasPlaced(Vector3Int position){
        List<Vector3Int> neighbors = GetCrossAroundPosition(position).Intersect(otherFloors).ToList();
        foreach (Vector3Int cell in neighbors) UpdateTile(cell);
    }

    private void UpdateTile(Vector3Int position){
        int height = GetFloorFlagsSum(position);
        Sprite floorSprite = atlas.GetSprite($"Wood{height}");
        tilemap.GetComponent<Tilemap>().SetTile(position, SplitSprite(floorSprite)[0]);
    }

    private int GetFloorFlagsSum(Vector3Int position){
        List<FloorFlag> flags = new List<FloorFlag>();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (otherFloors.Contains(neighbors[0])) flags.Add(FloorFlag.LEFT_ATTACHED);
        if (otherFloors.Contains(neighbors[1])) flags.Add(FloorFlag.RIGHT_ATTACHED);
        if (otherFloors.Contains(neighbors[2])) flags.Add(FloorFlag.BOTTOM_ATTACHED);
        if (otherFloors.Contains(neighbors[3])) flags.Add(FloorFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }
}
