using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using static Utility.TilemapManager;
using static Utility.ClassManager;
using static Utility.SpriteManager;
using UnityEngine.Tilemaps;

public class Fence : Building{

    public enum Type{
        Wood,
        Stone,
        Iron,
        Hardwood
    }

    private Type type;
    public static Type currentType = Type.Wood;
    private SpriteAtlas atlas;
    // private delegate void FencePlacedDelegate(Vector3Int position);
    public static event Action<Vector3Int> FenceWasPlaced;
    private Vector3Int position;
    private static readonly List<Vector3Int> fences = new List<Vector3Int>();
    public override List<MaterialInfo> GetMaterialsNeeded(){
        return type switch{
            Type.Wood => new List<MaterialInfo>(){
                new MaterialInfo(2, Materials.Wood)
            },
            Type.Stone => new List<MaterialInfo>(){
                new MaterialInfo(2, Materials.Stone)
            },
            Type.Iron => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.IronBar)
            },
            Type.Hardwood => new List<MaterialInfo>(){
                new MaterialInfo(1, Materials.Hardwood)
            },
            _ => throw new System.Exception("Invalid Fence Type")
        };
    }

    public void SetType(Type type){
        currentType = type;
        this.type = type;
        // Debug.Log(atlas == null);
        // Debug.Log(atlas.GetSprite($"{type}Fence0")==null);
        UpdateTexture(atlas.GetSprite($"{type}Fence0"));
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        throw new System.NotImplementedException();
    }

    void Awake(){
        atlas = Resources.Load<SpriteAtlas>("Fences/FencesAtlas");
        // Debug.Log($"Fence Atlas is null at start: {atlas == null}");
        baseHeight = 1;
        base.Start();
        FenceWasPlaced += AnotherFenceWasPlaced;
        SetType(currentType);
        sprite = atlas.GetSprite($"{type}Fence0");
    }

    public override void Place(Vector3Int position){
        base.Place(position);
        fences.Add(position);
        this.position = position;
        // Debug.Log("Added " + position + " length = " + fences.Count);
        hasBeenPlaced = true;
        FenceWasPlaced?.Invoke(position);
        UpdateTexture(atlas.GetSprite($"{type}Fence{GetFencesFlagsSum(position)}"));
    }

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        UpdateTexture(atlas.GetSprite($"{type}Fence{GetFencesFlagsSum(position)}"));
        base.PlacePreview(position);
    }

    public override void Delete(){
        if (!hasBeenPlaced) return;
        fences.Remove(position);
        FenceWasPlaced -= AnotherFenceWasPlaced;//for some reason this is needed or it crashes at line 'this.gameObject?.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), buildingTiles);' at base.UpdateTexture
        FenceWasPlaced?.Invoke(position);
        base.Delete();
    }

    private void AnotherFenceWasPlaced(Vector3Int position){
        List<Vector3Int> neighbors = GetCrossAroundPosition(position).ToList();
        // Debug.Log(gameObject == null);
        if (neighbors.Contains(this.position)){
            UpdateTexture(atlas?.GetSprite($"{type}Fence{GetFencesFlagsSum(this.position)}"));
        }
    }

    private int GetFencesFlagsSum(Vector3Int position){
        List<FloorFlag> flags = new List<FloorFlag>();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (fences.Contains(neighbors[0])) flags.Add(FloorFlag.LEFT_ATTACHED);
        if (fences.Contains(neighbors[1])) flags.Add(FloorFlag.RIGHT_ATTACHED);
        if (fences.Contains(neighbors[3])) flags.Add(FloorFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

}
