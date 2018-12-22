using System;
using UnityEngine;

public class PieceBonusDestroy : AbstractPiece
{
    private void Start()
    {
        PieceColor = Color.gray;
        //transform.localScale = Vector3.Scale(transform.localScale, FindObjectOfType<Board>().transform.lossyScale);
    }
}
