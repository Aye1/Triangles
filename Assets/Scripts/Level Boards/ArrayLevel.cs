using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArrayLevel : Level {

    public override void GenerateShapes()
    {
        int[,] data = GetBoardData();
        CreateBoardFromArray(data);
    }


    protected abstract int[,] GetBoardData();
   
    protected void CreateBoardFromArray(int[,] data)
    {
        ComputeBoardDimensions(data.GetLength(1), data.GetLength(0));
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
