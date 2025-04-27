using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.U2D;
using static Utility.SpriteManager;
using static Utility.TilemapManager;
using System.Linq;
using static BuildingData;
using Newtonsoft.Json.Linq;

public class WallsComponent : BuildingComponent {

    private class WallStrip {
        public readonly Vector3Int[] strip = new Vector3Int[3];

        public WallStrip(Vector3Int stripBottomVector) {
            strip[0] = stripBottomVector;
            strip[1] = stripBottomVector + new Vector3Int(0, 1, 0);
            strip[2] = stripBottomVector + new Vector3Int(0, 2, 0);
        }

        public bool StripContains(Vector3Int point) {
            return strip.Contains(point);
        }
    }

    private class Wall {

        public int Width => strips.Count;
        public Vector3Int LowerLeftCorner => GetAllWallCordinates().ToList()[0];

        private List<WallStrip> strips;
        public int wallpaperId;
        private readonly Tilemap wallpaperTilemap;
        private static readonly SpriteAtlas wallpaperAtlas = Resources.Load<SpriteAtlas>("BuildingInsides/WallsAtlas");
        public KeyValuePair<bool, WallOrigin> wasModifiedByRenovation = new(false, null); //I dont think is is needed anymore but it doesnt interfere with anything and might be useful in the future

        public Wall(Vector3Int lowerLeftCorner, int width, Tilemap wallpaperTilemap, int wallpaperId = 0) {
            strips = new();
            for (int x = lowerLeftCorner.x; x <= lowerLeftCorner.x + width - 1; x++) {
                strips.Add(new WallStrip(new Vector3Int(x, lowerLeftCorner.y, 0)));
            }
            this.wallpaperTilemap = wallpaperTilemap;
            ApplyWallpaper(wallpaperId);
        }

        public void ApplyWallpaper(int wallpaperId) {
            WallOrigin oldOrigin = GetOriginRepresentingThisWall();
            Sprite sprite = wallpaperAtlas.GetSprite($"Walls_{wallpaperId}");
            foreach (WallStrip strip in strips) {
                wallpaperTilemap.SetTiles(strip.strip, SplitSprite(sprite));
            }
            this.wallpaperId = wallpaperId;
            // UndoRedoController.AddActionToLog(new UserAction());
        }

        public IEnumerable<Vector3Int> GetAllWallCordinates() {
            List<Vector3Int> wallCordinates = new();
            foreach (WallStrip strip in strips) {
                foreach (Vector3Int cordinate in strip.strip) {
                    wallCordinates.Add(cordinate);
                }
            }
            return wallCordinates;
        }

        public IEnumerable<Vector3Int> GetWallBaseCordinates() {
            List<Vector3Int> wallCordinates = new();
            foreach (WallStrip strip in strips) {
                wallCordinates.Add(strip.strip[0]);
            }
            return wallCordinates;
        }

        public bool WallContains(Vector3Int point) {
            return strips.Any(strip => strip.StripContains(point));
        }

        public WallOrigin GetOriginRepresentingThisWall(bool giveOriginal = true) {
            if (giveOriginal) if (wasModifiedByRenovation.Key) return wasModifiedByRenovation.Value;
            return new(LowerLeftCorner, Width, wallpaperId);

        }

        public void MoveWall(Vector3Int lowerLeftCorner, int width, int wallpaperId = 0) {
            if (!wasModifiedByRenovation.Key) wasModifiedByRenovation = new(true, new(LowerLeftCorner, Width, this.wallpaperId)); //dont overwrite the first ever version
            foreach (Vector3Int oldTile in GetAllWallCordinates()) {
                wallpaperTilemap.SetTile(oldTile, null);
            }

            strips = new();
            for (int x = lowerLeftCorner.x; x <= lowerLeftCorner.x + width - 1; x++) {
                strips.Add(new WallStrip(new Vector3Int(x, lowerLeftCorner.y, 0)));
            }
            ApplyWallpaper(wallpaperId);
        }

        public static Sprite GetSpriteFromWallpaperID(int wallpaperId) {
            return wallpaperAtlas.GetSprite($"Walls_{wallpaperId}");
        }

        public static int GetTotalWallpaperTextures() {
            return wallpaperAtlas.spriteCount;
        }
    }

    [Serializable]
    public class WallOrigin {
        public Vector3Int lowerLeftCorner;
        public int width;
        public int wallpaperId;
        public WallOrigin(Vector3Int lowerLeftCorner, int width, int wallpaperId = 0) {
            this.lowerLeftCorner = lowerLeftCorner;
            this.width = width;
            this.wallpaperId = wallpaperId;
        }

        // public bool IsWhole() {
        //     if (lowerLeftCorner == null) return false;
        //     if (width == null) return false;
        //     if (wallpaperId == null) return true;
        //     return false;
        // }
    }

    private List<Wall> walls = new();
    public Tilemap wallPaperTilemap;
    public static Sprite SelectedWallpaperSprite => Wall.GetSpriteFromWallpaperID(selectedWallpaperId);
    public static int TotalWallpaperTextures => Wall.GetTotalWallpaperTextures();
    public static int selectedWallpaperId;

    public void SetWalls(List<WallOrigin> origins, Tilemap wallpaperTilemap) {
        if (origins == null) return;
        if (origins.Count == 0) return;
        walls = new();

        wallPaperTilemap = wallpaperTilemap;
        foreach (WallOrigin origin in origins) {
            AddWall(origin);
        }
    }

