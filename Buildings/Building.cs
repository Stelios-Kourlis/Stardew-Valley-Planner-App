using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Runtime.Remoting.Messaging;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using System.Linq;
using UnityEngine.Events;
using System.Runtime.ConstrainedExecution;
using UnityEngine.EventSystems;
// #pragma warning disable IDE1006 // Naming Styles

///<summary>Base class for representing a building, can be extended for specific buildings</summary>
public abstract class Building : MonoBehaviour {
    protected readonly Color SEMI_TRANSPARENT = new Color(1,1,1,0.5f);
    protected readonly Color SEMI_TRANSPARENT_INVALID = new Color(1,0.5f,0.5f,0.5f);
    protected readonly Color OPAQUE = new Color(1,1,1,1);

    ///<summary>The array containing the coordinates of each sprite tile, probably reversed y-wise</summary>
    public Vector3Int[] spriteCoordinates { get; protected set;}
     ///<summary>The coordinates of the base</summary>
    public Vector3Int[] baseCoordinates { get; protected set;}
   
    public Texture2D insideAreaTexture {get; protected set;}
    ///<summary>The sprite of the building</summary>
    public Sprite sprite;
    public ButtonTypes[] buildingInteractions { get; protected set;} = new ButtonTypes[0];//backing field
    public int baseHeight { get; protected set; } 
    public int height {get {return (int) sprite.textureRect.height / 16;}}
    public int width {get {return (int) sprite.textureRect.width / 16;}}
    ///<summary>The tilemap this building is attached to</summary>
    public Tilemap tilemap {get {return gameObject.GetComponent<Tilemap>();}} //the tilemap this building is attached to
    public GameObject buttonParent;
    protected bool hasBeenPlaced = false;
    private bool hasBeenPickedUp = false;
    public static Actions currentAction {get; set;} = Actions.PLACE;
    // Define a delegate type for the event.
    public delegate void BuildingPlacedDelegate();

    // Define a static event of the delegate type.
    public static event BuildingPlacedDelegate buildingWasPlaced;

    //public GameObject[] paintableParts;//todo figure out how to do paintable parts

    // protected delegate void PlaceDelegate(Vector3Int position);
    // protected PlaceDelegate PlaceBuilding;
    // protected delegate void PickupDelegate();
    // protected PickupDelegate PickupBuilding;
    // protected delegate void DeleteDelegate();
    // protected DeleteDelegate DeleteBuilding;
    // protected delegate void PlacePreviewDelegate();
    // protected PlacePreviewDelegate PlacePreview;
    // protected delegate void EditPreviewDelegate();
    // protected EditPreviewDelegate EditPreview;
    // protected delegate void DeletePreviewDelegate();
    // protected DeletePreviewDelegate DeletePreview;
#pragma warning restore IDE1006 // Naming Styles

    //protected abstract void Init();
    public abstract Dictionary<Materials,int> GetMaterialsNeeded();

    public void Start(){    
        AddTilemapToObject(gameObject);
        // PlaceBuilding = Place;
        // PickupBuilding = Pickup;
        // DeleteBuilding = Delete;
        // PlacePreview = PlaceMouseoverEffect;
        // EditPreview = EditMouseoverEffect;
        // DeletePreview = DeleteMouseoverEffect;
        //texture = Resources.Load($"Buildings/{name}") as Texture2D;
        sprite = Resources.Load<Sprite>($"Buildings/{name}");
        // gameObject.GetComponent<Tilemap>().color = new Color(1,1,1,0.5f);
    }

