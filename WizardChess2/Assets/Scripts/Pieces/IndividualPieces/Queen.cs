using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece{

    private Vector2Int[] directions = new Vector2Int[] { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down, new Vector2Int(1, 1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(-1, -1) };

    public override List<Vector2Int> SelectAvailableSquares() {
        availableMoves.Clear();
        float range = Board.BOARD_SIZE;
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
        return availableMoves;
    }

}
