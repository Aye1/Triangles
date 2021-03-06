﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartLevel : ArrayLevel
{
    protected override void SetVisualSize()
    {
        _visualWidth = 13;
        _visualHeight = 7;
    }
    protected override int[,] GetBoardData()
	 {
        int[,] data = new int[,] {
            {0,0,1,1,1,1,1,0,1,1,1,1,1,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {0,1,1,1,1,1,1,1,1,1,1,1,1,1},
            {0,0,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,0,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,0,0,1,1,1,1,1,0,0,0,0}
        };
        return data;
    }
}
