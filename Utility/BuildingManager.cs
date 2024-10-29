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
        /// <summary>
        /// Check if a building can be placed at a certain position
        /// <param name="position"> The lower left corner of the building position </param>
        /// <param name="building"> The building you want to place </param>
        /// </summary>
        /// <returns> If the position is unavailable return (false, reason), with reason being a string with the issue, else returns (true, null)</returns>
        public static bool BuildingCanBePlacedAtPosition(Vector3Int position, Building building, out string errorMessage) {
            errorMessage = "";
            if (BuildingController.IsLoadingSave) return true;
            //todo rewrite this to better utilize the new coordinate system
            // Debug.Log(BuildingController.GetUnavailableCoordinates().Contains(new Vector3Int(32, 12, 0)));
            if (!building.BuildingSpecificPlacementPreconditionsAreMet(position, out errorMessage)) return false;
            if (building.CurrentBuildingState == Building.BuildingState.PLACED) { errorMessage = "Building has already been placed"; return false; }
            HashSet<Vector3Int> unavailableCoordinates = InvalidTilesManager.Instance.AllInvalidTiles;
            HashSet<Vector3Int> plantableCoordinates = InvalidTilesManager.Instance.AllPlantableTiles;


            List<Vector3Int> baseCoordinates = building.BaseCoordinates.ToList();
            if (building.type == BuildingType.Floor) { //if floor ignore other buildings //todo Things like mailbox and greenhouse porch still interfere
                List<Vector3Int> buildingBaseCoordinates = BuildingController.buildings.SelectMany(building => building.BaseCoordinates).ToList();
                unavailableCoordinates = unavailableCoordinates.Except(buildingBaseCoordinates).ToHashSet();
            }
            if (unavailableCoordinates.Intersect(baseCoordinates).Count() > 0) { errorMessage = $"Can't place {building.BuildingName} there"; return false; }
            if (BuildingController.isInsideBuilding.Key && baseCoordinates.Any(coord => !CoordinateIsWithinBounds(coord, unavailableCoordinates))) { errorMessage = $"Can't place {building.BuildingName} outside of bounds"; return false; }

            MapController.MapTypes mapType = GetMapController().CurrentMapType;
            HashSet<BuildingType> actualBuildings = new(){
                BuildingType.Barn,
                BuildingType.Cabin,
                BuildingType.Coop,
                BuildingType.FishPond,
                BuildingType.GoldClock,
                BuildingType.Greenhouse,
                BuildingType.House,
                BuildingType.JunimoHut,
                BuildingType.Mill,
                BuildingType.Obelisk,
                BuildingType.Shed,
                BuildingType.ShippingBin,
                BuildingType.Silo,
                BuildingType.SlimeHutch,
                BuildingType.Stable,
                BuildingType.Well,
                BuildingType.PetBowl
            };
            if (mapType == MapController.MapTypes.GingerIsland && actualBuildings.Contains(building.type)) { errorMessage = $"{building.type} can't be placed on Ginger Island"; return false; }

            if ((building.type == BuildingType.Crop || building.type == BuildingType.Tree) && !plantableCoordinates.Contains(position)) { errorMessage = $"Can't place a {building.type} there"; return false; }

            if (BuildingController.isInsideBuilding.Key && actualBuildings.Contains(building.type)) { errorMessage = "Can't place a building inside another building"; return false; }
            errorMessage = "";
            return true;
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

        public static bool IsMouseCurrentlyOverBuilding(Building building) {
            Vector3Int mousePosition = GetMousePositionInTilemap();
            return building.BaseCoordinates.Contains(mousePosition);
        }

        public static void PlayParticleEffect(Building building, bool isPlace = false) {
            if (BuildingController.IsLoadingSave) return;
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
