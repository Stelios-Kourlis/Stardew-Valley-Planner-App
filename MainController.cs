// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Tilemaps;
// using System;
// using System.IO;
// using System.Reflection;
// using System.Linq;
// using UnityEngine.EventSystems;
// using UnityEngine.UI;
// using static Utility.SpriteManager;
// using static Utility.TilemapManager;
// using static Utility.BuildingManager;

// public class MainController : MonoBehaviour {
//     private GameObject grid;
//     private TileBuildingData dataScript;
//     public Tilemap tilemapTemplate;
//     public Tilemap tilemap;
//     Vector3Int currentCell;
//     Tilemap mapTilemap;
//     Tilemap floorTilemap;
//     HashSet<Vector3Int> floorCells;
//     Tilemap tempTilemap;

//     void Start() {

//         // grid = GameObject.FindGameObjectWithTag("Grid");
//         // //placeHouse(1);
//         // currentCell = tilemap.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

//         // mapTilemap = Instantiate<Tilemap>(tilemapTemplate, grid.transform);
//         // mapTilemap.GetComponent<TilemapRenderer>().sortingOrder = -1000; //SortingOrder is Layer Number
//         // mapTilemap.name = "Map";
//         // mapTilemap.tag = "CurrentMap";

//         // floorTilemap = GameObject.FindGameObjectWithTag("FloorTilemap").GetComponent<Tilemap>();
//         // floorCells = new HashSet<Vector3Int>();

//         // //camera = GameObject.FindGameObjectWithTag("MainCamera");

//         // tilemap = GameObject.FindGameObjectWithTag("NormalMap").GetComponent<Tilemap>();


//         // tempTilemap = Instantiate<Tilemap>(tilemapTemplate, grid.transform);
//         // tempTilemap.name = "TempTilemap";



//         // dataScript = gameObject.AddComponent(typeof(TileBuildingData)) as TileBuildingData;
//         //dataScript.addInvalidTilesData("Normal");

//         //mouseHandler = gameObject.AddComponent(typeof(MouseHandler)) as MouseHandler;
//         //mouseHandler = gameObject.GetComponent<MouseHandler>();

//         //utils = gameObject.GetComponent<Utility>();
//         //setMap(MapTypes.Normal);
//         //grid.GetComponent<BuildingController>().PlaceBuilding(new ShippingBin(null, null, null), new Vector3Int(44, 14, 0));
//     }

// }
