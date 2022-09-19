using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(IObjectTweener))]
[RequireComponent(typeof(MaterialSetter))]
public abstract class Piece : MonoBehaviour {

    [SerializeField] MaterialSetter materialSetter;
    public Board board { protected get; set; }
    public Vector2Int occupiedSquare { get; set; }
    public TeamColor teamColor { get; set; }
    public bool hasMoved { get; private set; }
    public List<Vector2Int> availableMoves;
    private IObjectTweener tweener;

    public abstract List<Vector2Int> SelectAvailableSquares();

    private void Awake(){
        availableMoves = new List<Vector2Int>();
        tweener = GetComponent<IObjectTweener>();
        materialSetter = GetComponent<MaterialSetter>();
        hasMoved = false;
    }

    public void SetMaterial(Material material){
        if (materialSetter == null) {
            materialSetter = GetComponent<MaterialSetter>();
        }
        materialSetter.SetSingleMaterial(material);
    }

    public bool IsFromSameTeam(Piece piece){
        return teamColor == piece.teamColor;
    }

    internal bool IsAttackingPieceOfType<T>() where T : Piece {
        foreach (var square in availableMoves) {
            if(board.GetPieceOnSquare(square) is T) {
                return true;
            }
        }
        return false;
    }

    public bool CanMoveTo(Vector2Int coordinates){
        return availableMoves.Contains(coordinates);
    }

    public virtual void MovePiece(Vector2Int coordinates){
        Vector3 targetPosition = board.CalculatePositionFromCoordinates(coordinates);
        occupiedSquare = coordinates;
        hasMoved = true;
        tweener.MoveTo(transform, targetPosition);
    }

    protected void TryToAddMove(Vector2Int coordinates){
        availableMoves.Add(coordinates);
    }

    public void SetData(Vector2Int coordinates, TeamColor teamColor, Board board){
        this.teamColor = teamColor;
        occupiedSquare = coordinates;
        this.board = board;
        transform.position = board.CalculatePositionFromCoordinates(coordinates);

    }

    protected Piece GetPieceInDirection<T>(TeamColor teamColor, Vector2Int direction) where T: Piece {
        for (int i = 1; i <= Board.BOARD_SIZE; i++) {
            Vector2Int nextCoordinates = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoordinates);
            if (!board.ChecckIfCoordinatesAreOnBoard(nextCoordinates)) {
                return null;
            }
            if(piece !=null) {
                if(piece.teamColor != teamColor || !(piece is T)) {
                    return null;
                }
                else if (piece.teamColor == teamColor && piece is T) {
                    return piece;
                }
            }
        }
        return null;
    }
}
