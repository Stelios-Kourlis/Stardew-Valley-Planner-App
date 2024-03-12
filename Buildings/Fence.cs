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

    private Type type = Type.Wood;
    private SpriteAtlas atlas;
    // private delegate void FencePlacedDelegate(Vector3Int position);
    public static event Action<Vector3Int> FenceWasPlaced;
    private Vector3Int position;
    private static Dictionary<Vector3Int, Type> fences = new Dictionary<Vector3Int, Type>();
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

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        throw new System.NotImplementedException();
    }

    new void Start(){
        atlas = Resources.Load<SpriteAtlas>("Fences/FencesAtlas");
        baseHeight = 1;
        base.Start();
        FenceWasPlaced += AnotherFenceWasPlaced;
        sprite = atlas.GetSprite($"{type}Fence0");
    }

    public override void Place(Vector3Int position){
        if (hasBeenPlaced) return;
        if (GetBuildingController().GetUnavailableCoordinates().Contains(position)) return;
        base.Place(position);
        fences.Add(position, type);
        this.position = position;
        Debug.Log("Added " + position + " length = " + fences.Count);
        hasBeenPlaced = true;
        FenceWasPlaced?.Invoke(position);
        UpdateTexture(atlas.GetSprite($"{type}Fence{GetFencesFlagsSum(position)}"));
    }

    public override void Delete(){
        if (!hasBeenPlaced) return;
        fences.Remove(position);
        Debug.Log("Removing" + position + " length = " + fences.Count);
        FenceWasPlaced?.Invoke(position);
        FenceWasPlaced -= AnotherFenceWasPlaced;
        base.Delete();
    }

    private void AnotherFenceWasPlaced(Vector3Int position){
        List<Vector3Int> neighbors = GetCrossAroundPosition(position).ToList();
        // Debug.Log($"{type}Fence{GetFencesFlagsSum(baseCoordinates[0])}, null: {atlas.GetSprite($"{type}Fence{GetFencesFlagsSum(baseCoordinates[0])}") == null}");
        if (neighbors.Contains(this.position)){
            UpdateTexture(atlas.GetSprite($"{type}Fence{GetFencesFlagsSum(this.position)}"));
        }
    }

    private int GetFencesFlagsSum(Vector3Int position){
        List<FloorFlag> flags = new List<FloorFlag>();
        Vector3Int[] neighbors = GetCrossAroundPosition(position).ToArray();
        if (fences.Keys.Contains(neighbors[0])) flags.Add(FloorFlag.LEFT_ATTACHED);
        if (fences.Keys.Contains(neighbors[1])) flags.Add(FloorFlag.RIGHT_ATTACHED);
        if (fences.Keys.Contains(neighbors[3])) flags.Add(FloorFlag.TOP_ATTACHED);
        return flags.Cast<int>().Sum();
    }

}
