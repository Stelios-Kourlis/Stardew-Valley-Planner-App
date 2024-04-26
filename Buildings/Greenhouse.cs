using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System.Collections.ObjectModel;
using System.Linq;

public class Greenhouse : Building, IEnterableBuilding {

    private GameObject porchTilemapObject;
    private Sprite porchSprite;
    public override string TooltipMessage => "Right Click For More Options";
    public EnterableBuildingComponent EnterableBuildingComponent {get; private set;}

    public Vector3Int[] InteriorUnavailableCoordinates {get; private set;}

    public Vector3Int[] InteriorPlantableCoordinates {get; private set;}

    public override void OnAwake(){
        name = GetType().Name;
        BaseHeight = 6;
        // insideAreaTexture = Resources.Load("BuildingInsides/Greenhouse") as Texture2D;
        BuildingInteractions = new ButtonTypes[]{
            ButtonTypes.ENTER
        };
        EnterableBuildingComponent = new EnterableBuildingComponent(this);
        base.OnAwake();
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
        EnterableBuildingComponent.AddBuildingInterior();
        Vector3Int interiorLowerLeftCorner = EnterableBuildingComponent.InteriorAreaCoordinates[0];
        HashSet<Vector3Int> interiorUnavailableCoordinates = new();
        for (int i = 0; i<20; i++){
            if (i != 10) interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 0, 0));
            if (i <= 4 || i >= 16) interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 0, 0));
        }
        

        InteriorUnavailableCoordinates = interiorUnavailableCoordinates.ToArray();
    }

    protected override void Pickup(){
        base.Pickup();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    protected override void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        base.PlacePreview(position);
        Vector3Int porchBottomRight = position + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
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
        if (BaseCoordinates.Contains(currentCell)) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        else porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    public override void Delete(){
        return; //cant delete greenhouse
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return new List<MaterialInfo>{
            new("Complete the community center's pantry room.")
        };
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data)
    {
        throw new System.NotImplementedException();
    }

    public void ToggleBuildingInterior() => EnterableBuildingComponent.ToggleBuildingInterior();

    public void ShowBuildingInterior() => EnterableBuildingComponent.ShowBuildingInterior();

    public void HideBuildingInterior() => EnterableBuildingComponent.HideBuildingInterior();

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();
}
