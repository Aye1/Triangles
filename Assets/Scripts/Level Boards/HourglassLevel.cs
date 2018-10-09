public class HourglassLevel : Level {

    public override void GenerateShapes()
    {
        int minwidth = 5;
        int maxwidth = 11;
        int height = (maxwidth - minwidth) + 2;
        bool isMinReached = false;

        for (int j = 0; j < height; j++)
        {
            int currentWidth = isMinReached ? (int)(minwidth + 2 * (j - (height / 2.0f))) : (int)(maxwidth - 2 * j);
            if (!isMinReached)
            {
                isMinReached = currentWidth == minwidth;

            }
            int imin = (maxwidth - currentWidth) / 2 + 1;
            int imax = imin + currentWidth;

            for (int i = imin; i < imax; i++)
            {
                CreateBoardShape(i, j);
            }
        }
        ComputeBoardDimensions(maxwidth, height);
        AdjustBoardPosition();
    }
}
