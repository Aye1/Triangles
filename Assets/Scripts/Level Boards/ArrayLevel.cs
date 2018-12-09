using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArrayLevel : Level {

    int[,] data;

    public override void GenerateShapes()
    {
        data = GetBoardData();
        SetVisualSize();
        CreateBoardFromArray(data);
    }


    protected abstract int[,] GetBoardData();
    protected abstract void SetVisualSize();

    protected void CreateBoardFromArray(int[,] data)
    {
        ComputeBoardDimensions();
        for (int j = 0; j < data.GetLength(1); j++)
        {
            for (int i = 0; i < data.GetLength(0); i++)
            {
                if (data[i, j] == 1)
                {
                    CreateBoardShape(j, i);
                }
            }
        }
        AdjustBoardPosition();
    }
}
