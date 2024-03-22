using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Runtime.Remoting.Messaging;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using System.Linq;
using UnityEngine.Events;
using System.Runtime.ConstrainedExecution;
using UnityEngine.EventSystems;
#pragma warning disable IDE1006 // Naming Styles

///<summary>Base class for representing a building, can be extended for specific buildings</summary>
public abstract class Building : TooltipableGameObject {
    //Set values for the colors of the building
    protected readonly Color SEMI_TRANSPARENT = new Color(1,1,1,0.5f);
    protected readonly Color SEMI_TRANSPARENT_INVALID = new Color(1,0.5f,0.5f,0.5f);
    protected readonly Color OPAQUE = new Color(1,1,1,1);

    /// <summary> A unique ID that identifies this object </summary>
    public int UID {get; protected set;} = 0;

    ///<summary>The array containing the coordinates of each sprite tile</summary>
    public Vector3Int[] spriteCoordinates { get; protected set;}
     ///<summary>The coordinates of the base</summary>
    public Vector3Int[] baseCoordinates { get; protected set;}
    /// <summary> The texture of the building</summary>
    public Texture2D insideAreaTexture {get; protected set;}
    ///<summary>The sprite of the building </summary>
    public Sprite sprite;
    ///<summary>The ways the user can interact with the building </summary>
    public ButtonTypes[] buildingInteractions { get; protected set;} = new ButtonTypes[0];
    public int baseHeight { get; protected set; } 
    public int height {get { return sprite == null ? 0 : (int)sprite.textureRect.height / 16;}}
    public int width {get { return (int) sprite.textureRect.width / 16;} }
    ///<summary>The tilemap this building is attached to</summary>
    public Tilemap tilemap {get {return gameObject.GetComponent<Tilemap>();}}
    /// <summary> The parent of the buttons that are created for this building </summary>
    public GameObject buttonParent;
    protected bool hasBeenPlaced = false;
    private bool hasBeenPickedUp = false;
    public static Actions currentAction {get; set;} = Actions.PLACE;
    public delegate void BuildingPlacedDelegate();
    public static event BuildingPlacedDelegate buildingWasPlaced;
    private Vector3Int mousePositionOfLastFrame;
#pragma warning restore IDE1006 // Naming Styles

    /// <summary>
    /// Get the materials needed to build this building
    /// </summary>
    public abstract List<MaterialInfo> GetMaterialsNeeded();

    /// <summary>
    /// Recreate a building from its data, aquire data from <see cref="GetBuildingData()"/>
    /// </summary>
    /// <param name="x">The 2nd element in the data list</param>
    /// <param name="y">The 3rd element in the data list</param>
    /// <param name="data">All subsequent elements</param>
    public abstract void RecreateBuildingForData(int x, int y, params string[] data);

    protected void InvokeBuildingWasPlaced(){
        buildingWasPlaced?.Invoke();
    }

    public override bool ShowTooltipCondition(){
        if (!hasBeenPlaced) return false;
        // Debug.Log("Has Been Placed " + hasBeenPlaced);
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (baseCoordinates?.Contains(currentCell) ?? false) return true;
        else return false;
    }

