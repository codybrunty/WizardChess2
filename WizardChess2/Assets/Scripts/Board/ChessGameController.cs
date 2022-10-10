using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PieceCreator))]
public class ChessGameController : MonoBehaviour{

    private enum GameState { Init, Play, Finished}

    [SerializeField] private BoardLayout startingBoardLayout;
    [SerializeField] private Board board;
    [SerializeField] private ChessUIManager chessUIManager;
    private PieceCreator pieceCreator;
    private ChessPlayer bluePlayer;
    private ChessPlayer redPlayer;
    private ChessPlayer activePlayer;
    private GameState gameState;

    private void Awake(){
        SetDependencies();
        CreatePlayers();
    }

    private void SetDependencies(){
        pieceCreator = GetComponent<PieceCreator>();
    }

    private void CreatePlayers() {
        bluePlayer = new ChessPlayer(TeamColor.Blue, board);
        redPlayer = new ChessPlayer(TeamColor.Red, board);
    }

    private void Start(){
        StartNewGame();
    }

    private void StartNewGame(){
        chessUIManager.HideUI();
        SetGameState(GameState.Init);
        board.SetDependencies(this);
        CreatePiecesFromLayout(startingBoardLayout);
        activePlayer = bluePlayer;
        GenerateAllPossiblePlayerMoves(activePlayer);
        SetGameState(GameState.Play);
    }

    private void SetGameState(GameState gameState) {
        this.gameState = gameState;
    }

    public bool IsGameInProgress() {
        return gameState == GameState.Play;
    }

    private void CreatePiecesFromLayout(BoardLayout layout){
        for (int i = 0; i < layout.GetPiecesCount(); i++){
            Vector2Int squareCoordinates = layout.GetSquareCoordinatesAtIndex(i);
            TeamColor teamColor = layout.GetSquareTeamColorAtIndex(i);
            string pieceName = layout.GetSquarePieceNameAtIndex(i);

            Type type = Type.GetType(pieceName);
            CreatePieceAndInitialize(squareCoordinates, teamColor, type);
        }
    }

    public void CreateFire(Vector2Int coordinates) {
        Type type = Type.GetType("Fire");
        CreatePieceAndInitialize(coordinates, activePlayer.teamColor, type);
    }

    private void CreatePieceAndInitialize(Vector2Int squareCoordinates, TeamColor teamColor, Type type){
        Piece newPiece = pieceCreator.CreatePiece(type).GetComponent<Piece>();
        newPiece.SetData(squareCoordinates, teamColor, board);

        Material teamMaterial;
        if (type == Type.GetType("Fire")) {
            teamMaterial = pieceCreator.GetBlackMaterial();
        }
        else {
            teamMaterial = pieceCreator.GetTeamMaterial(teamColor);
        }
        newPiece.SetMaterial(teamMaterial); 
        
        board.SetPieceOnBoard(squareCoordinates,newPiece);
        ChessPlayer currentPlayer = teamColor == TeamColor.Blue ? bluePlayer : redPlayer;
        currentPlayer.AddPiece(newPiece);
    }

    public void GenerateAllPossiblePlayerMoves(ChessPlayer player) {
        player.GenerateAllPossibleMoves();
    }
    public bool IsTeamTurnActive(TeamColor teamColor) {
        return activePlayer.teamColor == teamColor;
    }

    internal void EndTurn() {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));

        //if (CheckIfChessGameIsFinished()) {
        if (CheckIfWizardChessGameIsFinished()) { 
            EndGame();
        }
        else {
            ChangeActiveTeam();
        }


    }

    public void UpdateAllMoves() {
        GenerateAllPossiblePlayerMoves(activePlayer);
        GenerateAllPossiblePlayerMoves(GetOpponentToPlayer(activePlayer));
    }

    private bool CheckIfChessGameIsFinished() {
        Piece[] kingAttackingPieces = activePlayer.GetPiecesAttackingOppositePieceOfType<King>();
        if(kingAttackingPieces.Length > 0) {
            ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
            Piece attackedKing = oppositePlayer.GetPiecesOfType<King>().FirstOrDefault();
            oppositePlayer.RemoveMovesEnablingAttackOnPiece<King>(activePlayer, attackedKing);

            int availableKingMoves = attackedKing.availableMoves.Count;
            if(availableKingMoves == 0) {
                bool canCoverKing = oppositePlayer.CanHidePieceFromAttack<King>(activePlayer);
                if (!canCoverKing) {
                    return true;
                }
            }
        }
        return false;
    }

    private bool CheckIfWizardChessGameIsFinished() {
        //check if opponent can make any moves
        ChessPlayer oppositePlayer = GetOpponentToPlayer(activePlayer);
        Piece[] allWizards = oppositePlayer.GetPiecesOfType<Wizard>();
        int count = 0;
        foreach (Piece piece in allWizards) { 
            if(piece.availableMoves.Count > 0) {
                count++;
            }
        }
        if (count == 0) {
            return true;
        }
        return false;
    }

    private void EndGame() {
        chessUIManager.OnGameFinished(activePlayer.teamColor.ToString());
        SetGameState(GameState.Finished);
    }

    private void ChangeActiveTeam() {
        activePlayer = activePlayer == bluePlayer ? redPlayer : bluePlayer;
    }

    private ChessPlayer GetOpponentToPlayer(ChessPlayer player) {
        return player == bluePlayer ? redPlayer : bluePlayer;
    }

    public void OnPieceRemoved(Piece piece) {
        ChessPlayer pieceOwner = (piece.teamColor == TeamColor.Blue) ? bluePlayer : redPlayer;
        pieceOwner.RemovePiece(piece);
        Destroy(piece.gameObject);
    }

    public void RemoveMovesEnablingAttackOnPieceOfType<T>(Piece piece) where T : Piece {
        activePlayer.RemoveMovesEnablingAttackOnPiece<T>(GetOpponentToPlayer(activePlayer), piece);
    }

    public void RestartGame() {
        DestroyPieces();
        board.OnGameRestarted();
        bluePlayer.OnGameRestarted();
        redPlayer.OnGameRestarted();
        StartNewGame();
    }

    private void DestroyPieces() {
        bluePlayer.activePieces.ForEach(p => Destroy(p.gameObject));
        redPlayer.activePieces.ForEach(p => Destroy(p.gameObject));
    }
}
