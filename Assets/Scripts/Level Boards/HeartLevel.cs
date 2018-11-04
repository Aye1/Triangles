using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLevel : ArrayLevel
{
    public override bool IsUpsideDown(int rest)
    {
        return rest != 0;
    }
    protected override int[,] GetBoardData()
	 {
        int[,] data = new int[,] {
            {0,1,1,1,1,1,0,1,1,1,1,1,0},
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {0,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,0,1,1,1,1,1,0,0,0,0}
        };
        return data;
    }
}
