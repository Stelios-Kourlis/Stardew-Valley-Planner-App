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

    public class Flooring {
        public Vector3Int lowerLeftCorner;
        public int width;
        public int height;
        public int floorTextureID;
        private static readonly SpriteAtlas floorAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/FloorsAtlas");
        private readonly Tilemap tilemap;
        public Action<int> floorTextureChanged;
        public KeyValuePair<bool, FlooringOrigin> wasModifiedByRenovation = new(false, null); //I dont think is is needed anymore but it doesnt interfere with anything and might be useful in the future


        public Flooring(Vector3Int lowerLeftCorner, int width, int height, Tilemap tilemap, int floorTextureID = 0) {
            this.lowerLeftCorner = lowerLeftCorner;
            this.width = width;
            this.height = height;
            this.tilemap = tilemap;
            ApplyFloorTexture(floorTextureID);
        }

        /// <summary>
        /// Create a linked floor, Changing the floor texture of one will change the texture for the other as well
        /// </summary>
        /// <param name="lowerLeftCorner"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="tilemap"></param>
        /// <param name="parentFloor"></param>
        // public Flooring(Vector3Int lowerLeftCorner, int width, int height, Tilemap tilemap, Flooring linkedFloor) {
        //     this.lowerLeftCorner = lowerLeftCorner;
        //     this.width = width;
        //     this.height = height;
        //     this.tilemap = tilemap;
        //     ApplyFloorTexture(linkedFloor.floorTextureID);
        //     linkedFloor.floorTextureChanged += newFloorTextureID => ApplyFloorTexture(newFloorTextureID);
        // }

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
                //for some reason negative values are not being processed correctly
                if (Mathf.Abs(position.x) % 2 == 0 && Mathf.Abs(position.y) % 2 == 0) tilemap.SetTile(position, SplitSprite(floorSprite)[0]);
                else if (Mathf.Abs(position.x) % 2 == 1 && Mathf.Abs(position.y) % 2 == 0) tilemap.SetTile(position, SplitSprite(floorSprite)[1]);
                else if (Mathf.Abs(position.x) % 2 == 0 && Mathf.Abs(position.y) % 2 == 1) tilemap.SetTile(position, SplitSprite(floorSprite)[2]);
                else if (Mathf.Abs(position.x) % 2 == 1 && Mathf.Abs(position.y) % 2 == 1) tilemap.SetTile(position, SplitSprite(floorSprite)[3]);
            }

            foreach (Sprite sprite in splitSprites) Destroy(sprite);
            floorTextureChanged?.Invoke(floorTextureID);
        }

        public void MoveFloor(Vector3Int lowerLeftCorner, int width, int height, int floorTextureID = 0) {
            if (wasModifiedByRenovation.Key) wasModifiedByRenovation = new(true, new(lowerLeftCorner, width, height, floorTextureID));
            foreach (Vector3Int oldTile in GetFloorPositions()) {
                tilemap.SetTile(oldTile, null);
            }
            this.lowerLeftCorner = lowerLeftCorner;
            this.width = width;
            this.height = height;
            ApplyFloorTexture(floorTextureID);
        }

        public FlooringOrigin GetOriginRepresentingThisFloor(bool giveOriginal = true) {
            if (giveOriginal) if (wasModifiedByRenovation.Key) return wasModifiedByRenovation.Value;
            return new(lowerLeftCorner, width, height, floorTextureID);

        }

        public bool FloorContains(Vector3Int point) {
            return GetFloorPositions().Contains(point);
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
        // public Flooring linkedFloor;
        public FlooringOrigin(Vector3Int lowerLeftCorner, int width, int height, int floorTextureID = 0) {
            this.lowerLeftCorner = lowerLeftCorner;
            this.width = width;
            this.height = height;
            this.floorTextureID = floorTextureID;
        }

        // public FlooringOrigin(Vector3Int lowerLeftCorner, int width, int height, Flooring linkedFloor) {
        //     this.lowerLeftCorner = lowerLeftCorner;
        //     this.width = width;
        //     this.height = height;
        //     this.linkedFloor = linkedFloor;
        // }
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
        var overlappingFloors = floors.Where(floor => floor.GetFloorPositions().Intersect(GetRectAreaFromPoint(origin.lowerLeftCorner, origin.height, origin.width)).Any());
        if (overlappingFloors.Any() && !BuildingController.IsLoadingSave) {
            string overlaps = string.Join(", ", overlappingFloors.Select(f => $"[{string.Join(", ", f.GetFloorPositions())}]"));
            throw new Exception($"Floorings overlap trigger: {origin.lowerLeftCorner} at {overlaps}");
        }
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
        int oldFloorTextureID = floor.floorTextureID;
        floor.ApplyFloorTexture(selectedFloorTextureID);
        UndoRedoController.AddActionToLog(new UserAction(this, floorPosition, oldFloorTextureID, selectedFloorTextureID));
    }

    public void ApplyFloorTexture(Vector3Int floorPosition, int floorTextureID) {
        Flooring floor = floors.Find(floor => floor.GetFloorPositions().Contains(floorPosition));
        if (floor == null) return;
        floor.ApplyFloorTexture(floorTextureID);
    }

    public void MoveFloor(Vector3Int oldFloor, FlooringOrigin newOrigin) {
        Flooring floor = GetFlooringFromPoint(oldFloor);
        if (floor == null) return;
        floor.MoveFloor(floor.lowerLeftCorner + newOrigin.lowerLeftCorner, floor.width + newOrigin.width, floor.height + newOrigin.height, newOrigin.floorTextureID == -1 ? floor.floorTextureID : newOrigin.floorTextureID);
    }

    private Flooring GetFlooringFromPoint(Vector3Int point) {
        return floors.FirstOrDefault(wall => wall.FloorContains(point));
    }

    public override void Load(ComponentData data) {
        // return;
        for (int i = 0; i < floors.Count; i++) {
            RemoveFloor(floors[i].GetFloorPositions().First());
        }

        foreach (JProperty property in data.GetAllComponentDataProperties()) {
            JObject buildingData = (JObject)property.Value;
            Vector3Int lowerLeftCorner = new(
                buildingData["Origin"][0].Value<int>(),
                buildingData["Origin"][1].Value<int>(),
                0
            );
            int width = buildingData["Width"].Value<int>();
            int height = buildingData["Height"].Value<int>();
            int wallpaperId = buildingData["Floor Texture ID"].Value<int>();

            FlooringOrigin origin = new(lowerLeftCorner, width, height, wallpaperId);
            AddFloor(origin);
        }
    }

    public override ComponentData Save() {
        // return null;
        ComponentData data = new(typeof(FlooringComponent));
        int index = 0;

        foreach (Flooring floor in floors) {
            FlooringOrigin origin = floor.GetOriginRepresentingThisFloor(false);
            JProperty floorProperty = new(index.ToString(),
                new JObject(
                    new JProperty("Origin", new JArray(origin.lowerLeftCorner.x, origin.lowerLeftCorner.y)),
                    new JProperty("Width", origin.width),
                    new JProperty("Height", origin.height),
                    new JProperty("Floor Texture ID", origin.floorTextureID)
                )
            );

            data.AddProperty(floorProperty);
            index++;
        }

        return data;
    }
}
