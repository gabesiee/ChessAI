using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessEngine;

public class IteractionManager : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    Renderer objRenderer;
    Material oldMaterial;
    Material newMaterial;
    GameObject obj;
    Vector3 position;

    PiecesManager pm = new PiecesManager();

    bool isSelected = false;

    // Update is called once per frame
    void Update()
    {
        HighlightSquare();
        
    }

    void HighlightSquare()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                position = hit.transform.position;

                int positionForEngine = (int)((7 - position.z) * 8 + position.x);

                if (pm.IsPlayableSquare(positionForEngine))
                {
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    hit.collider.gameObject.GetComponent<Renderer>().material.color = Color.red;
                }
            }
                
            if (obj != hit.collider.gameObject && isSelected == false)//If we're not pointing at the previous target
            {
                if (obj != null)//If previous target is set, reset its material
                {
                    objRenderer.material = oldMaterial;
                }

                obj = hit.collider.gameObject;//Store reference of target to a variable
                objRenderer = obj.GetComponent<Renderer>();//Get targets Renderer
                oldMaterial = objRenderer.material;//Store targets current material
                objRenderer.material = newMaterial;//Set target to new material
            }
        }
        else
        {//If we're not pointing at anything
            if (obj != null && isSelected == false)
            {
                objRenderer.material = oldMaterial;//Reset targets material
                obj = null;//Clear reference
            }
        }
    }

}
