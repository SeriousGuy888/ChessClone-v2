using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public GameObject tilePrefab;
    public GameObject piecePrefab;

    [HideInInspector] public static string[] fileLetters = new string[] { "a", "b", "c", "d", "e", "f", "g", "h" };

    public GameObject[,] squares = new GameObject[8, 8];
    public char[,] pieceArrangement = new char[8, 8] {
        { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' },
        { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
        // { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
        { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
        // { ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ' },
        { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
        { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' },
    };



    public void CreateBoard() {
        for(int file = 0; file < 8; file++) {
            for(int rank = 0; rank < 8; rank++) {
                squares[file, rank] = Instantiate(tilePrefab, new Vector2(file - 3.5f, rank - 3.5f), Quaternion.identity);
                squares[file, rank].transform.SetParent(transform);
                squares[file, rank].name = fileLetters[file] + (rank + 1);

                squares[file, rank].GetComponent<Tile>().Setup(file, rank);
            }
        }
    }

    public void SetupPieces() {
        for(int row = 0; row < pieceArrangement.GetLength(0); row++) {
            for(int col = 0; col < pieceArrangement.GetLength(1); col++) {
                char currChar = pieceArrangement[row, col];
                if(currChar == ' ')
                    continue;
                
                GameObject piece = Instantiate(piecePrefab, squares[col, row].transform);
                piece.GetComponent<Piece>().Setup(currChar);
            }
        }
    }

    public GameObject GetSquare(Vector2Int coords) {
        if(
            squares.GetLength(0) <= coords.x || coords.x < 0 ||
            squares.GetLength(1) <= coords.y || coords.y < 0
        ) return null;

        return squares[coords.x, coords.y];
    }
}