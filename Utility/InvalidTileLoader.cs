using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public static class InvalidTileLoader {

        public static SpecialCoordinateSet GetSpecialCoordinateSet(string buildingName, TileType tileType) {
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}{tileType}");
            if (image == null) return new($"{buildingName}{tileType}", tileType);

            SpecialCoordinateSet specialCoordinateSet = new($"{buildingName}{tileType}", tileType);
            Color colorToMatch = tileType switch {
                TileType.Invalid => Color.black,
                TileType.Plantable => Color.green,
                TileType.Neutral => Color.white,
                TileType.NeutralTreeDisabled => Color.red,
                _ => throw new System.Exception("Invalid tile type")
            };
            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == colorToMatch) specialCoordinateSet.Add(new Vector3Int(x, y, 0));
                }
            }

            Resources.UnloadAsset(image);
            return specialCoordinateSet;
        }


        public static SpecialCoordinateSet GetInsideUnavailableCoordinates(string buildingName) {
            SpecialCoordinateSet UnavailableCoordinates = new($"{buildingName}Invalid", TileType.Invalid);
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
            if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == Color.black) UnavailableCoordinates.Add(new Vector3Int(x, y, 0));
                }
            }

            Resources.UnloadAsset(image);
            return UnavailableCoordinates;
        }

        public static SpecialCoordinatesCollection GetInsidePlantableCoordinates(string buildingName) {
            SpecialCoordinatesCollection PlantableCoordinates = new();
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
            if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == Color.green) PlantableCoordinates.AddSpecialTileSet(new($"{buildingName}Plantable", new Vector3Int(x, y, 0), TileType.Plantable));
                }
            }

            Resources.UnloadAsset(image);
            return PlantableCoordinates;
        }

        public static SpecialCoordinatesCollection GetInsideNeutralCoordinates(string buildingName) {
            SpecialCoordinatesCollection NeutralCoordinates = new();
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
            if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == Color.white) NeutralCoordinates.AddSpecialTileSet(new($"{buildingName}Neutral", new Vector3Int(x, y, 0), TileType.Neutral));
                }
            }

            Resources.UnloadAsset(image);
            return NeutralCoordinates;
        }
    }
}
