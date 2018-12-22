using UnityEngine;
using UnityEngine.EventSystems;

public class BonusViewMouseHandler : MonoBehaviour, IPointerDownHandler {

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponentInParent<BonusView>().HideView();
    }
}
