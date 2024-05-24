using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility{
    public static class ClassManager{
        public static BuildingController GetBuildingController(){ return GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>(); }

        public static GameObject GetCamera(){ return GameObject.FindGameObjectWithTag("MainCamera"); }

        public static MapController GetMapController(){ return GameObject.FindGameObjectWithTag("Grid").GetComponent<MapController>(); }

        public static Tilemap GetFloorTilemap(){ return GameObject.FindWithTag("FloorTilemap").GetComponent<Tilemap>(); }

        public static GameObject GetCanvasGameObject(){ return GameObject.FindWithTag("Canvas"); }

        public static ButtonController GetButtonController(){ return GameObject.FindWithTag("LogicComponent").GetComponent<ButtonController>(); }

        public static NotificationManager GetNotificationManager(){ return GameObject.FindWithTag("LogicComponent").GetComponent<NotificationManager>(); }

        public static InputHandler GetInputHandler(){ return GameObject.FindWithTag("LogicComponent").GetComponent<InputHandler>(); }

        public static SettingsModalController GetSettingsModalController(){ return GameObject.FindWithTag("SettingsModal").GetComponent<SettingsModalController>(); }

        public static TotalMaterialsCalculator GetTotalMaterialsCalculator(){ return GameObject.FindWithTag("TotalMaterialsNeededPanel").GetComponent<TotalMaterialsCalculator>(); }

        public static bool BuildingHasMoreThanOneBuildingInterface(Building building, Type type){
            List<Type> buildingInterfaces = new(){
                typeof(IMultipleTypeBuilding<>),
                typeof(IConnectingBuilding),
                typeof(ITieredBuilding),
                };
            List<Type> interfaces = new();
            foreach (Type itype in building.GetType().GetInterfaces().ToList()){
                if (itype.IsGenericType) interfaces.Add(itype.GetGenericTypeDefinition());
                else interfaces.Add(itype);
            }
            List<Type> interfacesImplementedByBuilding = interfaces.Where(i => buildingInterfaces.Contains(i)).ToList();
            interfacesImplementedByBuilding.Remove(type);
            return interfacesImplementedByBuilding.Count > 0;
        }
    }
}
