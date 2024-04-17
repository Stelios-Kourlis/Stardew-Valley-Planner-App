using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using static Utility.ClassManager;
using static Utility.BuildingManager;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Diagnostics;

///<summary>Base class for representing a building, can be extended for specific buildings</summary>
public abstract class Building : TooltipableGameObject {
    //Set values for the colors of the building
    protected readonly Color SEMI_TRANSPARENT = new(1,1,1,0.5f);
    protected readonly Color SEMI_TRANSPARENT_INVALID = new(1,0.5f,0.5f,0.5f);
    protected readonly Color OPAQUE = new(1,1,1,1);

    /// <summary> A unique ID that identifies this object, 2 building of the same type, on the same location will have the same UID</summary>
    public int UID {get; protected set;} = 0;

    ///<summary>The array containing the coordinates of each sprite tile</summary>
    public Vector3Int[] SpriteCoordinates { get; protected set;}
     ///<summary>The coordinates of the base</summary>
    public Vector3Int[] BaseCoordinates { get; protected set;}
    ///<summary>The sprite of the building </summary>
    public Sprite sprite;
    ///<summary>The ways the user can interact with the building </summary>
    public ButtonTypes[] BuildingInteractions { get; protected set;} = new ButtonTypes[0];
    public int BaseHeight { get; protected set; } 
    public int Height {get { 
        UnityEngine.Debug.Assert(sprite != null, $"Sprite is null for {GetType()}");
        return sprite != null ? (int) sprite.textureRect.height / 16 : 0;}}
    public int Width {get { 
        UnityEngine.Debug.Assert(sprite != null, $"Sprite is null for {GetType()}");
        return sprite != null ? (int) sprite.textureRect.width / 16 : 0;}}
    ///<summary>The tilemap this building is attached to</summary>
    public Tilemap Tilemap {get {return gameObject.GetComponent<Tilemap>();}}
    /// <summary> The parent of the buttons that are created for this building </summary>
    public GameObject buttonParent;
    protected bool hasBeenPlaced = false;
    private bool hasBeenPickedUp = false;
    private bool isMouseOverBuilding = false;
    public static Actions CurrentAction {get; set;} = Actions.PLACE;
    private Vector3Int mousePositionOfLastFrame;
    public delegate void BuildingPlacedDelegate();
    public static event BuildingPlacedDelegate BuildingWasPlaced;

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
        BuildingWasPlaced?.Invoke();
    }

    public override bool ShowTooltipCondition(){
        if (!hasBeenPlaced) return false;
        // Debug.Log("Has Been Placed " + hasBeenPlaced);
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (BaseCoordinates?.Contains(currentCell) ?? false) return true;
        else return false;
    }

    public override void OnAwake(){    
        AddTilemapToObject(gameObject);
        if (sprite == null) sprite = Resources.Load<Sprite>($"Buildings/{GetType()}");
        mousePositionOfLastFrame = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public override void OnUpdate(){
        UnityEngine.Debug.Assert(sprite != null, $"Sprite is null for {GetType()}");
        if (BuildingInteractions.Length != 0 && hasBeenPlaced) GetButtonController().UpdateButtonPositionsAndScaleForBuilding(this);
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (currentCell == mousePositionOfLastFrame) return;
        if (CurrentAction == Actions.PLACE || CurrentAction == Actions.PLACE_PICKED_UP){
            GetInputHandler().SetCursor(InputHandler.CursorType.Place);
            PlacePreviewWrapper(currentCell);
        }
        else if (CurrentAction == Actions.EDIT){
            GetInputHandler().SetCursor(InputHandler.CursorType.Pickup);
            PickupPreview();
        }
        else if (CurrentAction == Actions.DELETE){
            GetInputHandler().SetCursor(InputHandler.CursorType.Delete);
            DeletePreview();
        }

       
        if (Input.GetKeyUp(KeyCode.Mouse0)){
            if (EventSystem.current.IsPointerOverGameObject() && EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name != "TopRightButtons") return;
            
            if (CurrentAction == Actions.PLACE || CurrentAction == Actions.PLACE_PICKED_UP) PlaceWrapper(currentCell);
            else if (CurrentAction == Actions.EDIT) PickupWrapper();
            else if (CurrentAction == Actions.DELETE) DeleteWrapper();
        }

        if (Input.GetKeyUp(KeyCode.Mouse1)){
            if (BaseCoordinates?.Contains(currentCell) ?? false) OnMouseRightClick();
        }

        if (BaseCoordinates?.Contains(currentCell) ?? false){
            if (!isMouseOverBuilding){
                OnMouseEnter();
                isMouseOverBuilding = true;
            }
        }
        if ((!BaseCoordinates?.Contains(currentCell)) ?? false){
            if (isMouseOverBuilding){
                OnMouseExit();
                isMouseOverBuilding = false;
            }
        }
    }

    private void PlaceWrapper(Vector3Int position){
        if (hasBeenPlaced) return;
        if (!CanBuildingBePlacedThere(position, this)) return;
        // Debug.Log($"{this} can be placed at {position}");
        List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, BaseHeight, Width);
        if (GetBuildingController().GetUnavailableCoordinates().Intersect(baseCoordinates).Count() != 0){
            GetNotificationManager().SendNotification($"Cannot place {GetType()} here");
            return;
        }
        Place(position);
        GetBuildingController().buildingGameObjects.Add(gameObject);
        GetBuildingController().buildings.Add(this);
        UID = (name + baseCoordinates[0].x + baseCoordinates[0].y).GetHashCode();
        GetBuildingController().AddActionToLog(new UserAction(Actions.PLACE, UID, GetBuildingData()));
        if (CurrentAction == Actions.PLACE) BuildingWasPlaced?.Invoke();
    }

    private void PickupWrapper(){
        if (!hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!BaseCoordinates?.Contains(currentCell) ?? false) return;
        GetBuildingController().AddActionToLog(new UserAction(Actions.EDIT, UID, GetBuildingData()));
        Pickup();
    }

    private void DeleteWrapper(){
        if (!hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (!BaseCoordinates?.Contains(currentCell) ?? false) return;
        GetBuildingController().AddActionToLog(new UserAction(Actions.DELETE, UID, GetBuildingData()));
        GetBuildingController().buildingGameObjects.Remove(gameObject);
        GetBuildingController().buildings.Remove(this);
        Delete();
    }

    private void PlacePreviewWrapper(Vector3Int position){
        if (hasBeenPlaced) return;
        PlacePreview(position);
    }
    protected virtual void PickupPreview(){
        if (!hasBeenPlaced){
            gameObject.GetComponent<Tilemap>().ClearAllTiles();
            return;
        }
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (BaseCoordinates.Contains(currentCell)) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        else gameObject.GetComponent<Tilemap>().color = OPAQUE;
    }

    protected virtual void PlacePreview(Vector3Int position){
        if (hasBeenPlaced) return;
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        gameObject.GetComponent<Tilemap>().color = new Color(1,1,1,0.5f);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -currentCell.y + 50;

        Vector3Int[] unavailableCoordinates = GetBuildingController().GetUnavailableCoordinates().ToArray();
        Vector3Int[] buildingBaseCoordinates = GetAreaAroundPosition(currentCell, BaseHeight, Width).ToArray();
        if (unavailableCoordinates.Intersect(buildingBaseCoordinates).Count() != 0) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
        else gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT;
        gameObject.GetComponent<Tilemap>().ClearAllTiles();
        Vector3Int[] mouseoverEffectArea = GetAreaAroundPosition(currentCell, Height, Width).ToArray();
        gameObject.GetComponent<Tilemap>().SetTiles(mouseoverEffectArea, SplitSprite(sprite));
    }

    protected virtual void DeletePreview(){
        if (!hasBeenPlaced){
            gameObject.GetComponent<Tilemap>().ClearAllTiles();
            return;
        }
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (BaseCoordinates.Contains(currentCell)) gameObject.GetComponent<Tilemap>().color = SEMI_TRANSPARENT_INVALID;
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
        List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, BaseHeight, Width);
        List<Vector3Int> spriteCoordinates = GetAreaAroundPosition(position, Height, Width);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50;
        gameObject.GetComponent<Tilemap>().color = OPAQUE;
        gameObject.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), SplitSprite(sprite));

        GetBuildingController().GetUnavailableCoordinates().UnionWith(baseCoordinates);

        this.BaseCoordinates = baseCoordinates.ToArray();
        this.SpriteCoordinates = spriteCoordinates.ToArray();
        if (BuildingInteractions.Length != 0) GetButtonController().CreateButtonsForBuilding(this);

        hasBeenPlaced = true;
        

        if (hasBeenPickedUp){
            hasBeenPickedUp = false;
            CurrentAction = Actions.EDIT;
        }

        // if (this is TieredBuilding tieredBuilding) tieredBuilding.ChangeTier(tieredBuilding.Tier);//todo is this needed?

        // Debug.Log($"UID: {UID}");
    }

    /// <summary>
    /// Update the sprite of the building
    /// </summary>
    public void UpdateTexture(Sprite newSprite){
        UnityEngine.Debug.Assert(newSprite != null, $"UpdateTexture called for {GetType()} with null sprite/Called from: {new StackTrace()}");
        sprite = newSprite;
        if (!hasBeenPlaced) return;
        Tile[] buildingTiles = SplitSprite(sprite);
        // Debug.Log(this.gameObject == null);
        gameObject?.GetComponent<Tilemap>().SetTiles(SpriteCoordinates.ToArray(), buildingTiles);
    }

    public bool VectorInBaseCoordinates(Vector3Int checkForMe) {
        foreach (Vector3Int vector in BaseCoordinates) if (checkForMe == vector) return true;
        return false;
    }

    /// <summary>
    /// Pickup the building
    /// </summary>
    protected virtual void Pickup(){
        Vector3Int currentCell = GetBuildingController().GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if(!BaseCoordinates.Contains(currentCell)) return;

        GetBuildingController().GetUnavailableCoordinates().RemoveWhere(x => BaseCoordinates.Contains(x));
        BaseCoordinates = null;
        SpriteCoordinates = null;
        gameObject.GetComponent<Tilemap>().ClearAllTiles();
        hasBeenPlaced = false;
        hasBeenPickedUp = true;
        CurrentAction = Actions.PLACE_PICKED_UP;

    }

    /// <summary>
    /// Delete the building
    /// If this is called then:
    /// <code>
    ///     hasBeenPlaced = true;
    ///     The mouse is over this building
    /// </code>
    /// </summary>
    public virtual void Delete() {
        if (!hasBeenPlaced) return;
        ForceDelete();
    }

    /// <summary>
    /// Force delete the building, meaning this deletes buildings that cant be deleted any other way (house, greenhouse)
    /// </summary>
    public virtual void ForceDelete(){
        if (BaseCoordinates != null ) GetBuildingController().GetUnavailableCoordinates().RemoveWhere(x => BaseCoordinates.Contains(x));
        Destroy(buttonParent);
        Destroy(gameObject);
    }

    /// <summary>
    /// Get all the date this building need to be recreated, for saving purposes
    /// </summary>
    /// <returns>a string starting with Type,base[0].x,base[0].y and followed by building specific data, fields seperated by | </returns>
    public virtual string GetBuildingData(){
        return $"{GetType()}|{BaseCoordinates[0].x}|{BaseCoordinates[0].y}";
    }

    protected virtual void OnMouseRightClick(){
        if (BuildingInteractions.Length != 0 && hasBeenPlaced){
            buttonParent.SetActive(!buttonParent.activeSelf);
        }
    }

    protected virtual void OnMouseEnter(){
        //ehm.... override this?
    }

    protected virtual void OnMouseExit(){
        //ehm.... override this?
    }

    /// <summary>
    /// Create a button that sets the current building to this building
    /// </summary>
    /// <returns>The game object of the button, with no parent, caller should use transform.SetParent()</returns>
    public virtual GameObject CreateButton(){
        GameObject button = Instantiate(Resources.Load<GameObject>("UI/BuildingButton"));
        button.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
        button.name = $"{GetType()}Button";
        button.GetComponent<Image>().sprite = sprite;

        Type buildingType = GetType();
        BuildingController buildingController = GetBuildingController();
        button.GetComponent<Button>().onClick.AddListener(() => { 
                // Debug.Log($"Setting current building to {buildingType}");
                buildingController.SetCurrentBuildingType(buildingType);
                buildingController.SetCurrentAction(Actions.PLACE); 
                });
        return button;
    }
}

