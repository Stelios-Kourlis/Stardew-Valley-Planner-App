using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility{
    public static class ClassManager{
    public static  BuildingController GetBuildingController(){ return GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>(); }

    public static GameObject GetCamera(){ return GameObject.FindGameObjectWithTag("MainCamera"); }

    public static MapController GetMapController(){ return GameObject.FindGameObjectWithTag("Grid").GetComponent<MapController>(); }

    public static Tilemap GetFloorTilemap(){ return GameObject.FindWithTag("FloorTilemap").GetComponent<Tilemap>(); }

    public static GameObject GetCanvasGameObject(){ return GameObject.FindWithTag("Canvas"); }

    public static ButtonController GetButtonController(){ return GameObject.FindWithTag("LogicComponent").GetComponent<ButtonController>(); }

    public static NotificationManager GetNotificationManager(){ return GameObject.FindWithTag("LogicComponent").GetComponent<NotificationManager>(); }

    }
}
