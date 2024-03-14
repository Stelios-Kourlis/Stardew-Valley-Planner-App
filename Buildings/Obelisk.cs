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

    public override string TooltipMessage => "Right Click To Cycle Through Obelisks";

    public override void OnAwake(){
        //name = GetType().Name;
        baseHeight = 3;
        base.OnAwake(); 
        atlas = Resources.Load<SpriteAtlas>("Buildings/ObeliskAtlas");
        UpdateTexture(atlas.GetSprite(obeliskType.ToString()));
        //SetObeliskType(ObeliskTypes.DesertObelisk);
    }

    public override List<MaterialInfo> GetMaterialsNeeded(){
        return obeliskType switch{
            ObeliskTypes.WaterObelisk => new List<MaterialInfo>{
                new MaterialInfo(500000, Materials.Coins),
                new MaterialInfo(5, Materials.IridiumBar),
                new MaterialInfo(10, Materials.Clam),
                new MaterialInfo(10, Materials.Coral)
            },
            ObeliskTypes.DesertObelisk => new List<MaterialInfo>{
                new MaterialInfo(1000000, Materials.Coins),
                new MaterialInfo(20, Materials.IridiumBar),
                new MaterialInfo(10, Materials.Coconut),
                new MaterialInfo(10, Materials.CactusFruit)
            },
            ObeliskTypes.IslandObelisk => new List<MaterialInfo>{
                new MaterialInfo(1000000, Materials.Coins),
                new MaterialInfo(10, Materials.IridiumBar),
                new MaterialInfo(10, Materials.DragonTooth),
                new MaterialInfo(10, Materials.Banana)
            },
            ObeliskTypes.EarthObelisk => new List<MaterialInfo>{
                new MaterialInfo(500000, Materials.Coins),
                new MaterialInfo(10, Materials.IridiumBar),
                new MaterialInfo(10, Materials.EarthCrystal)
            },
            _ => new List<MaterialInfo>()
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
        OnAwake();
        Place(new Vector3Int(x,y,0));
        SetObeliskType((ObeliskTypes) int.Parse(data[0]));
    }
}
