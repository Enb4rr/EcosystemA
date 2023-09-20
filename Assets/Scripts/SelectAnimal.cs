using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAnimal : MonoBehaviour
{
    public GameObject selectedAnimal;
    [Range(0,15)]
    [SerializeField] private float selectionRange;
    
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            float distance = (hit.collider.gameObject.transform.position - hit.point).magnitude;
            if (hitObject != null && (hitObject.CompareTag("Monkey") || hitObject.CompareTag("Snake")) && distance>selectionRange)
            {
                SelectanAnimal(hitObject);
            }
            else
            {
                DeselectAnimal();
            }
        }
        else
        {
            DeselectAnimal();
        }
    }

    void SelectanAnimal(GameObject animal)
    {

        if (selectedAnimal != null)
        {
            selectedAnimal.GetComponentInChildren<CanvasGroup>().alpha = 0f;
            selectedAnimal.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 0);
        }
        selectedAnimal = animal;
        CanvasGroup canvasGroup = selectedAnimal.GetComponentInChildren<CanvasGroup>();
        canvasGroup.alpha = 1f;
        Vector2 selecetedObjectRect = Camera.main.WorldToScreenPoint(selectedAnimal.transform.position)+new Vector3(0,1,0);
        canvasGroup.gameObject.GetComponent<RectTransform>().position = selecetedObjectRect;

        selectedAnimal.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 0.009f);
        if(selectedAnimal.name == "Fly")
        {
            selectedAnimal.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 0.1f);
        }
    }

    void DeselectAnimal()
    {
        if (selectedAnimal != null && (selectedAnimal.CompareTag("Monkey") || selectedAnimal.CompareTag("Snake")))
        {
            selectedAnimal.GetComponentInChildren<CanvasGroup>().alpha = 0f;
            selectedAnimal.GetComponentInChildren<SkinnedMeshRenderer>().material.SetFloat("_OutlineWidth", 0);
            selectedAnimal = null;
        }
    }
}
