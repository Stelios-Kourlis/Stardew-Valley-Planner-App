using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;

public static class BuildingUnavailableCoordinatesController {

    public static HashSet<Vector3Int> GetInsideUnavailableCoordinates(string buildingName) {
        HashSet<Vector3Int> UnavailableCoordinates = new();
        Texture2D image = Resources.Load<Texture2D>($"BuildingInsides/{buildingName}Invalid");
        if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

        for (int y = 0; y < image.height; y++) {
            for (int x = 0; x < image.width; x++) {
                Color pixelColor = image.GetPixel(x, y);
                if (pixelColor == Color.black) UnavailableCoordinates.Add(new Vector3Int(x, y, 0));
            }
        }

        // Debug.Log($"Building {buildingName} has {UnavailableCoordinates.Count} unavailable coordinates");
        return UnavailableCoordinates;
    }

    public static HashSet<Vector3Int> GetInsidePlantableCoordinates(string buildingName) {
        HashSet<Vector3Int> PlantableCoordinates = new();
        Texture2D image = Resources.Load<Texture2D>($"BuildingInsides/{buildingName}Invalid");
        if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

        for (int y = 0; y < image.height; y++) {
            for (int x = 0; x < image.width; x++) {
                Color pixelColor = image.GetPixel(x, y);
                if (pixelColor == Color.green) PlantableCoordinates.Add(new Vector3Int(x, y, 0));
            }
        }

        // Debug.Log($"Building {buildingName} has {PlantableCoordinates.Count} plantable coordinates");
        return PlantableCoordinates;
    }
}
