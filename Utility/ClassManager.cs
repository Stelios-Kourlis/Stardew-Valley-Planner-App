using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

namespace Utility {
    public static class ClassManager {
        // [[Obsolete("Use BuildingController instead")]]
        // public static BuildingController GetBuildingController() { return GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>(); }

        public static Tilemap GetGridTilemap() {
            if (BuildingController.isInsideBuilding.Key) return SceneManager.GetActiveScene().GetRootGameObjects().First(go => go.name == "Grid").GetComponent<Tilemap>();
            return GameObject.FindGameObjectWithTag("Grid").GetComponent<Tilemap>();
        }

        public static GameObject GetCamera() { return GameObject.FindGameObjectWithTag("MainCamera"); }

        public static MapController GetMapController() { return GameObject.FindGameObjectWithTag("Grid").GetComponent<MapController>(); }

        public static GameObject GetCanvasGameObject() { return GameObject.FindWithTag("Canvas"); }

        public static BuildingButtonController GetButtonController() { return GameObject.FindWithTag("LogicComponent").GetComponent<BuildingButtonController>(); }

        public static GameObject GetParticleSystem() { return GameObject.Find("BuildingParticle"); }

        public static UpdateChecker GetUpdateChecker() { return GameObject.FindWithTag("UpdateChecker").GetComponent<UpdateChecker>(); }

        public static GameObject GetSettingsModal() { return GameObject.FindWithTag("SettingsModal"); }

        public static TotalMaterialsCalculator GetTotalMaterialsCalculator() { return GameObject.FindWithTag("TotalMaterialsNeededPanel").GetComponent<TotalMaterialsCalculator>(); }
    }
}
