using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Microsoft.Win32;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using UnityEngine.UIElements;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using static Utility.TilemapManager;

public class Floor : Building {
    
    private SpriteAtlas atlas;
    private delegate void FloorPlacedDelegate(Vector3Int position);
    public static event Action<Vector3Int> FloorWasPlaced;
    private Vector3Int position;
    //private static List<Vector3Int> otherFloors = new List<Vector3Int>();
    private static readonly Dictionary<Vector3Int, FloorType> floors = new Dictionary<Vector3Int, FloorType>();
    public static FloorType floorType = FloorType.WOOD_FLOOR;
    private new static Tilemap tilemap;

    public new void Start(){
        baseHeight = 1;//todo do I need this? can it be 0?
        base.Start();
        FloorWasPlaced += AnotherFloorWasPlaced;
        atlas = Resources.Load<SpriteAtlas>("Buildings/FloorAtlas");
        sprite = atlas.GetSprite($"WOOD_FLOOR0");
        tilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
    }

    public override void Place(Vector3Int position){
        if (hasBeenPlaced) return;
        int height = GetFloorFlagsSum(position);
        Sprite floorSprite = atlas.GetSprite($"{floorType}{height}");
        tilemap.SetTile(position, SplitSprite(floorSprite)[0]);
        tilemap.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50;
        if (floors.Keys.Contains(position)) floors[position] = floorType;
        else floors.Add(position, floorType);
        FloorWasPlaced?.Invoke(position);
        baseCoordinates = new Vector3Int[]{new Vector3Int(position.x, position.y, position.z)};
        spriteCoordinates = new Vector3Int[]{new Vector3Int(position.x, position.y, position.z)};
        this.position = position;
        hasBeenPlaced = true;
        UID = name.GetHashCode() + baseCoordinates[0].x + baseCoordinates[0].y;
        // Debug.Log(baseCoordinates.Count());
        // Debug.Log(baseCoordinates[0]);
        InvokeBuildingWasPlaced();
    }

    public override void Delete(){
        // Debug.Log("Delete");
        if(!hasBeenPlaced) return;
        //Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // if (!floors.Keys.Contains(currentCell)) return;
        floors.Remove(position);
        tilemap.SetTile(position, null);
        FloorWasPlaced?.Invoke(position);
        ForceDelete();
    }

    protected override void PlacePreview(){
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        int height = GetFloorFlagsSum(currentCell);
        Sprite floorSprite = atlas.GetSprite($"{floorType}{height}");
        if (floors.Keys.Contains(currentCell)) GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        GetComponent<Tilemap>().ClearAllTiles();
        GetComponent<Tilemap>().SetTile(currentCell, SplitSprite(floorSprite)[0]);
    }

    protected override void DeletePreview(){
        //Debug.Log("DeleteMouseoverEffect");
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        GetComponent<Tilemap>().ClearAllTiles();
        int height = GetFloorFlagsSum(currentCell);
        Sprite floorSprite = atlas.GetSprite($"{floorType}{height}");
        GetComponent<TilemapRenderer>().sortingOrder = 500;
        if (currentCell == position){
            GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
            GetComponent<Tilemap>().SetTile(currentCell, SplitSprite(floorSprite)[0]);
        }else GetComponent<Tilemap>().color = OPAQUE;
    }

    private void AnotherFloorWasPlaced(Vector3Int position){
        List<Vector3Int> neighbors = GetCrossAroundPosition(position).Intersect(floors.Keys).ToList();
        foreach (Vector3Int cell in neighbors) UpdateTile(cell);
    }

    private void UpdateTile(Vector3Int position){
        int height = GetFloorFlagsSum(position);
        Sprite floorSprite = atlas.GetSprite($"{floors[position]}{height}");
        tilemap.GetComponent<Tilemap>().SetTile(position, SplitSprite(floorSprite)[0]);
    }

    private int GetFloorFlagsSum(Vector3Int position){
        List<FloorFlag> flags = new List<FloorFlag>();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (floors.Keys.Contains(neighbors[0])) flags.Add(FloorFlag.LEFT_ATTACHED);
        if (floors.Keys.Contains(neighbors[1])) flags.Add(FloorFlag.RIGHT_ATTACHED);
        if (floors.Keys.Contains(neighbors[2])) flags.Add(FloorFlag.BOTTOM_ATTACHED);
        if (floors.Keys.Contains(neighbors[3])) flags.Add(FloorFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        throw new NotImplementedException();
    }

    public override string GetBuildingData(){
        return $"{GetType()}|{position.x}|{position.y}|{floorType}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        throw new NotImplementedException();
    }
}