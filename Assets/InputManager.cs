using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

    private static InputManager _instance;

    public static InputManager Instance {
        get { return _instance; }
    }

	// Use this for initialization
	void Start () {
        if(_instance != null && _instance != this){
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
	}
	
	// Update is called once per frame
	void Update () {
        ManageMouse();
	}

    private void ManageMouse() {
        /*ManageMouseDown();
        ManageMouseUp();*/
    }

   /*private void ManageMouseDown() {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider != null)
            {
                GameObject collided = hit.collider.gameObject;
                foreach (MonoBehaviour b in collided.GetComponents<MonoBehaviour>())
                {
                    if (b is IClickable)
                    {
                        ((IClickable)b).OnClickDown();
                    }
                }
            }
        }
    }

    private void ManageMouseUp()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if (hit.collider != null)
            {
                GameObject collided = hit.collider.gameObject;
                foreach (MonoBehaviour b in collided.GetComponents<MonoBehaviour>())
                {
                    if (b is IClickable)
                    {
                        ((IClickable)b).OnClickUp();
                    }
                }
            }
        }
    }*/
}
