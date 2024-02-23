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
using UnityEditor.UI;
#pragma warning disable IDE1006 // Naming Styles

///<summary>Base class for representing a building, can be extended for specific buildings</summary>
public abstract class Building : MonoBehaviour {
    private readonly Color SEMI_TRANSPARENT = new Color(1,1,1,0.5f);
    private readonly Color SEMI_TRANSPARENT_INVALID = new Color(1,0.5f,0.5f,0.5f);
    private readonly Color OPAQUE = new Color(1,1,1,1);

    ///<summary>The array containing the coordinates of each sprite tile, probably reversed y-wise</summary>
    public Vector3Int[] spriteCoordinates { get; private set;}
     ///<summary>The coordinates of the base</summary>
    public Vector3Int[] baseCoordinates { get; private set;}
   
    public Texture2D insideAreaTexture {get; protected set;}
    ///<summary>The sprite of the building</summary>
    [Obsolete("Use sprite.texture instead")]
    public Texture2D texture {get; protected set;}
    public Sprite sprite;
    public ButtonTypes[] buildingInteractions { get; protected set;} = new ButtonTypes[0];//backing field

    public int baseHeight { get; protected set; } 
    public int height {get {return (int) sprite.textureRect.height / 16;}}
    public int width {get {return (int) sprite.textureRect.width / 16;}}
    ///<summary>The tilemap this building is attached to</summary>
    // [Obsolete("Use gameObject intead")]
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

    protected delegate void PlaceDelegate(Vector3Int position);
    protected PlaceDelegate PlaceBuilding;
    protected delegate void PickupDelegate();
    protected PickupDelegate PickupBuilding;
    protected delegate void DeleteDelegate();
    protected DeleteDelegate DeleteBuilding;
    protected delegate void PlacePreviewDelegate();
    protected PlacePreviewDelegate PlacePreview;
    protected delegate void EditPreviewDelegate();
    protected EditPreviewDelegate EditPreview;
    protected delegate void DeletePreviewDelegate();
    protected DeletePreviewDelegate DeletePreview;


    [Range(0, 1)]
    public float red=1,green=1,blue=1,alpha=0.5f;

    public Dictionary<Materials,int> materialsNeeded { get; protected set;} = new Dictionary<Materials, int>();
    // public Dictionary<Materials,int> materialsNeeded{ 
    //     get {return new Dictionary<Materials,int>(_materialsNeeded);} 
    // }
#pragma warning restore IDE1006 // Naming Styles

    protected abstract void Init();
    //protected abstract Dictionary<Materials,int> GetMaterialsNeeded();

    public void Start(){    
        AddTilemapToObject(gameObject);
        PlaceBuilding = Place;
        PickupBuilding = Pickup;
        DeleteBuilding = Delete;
        PlacePreview = PlaceMouseoverEffect;
        EditPreview = EditMouseoverEffect;
        DeletePreview = DeleteMouseoverEffect;
        //texture = Resources.Load($"Buildings/{name}") as Texture2D;
        sprite = Resources.Load<Sprite>($"Buildings/{name}");
        // gameObject.GetComponent<Tilemap>().color = new Color(1,1,1,0.5f);
    }

    void Update(){
        if (buildingInteractions.Length != 0 && hasBeenPlaced) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(this);
        //gameObject.GetComponent<Tilemap>().color = new Color(red,green,blue,alpha);
        if (currentAction == Actions.PLACE || currentAction == Actions.PLACE_PICKED_UP) PlacePreview();
        else if (currentAction == Actions.EDIT) EditPreview();
        else if (currentAction == Actions.DELETE) DeletePreview();

        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            if(EventSystem.current.IsPointerOverGameObject()) return;
            if ((currentAction == Actions.PLACE || currentAction == Actions.PLACE_PICKED_UP)  && !hasBeenPlaced) PlaceBuilding(currentCell);
            else if (currentAction == Actions.EDIT && hasBeenPlaced) PickupBuilding();
            else if (currentAction == Actions.DELETE && hasBeenPlaced) DeleteBuilding();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1) && buildingInteractions.Length != 0 && hasBeenPlaced && baseCoordinates.Contains(currentCell)){
            buttonParent.SetActive(!buttonParent.activeSelf);
        }
    }

    protected void EditMouseoverEffect(){
        if (!hasBeenPlaced){
            gameObject.GetComponent<Tilemap>().ClearAllTiles();
            return;
        }
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        else gameObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    protected void PlaceMouseoverEffect(){
        if (hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        gameObject.GetComponent<Tilemap>().color = new Color(1,1,1,0.5f);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -currentCell.y + 50;

        Vector3Int[] unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates().ToArray();
        Vector3Int[] buildingBaseCoordinates = GetAreaAroundPosition(currentCell, baseHeight, width).ToArray();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        gameObject.GetComponent<Tilemap>().ClearAllTiles();
        if (hasBeenPlaced) Debug.Log($"Cleared Tiles, building was placed: {hasBeenPlaced}");
        //Debug.Log(gameObject.GetComponent<TilemapRenderer>().sortingOrder);
        Vector3Int[] mouseoverEffectArea = GetAreaAroundPosition(currentCell, height, width).ToArray();
        gameObject.GetComponent<Tilemap>().SetTiles(mouseoverEffectArea, SplitSprite(sprite));
    }

    protected void DeleteMouseoverEffect(){
        if (!hasBeenPlaced){
            gameObject.GetComponent<Tilemap>().ClearAllTiles();
            return;
        }
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates.Contains(currentCell)) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else gameObject.GetComponent<Tilemap>().color = OPAQUE;
    }
    
    public void Place(Vector3Int position){
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

        this.baseCoordinates = baseCoordinates.ToArray();
        this.spriteCoordinates = spriteCoordinates.ToArray();
        if (buildingInteractions.Length != 0) GetButtonController().CreateButtonsForBuilding(this);
        //name = GetType().Name;

        hasBeenPlaced = true;
        

        if (hasBeenPickedUp){
            hasBeenPickedUp = false;
            currentAction = Actions.EDIT;
        }
        if (currentAction == Actions.PLACE) buildingWasPlaced.Invoke();
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

    protected void Pickup(){
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

    public void Delete() {
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(!baseCoordinates.Contains(currentCell)) return;

        GetBuildingController().GetUnavailableCoordinates().RemoveWhere(x => baseCoordinates.Contains(x));
        Destroy(buttonParent);
        Destroy(gameObject);
    }
}

