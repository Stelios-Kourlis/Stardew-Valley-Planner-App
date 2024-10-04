using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using SFB;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static BuildingData;
using static Utility.ClassManager;
using static Utility.TilemapManager;
using Newtonsoft.Json;

namespace Utility {
    public static class BuildingManager {

        public static bool LoadThenSwitchSceneToFarm() {//todo fix
            string defaultLoadPath = PlayerPrefs.GetString("DefaultLoadPath", Application.dataPath);
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultLoadPath, new ExtensionFilter[] { new("Stardew Valley Planner Files", "svp") }, false);
            // if (paths.Length <= 0) return false;
            // SceneManager.LoadScene("App");
            // Type currentType = BuildingController.currentBuildingType;
            // string directoryPath = Path.GetDirectoryName(paths[0]);
            // PlayerPrefs.SetString("DefaultLoadPath", directoryPath);
            // using StreamReader reader = new(paths[0]);
            // BuildingController.DeleteAllBuildings(true);
            // BuildingController.IsLoadingSave = true;
            // while (reader.Peek() >= 0) {
            //     string line = reader.ReadLine();
            //     if (line.Equals("")) continue;
            //     BuildingController.PlaceSavedBuilding(new BuildingData(line));
            // }
            // BuildingController.IsLoadingSave = false;
            // BuildingController.SetCurrentBuildingType(currentType);
            return paths.Length > 0;
        }


        /// <summary>
        /// Check if a building can be placed at a certain position
        /// <param name="position"> The lower left corner of the building position </param>
        /// <param name="building"> The building you want to place </param>
        /// </summary>
        /// <returns> If the position is unavailable return (false, reason), with reason being a string with the issue, else returns (true, null)</returns>
        public static (bool, string) BuildingCanBePlacedAtPosition(Vector3Int position, Building building) {
            // Debug.Log(BuildingController.GetUnavailableCoordinates().Contains(new Vector3Int(32, 12, 0)));
            if (building.CurrentBuildingState == Building.BuildingState.PLACED) return (false, "Building has already been placed");
            HashSet<Vector3Int> unavailableCoordinates = InvalidTilesManager.Instance.AllInvalidTiles;
            HashSet<Vector3Int> plantableCoordinates = InvalidTilesManager.Instance.AllPlantableTiles;


            List<Vector3Int> baseCoordinates = GetRectAreaFromPoint(position, building.BaseHeight, building.Width);
            if (building is Greenhouse) baseCoordinates.AddRange(GetRectAreaFromPoint(new Vector3Int(position.x + 2, position.y - 2, position.z), 2, 3));
            if (unavailableCoordinates.Intersect(baseCoordinates).Count() > 0) return (false, $"Can't place {building.BuildingName} there");
            if (BuildingController.isInsideBuilding.Key && baseCoordinates.Any(coord => !CoordinateIsWithinBounds(coord, unavailableCoordinates))) return (false, $"Can't place {building.BuildingName} outside of bounds");

            MapController.MapTypes mapType = GetMapController().CurrentMapType;
            HashSet<Type> actualBuildings = new(){
                typeof(Barn),
                typeof(Cabin),
                typeof(Coop),
                typeof(FishPond),
                typeof(GoldClock),
                typeof(Greenhouse),
                typeof(House),
                typeof(JunimoHut),
                typeof(Mill),
                typeof(Obelisk),
                typeof(Shed),
                typeof(ShippingBin),
                typeof(Silo),
                typeof(SlimeHutch),
                typeof(Stable),
                typeof(Well),
                typeof(PetBowl)
            };
            // Debug.Log($"{GetMousePositionInTilemap()} - {BuildingController.GetUnavailableCoordinates().Contains(new Vector3Int(32, 12, 0))} - {BuildingController.GetUnavailableCoordinates().Contains(GetMousePositionInTilemap())}");
            if (mapType == MapController.MapTypes.GingerIsland && actualBuildings.Contains(building.GetType())) return (false, $"{building.GetType()} can't be placed on Ginger Island");

            if ((building.GetType() == typeof(Crop) || building.GetType() == typeof(WoodTree)) && !plantableCoordinates.Contains(position)) return (false, "Can't place a Crop/Tree there");

            if (BuildingController.isInsideBuilding.Key && actualBuildings.Contains(building.GetType())) return (false, "Can't place a building inside another building");
            return (true, null);
        }

        private static bool CoordinateIsWithinBounds(Vector3Int coordinate, IEnumerable<Vector3Int> bounds) {
            bool isWithinX = bounds.Any(coord => coord.x <= coordinate.x && coord.y == coordinate.y) && bounds.Any(coord => coord.x >= coordinate.x && coord.y == coordinate.y);
            bool isWithinY = bounds.Any(coord => coord.y <= coordinate.y && coord.x == coordinate.x) && bounds.Any(coord => coord.y >= coordinate.y && coord.x == coordinate.x);

            return isWithinX && isWithinY;
        }

        public static bool LeftClickShouldRegister() {
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name == "TopRightButtons") return true;
            if (EventSystem.current.IsPointerOverGameObject()) return false;
            if (GetSettingsModal().GetComponent<MoveablePanel>().IsPanelOpen()) return false;
            return true;
        }

        public static void PlayParticleEffect(Building building, bool isPlace = false) {
            GameObject ParticleSystem = GetParticleSystem();
            ParticleSystem.transform.position = GetMiddleOfBuildingWorld(building);
            var particleSystem = ParticleSystem.GetComponent<ParticleSystem>();
            var shape = particleSystem.shape;
            shape.scale = new Vector3(building.Width, building.BaseHeight, building.BaseHeight);
            var emission = particleSystem.emission;
            var newBurst = emission.GetBurst(0);
            //put some extra particles in place just because
            (float, float) particleAmount = (building.Width * building.BaseHeight, building.Width * building.BaseHeight * 1.5f);
            if (isPlace) particleAmount = (particleAmount.Item1 * 1.5f, particleAmount.Item2 * 1.5f);
            newBurst.count = new ParticleSystem.MinMaxCurve(particleAmount.Item1, particleAmount.Item2);
            emission.SetBurst(0, newBurst);
            particleSystem.Play();
        }
    }
}