    public void UpdateWalls(List<WallOrigin> origins) {
        wallPaperTilemap.ClearAllTiles();
        walls = new();

        foreach (WallOrigin origin in origins) {
            AddWall(origin);
        }
    }

    public static Sprite GetWallpaperSprite(int wallpaperId) {
        return Wall.GetSpriteFromWallpaperID(wallpaperId);
    }

    public void AddWall(WallOrigin origin) {
        Wall wall = new(origin.lowerLeftCorner, origin.width, wallPaperTilemap, origin.wallpaperId);
        var overlappingWalls = walls.Where(wall => wall.GetWallBaseCordinates().Intersect(GetRectAreaFromPoint(origin.lowerLeftCorner, 3, origin.width)).Any());
        if (overlappingWalls.Any() && !BuildingController.IsLoadingSave) {
            string overlaps = string.Join(", ", overlappingWalls.Select(w => $"[{string.Join(", ", w.GetWallBaseCordinates())}]"));
            throw new Exception($"Walls overlap trigger: {origin.lowerLeftCorner} at {overlaps}");
        }
        walls.Add(wall);

        Building.GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(new($"Wall{origin.lowerLeftCorner}", wall.GetAllWallCordinates().ToHashSet(), TileType.Wall));
        wallPaperTilemap.CompressBounds();
    }

    /// <summary>
    /// Move a wall from an old position to a new while keeping some of its values
    /// </summary>
    /// <param name="oldWall">Any point on the old wall</param>
    /// <param name="newOrigin">This will be ADDED as an OFFSET to the current wall, this means a width of 1 will increase the old wall width by 1 etc. Wallpaper ID works normally. Use -1 ID to keep old wallpaper</param>
    public void MoveWall(Vector3Int oldWall, WallOrigin newOrigin) {
        Wall wall = GetWallFromPoint(oldWall);
        if (wall == null) return;
        Building.GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(new($"Wall{wall.LowerLeftCorner}"));
        wall.MoveWall(wall.LowerLeftCorner + newOrigin.lowerLeftCorner, wall.Width + newOrigin.width, newOrigin.wallpaperId == -1 ? wall.wallpaperId : newOrigin.wallpaperId);
        Building.GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.AddSpecialTileSet(new($"Wall{wall.LowerLeftCorner}", wall.GetAllWallCordinates().ToHashSet(), TileType.Wall));
    }

    public void RemoveWall(Vector3Int point) {
        Wall wall = GetWallFromPoint(point);
        if (wall == null) return;
        walls.Remove(wall);
        Building.GetComponent<EnterableBuildingComponent>().InteriorSpecialTiles.RemoveSpecialTileSet(new($"Wall{wall.LowerLeftCorner}"));
        foreach (Vector3Int cordinate in wall.GetAllWallCordinates()) wallPaperTilemap.SetTile(cordinate, null);
    }

    public void ApplyCurrentWallpaper(Vector3Int point) {
        Wall wall = GetWallFromPoint(point);
        if (wall == null) return;
        int oldId = wall.wallpaperId;
        wall.ApplyWallpaper(selectedWallpaperId);
        UndoRedoController.AddActionToLog(new WallpaperChangeRecord(this, point, (oldId, selectedWallpaperId)));
    }

    public void ApplyWallpaper(Vector3Int point, int wallpaperId) {
        Wall wall = GetWallFromPoint(point);
        if (wall == null) return;
        wall.ApplyWallpaper(wallpaperId);
    }

    // private void ApplyCurrentWallpaper(Wall wall) {
    //     wall.ApplyWallpaper(selectedWallpaperId);
    // }

    public static void SetSelectedWallpaper(int wallpaperId, bool setActionToPlaceWallpaper = true) {
        selectedWallpaperId = wallpaperId;
        if (setActionToPlaceWallpaper) BuildingController.SetCurrentAction(Actions.PLACE_WALLPAPER);
    }

    private Wall GetWallFromPoint(Vector3Int point) {
        return walls.FirstOrDefault(wall => wall.WallContains(point));
    }

    public override void Load(BuildingScriptableObject bso) {
        return;
    }

    public override void Load(ComponentData data) {
        // return;
        for (int i = 0; i < walls.Count; i++) {
            RemoveWall(walls[i].GetAllWallCordinates().First());
        }

        foreach (JProperty property in data.GetAllComponentDataProperties()) {
            JObject buildingData = (JObject)property.Value;
            Vector3Int lowerLeftCorner = new(
                buildingData["Origin"][0].Value<int>(),
                buildingData["Origin"][1].Value<int>(),
                0
            );
            int width = buildingData["Width"].Value<int>();
            int wallpaperId = buildingData["Wallpaper ID"].Value<int>();

            WallOrigin origin = new(lowerLeftCorner, width, wallpaperId);
            AddWall(origin);
        }
    }

    public override ComponentData Save() {
        // return null;
        ComponentData data = new(typeof(WallsComponent));
        int index = 0;

        foreach (Wall wall in walls) {
            WallOrigin origin = wall.GetOriginRepresentingThisWall(false);
            JProperty wallProperty = new(index.ToString(),
                new JObject(
                    new JProperty("Origin", new JArray(origin.lowerLeftCorner.x, origin.lowerLeftCorner.y)),
                    new JProperty("Width", origin.width),
                    new JProperty("Wallpaper ID", origin.wallpaperId)
                )
            );

            data.AddProperty(wallProperty);
            index++;
        }

        return data;
    }
}
