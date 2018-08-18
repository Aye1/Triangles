using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    private readonly int minWidth = 9;
    public Shape basicShape;

    private float sqr3 = Mathf.Sqrt(3);

	// Use this for initialization
	void Start () {
        CreateBoard();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void CreateBoard()
    {
        int height = minWidth - 1;
        int maxWidth = 2 * minWidth - 3;

        // Décalage de 1 pour rester sur la bonne parité de grille
        int offset = height / 2 % 2 == 0 ? 1 : 0;

        for (int j = 0; j < height; j++)
        {
            int currentWidth = maxWidth - (int)Mathf.Abs(minWidth/2.0f-(j+1))*2;
            //int imin = (maxWidth - currentWidth) / 2 + offset;
            //int imax = imin + currentWidth;
            int imin = 0;
            int imax = 10;
            for (int i = imin; i < imax; i++)
            {
                Shape newShape = Instantiate(basicShape);
                Vector2 vecB = new Vector2(sqr3 * i / 2, -j / 2);
                Vector2 vecC = new Vector2(-sqr3 * i / 2, -j / 2);
                float A = j;
                float B = Mathf.Ceil((-i - j) / 2.0f);
                float C = Mathf.Floor(-(i - j) / 2.0f);
                newShape.Position = new Vector3(A, (int)B, (int)C);
                if ((i+j) % 2 == 0)
                {
                    newShape.transform.Rotate(0.0f, 0.0f, 180.0f);
                }
                newShape.transform.position = new Vector3(i / 2.0f, -j*Mathf.Sqrt(2)/2, newShape.transform.position.z);
                newShape.transform.parent = transform;
            }
        }
    }
}