    public override void OnAwake(){    
        AddTilemapToObject(gameObject);
        if (sprite == null) sprite = Resources.Load<Sprite>($"Buildings/{name}");
        mousePositionOfLastFrame = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public override void OnUpdate(){
        if (buildingInteractions.Length != 0 && hasBeenPlaced) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(this);
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (currentCell == mousePositionOfLastFrame) return;
        if (currentAction == Actions.PLACE || currentAction == Actions.PLACE_PICKED_UP){
            GetInputHandler().SetCursor(InputHandler.CursorType.Place);
            PlacePreview(currentCell);
        }
        else if (currentAction == Actions.EDIT){
            GetInputHandler().SetCursor(InputHandler.CursorType.Pickup);
            PickupPreview();
        }
        else if (currentAction == Actions.DELETE){
            GetInputHandler().SetCursor(InputHandler.CursorType.Delete);
            DeletePreview();
        }

       
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            if (EventSystem.current.IsPointerOverGameObject()) return;
            
            if (currentAction == Actions.PLACE || currentAction == Actions.PLACE_PICKED_UP) PlaceWrapper(currentCell);
            else if (currentAction == Actions.EDIT) PickupWrapper();
            else if (currentAction == Actions.DELETE) DeleteWrapper();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1)){
            if (baseCoordinates?.Contains(currentCell) ?? false) OnMouseRightClick();
        }
    }

    private void PlaceWrapper(Vector3Int position){
        if (hasBeenPlaced) return;
        if (!CanBuildingBePlacedThere(position, this)) return;
        // Debug.Log($"{this} can be placed at {position}");
        List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, baseHeight, width);
        if (GetBuildingController().GetUnavailableCoordinates().Intersect(baseCoordinates).Count() != 0){
            GetNotificationManager().SendNotification($"Cannot place {GetType()} here");
            return;
        }
        Place(position);
        GetBuildingController().buildingGameObjects.Add(gameObject);
        GetBuildingController().buildings.Add(this);
        UID = (name + baseCoordinates[0].x + baseCoordinates[0].y).GetHashCode();
        GetBuildingController().AddActionToLog(new UserAction(Actions.PLACE, UID, GetBuildingData()));
        if (currentAction == Actions.PLACE) buildingWasPlaced?.Invoke();
    }

    private void PickupWrapper(){
        if (!hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!baseCoordinates?.Contains(currentCell) ?? false) return;
        GetBuildingController().AddActionToLog(new UserAction(Actions.EDIT, UID, GetBuildingData()));
        Pickup();
    }

    private void DeleteWrapper(){
        if (!hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!baseCoordinates?.Contains(currentCell) ?? false) return;
        GetBuildingController().AddActionToLog(new UserAction(Actions.DELETE, UID, GetBuildingData()));
        Delete();
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

    protected virtual void PlacePreview(Vector3Int position){
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
    
    /// <summary>
    /// Place the building at the given position.
    /// <para>When this is called it means that:</para>
    /// <code>
    ///     hasBeenPlaced = false;
    ///     baseCoordinates are available
    /// </code>
    /// <para>After this is called:</para>
    /// <code>
    ///     buildingWasPlaced will be invoked
    ///     The building will have been added to GetBuildingController().buildingGameObjects
    /// </code>
    /// </summary>
    /// <param name="position">The position you want to place the building</param>
    public virtual void Place(Vector3Int position){
        List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, baseHeight, width);
        List<Vector3Int> spriteCoordinates = GetAreaAroundPosition(position, height, width);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50;
        gameObject.GetComponent<Tilemap>().color = OPAQUE;
        gameObject.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), SplitSprite(sprite));

        GetBuildingController().GetUnavailableCoordinates().UnionWith(baseCoordinates);

        this.baseCoordinates = baseCoordinates.ToArray();
        this.spriteCoordinates = spriteCoordinates.ToArray();
        if (buildingInteractions.Length != 0) GetButtonController().CreateButtonsForBuilding(this);

        hasBeenPlaced = true;
        

        if (hasBeenPickedUp){
            hasBeenPickedUp = false;
            currentAction = Actions.EDIT;
        }

        if (this is ITieredBuilding tieredBuilding) tieredBuilding.ChangeTier(tieredBuilding.Tier);

        // Debug.Log($"UID: {UID}");
    }

    /// <summary>
    /// Update the sprite of the building
    /// </summary>
    protected void UpdateTexture(Sprite newSprite){
        sprite = newSprite;
        if (!hasBeenPlaced) return;
        Tile[] buildingTiles = SplitSprite(sprite);
        // Debug.Log(this.gameObject == null);
        this.gameObject?.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), buildingTiles);
    }

    public bool VectorInBaseCoordinates(Vector3Int checkForMe) {
        foreach (Vector3Int vector in baseCoordinates) if (checkForMe == vector) return true;
        return false;
    }

    /// <summary>
    /// Pickup the building
    /// </summary>
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

    /// <summary>
    /// Delete the building
    /// </summary>
    public virtual void Delete() {
        if (!hasBeenPlaced) return;
        ForceDelete();
    }

    /// <summary>
    /// Force delete the building, meaning this deletes buildings that cant be deleted any other way (house, greenhouse)
    /// </summary>
    public void ForceDelete(){
        GetBuildingController().GetUnavailableCoordinates().RemoveWhere(x => baseCoordinates.Contains(x));
        Destroy(buttonParent);
        Destroy(gameObject);
    }

    /// <summary>
    /// Get all the date this building need to be recreated, for saving purposes
    /// </summary>
    /// <returns>a string starting with Type,base[0].x,base[0].y and followed by building specific data, fields seperated by | </returns>
    public virtual string GetBuildingData(){
        return $"{GetType()}|{baseCoordinates[0].x}|{baseCoordinates[0].y}";
    }

    protected virtual void OnMouseRightClick(){
        if (buildingInteractions.Length != 0 && hasBeenPlaced){
            buttonParent.SetActive(!buttonParent.activeSelf);
        }
    }
}

