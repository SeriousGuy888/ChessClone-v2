using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingMoves {
    public static List<Vector2Int> GenerateMoves(Piece piece, Vector2Int coords) {
        bool isWhite = piece.isWhite;

        bool orthogonal = false;
        bool diagonal = false;
        bool isKnight = false;
        bool isPawn = false;
        bool isKing = false;

        switch(piece.pieceType) {
            case 'k':
                orthogonal = true;
                diagonal = true;
                isKing = true;
                break;
            case 'q':
                orthogonal = true;
                diagonal = true;
                break;
            case 'r':
                orthogonal = true;
                break;
            case 'b':
                diagonal = true;
                break;
            case 'n':
                isKnight = true;
                break;
            case 'p':
                isPawn = true;
                break;
            default:
                return null;
        }

        List<Vector2Int> legalSquares = new List<Vector2Int>();

        if(orthogonal)
            legalSquares.AddRange(FindMoves(coords, 'o', isKing, isWhite));
        if(diagonal)
            legalSquares.AddRange(FindMoves(coords, 'd', isKing, isWhite));
        if(isKnight)
            legalSquares.AddRange(FindMoves(coords, 'k', true, isWhite));
        if(isPawn)
            legalSquares.AddRange(FindPawnMoves(coords, isWhite));
        if(isKing)
            legalSquares.AddRange(FindCastlingMoves(piece, coords));

        return legalSquares;
    }

    private static List<Vector2Int> FindMoves(Vector2Int coords, char whichOffsets, bool stopAtOneLoop, bool isWhite) {
        Vector2Int[] orthogonalOffsets = {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
        };
        Vector2Int[] diagonalOffsets = {
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };
        Vector2Int[] knightOffsets = {
            new Vector2Int(2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(-2, -1),
            new Vector2Int(1, 2),
            new Vector2Int(-1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(-1, -2),
        };

        Vector2Int[] offsetArr = {};
        switch(whichOffsets) {
            case 'o':
                offsetArr = orthogonalOffsets;
                break;
            case 'd':
                offsetArr = diagonalOffsets;
                break;
            case 'k':
                offsetArr = knightOffsets;
                break;
        }


        List<Vector2Int> foundSquares = new List<Vector2Int>();

        foreach(Vector2Int offset in offsetArr) {
            Vector2Int currCoords = coords + offset;
            GameObject currSquare = GameManager.board.GetSquare(currCoords);

            while(currSquare != null) {
                Piece currPiece = currSquare.GetComponent<Tile>().GetPiece();

                if(currPiece != null && currPiece.isWhite == isWhite) // piece of same colour on square
                    break;                                            // break before square is added, making a capture illegal
                foundSquares.Add(currCoords);
                if(currPiece != null) // piece of opposite colour on square
                    break;            // break from loop after adding square to list, making capturing a legal move

                currCoords += offset;
                currSquare = GameManager.board.GetSquare(currCoords);

                if(stopAtOneLoop) // only loop once
                    break;
            }
        }

        return foundSquares;
    }

    private static List<Vector2Int> FindCastlingMoves(Piece piece, Vector2Int coords) {
        if(piece.pieceType != 'k' || piece.hasMoved)
            return null;


        List<Vector2Int> foundSquares = new List<Vector2Int>();

        for(int i = 0; i < 2; i++) {
            int direction = (i == 0) ? -1 : 1;
            Piece rook = piece.GetCastleRook(direction);
            if(rook != null)
                foundSquares.Add(coords + new Vector2Int(2 * direction, 0));
        }

        return foundSquares;
    }

    private static List<Vector2Int> FindPawnMoves(Vector2Int coords, bool isWhite) {
        List<Vector2Int> foundSquares = new List<Vector2Int>();


        Vector2Int[] pushes = {
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
        };
        Vector2Int[] captures = {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
        };

        if(!isWhite) {
            for(int i = 0; i < pushes.Length; i++)
                pushes[i] *= -1;
            for(int i = 0; i < captures.Length; i++)
                captures[i] *= -1;
        }

        foreach(Vector2Int offset in pushes) {
            if(Mathf.Abs(offset.y) == 2 && !canPushTwo(coords.y, isWhite))
                continue;
            Vector2Int pushCoords = coords + offset;
            GameObject pushSquare = GameManager.board.GetSquare(pushCoords);
            if(pushSquare == null)
                continue;
            Tile pushTile = pushSquare.GetComponent<Tile>();
            if(pushTile.GetPiece() != null)
                break;

            foundSquares.Add(pushCoords);
        }

        foreach(Vector2Int offset in captures) {
            Vector2Int captureCoords = coords + offset;
            GameObject captureSquare = GameManager.board.GetSquare(captureCoords);
            if(captureSquare == null)
                continue;
            Tile captureTile = captureSquare.GetComponent<Tile>();
            Piece capturePiece = captureTile.GetPiece();
            
            if(capturePiece?.isWhite == isWhite)
                continue;
            if(capturePiece == null && GameManager.enPassantSquare != captureCoords)
                continue;
            
            foundSquares.Add(captureCoords);
        }



        return foundSquares;
    }

    private static bool canPushTwo(int rank, bool isWhite) {
        if(isWhite && rank == 1)
            return true;
        if(!isWhite && rank == 6)
            return true;
        return false;
    }
}