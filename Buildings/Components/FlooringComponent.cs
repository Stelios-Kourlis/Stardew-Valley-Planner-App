using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.TilemapManager;
using static Utility.SpriteManager;
using System.Linq;
using static BuildingData;
using Newtonsoft.Json.Linq;

public class FlooringComponent : BuildingComponent {

    private class Flooring {
        public Vector3Int lowerLeftCorner;
        public int width;
        public int height;
        public int floorTextureID;
        private static readonly SpriteAtlas floorAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/FloorsAtlas");
        private readonly Tilemap tilemap;

        public Flooring(Vector3Int lowerLeftCorner, int width, int height, Tilemap tilemap, int floorTextureID = 0) {
            this.lowerLeftCorner = lowerLeftCorner;
            this.width = width;
            this.height = height;
            this.tilemap = tilemap;
            ApplyFloorTexture(floorTextureID);
        }

        public void ApplyFloorTexture(int floorTextureID) {
            this.floorTextureID = floorTextureID;
            Sprite floorSprite = floorAtlas.GetSprite($"Floors_{floorTextureID}"); //this is a 2x2 texture
            int spriteSize = 16;
            Sprite[] splitSprites = new Sprite[4];
            splitSprites[0] = Sprite.Create(floorSprite.texture, new Rect(0, 0, spriteSize, spriteSize), new Vector2(0.5f, 0.5f));
            splitSprites[1] = Sprite.Create(floorSprite.texture, new Rect(spriteSize, 0, spriteSize, spriteSize), new Vector2(0.5f, 0.5f));
            splitSprites[2] = Sprite.Create(floorSprite.texture, new Rect(0, spriteSize, spriteSize, spriteSize), new Vector2(0.5f, 0.5f));
            splitSprites[3] = Sprite.Create(floorSprite.texture, new Rect(spriteSize, spriteSize, spriteSize, spriteSize), new Vector2(0.5f, 0.5f));

            foreach (Vector3Int position in GetFloorPositions()) {
                if (position.x % 2 == 0 && position.y % 2 == 0) tilemap.SetTile(position, SplitSprite(floorSprite)[0]);
                if (position.x % 2 == 1 && position.y % 2 == 0) tilemap.SetTile(position, SplitSprite(floorSprite)[1]);
                if (position.x % 2 == 0 && position.y % 2 == 1) tilemap.SetTile(position, SplitSprite(floorSprite)[2]);
                if (position.x % 2 == 1 && position.y % 2 == 1) tilemap.SetTile(position, SplitSprite(floorSprite)[3]);
            }

            foreach (Sprite sprite in splitSprites) Destroy(sprite);
        }

        public IEnumerable<Vector3Int> GetFloorPositions() {
            return GetRectAreaFromPoint(lowerLeftCorner, height, width);
        }

        public static Sprite GetFloorSprite(int textureId) {
            return floorAtlas.GetSprite($"Floors_{textureId}");
        }

        public static int GetTotalFloorTextures() {
            return floorAtlas.spriteCount;
        }
    }

    [Serializable]
    public class FlooringOrigin {
        public Vector3Int lowerLeftCorner;
        public int width;
        public int height;
        public int floorTextureID;
        public FlooringOrigin(Vector3Int lowerLeftCorner, int width, int height, int floorTextureID = 0) {
            this.lowerLeftCorner = lowerLeftCorner;
            this.width = width;
            this.height = height;
            this.floorTextureID = floorTextureID;
        }
    }
    private List<Flooring> floors = new();
    public Tilemap flooringTilemap;
    public static int selectedFloorTextureID = 0;
    public static Sprite SelectedFloorSprite => Flooring.GetFloorSprite(selectedFloorTextureID);
    public static int TotalFloorTextures => Flooring.GetTotalFloorTextures();

    public void SetFloors(List<FlooringOrigin> origins, Tilemap flooringTilemap) {
        floors = new();
        this.flooringTilemap = flooringTilemap;

        foreach (FlooringOrigin origin in origins) {
            AddFloor(origin);
        }

    }

    public void UpdateFloors(List<FlooringOrigin> origins) {
        flooringTilemap.ClearAllTiles();
        floors = new();

        foreach (FlooringOrigin origin in origins) {
            AddFloor(origin);
        }

    }


    public static Sprite GetFloorSprite(int textureId) {
        return Flooring.GetFloorSprite(textureId);
    }

    public static void SetSelectedFloor(int floorTextureID, bool setActionToPlaceFlooring = true) {
        selectedFloorTextureID = floorTextureID;
        if (setActionToPlaceFlooring) BuildingController.SetCurrentAction(Actions.PLACE_FLOORING);
    }


    public void AddFloor(FlooringOrigin origin) {
        Flooring floor = new(origin.lowerLeftCorner, origin.width, origin.height, flooringTilemap, origin.floorTextureID);
        if (floors.Any(floor => floor.GetFloorPositions().Intersect(GetRectAreaFromPoint(origin.lowerLeftCorner, origin.height, origin.width)).Any())) throw new Exception($"Floorings overlap trigger: {origin.lowerLeftCorner}");
        floors.Add(floor);
    }

    public void RemoveFloor(Vector3Int floorPosition) {
        Flooring floor = floors.Find(floor => floor.GetFloorPositions().Contains(floorPosition));
        if (floor == null) return;
        floors.Remove(floor);
        foreach (Vector3Int position in floor.GetFloorPositions()) flooringTilemap.SetTile(position, null);

    }

    public void ApplyCurrentFloorTexture(Vector3Int floorPosition) {
        Flooring floor = floors.Find(floor => floor.GetFloorPositions().Contains(floorPosition));
        if (floor == null) return;
        ApplyCurrentFloorTexture(floor);
    }

    private void ApplyCurrentFloorTexture(Flooring floor) {
        floor.ApplyFloorTexture(selectedFloorTextureID);
    }

    public override void Load(ComponentData data) {
        for (int i = 0; i < floors.Count; i++) {
            RemoveFloor(floors[i].GetFloorPositions().First());
        }

        foreach (var property in data.componentData) {
            JObject buildingData = (JObject)property.Value;
            Vector3Int lowerLeftCorner = new(
                buildingData["Origin"][0].Value<int>(),
                buildingData["Origin"][1].Value<int>(),
                0
            );
            int width = buildingData["Width"].Value<int>();
            int height = buildingData["Height"].Value<int>();
            int wallpaperId = buildingData["WallpaperId"].Value<int>();

            FlooringOrigin origin = new(lowerLeftCorner, width, height, wallpaperId);
            AddFloor(origin);
        }
    }

    public override ComponentData Save() {
        // return null;
        ComponentData data = new(typeof(FlooringComponent), new());
        int index = 0;

        foreach (Flooring floor in floors) {
            FlooringOrigin origin = new(floor.GetFloorPositions().First(), floor.width, floor.height, floor.floorTextureID);
            JProperty floorProperty = new JProperty(index.ToString(),
                new JObject(
                    new JProperty("Origin", new JArray(origin.lowerLeftCorner.x, origin.lowerLeftCorner.y)),
                    new JProperty("Width", origin.width),
                    new JProperty("Height", origin.height),
                    new JProperty("WallpaperId", origin.floorTextureID)
                )
            );

            data.componentData.Add(floorProperty);
        }

        return data;
    }
}
