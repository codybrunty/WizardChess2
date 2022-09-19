using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SquareSelectorCreator))]
public class Board : MonoBehaviour{

    public const int BOARD_SIZE = 8;

    [SerializeField] private Transform bottomLeftSquareTransform;
    [SerializeField] private float squareSize;

    private Piece[,] grid;
    private Piece selectedPiece;
    private ChessGameController chessGameController;
    private SquareSelectorCreator squareSelectorCreator;

    private void Awake() {
        squareSelectorCreator = GetComponent<SquareSelectorCreator>();
        CreateGrid();
    }

    public void SetDependencies(ChessGameController chessGameController) {
        this.chessGameController = chessGameController;
    }

    private void CreateGrid() {
        grid = new Piece[BOARD_SIZE, BOARD_SIZE];
    }

    public Vector3 CalculatePositionFromCoordinates(Vector2Int coordinates){
        return bottomLeftSquareTransform.position + new Vector3(coordinates.x*squareSize,0f,coordinates.y*squareSize);
    }
    private Vector2Int CalculateCoordinatesFromPosition(Vector3 inputPosition) {
        int x = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).x / squareSize) + BOARD_SIZE / 2;
        int y = Mathf.FloorToInt(transform.InverseTransformPoint(inputPosition).z / squareSize) + BOARD_SIZE / 2;
        return new Vector2Int(x, y);
    }

    public void OnSquareSelected(Vector3 inputPosition) {
        if (!chessGameController.IsGameInProgress()) { return; }
        Vector2Int coordinates = CalculateCoordinatesFromPosition(inputPosition);
        Piece piece = GetPieceOnSquare(coordinates);
        if (selectedPiece) {
            if (piece != null && selectedPiece == piece) {
                DeselectPiece();
            }
            else if (piece != null && selectedPiece != piece && chessGameController.IsTeamTurnActive(piece.teamColor)) {
                SelectPiece(piece);
            }
            else if (selectedPiece.CanMoveTo(coordinates)) {
                OnSelectedPieceMoved(coordinates, selectedPiece);
            }
        }
        else {
            if (piece != null && chessGameController.IsTeamTurnActive(piece.teamColor)) {
                SelectPiece(piece);
            }
        }
    }

    private void DeselectPiece() {
        selectedPiece = null;
        squareSelectorCreator.ClearSelection();
    }
    private void SelectPiece(Piece piece) {
        chessGameController.RemoveMovesEnablingAttackOnPieceOfType<King>(piece);
        selectedPiece = piece;
        List<Vector2Int> selection = selectedPiece.availableMoves;
        ShowSelectionSquares(selection);
    }

    private void ShowSelectionSquares(List<Vector2Int> selection) {
        Dictionary<Vector3,bool> squaresData = new Dictionary<Vector3, bool>();
        for (int i = 0; i < selection.Count; i++) {
            Vector3 position = CalculatePositionFromCoordinates(selection[i]);
            Piece piece = GetPieceOnSquare(selection[i]);
            bool isSquareFree = piece == null;
            if(piece != null && chessGameController.IsTeamTurnActive (piece.teamColor)) {
                continue;
            }
            squaresData.Add(position, isSquareFree);
        }
        squareSelectorCreator.ShowSelection(squaresData);
    }

    private void OnSelectedPieceMoved(Vector2Int coordinates, Piece piece) {
        TryToTakeOpponentPiece(coordinates);
        UpdateBoardOnPieceMove(coordinates, piece.occupiedSquare, piece, null);
        selectedPiece.MovePiece(coordinates);
        DeselectPiece();
        EndTurn();
    }

    private void TryToTakeOpponentPiece(Vector2Int coordinates) {
        Piece piece = GetPieceOnSquare(coordinates);
        if(piece!=null && !selectedPiece.IsFromSameTeam(piece)) {
            TakePiece(piece);
        }
    }

    private void TakePiece(Piece piece) {
        if (piece) {
            grid[piece.occupiedSquare.x, piece.occupiedSquare.y] = null;
            chessGameController.OnPieceRemoved(piece);
        }
    }

    private void EndTurn() {
        chessGameController.EndTurn();
    }

    public void UpdateBoardOnPieceMove(Vector2Int newCoordinates, Vector2Int oldCoordinates, Piece newPiece, Piece oldPiece) {
        grid[oldCoordinates.x, oldCoordinates.y] = oldPiece;
        grid[newCoordinates.x, newCoordinates.y] = newPiece;
    }

    public Piece GetPieceOnSquare(Vector2Int coordinates) {
        if (ChecckIfCoordinatesAreOnBoard(coordinates)) {
            return grid[coordinates.x, coordinates.y];
        }
        return null;
    }

    public bool ChecckIfCoordinatesAreOnBoard(Vector2Int coordinates) {
        if(coordinates.x < 0 || coordinates.y <0 || coordinates.x >= BOARD_SIZE || coordinates.y >= BOARD_SIZE) {
            return false;
        }
        return true;
    }

    internal void OnGameRestarted() {
        selectedPiece = null;
        CreateGrid();
    }

    public bool HasPiece(Piece piece) {
        for (int i = 0; i < BOARD_SIZE; i++) {
            for (int y = 0; y < BOARD_SIZE; y++) {
                if (grid[i, y] == piece) {
                    return true;
                }
            }
        }
        return false;
    }
    public void SetPieceOnBoard(Vector2Int coordinates, Piece piece) {
        if (ChecckIfCoordinatesAreOnBoard(coordinates)) {
            grid[coordinates.x, coordinates.y] = piece;
        }
    }
}
