using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonLevel : Level {
    public override void GenerateShapes()
    {
        int minWidth = 9;
        int height = minWidth - 1;
        int maxWidth = 2 * minWidth - 3;

        _visualWidth = maxWidth;
        _visualHeight = height;
        ComputeBoardDimensions();

        // Décalage de 1 pour rester sur la bonne parité de grille
        int offset = height / 2 % 2 == 0 ? 1 : 0;

        for (int j = 0; j < height; j++)
        {
            int currentWidth = maxWidth - (int)Mathf.Abs(minWidth / 2.0f - (j + 1)) * 2;
            int imin = (maxWidth - currentWidth) / 2 + offset;
            int imax = imin + currentWidth;

            for (int i = imin; i < imax; i++)
            {
                CreateBoardShape(i, j);
            }
        }
        AdjustBoardPosition();
    }
}