    protected void Update(){
        if (buildingInteractions.Length != 0 && hasBeenPlaced) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(this);
        //gameObject.GetComponent<Tilemap>().color = new Color(red,green,blue,alpha);
        if (currentAction == Actions.PLACE || currentAction == Actions.PLACE_PICKED_UP) PlacePreview();
        else if (currentAction == Actions.EDIT) PickupPreview();
        else if (currentAction == Actions.DELETE) DeletePreview();

        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            if(EventSystem.current.IsPointerOverGameObject()) return;
            if ((currentAction == Actions.PLACE || currentAction == Actions.PLACE_PICKED_UP)  && !hasBeenPlaced) Place(currentCell);
            else if (currentAction == Actions.EDIT && hasBeenPlaced) Pickup();
            else if (currentAction == Actions.DELETE) Delete();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) && buildingInteractions.Length != 0 && hasBeenPlaced && baseCoordinates.Contains(currentCell)){
            buttonParent.SetActive(!buttonParent.activeSelf);
        }
    }

    protected void InvokeBuildingWasPlaced(){
        buildingWasPlaced?.Invoke();
    }

    protected virtual void PickupPreview(){
        if (!hasBeenPlaced){
            gameObject.GetComponent<Tilemap>().ClearAllTiles();
            return;
        }
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        else gameObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    protected virtual void PlacePreview(){
        if (hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        gameObject.GetComponent<Tilemap>().color = new Color(1,1,1,0.5f);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -currentCell.y + 50;

        Vector3Int[] unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates().ToArray();
        Vector3Int[] buildingBaseCoordinates = GetAreaAroundPosition(currentCell, baseHeight, width).ToArray();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        gameObject.GetComponent<Tilemap>().ClearAllTiles();
        Vector3Int[] mouseoverEffectArea = GetAreaAroundPosition(currentCell, height, width).ToArray();
        gameObject.GetComponent<Tilemap>().SetTiles(mouseoverEffectArea, SplitSprite(sprite));
    }

    protected virtual void DeletePreview(){
        if (!hasBeenPlaced){
            gameObject.GetComponent<Tilemap>().ClearAllTiles();
            return;
        }
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else gameObject.GetComponent<Tilemap>().color = OPAQUE;
    }
    
    public virtual void Place(Vector3Int position){
        List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, baseHeight, width);
        if (GetBuildingController().GetUnavailableCoordinates().Intersect(baseCoordinates).Count() != 0){
            Debug.LogWarning("Cannot place building here");
            return;
        }

        List<Vector3Int> spriteCoordinates = GetAreaAroundPosition(position, height, width);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50;
        gameObject.GetComponent<Tilemap>().color = OPAQUE;
        gameObject.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), SplitSprite(sprite));

        GetBuildingController().GetUnavailableCoordinates().UnionWith(baseCoordinates);
        GetBuildingController().buildingGameObjects.Add(gameObject);
        GetBuildingController().buildings.Add(this);

        this.baseCoordinates = baseCoordinates.ToArray();
        this.spriteCoordinates = spriteCoordinates.ToArray();
        if (buildingInteractions.Length != 0) GetButtonController().CreateButtonsForBuilding(this);
        //name = GetType().Name;

        hasBeenPlaced = true;
        

        if (hasBeenPickedUp){
            hasBeenPickedUp = false;
            currentAction = Actions.EDIT;
        }
        if (currentAction == Actions.PLACE) InvokeBuildingWasPlaced();
    }

    protected void UpdateTexture(Sprite newSprite){
        sprite = newSprite;
        if (!hasBeenPlaced) return;
        Tile[] buildingTiles = SplitSprite(sprite);
        gameObject.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), buildingTiles);
    }

    public bool VectorInBaseCoordinates(Vector3Int checkForMe) {
        foreach (Vector3Int vector in baseCoordinates) if (checkForMe == vector) return true;
        return false;
    }

    protected virtual void Pickup(){
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(!baseCoordinates.Contains(currentCell)) return;

        GetBuildingController().GetUnavailableCoordinates().RemoveWhere(x => baseCoordinates.Contains(x));
        baseCoordinates = null;
        spriteCoordinates = null;
        gameObject.GetComponent<Tilemap>().ClearAllTiles();
        hasBeenPlaced = false;
        hasBeenPickedUp = true;
        currentAction = Actions.PLACE_PICKED_UP;
    }

    public virtual void Delete() {
        if (!hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(!baseCoordinates.Contains(currentCell)) return;

        ForceDelete();
    }

    public void ForceDelete(){
        GetBuildingController().GetUnavailableCoordinates().RemoveWhere(x => baseCoordinates.Contains(x));
        Destroy(buttonParent);
        Destroy(gameObject);
    }

    /// <summary>
    /// Get all the date this building need to be recreated, for saving purposes
    /// </summary>
    /// <returns></returns>
    public virtual string GetBuildingData(){
        return $"{GetType()}|{baseCoordinates[0].x}|{baseCoordinates[0].y}";
    }

    public virtual void RecreateBuildingForData(int x, int y, params string[] data){
        baseCoordinates = GetAreaAroundPosition(new Vector3Int(x,y,0), baseHeight, width).ToArray();
        spriteCoordinates = GetAreaAroundPosition(new Vector3Int(x,y,0), height, width).ToArray();
        Place(new Vector3Int(x,y,0));
    }
}

