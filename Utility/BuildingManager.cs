using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.TilemapManager;

namespace Utility {
    public static class BuildingManager {

        /// <summary>
        /// Save the current state of the buildings to a file
        /// </summary>
        /// <returns>true, if the user saved, false if the user cancelled</returns>
        public static bool Save() {
            string defaultSavePath = PlayerPrefs.GetString("DefaultSavePath", Application.dataPath);
            string savePath = StandaloneFileBrowser.SaveFilePanel("Choose a save location", defaultSavePath, "Farm", "svp"); ;
            if (savePath != "") {
                string directoryPath = Path.GetDirectoryName(savePath);
                PlayerPrefs.SetString("DefaultSavePath", directoryPath);
                using StreamWriter writer = new(savePath);
                foreach (Building building in BuildingController.GetBuildings()) {
                    writer.WriteLine(building.GetBuildingData());
                }
            }
            return savePath != "";
        }

        /// <summary>
        /// Load a farm from a file
        /// </summary>
        /// <returns>true, if the user chose a file, false if the user cancelled</returns>
        public static bool Load() {
            string defaultLoadPath = PlayerPrefs.GetString("DefaultLoadPath", Application.dataPath);
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultLoadPath, new ExtensionFilter[] { new("Stardew Valley Planner Files", "svp") }, false);
            Type currentType = BuildingController.currentBuildingType;
            if (paths.Length > 0) {
                string directoryPath = Path.GetDirectoryName(paths[0]);
                PlayerPrefs.SetString("DefaultLoadPath", directoryPath);
                using StreamReader reader = new(paths[0]);
                BuildingController.DeleteAllBuildings(true);
                BuildingController.IsLoadingSave = true;
                while (reader.Peek() >= 0) {
                    string line = reader.ReadLine();
                    if (line.Equals("")) continue;
                    BuildingController.PlaceSavedBuilding(line);
                }
                BuildingController.IsLoadingSave = false;
            }
            BuildingController.SetCurrentBuildingType(currentType);
            return paths.Length > 0;
        }

        public static bool LoadThenSwitchSceneToFarm() {
            string defaultLoadPath = PlayerPrefs.GetString("DefaultLoadPath", Application.dataPath);
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultLoadPath, new ExtensionFilter[] { new("Stardew Valley Planner Files", "svp") }, false);
            if (paths.Length <= 0) return false;
            SceneManager.LoadScene("App");
            Type currentType = BuildingController.currentBuildingType;
            string directoryPath = Path.GetDirectoryName(paths[0]);
            PlayerPrefs.SetString("DefaultLoadPath", directoryPath);
            using StreamReader reader = new(paths[0]);
            BuildingController.DeleteAllBuildings(true);
            BuildingController.IsLoadingSave = true;
            while (reader.Peek() >= 0) {
                string line = reader.ReadLine();
                if (line.Equals("")) continue;
                BuildingController.PlaceSavedBuilding(line);
            }
            BuildingController.IsLoadingSave = false;
            BuildingController.SetCurrentBuildingType(currentType);
            return paths.Length > 0;
        }


        /// <summary>
        /// Check if a building can be placed at a certain position
        /// <param name="position"> The lower left corner of the building position </param>
        /// <param name="building"> The building you want to place </param>
        /// </summary>
        /// <returns> If the position is unavailable return (false, reason), with reason being a string with the issue, else returns (true, null)</returns>
        public static (bool, string) BuildingCanBePlacedAtPosition(Vector3Int position, Building building) {
            if (building.IsPlaced) return (false, "Building has already been placed");
            Vector3Int[] unavailableCoordinates, plantableCoordinates;
            if (BuildingController.isInsideBuilding.Key) {
                IEnterableBuilding enterableBuilding = BuildingController.isInsideBuilding.Value.parent.gameObject.GetComponent<IEnterableBuilding>();
                unavailableCoordinates = enterableBuilding.InteriorUnavailableCoordinates;
                plantableCoordinates = enterableBuilding.InteriorPlantableCoordinates;
            }
            else {
                unavailableCoordinates = BuildingController.GetUnavailableCoordinates().ToArray();
                plantableCoordinates = BuildingController.GetPlantableCoordinates().ToArray();
            }

            List<Vector3Int> baseCoordinates = GetAreaAroundPosition(position, building.BaseHeight, building.Width);
            if (unavailableCoordinates.Intersect(baseCoordinates).Count() > 0) return (false, $"Can't place {building.BuildingName} there");

            MapController.MapTypes mapType = GetMapController().CurrentMapType;
            HashSet<Type> actualBuildings = new(){
                // typeof(Barn),
                // typeof(Cabin),
                // typeof(Coop),
                typeof(FishPond),
                typeof(GoldClock),
                typeof(Greenhouse),
                // typeof(House),
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
            if (mapType == MapController.MapTypes.GingerIsland && actualBuildings.Contains(building.GetType())) return (false, $"{building.GetType()} can't be placed on Ginger Island");

            if (building.GetType() == typeof(Crop) && !plantableCoordinates.Contains(position)) return (false, "Can't place a crop there");

            if (BuildingController.isInsideBuilding.Key && actualBuildings.Contains(building.GetType())) return (false, "Can't place a building inside another building");
            return (true, null);
        }

        public static bool LeftClickShouldRegister() {
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name == "TopRightButtons") return true;
            if (EventSystem.current.IsPointerOverGameObject()) return false;
            if (GetSettingsModalController().IsOpen) return false;
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

        public static void AddHoverEffect(Button button) {
            EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnterEntry = new();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((eventData) => {
                button.transform.localScale = new Vector3(1.2f, 1.2f);
            });
            eventTrigger.triggers.Add(pointerEnterEntry);
            EventTrigger.Entry pointerExitEntry = new();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((eventData) => {
                button.transform.localScale = new Vector3(1, 1);
            });
            eventTrigger.triggers.Add(pointerExitEntry);
        }
    }
}
