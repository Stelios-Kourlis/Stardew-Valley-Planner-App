using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "modification", menuName = "ScriptableObjects/HouseModification", order = 2)]
public class HouseModificationScriptableObject : ScriptableObject {
    public HouseExtensionsComponent.HouseModifications type;
    public Vector3Int spriteOrigin;
    public Sprite backSprite, frontSprite, backRemoved;
    public List<WallsComponent.WallOrigin> wallOrigins;
    public List<FlooringComponent.FlooringOrigin> floorOrigins;
    public List<WallMove> wallModifications;
    public List<FlooringMove> floorModifications;
    public bool reverseActivation;
    public HouseExtensionsComponent.HouseModifications preexistingModification;
}

[Serializable]
public class WallMove {
    public Vector3Int oldWallPoint;
    public WallsComponent.WallOrigin newWall;


    /// <summary>
    /// If used MoveWall from this class, use it again with the returned WallOrigin to revert the Modification
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

[Serializable]
public class FlooringMove {
    public Vector3Int oldFlooringPoint;
    public FlooringComponent.FlooringOrigin newFlooring;

    /// <summary>
    /// If used MoveFloor from this class, use it again with the returned FlooringOrigin to revert the Modification
    /// </summary>
    /// <returns></returns>
    public FlooringComponent.FlooringOrigin GetReverseModification() {
        return new FlooringComponent.FlooringOrigin(
            newFlooring.lowerLeftCorner * new Vector3Int(-1, -1, 0),
            newFlooring.width * -1,
            newFlooring.height * -1,
            newFlooring.floorTextureID
        );
    }
}
