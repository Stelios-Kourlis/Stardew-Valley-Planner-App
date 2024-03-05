using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.ClassManager;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using System.Linq;

public class JunimoHut : Building {

    private Tile greenTile;
    public new void Start(){
        name = GetType().Name;
        baseHeight = 2;
        base.Start();
        greenTile = LoadTile("GreenTile");
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
    return new List<MaterialInfo>(){
        new MaterialInfo(20000, Materials.Coins),
        new MaterialInfo(200, Materials.Stone),
        new MaterialInfo(9, Materials.Starfruit),
        new MaterialInfo(100, Materials.Fiber)
    };
}

    protected override void PlacePreview(){//todo add range
        if (hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        base.PlacePreview();
        List<Vector3Int> coverageArea = GetAreaAroundPosition(new Vector3Int(currentCell.x-7, currentCell.y-8, 0), 17, 17);
        coverageArea.Remove(currentCell);
        coverageArea.Remove(new Vector3Int(currentCell.x+1, currentCell.y, 0));
        coverageArea.Remove(new Vector3Int(currentCell.x+2, currentCell.y, 0));
        coverageArea.Remove(new Vector3Int(currentCell.x, currentCell.y+1, 0));
        coverageArea.Remove(new Vector3Int(currentCell.x+1, currentCell.y+1, 0));
        coverageArea.Remove(new Vector3Int(currentCell.x+2, currentCell.y+1, 0));
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, greenTile);
    }

    public override void Place(Vector3Int position){
        base.Place(position);
        List<Vector3Int> coverageArea = GetAreaAroundPosition(new Vector3Int(position.x-7, position.y-8, 0), 17, 17);
        coverageArea.Remove(position);
        coverageArea.Remove(new Vector3Int(position.x+1, position.y, 0));
        coverageArea.Remove(new Vector3Int(position.x-1, position.y, 0));
        coverageArea.Remove(new Vector3Int(position.x, position.y+1, 0));
        coverageArea.Remove(new Vector3Int(position.x+1, position.y+1, 0));
        coverageArea.Remove(new Vector3Int(position.x-1, position.y+1, 0));
        foreach (Vector3Int cell in coverageArea) GetComponent<Tilemap>().SetTile(cell, null);
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        Place(new Vector3Int(x,y,0));
    }
}