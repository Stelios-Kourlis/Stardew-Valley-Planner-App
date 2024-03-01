using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System.Collections.ObjectModel;
using System.Linq;

public class Greenhouse : Building {

    private GameObject porchTilemapObject;
    private Sprite porchSprite;

    public new void Start(){
        name = GetType().Name;
        baseHeight = 6;
        insideAreaTexture = Resources.Load("BuildingInsides/Greenhouse") as Texture2D;
        buildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
        base.Start();
        porchSprite = Resources.Load<Sprite>("Buildings/GreenhousePorch");
        porchTilemapObject = CreateTilemapObject(transform, 0, "Porch");
    }

    public override void Place(Vector3Int position){
        Vector3Int porchBottomRight = position + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetAreaAroundPosition(porchBottomRight, 2, 3).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates();
        if (unavailableCoordinates.Intersect(porchCoordinates).Count() > 0) return;
        base.Place(position);
        porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        porchTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
    }

    protected override void Pickup(){
        base.Pickup();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    protected override void PlacePreview(){
        if (hasBeenPlaced) return;
        base.PlacePreview();
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        Vector3Int porchBottomRight = currentCell + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetAreaAroundPosition(porchBottomRight, 2, 3).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        if (unavailableCoordinates.Intersect(porchCoordinates).Count() > 0) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
    }

    protected override void DeletePreview(){
        return;
        // if (!hasBeenPlaced) return;
        // base.DeletePreview();
        // Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        // if (baseCoordinates.Contains(currentCell)) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        // else porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    protected override void PickupPreview(){
        if (!hasBeenPlaced) return;
        base.PickupPreview();
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        else porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    public override void Delete(){
        return; //cant delete greenhouse
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>{
            new MaterialInfo("Complete the community center's pantry room.")
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data)
    {
        throw new System.NotImplementedException();
    }
}
