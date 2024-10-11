using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "modification", menuName = "ScriptableObjects/HouseModification", order = 1)]
public class HouseModificationScriptableObject : ScriptableObject {
    public HouseExtensionsComponent.HouseModifications type;
    public Vector3Int spriteOrigin;
    public Sprite backSprite, frontSprite, backRemoved;
    public List<WallsComponent.WallOrigin> wallOrigins;
    public List<FlooringComponent.FlooringOrigin> floorOrigins;
    public List<WallMove> wallModifications;
}

[Serializable]
public class WallMove {
    public Vector3Int oldWallPoint;
    public WallsComponent.WallOrigin newWall;


    /// <summary>
    /// If used MoveWall from this class, use it again with the return WallOrigin to revert the Modification
    /// </summary>
    /// <returns></returns>
    public WallsComponent.WallOrigin GetReverseModification() {
        return new WallsComponent.WallOrigin(
            newWall.lowerLeftCorner * new Vector3Int(-1, -1, 0),
            newWall.width * -1,
            newWall.wallpaperId
        );
    }
}
