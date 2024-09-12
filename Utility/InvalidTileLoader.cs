using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public static class InvalidTileLoader {
        public static HashSet<Vector3Int> GetInsideUnavailableCoordinates(string buildingName) {
            HashSet<Vector3Int> UnavailableCoordinates = new();
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
            if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == Color.black) UnavailableCoordinates.Add(new Vector3Int(x, y, 0));
                }
            }

            // Resources.UnloadAsset(image);
            return UnavailableCoordinates;
        }

        public static HashSet<Vector3Int> GetInsidePlantableCoordinates(string buildingName) {
            HashSet<Vector3Int> PlantableCoordinates = new();
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
            if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == Color.green) PlantableCoordinates.Add(new Vector3Int(x, y, 0));
                }
            }

            // Resources.UnloadAsset(image);
            return PlantableCoordinates;
        }

        public static HashSet<Vector3Int> GetInsideNeutralCoordinates(string buildingName) {
            HashSet<Vector3Int> NeutralCoordinates = new();
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
            if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == Color.white) NeutralCoordinates.Add(new Vector3Int(x, y, 0));
                }
            }

            // Resources.UnloadAsset(image);
            return NeutralCoordinates;
        }
    }
}
