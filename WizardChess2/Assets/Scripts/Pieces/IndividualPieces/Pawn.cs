using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece{

    public override List<Vector2Int> SelectAvailableSquares() {
        availableMoves.Clear();
        Vector2Int direction = teamColor == TeamColor.Blue ? Vector2Int.up : Vector2Int.down;
        float range = hasMoved ? 1 : 2;

        //incase piece in front of pawn at the begining
        Vector2Int tmp_nextCoordinates = occupiedSquare + direction * 1;
        Piece tmp_piece = board.GetPieceOnSquare(tmp_nextCoordinates);
        if (tmp_piece != null) {
            range = 1;
        }


        for (int i = 1; i <= range; i++) {
            Vector2Int nextCoordinates = occupiedSquare + direction * i;
            Piece piece = board.GetPieceOnSquare(nextCoordinates);
            if (!board.ChecckIfCoordinatesAreOnBoard(nextCoordinates)) {
                break;
            }
            if (piece == null) {
                TryToAddMove(nextCoordinates);
            }
            else if (piece.IsFromSameTeam(this)) {
                break;
            }
        }

        Vector2Int[] takeDirections = new Vector2Int[] { new Vector2Int(1, direction.y), new Vector2Int(-1,direction.y) };
        for (int i = 0;i < takeDirections.Length;i++) {
            Vector2Int nextCoordinates = occupiedSquare + takeDirections[i];
            Piece piece = board.GetPieceOnSquare(nextCoordinates);
            if (!board.ChecckIfCoordinatesAreOnBoard(nextCoordinates)) {
                continue;
            }
            if(piece!=null && !piece.IsFromSameTeam(this)) {
                TryToAddMove(nextCoordinates);
            }
        }
        return availableMoves;
    }

}
