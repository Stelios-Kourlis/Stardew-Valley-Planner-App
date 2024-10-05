using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility {
    public static class InvalidTileLoader {

        private static readonly Color INVALID_COLOR = Color.black;
        private static readonly Color PLANTABLE_COLOR = Color.green;
        private static readonly Color NEUTRAL_COLOR = Color.white;
        private static readonly Color NEUTRAL_TREE_DISABLED_COLOR = Color.red;

        public static SpecialCoordinateRect GetSpecialCoordinateSet(string buildingName) {
            Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Special");
            if (image == null) return new($"{buildingName}", new TileType[0, 0]);

            // SpecialCoordinateRect specialCoordinateSet = new($"{buildingName}{tileType}", tileType);
            TileType[,] tiles = new TileType[image.height, image.width];
            for (int y = 0; y < image.height; y++) {
                for (int x = 0; x < image.width; x++) {
                    Color pixelColor = image.GetPixel(x, y);
                    if (pixelColor == INVALID_COLOR) tiles[y, x] = TileType.Invalid;
                    else if (pixelColor == PLANTABLE_COLOR) tiles[y, x] = TileType.Plantable;
                    else if (pixelColor == NEUTRAL_COLOR) tiles[y, x] = TileType.Neutral;
                    else if (pixelColor == NEUTRAL_TREE_DISABLED_COLOR) tiles[y, x] = TileType.NeutralTreeDisabled;
                    else throw new System.Exception($"Invalid color found in {buildingName}Special image");
                }
            }

            Resources.UnloadAsset(image);
            return new SpecialCoordinateRect($"{buildingName}", tiles);
        }


        // public static SpecialCoordinateRect GetInsideUnavailableCoordinates(string buildingName) {
        //     SpecialCoordinateRect UnavailableCoordinates = new($"{buildingName}Invalid", TileType.Invalid);
        //     Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
        //     if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

        //     for (int y = 0; y < image.height; y++) {
        //         for (int x = 0; x < image.width; x++) {
        //             Color pixelColor = image.GetPixel(x, y);
        //             if (pixelColor == Color.black) UnavailableCoordinates.Add(new Vector3Int(x, y, 0));
        //         }
        //     }

        //     Resources.UnloadAsset(image);
        //     return UnavailableCoordinates;
        // }

        // public static SpecialCoordinatesCollection GetInsidePlantableCoordinates(string buildingName) {
        //     SpecialCoordinatesCollection PlantableCoordinates = new();
        //     Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
        //     if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

        //     for (int y = 0; y < image.height; y++) {
        //         for (int x = 0; x < image.width; x++) {
        //             Color pixelColor = image.GetPixel(x, y);
        //             if (pixelColor == Color.green) PlantableCoordinates.AddSpecialTileSet(new($"{buildingName}Plantable", new Vector3Int(x, y, 0), TileType.Plantable));
        //         }
        //     }

        //     Resources.UnloadAsset(image);
        //     return PlantableCoordinates;
        // }

        // public static SpecialCoordinatesCollection GetInsideNeutralCoordinates(string buildingName) {
        //     SpecialCoordinatesCollection NeutralCoordinates = new();
        //     Texture2D image = Resources.Load<Texture2D>($"InvalidTiles/{buildingName}Invalid");
        //     if (image == null) throw new System.Exception($"Invalid building name: {buildingName}");

        //     for (int y = 0; y < image.height; y++) {
        //         for (int x = 0; x < image.width; x++) {
        //             Color pixelColor = image.GetPixel(x, y);
        //             if (pixelColor == Color.white) NeutralCoordinates.AddSpecialTileSet(new($"{buildingName}Neutral", new Vector3Int(x, y, 0), TileType.Neutral));
        //         }
        //     }

        //     Resources.UnloadAsset(image);
        //     return NeutralCoordinates;
        // }
    }
}
