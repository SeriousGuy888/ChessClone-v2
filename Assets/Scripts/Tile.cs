using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {
    public Color lightSquareCol,
                 darkSquareCol,
                 legalMoveCol,
                 tileSelectedCol;

    public Vector2Int coords;
    public bool isLightSquare;
    public bool tileSelected = false;
    public bool legalMove = false;

    public void Setup(int file, int rank) {
        this.coords = new Vector2Int(file, rank);

        bool lightSquare = (file + rank) % 2 != 0;
        isLightSquare = lightSquare;
    }

    void OnMouseDown() {
        Piece clickedPiece = GetPiece();
        Piece selectedPiece = GameManager.selectedSquare
                ?.GetComponent<Tile>()
                .GetPiece()
                .GetComponent<Piece>();


        if(GameManager.selectedSquare != null && selectedPiece != null && clickedPiece?.isWhite != selectedPiece?.isWhite) { // some square already selected
            GameManager.SetSelectedSquare(null);

            if(!selectedPiece.CheckMoveLegality(coords))
                return;


            if(selectedPiece.pieceType == 'p' && coords == GameManager.enPassantSquare) {
                Piece pieceTakenEnPassant = GameManager.board
                    .GetSquare((Vector2Int) GameManager.enPassantSquareTarget)
                    .GetComponent<Tile>()
                    .GetPiece();
                Destroy(pieceTakenEnPassant.gameObject);
            } else {
                GameManager.enPassantSquare = null;
                GameManager.enPassantSquareTarget = null;
            }

            Vector2Int? enPassantCoords = selectedPiece.GetEnPassantCoords(coords);
            if(enPassantCoords != null) {
                GameManager.enPassantSquare = enPassantCoords;
                GameManager.enPassantSquareTarget = coords;
            }


            (Transform newRookTransform, Piece rook)? castle = selectedPiece.GetCastle(coords);
            if(castle != null) {
                castle?.rook.MoveTo(castle?.newRookTransform);
            }




            if(clickedPiece != null)
                Destroy(clickedPiece.gameObject);
            selectedPiece.MoveTo(transform);
            GameManager.ToggleTurn();
        }
        else if(clickedPiece != null) { // non-empty square, no selection or clicked on new square with same colour piece as selected
            if(GameManager.whiteToMove != clickedPiece.isWhite)
                return;
            GameManager.SetSelectedSquare(transform); // select square
        }
    }

    public Piece GetPiece() {
        if(transform.childCount == 0)
            return null;
        return transform.GetChild(0).GetComponent<Piece>();
    }

    void Update() {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        if(tileSelected)
            spriteRenderer.color = tileSelectedCol;
        else if (legalMove)
            spriteRenderer.color = legalMoveCol;
        else
            spriteRenderer.color = isLightSquare ? lightSquareCol : darkSquareCol;
    }
}