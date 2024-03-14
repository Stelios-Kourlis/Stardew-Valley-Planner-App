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

    public enum Type {
        WOOD_FLOOR,
        RUSTIC_PLANK_FLOOR,
        STRAW_FLOOR,
        WEATHERED_FLOOR,
        CRYSTAL_FLOOR,
        STONE_FLOOR,
        STONE_WALKWAY_FLOOR,
        BRICK_FLOOR,
        WOOD_PATH,
        GRAVEL_PATH,
        COBBLESTONE_PATH,
        STEPPING_STONE_PATH,
        CRYSTAL_PATH
    }
    
    private SpriteAtlas atlas;
    // private delegate void FloorPlacedDelegate(Vector3Int position);
    public static event Action<Vector3Int> FloorWasPlaced;
    // private Vector3Int position;
    //private static List<Vector3Int> otherFloors = new List<Vector3Int>();
    private static readonly List<Vector3Int> floors = new List<Vector3Int>();
    // public static FloorType floorType = FloorType.WOOD_FLOOR;
    private Type type = Type.WOOD_FLOOR;
    private new static Tilemap tilemap;

    public new void Start(){
        baseHeight = 1;
        base.Start();
        FloorWasPlaced += AnotherFloorWasPlaced;
        atlas = Resources.Load<SpriteAtlas>("Buildings/FloorAtlas");
        sprite = atlas.GetSprite($"{type}0");
        tilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
    }

    public override void Place(Vector3Int position){
        base.Place(position);
        floors.Add(position);
        hasBeenPlaced = true;
        UpdateTexture(atlas.GetSprite($"{type}{GetFloorFlagsSum(position)}"));
        Tile[] buildingTiles = SplitSprite(sprite);
        tilemap.SetTiles(spriteCoordinates.ToArray(), buildingTiles);
        FloorWasPlaced?.Invoke(position);
    }

    public override void Delete(){
        if(!hasBeenPlaced) return;
        floors.Remove(baseCoordinates[0]);
        tilemap.SetTile(baseCoordinates[0], null);
        GetBuildingController().GetBuildings().Remove(this);
        FloorWasPlaced -= AnotherFloorWasPlaced;
        FloorWasPlaced?.Invoke(baseCoordinates[0]);
        ForceDelete();
    }

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        int height = GetFloorFlagsSum(position);
        Sprite floorSprite = atlas.GetSprite($"{type}{height}");
        if (floors.Contains(position)) GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        GetComponent<Tilemap>().ClearAllTiles();
        GetComponent<Tilemap>().SetTile(position, SplitSprite(floorSprite)[0]);
    }

    protected override void DeletePreview(){
        base.DeletePreview();
        // if (!hasBeenPlaced) return;
        // Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // GetComponent<Tilemap>().ClearAllTiles();
        // int height = GetFloorFlagsSum(currentCell);
        // Sprite floorSprite = atlas.GetSprite($"{type}{height}");
        // GetComponent<TilemapRenderer>().sortingOrder = 500;
        // // Debug.Log(baseCoordinates.Length);
        // if (currentCell == baseCoordinates[0]){
        //     GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        //     GetComponent<Tilemap>().SetTile(currentCell, SplitSprite(floorSprite)[0]);
        // }else GetComponent<Tilemap>().color = OPAQUE;
    }

    private void AnotherFloorWasPlaced(Vector3Int position){
        if(!hasBeenPlaced) return;
        List<Vector3Int> neighbors = GetCrossAroundPosition(position).ToList();
        if (neighbors.Contains(baseCoordinates[0])){
            Debug.Log("Neibhor Floor detected");
            UpdateTexture(atlas.GetSprite($"{type}{GetFloorFlagsSum(baseCoordinates[0])}"));
            // sprite = atlas.GetSprite($"{type}{GetFloorFlagsSum(baseCoordinates[0])}");
            // Tile[] buildingTiles = SplitSprite(sprite);
            // tilemap.SetTiles(spriteCoordinates.ToArray(), buildingTiles);
        }
    }

    public void SetType(Type type){
        this.type = type;
        UpdateTexture(atlas.GetSprite($"{type}0"));
    }

    private int GetFloorFlagsSum(Vector3Int position){
        List<FloorFlag> flags = new List<FloorFlag>();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (floors.Contains(neighbors[0])) flags.Add(FloorFlag.LEFT_ATTACHED);
        if (floors.Contains(neighbors[1])) flags.Add(FloorFlag.RIGHT_ATTACHED);
        if (floors.Contains(neighbors[2])) flags.Add(FloorFlag.BOTTOM_ATTACHED);
        if (floors.Contains(neighbors[3])) flags.Add(FloorFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return type switch{
            Type.WOOD_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Wood)
            },
            Type.RUSTIC_PLANK_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Wood)
            },
            Type.STRAW_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Wood),
                new MaterialInfo(1, Materials.Fiber)
            },
            Type.WEATHERED_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Wood)
            },
            Type.CRYSTAL_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.RefinedQuartz)
            },
            Type.STONE_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Stone)
            },
            Type.STONE_WALKWAY_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Stone)
            },
            Type.BRICK_FLOOR => new List<MaterialInfo>(){
                new MaterialInfo(2, Materials.Clay),
                new MaterialInfo(5, Materials.Stone)
            },
            Type.WOOD_PATH => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Wood)
            },
            Type.GRAVEL_PATH => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Stone)
            },
            Type.COBBLESTONE_PATH => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Stone)
            },
            Type.STEPPING_STONE_PATH => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Stone)
            },
            Type.CRYSTAL_PATH => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.RefinedQuartz)
            },
            _ => throw new Exception("Invalid Floor Type")
        };
    }

    public override string GetBuildingData(){
        return $"{GetType()}|{baseCoordinates[0].x}|{baseCoordinates[0].y}|{type}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        throw new NotImplementedException();
    }
}
