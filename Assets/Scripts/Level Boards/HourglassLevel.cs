public class HourglassLevel : ArrayLevel
{

    protected override void SetVisualSize()
    {
        _visualWidth = 13;
        _visualHeight = 10;
    }

    protected override int[,] GetBoardData()
    {
        int[,] data = new int[,] {
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {0,1,1,1,1,1,1,1,1,1,1,1,0},
            {0,0,1,1,1,1,1,1,1,1,1,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,0,1,1,1,1,1,1,1,0,0,0},
            {0,0,1,1,1,1,1,1,1,1,1,0,0},
            {0,1,1,1,1,1,1,1,1,1,1,1,0},
            {1,1,1,1,1,1,1,1,1,1,1,1,1},
            {1,1,1,1,1,1,1,1,1,1,1,1,1}
        };
        return data;
    }
}
