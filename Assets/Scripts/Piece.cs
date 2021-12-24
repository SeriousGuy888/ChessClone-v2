using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {
    public Sprite pawnSprite,
                  knightSprite,
                  bishopSprite,
                  rookSprite,
                  queenSprite,
                  kingSprite;


    public char pieceType;
    public bool isWhite;
    public bool hasMoved = false;


    public void Setup(char pieceChar) {
        (Sprite sprite, bool isWhite) pieceData = getPieceData(pieceChar);

        pieceType = char.ToLower(pieceChar);
        isWhite = pieceData.isWhite;

        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = pieceData.sprite;
        spriteRenderer.color = isWhite ? Color.white : Color.black;
    }

    public bool CheckMoveLegality(Vector2Int coords) {
        Tile oldTile = GetTile();
        Tile newTile = GameManager.board.GetSquare(coords).GetComponent<Tile>();

        List<Vector2Int> legalSquares = SlidingMoves.GenerateMoves(this, oldTile.coords);
        if(!legalSquares.Contains(coords))
            return false;
        return true;
    }

    public Vector2Int? GetEnPassantCoords(Vector2Int coords) {
        Tile oldTile = GetTile();
        if(pieceType == 'p' && (Mathf.Abs((oldTile.coords - coords).y) == 2))
            return coords - new Vector2Int(0, 1) * (isWhite ? 1 : -1);
        return null;
    }

    public (Transform, Piece)? GetCastle(Vector2Int coords) {
        if(pieceType != 'k')
            return null;
        
        Tile oldTile = GetTile();
        int diff = (oldTile.coords - coords).x;
        if(Mathf.Abs(diff) == 2) {
            Piece rook = GetCastleRook(diff < 0 ? 1 : -1);
            if(rook == null)
                return null;

            Vector2Int newCoords = coords + new Vector2Int((diff > 0 ? 1 : -1), 0);
            return (GameManager.board.GetSquare(newCoords).transform, rook); // return where the rook should move and the rook to be moved
        }
        
        return null;
    }

    public Piece GetCastleRook(int direction) { // direction is -1 for left and 1 for right
        Vector2Int coords = GetTile().coords;
        Vector2Int currCoords = coords;
        GameObject currSquare = GameManager.board.GetSquare(coords);
        Piece rook = null;

        while(rook == null && currSquare != null) {
            currCoords += new Vector2Int(direction, 0);
            currSquare = GameManager.board.GetSquare(currCoords);

            Piece currPiece = currSquare?.GetComponent<Tile>().GetPiece();
            if(currPiece != null && currPiece.pieceType != 'r')
                break;
            if(currPiece?.pieceType == 'r' && !currPiece.hasMoved) {
                rook = currPiece;
                break;
            }
        }

        return rook;
    }


    public void MoveTo(Transform newSquare) {
        transform.parent = newSquare;
        transform.localPosition = Vector2.zero;
        
        hasMoved = true;
    }


    private Tile GetTile() {
        return transform.parent.GetComponent<Tile>();
    }

    private (Sprite sprite, bool isWhite) getPieceData(char pieceChar) {
        Dictionary<char, Sprite> dictionary = new Dictionary<char, Sprite> {
            { 'k', kingSprite },
            { 'q', queenSprite },
            { 'r', rookSprite },
            { 'b', bishopSprite },
            { 'n', knightSprite },
            { 'p', pawnSprite },
            { ' ', null },
        };

        Sprite sprite = dictionary[char.ToLower(pieceChar)];
        return (sprite, char.IsUpper(pieceChar));
    }
}