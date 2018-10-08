using UnityEngine;

public static class UIHelper
{
    public static void DisplayGameObject(GameObject obj)
    {
        obj.SetActive(true);
    }

    public static void HideGameObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public static void DisplayGameObject(GameObject obj, bool shouldDisplay) {
        obj.SetActive(shouldDisplay);
    }
}
