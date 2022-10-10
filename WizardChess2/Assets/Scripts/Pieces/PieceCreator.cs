using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceCreator : MonoBehaviour{

    [SerializeField] private GameObject[] piecesPrefab;
    [SerializeField] private Material blue;
    [SerializeField] private Material red;
    [SerializeField] private Material black;
    private Dictionary<string, GameObject> nameToPieceDict = new Dictionary<string, GameObject>();

    private void Awake(){
        foreach (var piece in piecesPrefab) {
            nameToPieceDict.Add(piece.GetComponent<Piece>().GetType().ToString(),piece);
        }
    }

    public GameObject CreatePiece(Type type){
        GameObject prefab = nameToPieceDict[type.ToString()];
        if (prefab) {
            GameObject newPiece = Instantiate(prefab);
            return newPiece;
        }
        return null;
    }

    public Material GetTeamMaterial(TeamColor color){
        return color == TeamColor.Blue ? blue : red;
    }
    public Material GetBlackMaterial() {
        return black;
    }
}
