using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static Board board;

    public static bool whiteToMove = true;
    public static Transform selectedSquare;
    public static Vector2Int? enPassantSquare; // square to move onto to capture
    public static Vector2Int? enPassantSquareTarget; // square whose piece will be captured en passant

    void Start() {
        board = gameObject.GetComponent<Board>();
        board.CreateBoard();
        board.SetupPieces();
    }

    public static void SetSelectedSquare(Transform newSelectedSquare) {
        Tile newSelectedTile = newSelectedSquare?.GetComponent<Tile>();

        if(selectedSquare != null)
            selectedSquare.GetComponent<Tile>().tileSelected = false;
        if(newSelectedTile != null)
            newSelectedTile.tileSelected = true;
        
        
        foreach(GameObject square in board.squares) {
            square.GetComponent<Tile>().legalMove = false;
        }

        Piece selectedPiece = newSelectedTile?.GetPiece();
        if(selectedPiece != null) {
            List<Vector2Int> legalSquares = SlidingMoves.GenerateMoves(selectedPiece, newSelectedTile.coords);

            if(legalSquares != null) {
                legalSquares.ForEach(coords => {
                    Tile tile = GameManager.board.GetSquare(coords).GetComponent<Tile>();
                    tile.legalMove = true;
                });
            }
        }


        selectedSquare = newSelectedSquare;
    }

    public static void ToggleTurn() {
        whiteToMove = !whiteToMove;
    }
}