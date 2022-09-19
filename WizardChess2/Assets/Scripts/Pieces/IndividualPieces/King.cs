using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece {


    private Vector2Int[] directions = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down, new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };
    private Vector2Int leftCastlingMove;
    private Vector2Int rightCastlingMove;
    private Piece leftRook;
    private Piece rightRook;

    public override List<Vector2Int> SelectAvailableSquares() {
        availableMoves.Clear();
        AssignStandardMoves();
        AssignCastlingMoves();
        return availableMoves;
    }

    private void AssignCastlingMoves() {
        if (hasMoved) { return; }
        leftRook = GetPieceInDirection<Rook>(teamColor, Vector2Int.left);
        if(leftRook && !leftRook.hasMoved) {
            leftCastlingMove = occupiedSquare + Vector2Int.left * 2;
            availableMoves.Add(leftCastlingMove);
        }

        rightRook = GetPieceInDirection<Rook>(teamColor, Vector2Int.right);
        if (rightRook && !rightRook.hasMoved) {
            rightCastlingMove = occupiedSquare + Vector2Int.right * 2;
            availableMoves.Add(rightCastlingMove);
        }
    }

    private void AssignStandardMoves() {
        float range = 1;
        foreach (var direction in directions) {
            for (int i = 1; i <= range; i++) {
                Vector2Int nextCoordinates = occupiedSquare + direction * i;
                Piece piece = board.GetPieceOnSquare(nextCoordinates);
                if (!board.ChecckIfCoordinatesAreOnBoard(nextCoordinates)) {
                    break;
                }
                if (piece == null) {
                    TryToAddMove(nextCoordinates);
                }
                else if (!piece.IsFromSameTeam(this)) {
                    TryToAddMove(nextCoordinates);
                    break;
                }
                else if (piece.IsFromSameTeam(this)) {
                    break;
                }
            }
        }
    }

    public override void MovePiece(Vector2Int coordinates) {
        base.MovePiece(coordinates);
        if(coordinates == leftCastlingMove) {
            board.UpdateBoardOnPieceMove(coordinates + Vector2Int.right, leftRook.occupiedSquare, leftRook, null);
            leftRook.MovePiece(coordinates+Vector2Int.right);
        }
        else if(coordinates == rightCastlingMove) {
            board.UpdateBoardOnPieceMove(coordinates + Vector2Int.left, rightRook.occupiedSquare, rightRook, null);
            rightRook.MovePiece(coordinates + Vector2Int.left);
        }
    }
}
