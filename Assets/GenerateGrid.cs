using UnityEngine;

public class GeneratePlaneGrid : MonoBehaviour
{
    public GameObject PlanBoardBlack;
    public GameObject PlanBoardWhite; // assign PlanBoard1 here

    public int xCount = 1000;
    public float xStep = -210f;

    private readonly float[] zPositions = { -30f, 0f, 30f };

    void Start()
    {
        GenerateGrid();
    }
    bool IsEven(int x)
    {
        return x % 2 == 0;
    }

    void GenerateGrid()
    {
        if (PlanBoardBlack == null || PlanBoardWhite == null)
        {
            Debug.LogError("Assign PlanBoardBlack and PlanBoardWhite.");
            return;
        }

        Vector3 scale = new Vector3(3f, 3f, 3f);

        for (int i = 1; i <= xCount; i++)
        {
            float x = -240 + i * - 30;

            foreach (float z in zPositions)
            {
                GameObject prefabToUse;
                if (IsEven(i) == true)
                {
                    // Mix black and white depending on Z
                    if (z == -30f)
                        prefabToUse = PlanBoardWhite;
                    else if (z == 0f)
                        prefabToUse = PlanBoardBlack;
                    else 
                        prefabToUse = PlanBoardWhite;      
                }
                else
                     // Mix black and white depending on Z
                    if (z == -30f)
                        prefabToUse = PlanBoardBlack;
                    else if (z == 0f)
                        prefabToUse = PlanBoardWhite;
                    else 
                        prefabToUse = PlanBoardBlack;  

               

                GameObject clone = Instantiate(
                    prefabToUse,
                    new Vector3(x, 0f, z),
                    prefabToUse.transform.rotation
                );

                clone.transform.localScale = scale;
                clone.name = prefabToUse.name + "_X_" + x + "_Z_" + z;
            }
        }
    }

}