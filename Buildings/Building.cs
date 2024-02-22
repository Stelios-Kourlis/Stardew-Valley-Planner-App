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
#pragma warning disable IDE1006 // Naming Styles

///<summary>Base class for representing a building, can be extended for specific buildings</summary>
public abstract class Building : MonoBehaviour, ICloneable {
    private Vector3Int[] _spriteCoordinates;//backking field
    ///<summary>The array containing the coordinates of each sprite tile, probably reversed y-wise</summary>
    public Vector3Int[] spriteCoordinates { get { return (Vector3Int[])_spriteCoordinates.Clone();} }
        //private set {_spriteCoordinates = value;} }
    private Vector3Int[] _baseCoordinates;//backing field
    ///<summary>The coordinates of the base</summary>
    public Vector3Int[] baseCoordinates { 
        get {return (Vector3Int[])_baseCoordinates.Clone();} }
    ///<summary>Name of building</summary>
    public new string name { get; protected set;}
    public Texture2D insideAreaTexture {get; protected set;}
    ///<summary>The sprite of the building</summary>
    public Texture2D texture {get; protected set;}
    protected ButtonTypes[] _buildingInteractions = new ButtonTypes[0];//backing field
    public ButtonTypes[] buildingInteractions{ get {return (ButtonTypes[])_buildingInteractions.Clone();} }
    public int baseHeight { get; protected set; } 
    public int height {get {return texture.height / 16;}}
    public int width {get {return texture.width / 16;}}
    ///<summary>The tilemap this building is attached to</summary>
    public Tilemap tilemap {get; private set;} //the tilemap this building is attached to
    ///<summary>The gameobject this building is attached to</summary>
    //public GameObject gameObject {get {return tilemap.gameObject;}}
    ///<summary>The parent of the buttons of this building</summary>
    public GameObject buttonParent;

    private bool hasStarted = false;

    //public GameObject[] paintableParts;//todo figure out how to do paintable parts

    protected Dictionary<Materials,int> _materialsNeeded = new Dictionary<Materials, int>();
    public Dictionary<Materials,int> materialsNeeded{ 
        get {return new Dictionary<Materials,int>(_materialsNeeded);} 
    }
#pragma warning restore IDE1006 // Naming Styles

    public Building(Vector3Int[] position, Vector3Int[] basePosition, Tilemap tilemap) {
        //this.tilemap = tilemap;
        _spriteCoordinates = position;
        _baseCoordinates = basePosition;
    }

    public Building(){
        //tilemap = null;
        _spriteCoordinates = null;
        _baseCoordinates = null;
    }

    public Building(Vector3Int lowerLeftCorner){
        //tilemap = null;
        _spriteCoordinates = null;
        _baseCoordinates = null;
        // CalculateBasePositionsAndSpritePositions(lowerLeftCorner);
    }

    protected abstract void Init();

    public bool VectorInBaseCoordinates(Vector3Int checkForMe) {
        foreach (Vector3Int vector in baseCoordinates) if (checkForMe == vector) return true;
        return false;
    }

    public void Delete() {
        GameObject.Destroy(buttonParent);
        GameObject.Destroy(tilemap.gameObject);
    }

    public bool Equals(Building other){
        if (other.name != name) return false;
        if (other.baseCoordinates != baseCoordinates) return false;
        return true;
    }

    public override int GetHashCode(){
        int hash = 0;
        hash += name != null ? name.GetHashCode() : 0;
        hash += baseCoordinates != null ? baseCoordinates.GetHashCode() : 0;
        return hash;
    }

    public override string ToString() {
        return name;
    }

    protected void PlaceBuildingAfterStartIsDone(Vector3Int position){
        Debug.Log("Placeing building");
        List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, baseHeight, width);
        if (GetBuildingController().GetUnavailableCoordinates().Intersect(baseCoordinates).Count() != 0) Destroy(gameObject);

        List<Vector3Int> spriteCoordinates = GetAreaAroundPosition(position, height, width, true);
        Tile[] buildingTiles = SplitSprite(this, true);
        gameObject.GetComponent<TilemapRenderer>().sortingOrder = -position.y + 50;
        gameObject.GetComponent<Tilemap>().SetTiles(spriteCoordinates.ToArray(), buildingTiles);

        GetBuildingController().GetUnavailableCoordinates().UnionWith(baseCoordinates);

        _baseCoordinates = GetAreaAroundPosition(position, baseHeight, width).ToArray();
        _spriteCoordinates = spriteCoordinates.ToArray();
    }

    private IEnumerator PlaceBuildingCoroutine(Vector3Int position){
        while (!hasStarted) yield return null;
        PlaceBuildingAfterStartIsDone(position);
    }

    public void PlaceBuilding(Vector3Int position){
        StartCoroutine(PlaceBuildingCoroutine(position));
    }


    public void Start(){
        Debug.Log("Building starts");
        AddTilemapToObject(gameObject);
        texture = Resources.Load($"Buildings/{name}") as Texture2D;

        hasStarted = true;
    }

    public object Clone(){
        return Activator.CreateInstance(GetType(), _spriteCoordinates, _baseCoordinates, tilemap) as Building;
    }
}
