using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class Obelisk : Building{

    public enum ObeliskTypes{
        WaterObelisk,
        DesertObelisk,
        IslandObelisk,
        EarthObelisk
    }

    private ObeliskTypes obeliskType = ObeliskTypes.WaterObelisk;
    private SpriteAtlas atlas;

    public new void Start(){
        //name = GetType().Name;
        baseHeight = 3;
        base.Start(); 
        atlas = Resources.Load<SpriteAtlas>("Buildings/ObeliskAtlas");
        UpdateTexture(atlas.GetSprite(obeliskType.ToString()));
        //SetObeliskType(ObeliskTypes.DesertObelisk);
    }

    public override Dictionary<Materials, int> GetMaterialsNeeded(){
        return obeliskType switch{
            ObeliskTypes.WaterObelisk => new Dictionary<Materials, int>(){
                {Materials.Coins, 500_000},
                {Materials.IridiumBar, 5},
                {Materials.Clam, 10},
                {Materials.Coral, 10}
            },
            ObeliskTypes.DesertObelisk => new Dictionary<Materials, int>{
                {Materials.Coins, 1_000_000},
                {Materials.IridiumBar, 20},
                {Materials.Coconut, 10},
                {Materials.CactusFruit, 10}
            },
            ObeliskTypes.IslandObelisk => new Dictionary<Materials, int>(){
                {Materials.Coins, 1_000_000},
                {Materials.IridiumBar, 10},
                {Materials.DragonTooth, 10},
                {Materials.Banana, 10}
            },
            ObeliskTypes.EarthObelisk => new Dictionary<Materials, int>{
                {Materials.Coins, 500_000},
                {Materials.IridiumBar, 10},
                {Materials.EarthCrystal, 10}
            },
            _ => new Dictionary<Materials, int>()
        };
    }

    public void SetObeliskType(ObeliskTypes type){
        obeliskType = type;
        UpdateTexture(atlas.GetSprite(type.ToString()));
    }

    protected override void OnMouseRightClick(){
        SetObeliskType( (ObeliskTypes) (((int) obeliskType + 1) % 4) );
    }

    public override string GetBuildingData(){
        return base.GetBuildingData() + $"|{(int) obeliskType}";
    }

    public override void RecreateBuildingForData(int x, int y, params string[] data){
        Start();
        base.RecreateBuildingForData(x, y);
        SetObeliskType((ObeliskTypes) int.Parse(data[0]));
    }
}
