using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System.Collections.ObjectModel;
using System.Linq;

public class Greenhouse : Building, IEnterableBuilding, IInteractableBuilding, IExtraActionBuilding {

    private GameObject porchTilemapObject;
    private Sprite porchSprite;
    public EnterableBuilding EnterableBuildingComponent { get; private set; }
    public InteractableBuildingComponent InteractableBuildingComponent { get; private set; }

    public Vector3Int[] InteriorUnavailableCoordinates { get; private set; }

    public Vector3Int[] InteriorPlantableCoordinates { get; private set; }

    public ButtonTypes[] BuildingInteractions => InteractableBuildingComponent.BuildingInteractions;

    public GameObject ButtonParentGameObject => InteractableBuildingComponent.ButtonParentGameObject;

    public override void OnAwake() {
        BuildingName = "Greenhouse";
        BaseHeight = 6;
        // insideAreaTexture = Resources.Load("BuildingInsides/Greenhouse") as Texture2D;
        InteractableBuildingComponent = new InteractableBuildingComponent(this, new ButtonTypes[] { ButtonTypes.ENTER });
        EnterableBuildingComponent = new EnterableBuilding(this);
        base.OnAwake();
        porchSprite = Resources.Load<Sprite>("Buildings/GreenhousePorch");
        porchTilemapObject = CreateTilemapObject(transform, 0, "Porch");
    }

    public void PerformExtraActionsOnPlace(Vector3Int position) {
        Vector3Int porchBottomRight = position + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetAreaAroundPosition(porchBottomRight, 2, 3).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = BuildingController.GetUnavailableCoordinates();
        if (unavailableCoordinates.Intersect(porchCoordinates).Count() > 0) return;
        porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        porchTilemapObject.GetComponent<TilemapRenderer>().sortingOrder = gameObject.GetComponent<TilemapRenderer>().sortingOrder + 1;
        EnterableBuildingComponent.AddBuildingInterior();
    }

    protected void PerformExtraActionsOnPickup() {
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    protected void PerformExtraActionsOnPlacePreview(Vector3Int position) {
        Vector3Int porchBottomRight = position + new Vector3Int(2, 0, 0) - new Vector3Int(0, 2, 0);
        Vector3Int[] porchCoordinates = GetAreaAroundPosition(porchBottomRight, 2, 3).ToArray();
        HashSet<Vector3Int> unavailableCoordinates = BuildingController.GetUnavailableCoordinates();
        porchTilemapObject.GetComponent<Tilemap>().ClearAllTiles();
        porchTilemapObject.GetComponent<Tilemap>().SetTiles(porchCoordinates, SplitSprite(porchSprite));
        if (unavailableCoordinates.Intersect(porchCoordinates).Count() > 0) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
    }

    protected void PerformExtraActionsOnDeletePreview() {
        Vector3Int currentCell = GetMousePositionInTilemap();
        // Debug.Log(currentCell);
        if (BaseCoordinates?.Contains(currentCell) ?? false) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    protected void PerformExtraActionsOnPickupPreview() {
        Vector3Int currentCell = GetMousePositionInTilemap();
        if (BaseCoordinates.Contains(currentCell)) porchTilemapObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        else porchTilemapObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    public override List<MaterialInfo> GetMaterialsNeeded() {
        return new List<MaterialInfo>{
            new("Complete the community center's pantry room.")
        };
    }

    public void ToggleEditBuildingInterior() => EnterableBuildingComponent.ToggleEditBuildingInterior();

    public void EditBuildingInterior() => EnterableBuildingComponent.EditBuildingInterior();

    public void ExitBuildingInteriorEditing() => EnterableBuildingComponent.ExitBuildingInteriorEditing();

    public void CreateInteriorCoordinates() {
        Vector3Int interiorLowerLeftCorner = EnterableBuildingComponent.InteriorAreaCoordinates[0];
        HashSet<Vector3Int> interiorUnavailableCoordinates = new();
        for (int i = 0; i < 20; i++) {
            if (i != 10) interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 0, 0));
            if (i <= 4 || i >= 16) interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 1, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 16, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 17, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 18, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 19, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 20, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 21, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 22, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(i, 23, 0));
        }
        for (int i = 0; i < 24; i++) {
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(0, i, 0));
            interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(19, i, 0));
        }
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(1, 2, 0));
        interiorUnavailableCoordinates.Add(interiorLowerLeftCorner + new Vector3Int(20, 2, 0));


        InteriorUnavailableCoordinates = GetAllInteriorUnavailableCoordinates(interiorUnavailableCoordinates.ToArray()).ToArray();
    }

    public void OnMouseRightClick() {
        throw new System.NotImplementedException();
    }
}
